using PNG_Editor_Application.Models.ImageData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNG_Editor_Application.Models.Services
{
    class ImageManagerService
    {
        private LoadedPNG _loadedImage, _loadedAdditionalImage;
        public LoadedPNG LoadedImage
        {
            get
            {
                return _loadedImage;
            }
        }
        public LoadedPNG LoadedAdditionalImage
        {
            get
            {
                return _loadedAdditionalImage;
            }
        }

        private BitmapConverterService bitmapConverter;
        private ImageEditorService ImageEditor;
        private ImageDecompressorService ImageDecompressor;

        public ImageManagerService(ImageEditorService imageEditorService, ImageDecompressorService imageDecompressorService, BitmapConverterService bitmapConverterService)
        {
            this.bitmapConverter = bitmapConverterService;
            this.ImageEditor = imageEditorService;
            this.ImageDecompressor = imageDecompressorService;
        }


    }

}
