using System.Collections.Generic;
using System.Drawing;

namespace ACQRGenerator.Interfaces
{
    public interface IImageProcessor
    {
        void SetImage(string path);

        IEnumerable<Bitmap> GenerateQrCodes();
    }
}