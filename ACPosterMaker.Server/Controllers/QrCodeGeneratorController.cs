using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using ACQRGenerator.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ACPosterMaker.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QrCodeGeneratorController : ControllerBase
    {
        private readonly ILogger<QrCodeGeneratorController> _logger;
        private readonly IImageProcessor _imageProcessor;

        public QrCodeGeneratorController(ILogger<QrCodeGeneratorController> logger, IImageProcessor imageProcessor)
        {
            _logger = logger;
            _imageProcessor = imageProcessor;
        }

        [HttpPost]
        public async Task<FileContentResult> Post()
        {
            using var reader = new StreamReader(Request.Body);
            var requestBody = await reader.ReadToEndAsync();
            reader.Close();

            var imageBase64 = RemoveDataUrlPrefix(requestBody);
            var imageBytes = Convert.FromBase64String(imageBase64);
            
            await using var ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            _imageProcessor.SetImage(ms);
            var qrCodeMontage = _imageProcessor.GenerateMontage();
            var bytes = ImageToByteArray(qrCodeMontage);

            return File(bytes, "image/png");
        }

        private static string RemoveDataUrlPrefix(string imageBase64)
        {
            return imageBase64.Substring(23);
        }

        public byte[] ImageToByteArray(Bitmap imageIn)
        {
            var ms = new MemoryStream();
            imageIn.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }
    }
}