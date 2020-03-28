using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace ACQRGenerator.Interfaces
{
    public interface IImageProcessor
    {
        void SetImage(Stream path, int width = 4, int height = 4);

        IEnumerable<Bitmap> GenerateQrCodes();

        Bitmap GenerateMontage();
    }
}