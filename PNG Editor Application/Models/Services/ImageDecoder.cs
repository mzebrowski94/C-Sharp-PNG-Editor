using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PNG_Editor_Application.Models.ImageData;
using PNG_Editor_Application.Models.ImageData.PNGData;
using PNG_Editor_Application.Models.ImageData.PNGData.Chunks;
using System.IO;
using System.IO.Compression;
using PNG_Editor_Application.Models.ImageData.Enums;

namespace PNG_Editor_Application.Models.Services
{
    public class ImageDecoder
    {
        public ImageDecoder()
        {

        }

        internal DecodedPNGData DecodeByteArray(byte[] arrayByte)
        {

            IdatChunk idatChunk = ReadIdatChunk(arrayByte);
            PlteChunk plteChunk = ReadPlteChunk(arrayByte);
            IhdrChunk ihdrChunk = ReadIhdrChunk(arrayByte);

            int scanlineLenght = CalculateScanlineLenght(ihdrChunk);
            byte[] unfilteredIdatDatastream = UnfilterImageDatastream(scanlineLenght, idatChunk, ihdrChunk);

            string decodedBytes = PrintBytesToString(arrayByte, 8);
            string decodedAscii = PrintBytesToAsciiString(arrayByte);

            string decodedIdat = PrintBytesToString(idatChunk.DatastreamBytes, scanlineLenght);
            string unfilteredDatastreamString = PrintBytesToString(unfilteredIdatDatastream, scanlineLenght - 1);
            string filteredDatastreamString = PrintBytesToString(idatChunk.DatastreamBytes, scanlineLenght);

            return new DecodedPNGData(idatChunk, ihdrChunk, plteChunk, decodedIdat, unfilteredDatastreamString, filteredDatastreamString, unfilteredIdatDatastream, scanlineLenght);
        }

        private IdatChunk ReadIdatChunk(byte[] loadedDatastream)
        {
            byte[] datastreamBytes;
            MemoryStream datastream;

            byte[] idatPattern = new byte[] { 73, 68, 65, 84 };
            byte[] iendPattern = new byte[] { 73, 68, 78, 68 };
            int idatAmount = 0;
            List<MemoryStream> memoryStreamList = new List<MemoryStream>();

            for (int idatIndex = FindChunkPosition(loadedDatastream, idatPattern); idatIndex != -1; idatIndex = FindChunkPosition(loadedDatastream, idatPattern, idatIndex))
            {
                idatAmount++;

                //Calculates when idat lenght field begins
                int idatLenghtIndex = idatIndex - 4;

                //Gets the idat lenght
                byte[] idatLenghtByteArray = CutFromDatastream(loadedDatastream, idatLenghtIndex, 4);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(idatLenghtByteArray);
                int chunkLenght = BitConverter.ToInt32(idatLenghtByteArray, 0);
                //Calculates when idat datastream begins
                int idatDataIndex = idatIndex + idatPattern.Length;

                byte[] datastreamIdat = CutFromDatastream(loadedDatastream, idatDataIndex, chunkLenght);

                MemoryStream idatChunkDataStream = new MemoryStream();
                idatChunkDataStream.Write(datastreamIdat, 0, datastreamIdat.Length);
                memoryStreamList.Add(idatChunkDataStream);
                idatIndex = idatDataIndex + chunkLenght;
            }

            int idatDataStreamLenght = 0;
            for (int i = 0; i < memoryStreamList.Count; i++)
                idatDataStreamLenght += (int)memoryStreamList[i].Length;

            using (MemoryStream idatDataStream = new MemoryStream(idatDataStreamLenght))
            {
                for (int i = 0; i < memoryStreamList.Count; i++)
                {
                    memoryStreamList[i].Position = 0;
                    memoryStreamList[i].CopyTo(idatDataStream);
                    memoryStreamList[i].Dispose();
                }

                //using (Stream input = new DeflateStream(new MemoryStream(datastreamIdat), CompressionMode.Decompress))
                idatDataStream.Position = 2;
                using (Stream input = new DeflateStream(idatDataStream, CompressionMode.Decompress))
                {
                    using (MemoryStream resultStream = new MemoryStream())
                    {
                        input.CopyTo(resultStream);
                        datastream = resultStream;
                        datastreamBytes = resultStream.GetBuffer();
                    }
                }
            }

            return new IdatChunk(datastreamBytes, datastream);
        }

        private IhdrChunk ReadIhdrChunk(byte[] loadedDatastream)
        {
            int width;
            int height;
            E_BitDepth bitDepth = E_BitDepth.NO_DATA;
            E_ColorType colorType = E_ColorType.NO_DATA;
            int compressionMetod;
            int filter;
            E_Interlace interlace = E_Interlace.NO_DATA;

            byte[] ihdrPattern = new byte[] { 73, 72, 68, 82 };
            int index = FindChunkPosition(loadedDatastream, ihdrPattern) + ihdrPattern.Length;

            width = calculateBytesArray(loadedDatastream, index, 4);
            height = calculateBytesArray(loadedDatastream, index + 4, 4);
            switch (calculateBytesArray(loadedDatastream, index + 8, 1))
            {
                case 1: bitDepth = E_BitDepth.One; break;
                case 2: bitDepth = E_BitDepth.Two; break;
                case 4: bitDepth = E_BitDepth.Four; break;
                case 8: bitDepth = E_BitDepth.Eight; break;
                case 16: bitDepth = E_BitDepth.Sixteen; break;
            }
            switch (calculateBytesArray(loadedDatastream, index + 9, 1))
            {
                case 0: colorType = E_ColorType.GRAYSCALE; break;
                case 2: colorType = E_ColorType.TRUECOLOR; break;
                case 3: colorType = E_ColorType.INDEXED; break;
                case 4: colorType = E_ColorType.GRAYSCALE_ALPHA; break;
                case 6: colorType = E_ColorType.TRUECOLOR_ALPHA; break;
            }
            compressionMetod = calculateBytesArray(loadedDatastream, index + 10, 1);
            filter = calculateBytesArray(loadedDatastream, index + 11, 1);
            switch (calculateBytesArray(loadedDatastream, index + 12, 1))
            {
                case 0: interlace = E_Interlace.NO_INTERLACE; break;
                case 1: interlace = E_Interlace.ADAM7; break;
            }

            return new IhdrChunk(width, height, bitDepth, colorType, compressionMetod, filter, interlace);
        }

        private PlteChunk ReadPlteChunk(byte[] loadedDatastream)
        {
            Boolean isExist = false;
            int red = 0;
            int green = 0;
            int blue = 0;

            byte[] pltePattern = new byte[] { 80, 76, 84, 69 };
            int preIndex = FindChunkPosition(loadedDatastream, pltePattern);
            if (preIndex != -1)
            {
                isExist = true;
                int index = preIndex + pltePattern.Length;

                red = calculateBytesArray(loadedDatastream, index, 1);
                green = calculateBytesArray(loadedDatastream, index + 1, 1);
                blue = calculateBytesArray(loadedDatastream, index + 1, 1);
            }
            return new PlteChunk(isExist, red, green, blue);
        }

        private byte[] CutFromDatastream(byte[] datastream, int startIndex, int lenght)
        {
            byte[] newBytesArray = new byte[lenght];

            if (startIndex + lenght < datastream.Length)
            {
                for (int i = startIndex, j = 0; j < lenght; i++, j++)
                {
                    newBytesArray[j] = (byte)datastream.GetValue(i);
                }
            }

            return newBytesArray;
        }

        private int CalculateScanlineLenght(IhdrChunk ihdrChunk)
        {
            int scanlineLenght = ihdrChunk.Width;

            if (ihdrChunk.ColorType == E_ColorType.GRAYSCALE)
                scanlineLenght *= 1;
            else if (ihdrChunk.ColorType == E_ColorType.GRAYSCALE_ALPHA)
                scanlineLenght *= 2;
            else if (ihdrChunk.ColorType == E_ColorType.TRUECOLOR)
                scanlineLenght *= 3;
            else if (ihdrChunk.ColorType == E_ColorType.TRUECOLOR_ALPHA)
                scanlineLenght *= 4;
            else
                scanlineLenght *= 1;

            scanlineLenght = scanlineLenght + 1;  //Adding filter byte

            return scanlineLenght;
        }

        private byte[] UnfilterImageDatastream(int scanlineLenght, IdatChunk idatChunk, IhdrChunk ihdrChunk)
        {
            //int datastreamLenght = chunkIhdr.height * chunkIhdr.width * scanlineLenght;
            byte[] datastreamIdat = idatChunk.DatastreamBytes;
            int datastreamLenght = datastreamIdat.Length;
            // Datastream after unfitering = size - amount of filter bytes
            int amountOfScanLines = ihdrChunk.Height;
            byte[] refilteredDatastream = new byte[datastreamLenght];

            // Lenght of the scanLine without filter byte
            int scanLineLenWithoutFilter = scanlineLenght - 1;

            // pixel size (in bytes)
            int pixelbytesLenght = 4;

            E_ColorType colorType = ihdrChunk.ColorType;
            if (colorType == E_ColorType.GRAYSCALE)
                pixelbytesLenght = 1;
            else if (colorType == E_ColorType.GRAYSCALE_ALPHA)
                pixelbytesLenght = 2;
            else if (colorType == E_ColorType.TRUECOLOR)
                pixelbytesLenght = 3;
            else if (colorType == E_ColorType.TRUECOLOR_ALPHA)
                pixelbytesLenght = 4;

            //Actual used scanLine and unfilteredScanLine
            byte[] tempScanline; // Actual filtered scanline
            byte[] previousReconScanline;  // Previous scanline which was reconstructed;
            byte[] unfilteredScanline = new byte[scanlineLenght - 1];

            for (int datastreamScanlineIter = 0; datastreamScanlineIter < amountOfScanLines; datastreamScanlineIter++)
            {

                int datastreamIter = (datastreamScanlineIter * (scanlineLenght));
                // Each iteration is on another scanline
                tempScanline = CutFromDatastream(datastreamIdat, datastreamIter, scanlineLenght);

                //Checking tempScanine FILTER TYPE:
                if (tempScanline[0] == 0)
                {
                    //  Console.WriteLine("--- Filter method 0: Non ---");
                    //NOTHING TO DO - FILTER 0
                    for (int scanLineIter = 1; scanLineIter < tempScanline.Length; scanLineIter++)
                    {
                        unfilteredScanline[scanLineIter - 1] = tempScanline[scanLineIter];
                    }
                }
                else if (tempScanline[0] == 1)
                {
                    //   Console.WriteLine("--- Filter method 1: Sub ---");

                    //Actual and previous used pixel (in bytes)
                    byte[] tempPixel = new byte[pixelbytesLenght];
                    byte[] reconAPixel = new byte[pixelbytesLenght]; // recon(a) - pixel on the left to actual pixel (tempPiexl)
                    byte[] reconXPixel = new byte[pixelbytesLenght]; // recon(x) - reconstructed actual pixel;

                    // Pixel on the left from the first pixel in scanline stores 0 values
                    for (int k = 0; k < pixelbytesLenght; k++)
                        reconAPixel[k] = 0;

                    for (int scanLineIter = 1; scanLineIter < tempScanline.Length; scanLineIter += pixelbytesLenght)    // scanLineIter=1 becouse of filter byte
                    {
                        //Update pixel data
                        for (int k = 0; k < pixelbytesLenght; k++)
                            tempPixel[k] = tempScanline[scanLineIter + k];

                        for (int pixelIter = 0; pixelIter < pixelbytesLenght; pixelIter++)
                        {
                            reconXPixel[pixelIter] = (byte)(reconAPixel[pixelIter] + tempPixel[pixelIter]);
                            unfilteredScanline[scanLineIter + pixelIter - 1] = reconXPixel[pixelIter];
                            reconAPixel[pixelIter] = reconXPixel[pixelIter];
                        }
                    }

                }
                else if (tempScanline[0] == 2)
                {
                    //   Console.WriteLine("--- Filter method 2: Up ---");

                    //Actual and previous used pixel (in bytes)
                    byte[] tempPixel = new byte[pixelbytesLenght];
                    byte[] reconBPixel = new byte[pixelbytesLenght]; // recon(b) - pixel above actual pixel (tempPiexl)


                    if (datastreamScanlineIter == 0)
                        previousReconScanline = new byte[scanlineLenght];
                    else
                        previousReconScanline = CutFromDatastream(refilteredDatastream, (datastreamIter - datastreamScanlineIter) - (scanlineLenght - 1), scanlineLenght - 1);

                    for (int scanLineIter = 1; scanLineIter < tempScanline.Length; scanLineIter += pixelbytesLenght)    // scanLineIter=1 becouse of filter byte
                    {
                        //Update pixel data
                        for (int k = 0; k < pixelbytesLenght; k++)
                            tempPixel[k] = tempScanline[scanLineIter + k];
                        for (int k = 0; k < pixelbytesLenght; k++)
                            reconBPixel[k] = previousReconScanline[scanLineIter - 1 + k];

                        for (int pixelIter = 0; pixelIter < pixelbytesLenght; pixelIter++)
                            unfilteredScanline[scanLineIter + pixelIter - 1] = (byte)(reconBPixel[pixelIter] + tempPixel[pixelIter]);
                    }

                }
                else if (tempScanline[0] == 3)
                {
                    //   Console.WriteLine("--- Filter method 3: Average ---");


                    byte[] tempPixel = new byte[pixelbytesLenght];   //Actual and previous used pixel (in bytes)
                    byte[] reconBPixel = new byte[pixelbytesLenght]; // recon(b) - pixel above actual pixel (tempPiexl)
                    byte[] reconAPixel = new byte[pixelbytesLenght]; // recon(a) - pixel on the left to actual pixel (tempPiexl)
                    byte[] reconXPixel = new byte[pixelbytesLenght]; // recon(x) - reconstructed actual pixel;

                    if (datastreamScanlineIter == 0)
                        previousReconScanline = new byte[scanlineLenght]; // stores 0 if there is no previously reconstructed scanline
                    else
                        previousReconScanline = CutFromDatastream(refilteredDatastream, (datastreamIter - datastreamScanlineIter) - (scanlineLenght - 1), scanlineLenght - 1);

                    // Pixel on the left from the first pixel in scanline stores 0 values
                    for (int k = 0; k < pixelbytesLenght; k++)
                        reconAPixel[k] = 0;

                    for (int scanLineIter = 1; scanLineIter < tempScanline.Length; scanLineIter += pixelbytesLenght)    // scanLineIter=1 becouse of filter byte
                    {
                        //Update pixel data
                        for (int k = 0; k < pixelbytesLenght; k++)
                            tempPixel[k] = tempScanline[scanLineIter + k];
                        for (int k = 0; k < pixelbytesLenght; k++)
                            reconBPixel[k] = previousReconScanline[scanLineIter - 1 + k];

                        for (int pixelIter = 0; pixelIter < pixelbytesLenght; pixelIter++)
                        {
                            reconXPixel[pixelIter] = (byte)(tempPixel[pixelIter] + (reconBPixel[pixelIter] + reconAPixel[pixelIter]) / 2);
                            unfilteredScanline[scanLineIter + pixelIter - 1] = reconXPixel[pixelIter];
                            reconAPixel[pixelIter] = reconXPixel[pixelIter];
                        }
                    }
                }
                else if (tempScanline[0] == 4)
                {
                    //   Console.WriteLine("--- Filter method 4: PaethPredictor ---");

                    byte[] tempPixel = new byte[pixelbytesLenght];
                    byte[] reconAPixel = new byte[pixelbytesLenght]; // aPixel - pixel on the left to actual pixel 
                    byte[] reconAPixel2 = new byte[pixelbytesLenght];
                    byte[] reconBPixel = new byte[pixelbytesLenght]; // bPixel - pixel above actual pixel 
                    byte[] reconCPixel = new byte[pixelbytesLenght]; // cPixel - pixel on the left to bPixel pixel
                    byte[] reconXPixel = new byte[pixelbytesLenght]; // recon(x) - reconstructed actual pixel;
                    byte[] reconPPixel = new byte[pixelbytesLenght];

                    if (datastreamScanlineIter == 0)
                        previousReconScanline = new byte[scanlineLenght]; // stores 0 if there is no previously reconstructed scanline
                    else
                        previousReconScanline = CutFromDatastream(refilteredDatastream, (datastreamIter - datastreamScanlineIter) - (scanlineLenght - 1), scanlineLenght - 1);

                    for (int k = 0; k < pixelbytesLenght; k++)
                    {
                        // Pixel on the left from the first pixel in scanline stores 0 values
                        reconAPixel[k] = 0;
                        // Pixel on the above-left from the first pixel in scanline stores 0 values
                        reconCPixel[k] = 0;
                        // Pixel on the above from the first pixel in scanline stores 0 values
                        reconBPixel[k] = 0;
                    }

                    for (int scanLineIter = 1; scanLineIter < tempScanline.Length; scanLineIter += pixelbytesLenght)    // scanLineIter=1 becouse of filter byte
                    {
                        //Update pixel data
                        if (datastreamScanlineIter != 0)
                        {
                            for (int k = 0; k < pixelbytesLenght; k++)
                                reconBPixel[k] = previousReconScanline[scanLineIter - 1 + k];
                        }

                        if (scanLineIter != 1)
                        {
                            for (int k = 0; k < pixelbytesLenght; k++)
                                reconCPixel[k] = previousReconScanline[scanLineIter - 1 - pixelbytesLenght + k];
                        }

                        for (int k = 0; k < pixelbytesLenght; k++)
                        {
                            tempPixel[k] = tempScanline[scanLineIter + k];
                            reconAPixel2[k] = reconAPixel[k];
                            reconXPixel[k] = 0;
                            reconPPixel[k] = 0;
                        }

                        for (int pixelIter = 0; pixelIter < pixelbytesLenght; pixelIter++)
                        {
                            int p, pa, pb, pc = 0;

                            p = (reconAPixel2[pixelIter] + reconBPixel[pixelIter] - reconCPixel[pixelIter]);
                            pa = (Math.Abs(p - reconAPixel[pixelIter]));
                            pb = (Math.Abs(p - reconBPixel[pixelIter]));
                            pc = (Math.Abs(p - reconCPixel[pixelIter]));

                            if (pa <= pb && pa <= pc)
                                reconPPixel[pixelIter] = reconAPixel2[pixelIter];
                            else if (pb <= pc)
                                reconPPixel[pixelIter] = reconBPixel[pixelIter];
                            else
                                reconPPixel[pixelIter] = reconCPixel[pixelIter];

                            int x = (tempPixel[pixelIter] + reconPPixel[pixelIter]);
                            reconXPixel[pixelIter] = (byte)(tempPixel[pixelIter] + reconPPixel[pixelIter]);
                            unfilteredScanline[scanLineIter + pixelIter - 1] = reconXPixel[pixelIter];
                            reconAPixel[pixelIter] = reconXPixel[pixelIter];

                        }
                    }

                }
                else
                {
                    Console.WriteLine(" WRONG METHOD CODE !!!:  " + tempScanline[0] + "  Scanline: " + datastreamScanlineIter);
                    //NOTHING TO DO - FILTER 0
                    //for (int scanLineIter = 1; scanLineIter < tempScanline.Length; scanLineIter++)
                    //{
                    //    unfilteredScanline[scanLineIter - 1] = tempScanline[scanLineIter];
                    //}
                }

                // Fill refilteredDatastream with refiltered scanLine
                int refilteredDatastreamIter = datastreamScanlineIter * (scanlineLenght - 1);
                for (int k = 0; k < scanlineLenght - 1; k++) // k=1 becouse of filter byte 
                {
                    refilteredDatastream[refilteredDatastreamIter + k] = unfilteredScanline[k];
                }

            }
            return refilteredDatastream;
        }

        private int calculateBytesArray(byte[] data, int index, int length)
        {
            byte[] newBytesArray;

            if (length > 3)
            {
                newBytesArray = new byte[length];
                Array.Copy(data, index, newBytesArray, 0, length);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(newBytesArray);
            }
            else
            {
                newBytesArray = new byte[4];
                Array.Copy(data, index, newBytesArray, 0, length);
            }

            return BitConverter.ToInt32(newBytesArray, 0);
        }

        private int FindChunkPosition(byte[] byteArray, byte[] candidate)
        {
            if (byteArray == null
                || byteArray == null
                || byteArray.Length == 0
                || candidate.Length == 0
                || candidate.Length > byteArray.Length)
                return -1;

            for (int i = 0; i < byteArray.Length; i++)
            {
                if (!IsMatch(byteArray, i, candidate))
                    continue;

                return i;
            }

            return -1;
        }

        private int FindChunkPosition(byte[] byteArray, byte[] candidate, int searchFromIndex)
        {
            if (byteArray == null
                || candidate == null
                || byteArray.Length == 0
                || candidate.Length == 0
                || candidate.Length > byteArray.Length
                || searchFromIndex > byteArray.Length)
                return -1;

            for (int i = searchFromIndex; i < byteArray.Length; i++)
            {
                if (!IsMatch(byteArray, i, candidate))
                    continue;

                return i;
            }
            return -1;
        }

        private bool IsMatch(byte[] array, int position, byte[] candidate)
        {
            if (candidate.Length > (array.Length - position))
                return false;

            for (int i = 0; i < candidate.Length; i++)
                if (array[position + i] != candidate[i])
                    return false;

            return true;
        }

        private string PrintBytesToString(byte[] byteArray, int charsInLine)
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

        private string PrintBytesToAsciiString(byte[] arrayBytes)
        {
            return System.Text.Encoding.ASCII.GetString(arrayBytes);
        }
    }
}
