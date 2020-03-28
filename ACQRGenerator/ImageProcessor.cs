using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using ACPatternMaker;
using ACQRGenerator.Interfaces;
using ImageMagick;
using QRCoder;

namespace ACQRGenerator
{
    public class ImageProcessor : IImageProcessor
    {
        private MagickImage _image;
        private int _width;
        private int _height;


        public void SetImage(Stream stream, int width = 4, int height = 4)
        {
            _image = new MagickImage(stream);
            _width = width;
            _height = height;
        }

        public IEnumerable<Bitmap> GenerateQrCodes()
        {

            // TODO: Shape to the image ratio
            var geometry = new MagickGeometry(128, 128);
            geometry.FillArea = true;
            _image.Resize(geometry);
            _image.Crop(128, 128, Gravity.Center);
            _image.RePage();

            _image.Quantize(new QuantizeSettings {Colors = 15, DitherMethod = DitherMethod.FloydSteinberg});

            var colors = GenerateColorProfileImage();
            try
            {
                _image.Map(colors, new QuantizeSettings {Colors = 15, DitherMethod = DitherMethod.No});
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                return new List<Bitmap>();
            }

            var (paletteString, paletteIndex) = GeneratePaletteInfo(_image);

            var qrCodes = new List<Bitmap>();
            foreach (var tile in _image.CropToTiles(32, 32))
            {
                var pixels = tile.GetPixels();

                var leftSide = new List<string>();
                var rightSide = new List<string>();
                foreach (var pixel in pixels)
                {
                    var x = paletteIndex[pixel.ToColor().ToString()].ToString("X");
                    if (pixel.X % 2 == 0)
                        rightSide.Add(x);
                    else
                        leftSide.Add(x);
                }

                var zipped = leftSide.Zip(rightSide, (l, r) => $"{l}{r}").ToArray();

                var pixelString = string.Join("", zipped);

                var output = PatternStringBuilder.Build(paletteString, pixelString);
                var byteOutput = StringToByteArray(output);

                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(byteOutput, QRCodeGenerator.ECCLevel.M);
                var qrCode = new QRCode(qrCodeData);
                var qrCodeImage = qrCode.GetGraphic(4);
                qrCodes.Add(qrCodeImage);
            }

            return qrCodes;
        }

        public Bitmap GenerateMontage()
        {
            var qrCodes = GenerateQrCodes();
            var first = qrCodes.First();
            
            var montage = new Bitmap(_width * first.Width, _height * first.Height);
            var (x, y) = (0, 0);
            using (var canvas = Graphics.FromImage(montage))
            {
                foreach (var qrCode in GenerateQrCodes())
                {
                    if (x < _width)
                    {
                        canvas.DrawImage(qrCode, x * first.Width, y * first.Height);
                        x += 1;
                    }
                    else
                    {
                        x = 0;
                        y++;
                        canvas.DrawImage(qrCode, x * first.Width, y * first.Height);
                        x += 1;
                    }
                }

                canvas.Save();
            }

            return montage;
        }

        private static (string paletteString, Dictionary<string, int> paletteIndex) GeneratePaletteInfo(
            IMagickImage image)
        {
            var matchedColors = image.Histogram();

            var paletteString = string.Empty;
            var paletteIndex = new Dictionary<string, int>();
            var position = 0;
            foreach (var matchedColor in matchedColors)
            {
                var inGameValue = ColorStrings.Dictionary[matchedColor.Key.ToString()];
                paletteString += inGameValue;
                paletteIndex[matchedColor.Key.ToString()] = position;
                position++;
            }

            paletteString = paletteString.PadRight(30, '0');
            return (paletteString, paletteIndex);
        }

        private static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }

        private static IEnumerable<MagickColor> GenerateColorProfileImage()
        {
            return ColorStrings.Dictionary.Select(colorString => new MagickColor(colorString.Key));
        }
    }
}