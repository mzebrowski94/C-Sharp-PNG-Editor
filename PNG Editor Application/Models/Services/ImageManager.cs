using PNG_Editor_Application.Models.ImageData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNG_Editor_Application.Models.Services
{
    public class ImageManager
    {
        private PNGImage loadedImage, loadedAdditionalImage;
        public PNGImage LoadedImage
        {
            get { return loadedImage;}
        }
        public PNGImage LoadedAdditionalImage
        {
            get{ return loadedAdditionalImage;}
        }

        private readonly ImageEditor imageEditor;
        private readonly ImageLoader imageLoader;

        public ImageManager(ImageEditor imageEditor, ImageLoader imageLoader)
        {
            this.imageEditor = imageEditor;
            this.imageLoader = imageLoader;
        }

        internal void loadImageFile(string filePath, string safeFileName)
        {
            loadedImage = imageLoader.LoadImage(filePath, safeFileName);
        }
    }

}
