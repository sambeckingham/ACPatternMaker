﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImageMagick;
using QRCoder;

namespace ACPosterMaker.Server
{
    public class ImageProcessor : IImageProcessor
    {
        private MagickImage _image;
        private int _width;
        private int _height;


        public void SetImage(Stream stream, int width = 4, int height = 4)
        {
            if (_image != null)
            {
                _image.Dispose();
            }

            _image = new MagickImage(stream);
            _width = width;
            _height = height;
        }

        public MagickImageCollection GenerateQrCodes()
        {
            // TODO: Shape to the image ratio
            var geometry = new MagickGeometry(128, 128) {FillArea = true};
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
                return new MagickImageCollection();
            }

            var (paletteString, paletteIndex) = GeneratePaletteInfo(_image);

            var qrCodes = new MagickImageCollection();
            foreach (var tile in _image.CropToTiles(32, 32))
            {
                var leftSide = new List<string>();
                var rightSide = new List<string>();

                using (var pixels = tile.GetPixels())
                {
                    foreach (var pixel in pixels)
                    {
                        var x = paletteIndex[pixel.ToColor().ToString()].ToString("X");
                        if (pixel.X % 2 == 0)
                            rightSide.Add(x);
                        else
                            leftSide.Add(x);
                    }
                }

                var zipped = leftSide.Zip(rightSide, (l, r) => $"{l}{r}").ToArray();

                var pixelString = string.Join("", zipped);

                var output = PatternStringBuilder.Build(paletteString, pixelString);
                var byteOutput = StringToByteArray(output);

                // var qrGenerator = new QRCodeGenerator();
                // var qrCodeData = qrGenerator.CreateQrCode(byteOutput, QRCodeGenerator.ECCLevel.M);
                // var qrCode = new QRCode(qrCodeData);
                // var qrCodeImage = qrCode.GetGraphic(4);
                // qrCodes.Add(qrCodeImage);


                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(byteOutput, QRCodeGenerator.ECCLevel.M);
                var qrCode = new BitmapByteQRCode(qrCodeData);
                var qrCodeImage = qrCode.GetGraphic(4);
                using (var ms = new MemoryStream(qrCodeImage, 0, qrCodeImage.Length))
                {
                    qrCodes.Add(new MagickImage(ms));
                };
            }

            return qrCodes;
        }

        public MagickImage GenerateMontage()
        {
            var qrCodes = GenerateQrCodes();
            var first = qrCodes.First();

            var montageSettings = new MontageSettings()
            {
                BackgroundColor = MagickColors.None, // -background none
                Geometry = new MagickGeometry(1,1,0,0) // -geometry +5+5
            };

            var mont = qrCodes.Montage(montageSettings);

            return (MagickImage) mont;
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

    public class PatternStringBuilder
    {
        private const string FrontMatter =
            "410043005000610074007400650072006E004D0061006B00650072000000000000000000000000000000B6EC530061006D00000000000000000000000000000044C547006900740068007500620000000000000000001931";

        public static string Build(string paletteString, string pixelString)
        {
            var completeString = FrontMatter + paletteString + "CC0A090000" + pixelString;
            return completeString;
        }
    }
}