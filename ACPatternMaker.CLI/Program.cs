using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using ImageMagick;
using QRCoder;

namespace ACPatternMaker
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("AC Pattern Maker only expects one parameter, the path to your image file!");
                return;
            }

            var path = Path.GetFullPath(args[0]);
            var image = new MagickImage(path);

            // Crop and resize

            var geometry = new MagickGeometry(128, 128);
            geometry.FillArea = true;
            image.Resize(geometry);
            image.Crop(128, 128, Gravity.Center);
            image.RePage();

            image.Quantize(new QuantizeSettings {Colors = 15, DitherMethod = DitherMethod.FloydSteinberg});

            var colors = GenerateColorProfileImage();
            try
            {
                image.Map(colors, new QuantizeSettings {Colors = 15, DitherMethod = DitherMethod.No});
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                return;
            }

            var (paletteString, paletteIndex) = GeneratePaletteInfo(image);

            var index = 1;
            var builder = new PatternStringBuilder();
            foreach (var tile in image.CropToTiles(32, 32))
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
                qrCodeImage.Save($"./tile_{index++}_qr.png", ImageFormat.Png);
            }
        }

        private static (string paletteString, Dictionary<string, int> paletteIndex) GeneratePaletteInfo(
            MagickImage image)
        {
            var matchedColors = image.Histogram();

            var paletteString = string.Empty;
            var paletteIndex = new Dictionary<string, int>();
            var position = 0;
            foreach (var (key, _) in matchedColors)
            {
                var inGameValue = ColorStrings.Dictionary[key.ToString()];
                paletteString += inGameValue;
                paletteIndex[key.ToString()] = position;
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