using System.Collections.Generic;
using System.Drawing;
using System.IO;
using ImageMagick;

namespace ACPosterMaker.Server
{
    public interface IImageProcessor
    {
        void SetImage(Stream path, int width = 4, int height = 4);

        MagickImageCollection GenerateQrCodes();

        MagickImage GenerateMontage();
    }
}