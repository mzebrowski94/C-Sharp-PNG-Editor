using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNG_Editor_Application.Models.ImageData.PNGData.Chunks
{
    public class PlteChunk
    {
        private Boolean isExist;
        private int red;
        private int green;
        private int blue;

        public bool IsExist { get => isExist; set => isExist = value; }
        public int Red { get => red; set => red = value; }
        public int Green { get => green; set => green = value; }
        public int Blue { get => blue; set => blue = value; }

        public PlteChunk(bool isExist, int red, int green, int blue)
        {
            this.isExist = isExist;
            this.red = red;
            this.green = green;
            this.blue = blue;
        }
    }
}
