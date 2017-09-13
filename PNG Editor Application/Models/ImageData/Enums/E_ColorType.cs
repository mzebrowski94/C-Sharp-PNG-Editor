using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNG_Editor_Application.Models.ImageData.Enums
{
    public enum E_ColorType
    {
        [Description("No data")]
        NO_DATA,
        [Description("Indexed")]
        INDEXED,
        [Description("Grayscale")]
        GRAYSCALE,
        [Description("Grayscale alpha")]
        GRAYSCALE_ALPHA,
        [Description("Truecolor")]
        TRUECOLOR,
        [Description("Truecolor alpha")]
        TRUECOLOR_ALPHA
    }
}
