using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ImageMagick;

namespace ACPatternMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            IDictionary<string, string> layout = new Dictionary<string, string>()
            {
                {"Title", "00"},
                {"User ID", "2A"},
                {"User Name", "3F"},
                {"Town ID", "40"},
                {"Town Name", "42"},
                {"Palette", "58"},
                {"Pattern Type", "69"},
                {"Data", "6C"}
            };

            var file = File.ReadAllText("./test.acnl", Encoding.UTF8);
            Console.WriteLine($"\nRaw: {file}");

            var title = file.Substring(int.Parse(layout["Title"], NumberStyles.HexNumber), 42);
            var userId = file.Substring(int.Parse(layout["User ID"], NumberStyles.HexNumber), 2);
            var userName = file.Substring(int.Parse(layout["User Name"], NumberStyles.HexNumber), 20);
            var townId = file.Substring(int.Parse(layout["Town ID"], NumberStyles.HexNumber), 2);
            var townName = file.Substring(int.Parse(layout["Town Name"], NumberStyles.HexNumber), 20);

            Console.WriteLine($"Title: {title}");
            Console.WriteLine($"UserID: {userId}");
            Console.WriteLine($"User Name: {userName}");
            Console.WriteLine($"Town ID: {townId}");
            Console.WriteLine($"Town Name: {townName}");
            
            // Convert image to AC friendly

            using var image = new MagickImage("./bliss.png");

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

            // Build Palette

            var matchedColors = image.Histogram();

            var palleteString = string.Empty;
            foreach (var (key, _) in matchedColors)
            {
                var inGameValue = ColorStrings.Dictionary[key.ToString()];
                // Array.Reverse(inGameValue);
                // var reversedValue = string.Concat(inGameValue);
                palleteString += inGameValue;
            }

            var finalPalleteString = palleteString.PadRight(30, '0');

            Console.Write(finalPalleteString);

            // TODO: Tile and generate strings

            var index = 1;
            foreach (var tile in image.CropToTiles(32, 32))
            {
                // TODO: iterate over pixels, create string and complete file
            }

            // TODO: Produce QR Codes
        }

        private static IEnumerable<MagickColor> GenerateColorProfileImage()
        {
            return ColorStrings.Dictionary.Select(colorString => new MagickColor(colorString.Key));
        }
    }
}