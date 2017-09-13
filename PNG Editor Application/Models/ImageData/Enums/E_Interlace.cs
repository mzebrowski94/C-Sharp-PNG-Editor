using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNG_Editor_Application.Models.ImageData.Enums
{
    public enum E_Interlace
    {
        [Description("No data")]
        NO_DATA,
        [Description("No interlace")]
        NO_INTERLACE,
        [Description("Adam7")]
        ADAM7
    }
}
