﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using ImageMagick;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ACPosterMaker.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QrCodeGeneratorController : ControllerBase
    {
        private readonly IImageProcessor _imageProcessor;

        public QrCodeGeneratorController(IImageProcessor imageProcessor)
        {
            _imageProcessor = imageProcessor;
        }

        [HttpPost]
        public async Task<FileContentResult> Post()
        {
            using var reader = new StreamReader(Request.Body);
            var requestBody = await reader.ReadToEndAsync();
            reader.Close();

            if (requestBody.Length < 23)
            {
                return new FileContentResult(new byte[]{}, "text/plain");
            }
            
            var imageBase64 = RemoveDataUrlPrefix(requestBody);
            var imageBytes = Convert.FromBase64String(imageBase64);
            
            await using var ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            _imageProcessor.SetImage(ms);
            var qrCodeMontage = _imageProcessor.GenerateMontage();

            return File(qrCodeMontage.ToByteArray(MagickFormat.Png), "image/png");
        }

        private static string RemoveDataUrlPrefix(string imageBase64)
        {
            return imageBase64.Substring(23);
        }
    }
}