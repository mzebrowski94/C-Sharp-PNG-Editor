using Microsoft.Win32;
using PNG_Editor_Application.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PNG_Editor_Application.ViewModels.Commands
{
    public class OpenCommand : ICommand
    {
        private ImageManager _imageManagerService;

        public event EventHandler CanExecuteChanged;

        public OpenCommand(ImageManager imageManagerService)
        {
            _imageManagerService = imageManagerService;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png)|*.png|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
                _imageManagerService.loadImageFile(openFileDialog.FileName, openFileDialog.SafeFileName);
        }
    }
}
