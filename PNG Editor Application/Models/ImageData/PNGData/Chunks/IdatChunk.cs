using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNG_Editor_Application.Models.ImageData.PNGData.Chunks
{
    public class IdatChunk
    {
        private byte[] datastreamBytes;
        private MemoryStream datastream;

        public byte[] DatastreamBytes { get => datastreamBytes; set => datastreamBytes = value; }
        public MemoryStream Datastream { get => datastream; set => datastream = value; }

        public IdatChunk(byte[] datastreamBytes, MemoryStream datastream)
        {
            this.datastreamBytes = datastreamBytes;
            this.datastream = datastream;
        }
    }
}
