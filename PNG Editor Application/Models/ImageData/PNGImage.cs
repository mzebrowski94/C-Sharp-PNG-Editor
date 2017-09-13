using PNG_Editor_Application.Models.ImageData.Enums;
using PNG_Editor_Application.Models.ImageData.PNGData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PNG_Editor_Application.Models.ImageData
{
    /// <summary>
    /// PNG Image model abstract class.
    /// </summary>
    public class PNGImage : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly Bitmap imageBitmap;
        private readonly byte[] dataStream;
        public DecodedPNGData DecodedPNGData { get; }
        private Boolean isCalculated;
        private long[][] colorDataValues;

        private string _imageName;
        public string ImageName
        {
            get { return _imageName; }
            set { _imageName = value; }
        }

        private Size _imageSize;
        public Size ImageSize
        {
            get { return _imageSize; }
            set { _imageSize = value; }
        }

        private E_BitDepth _imageBitDepth;
        public E_BitDepth ImageBitDepth
        {
            get { return _imageBitDepth; }
            set { _imageBitDepth = value; }
        }

        private E_ColorType _imageColorType;
        public E_ColorType ImageColorType
        {
            get { return _imageColorType; }
            set { _imageColorType = value; }
        }

        private E_Interlace _imageInterlace;
        public E_Interlace ImageInterlace
        {
            get { return _imageInterlace; }
            set { _imageInterlace = value; }
        }

        public bool IsCalculated { get => isCalculated; private set => isCalculated = value; }
        public long[][] ColorDataValues { get => colorDataValues; private set => colorDataValues = value; }

        public PNGImage(Bitmap bitmap, DecodedPNGData decodedPNGData)
        {
            DecodedPNGData = decodedPNGData;
            this.imageBitmap = bitmap;
        }

        public PNGImage(Bitmap bitmap)
        {
            this.imageBitmap = bitmap;
        }

    }
    
}
