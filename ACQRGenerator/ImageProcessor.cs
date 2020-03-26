using System;
using System.Collections.Generic;
using System.Drawing;
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


        public void SetImage(string path)
        {
            _image = new MagickImage(path);
        }

        public IEnumerable<Bitmap> GenerateQrCodes()
        {
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
                var qrCodeImage = qrCode.GetGraphic(7);
                qrCodes.Add(qrCodeImage);
            }

            return qrCodes;
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