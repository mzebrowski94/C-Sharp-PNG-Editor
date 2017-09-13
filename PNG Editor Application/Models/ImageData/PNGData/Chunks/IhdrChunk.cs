using PNG_Editor_Application.Models.ImageData.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNG_Editor_Application.Models.ImageData.PNGData.Chunks
{
    public class IhdrChunk
    {
        private int width;
        private int height;
        private E_BitDepth bitDepth;
        private E_ColorType colorType;
        private int compressionMetod;
        private int filter;
        private E_Interlace interlace;

        public int Width { get => width; set => width = value; }
        public int Height { get => height; set => height = value; }
        public E_BitDepth BitDepth { get => bitDepth; set => bitDepth = value; }
        public E_ColorType ColorType { get => colorType; set => colorType = value; }
        public int CompressionMetod { get => compressionMetod; set => compressionMetod = value; }
        public int Filter { get => filter; set => filter = value; }
        public E_Interlace Interlace { get => interlace; set => interlace = value; }

        public IhdrChunk(int width, int height, E_BitDepth bitDepth, E_ColorType colorType, int compressionMetod, int filter, E_Interlace interlace)
        {
            this.width = width;
            this.height = height;
            this.bitDepth = bitDepth;
            this.colorType = colorType;
            this.compressionMetod = compressionMetod;
            this.filter = filter;
            this.interlace = interlace;
        }
    }



}
