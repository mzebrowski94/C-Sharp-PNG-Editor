using PNG_Editor_Application.Models.ImageData.PNGData.Chunks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNG_Editor_Application.Models.ImageData.PNGData
{
    public class DecodedPNGData
    {
        public IdatChunk IdatChunk { get; set; }
        public IhdrChunk IhdrChunk { get; set; }
        public PlteChunk PlteChunk { get; set; }
        public string DecodedIdat { get; set; }
        public string UnfilteredDatastreamString { get; set; }
        public string FilteredDatastreamString { get; set; }
        public byte[] UnfilteredIdatDatastream { get; set; }
        public int ScanlineLenght { get; set; }

        public DecodedPNGData(IdatChunk idatChunk, IhdrChunk ihdrChunk, PlteChunk plteChunk, string decodedIdat, string unfilteredDatastreamString, string filteredDatastreamString, byte[] unfilteredIdatDatastream, int scanlineLenght)
        {
            IdatChunk = idatChunk;
            IhdrChunk = ihdrChunk;
            PlteChunk = plteChunk;
            DecodedIdat = decodedIdat;
            UnfilteredDatastreamString = unfilteredDatastreamString;
            FilteredDatastreamString = filteredDatastreamString;
            UnfilteredIdatDatastream = unfilteredIdatDatastream;
            ScanlineLenght = scanlineLenght;
        }

    }
}
