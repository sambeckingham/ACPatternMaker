using System;
using System.Drawing.Imaging;
using System.IO;
using ACQRGenerator;
using ACQRGenerator.Interfaces;
using Microsoft.Extensions.DependencyInjection;

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

            var serviceProvider = new ServiceCollection()
                .AddSingleton<IImageProcessor, ImageProcessor>()
                .BuildServiceProvider();

            var path = Path.GetFullPath(args[0]);

            var imageProcessor = serviceProvider.GetService<IImageProcessor>();

            imageProcessor.SetImage(path);

            var qrCodes = imageProcessor.GenerateQrCodes();

            var index = 1;
            foreach (var qrCode in qrCodes) qrCode.Save($"./tile_{index++}_qr.png", ImageFormat.Png);
        }
    }
}