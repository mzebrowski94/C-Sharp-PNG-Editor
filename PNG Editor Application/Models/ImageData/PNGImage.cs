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
    abstract class PNGImage : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Bitmap imageBitmap;

        private string printBytesToString(byte[] byteArray, int charsInLine)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i < byteArray.Length + 1; i++)
            {
                int b = byteArray[i - 1];
                sb.Append(b);
                //sb.Append(" ");

                if (i % charsInLine == 0)
                {
                    sb.Append("\n");
                }
                else
                {
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }
    }
    
}
