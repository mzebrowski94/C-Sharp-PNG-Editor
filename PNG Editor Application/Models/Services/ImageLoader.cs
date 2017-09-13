using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PNG_Editor_Application.Models.ImageData;
using System.IO;
using PNG_Editor_Application.Models.ImageData.PNGData;

namespace PNG_Editor_Application.Models.Services
{
    public class ImageLoader
    {
        private readonly ImageDecoder imageDecoder;
        private readonly BitmapConverter bitmapConverter;

        public ImageLoader(ImageDecoder imageDecoder, BitmapConverter bitmapConverter)
        {
            this.imageDecoder = imageDecoder;
            this.bitmapConverter = bitmapConverter;
        }

        public PNGImage LoadImage(string filePath, string safeFileName)
        {
            byte[] arrayByte = File.ReadAllBytes(filePath);
            DecodedPNGData decodedData = imageDecoder.DecodeByteArray(arrayByte);

        }

    }
}
