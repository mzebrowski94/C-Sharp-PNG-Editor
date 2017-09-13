using PNG_Editor_Application.Models.ImageData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNG_Editor_Application.ViewModels.TopPanel
{
    class ImageInformationsViewModel
    {
        PNGImage loadedImage;

        private string _imageName;
        private string _imageSize;
        private string _imageDepth;
        private string _imageColorType;
        private string _imageInterlace;

        public string ImageName { get { return _imageName; } }
        public string ImageSize { get { return _imageSize; } }
        public string ImageBitDepth { get { return _imageDepth; } }
        public string ImageColorType { get { return _imageColorType; } }
        public string ImageInterlace { get { return _imageInterlace; } }


        public ImageInformationsViewModel()
        {
            resetImageInfo();
        }

        private void resetImageInfo()
        {
            _imageName = "-";
            _imageSize = "-";
            _imageDepth = "-";
            _imageColorType = "-";
            _imageInterlace = "-";
        }

        public void imageLoaded()
        {
            _imageName = loadedImage.ImageName;
            _imageSize = loadedImage.ImageSize.ToString();
            _imageDepth = loadedImage.ToString();
            _imageColorType = loadedImage.ToString();
            _imageInterlace = loadedImage.ToString();
        }
    }
}
