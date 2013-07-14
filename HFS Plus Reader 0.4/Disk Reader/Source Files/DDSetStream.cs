/*
 *  This file is part of HFS+ Reader.
 *
 *  HFS+ Reader is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  HFS+ Reader is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with HFS+ Reader.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Disk_Reader
{
    class DDSetStream : absImageStream
    {
        /*
         * Creates a stream representing a collection of ComponentStreams, which each read from 
         * a file which is a section of the overall image.
        */
        public override long Position
        {
            get
            {
                return this.position;
            }
            set
            {
                if (value <= this.Length && value >= 0)
                {
                    this.position = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("The image is smaller than the requested read position. This may indicate an incomplete image file.");
                }
            }
        }

        public DDSetStream(string filepath) : base(filepath)
        {
            componentSize = findEvidenceSize();

            buildSet();

            position = 0;

            findSchemeType();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            // work out which file of the set and what position of the file to start reading from
            int fileNumber = (int)(this.Position / this.componentSize);

            long offsetInFile = this.Position % this.componentSize;
            int remainingBytesToRead = count;
            int currentIndex = 0;
            int bytesRead = 0;

            // check that the offset does not try to read a file that is beyond the bounds of the set
            if (fileNumber < fileSet.Count)
            {
                while (remainingBytesToRead > 0)
                {
                    long currentFileBytesLeft = fileSet[fileNumber].Length - offsetInFile;

                    fileSet[fileNumber].Seek(offsetInFile, SeekOrigin.Begin);
                    // check whether the read operation will overlap into the next file
                    if (remainingBytesToRead > currentFileBytesLeft)
                    {
                        bytesRead = fileSet[fileNumber].Read(buffer, currentIndex, (int)currentFileBytesLeft);
                    }
                    else
                    {
                        bytesRead = fileSet[fileNumber].Read(buffer, currentIndex, (int)remainingBytesToRead);
                    }

                    remainingBytesToRead = remainingBytesToRead - bytesRead;
                    currentIndex += bytesRead;
                    this.Position += bytesRead;

                    fileNumber = fileNumber + 1;

                    // If the end of the stream is reached, no more bytes can be read,
                    // regardless of whether the read operation has requested more.
                    if (this.Position == this.Length)
                    {
                        remainingBytesToRead = 0;
                    }
                    offsetInFile = 0;
                    
                    // If no bytes have been read, the end of the stream has been reached
                    // or there was an error. Terminate the read operation.
                    if (bytesRead == 0)
                    {
                        return bytesRead;
                    }
                }
            }

            return bytesRead;
        }        
  
        protected override void buildSet()
        {
            // find all files in a contiguous sequence starting with the selected file
            string[] name = bf.F.Name.Split('.');

            string pattern = name[0];

            long size = 0;
            int counter = 1;

            if (name[1] == "001")
            {
                while (File.Exists(bf.F.DirectoryName + '\\' + name[0] + '.' + counter.ToString().PadLeft(3, '0')))
                {
                    // component streams are wrappers for StreamReader
                    ComponentStream sequenceFile = new ComponentStream(bf.F.DirectoryName + '\\' + name[0] + '.' + counter.ToString().PadLeft(3, '0'));
                    fileSet.Add(sequenceFile);

                    size += sequenceFile.F.Length;

                    counter++;
                }
            }
            else
            {
                ComponentStream singleDMG = new ComponentStream(bf.F.FullName);

                fileSet.Add(singleDMG);
                size = singleDMG.Length;
            }

            SetLength(size);
        }
        protected override long findEvidenceSize()
        {
            long calculatedSize;

            calculatedSize = (long)bf.F.Length;

            return calculatedSize;
        }
        protected override void findSectorSize()
        {
            // not sure how to work this out yet. Usually 512, which is set as default.
        }
        protected override void findSchemeType()
        {
            // look for partition scheme signatures
            byte[] findScheme = new byte[this.sectorSize * 2];

            this.Read(findScheme, 0, findScheme.Count());

            byte[] efiSig = new byte[8];
            byte[] mbrSig = new byte[2];
            Array.Copy(findScheme, this.sectorSize, efiSig, 0, 8);
            Array.Copy(findScheme, 510, mbrSig, 0, 2);

            if (System.Text.Encoding.UTF8.GetString(efiSig) == "EFI PART")
            {
                this.scheme = schemeType.GPT;
            }
            else if ((mbrSig[0] == 0x55) && (mbrSig[1] == 0xAA))
            {
                this.scheme = schemeType.MBR;
            }
        }
        public override long Seek(long offset, SeekOrigin origin)
        {
            long result;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    this.Position = offset;
                    result = this.Position;
                    break;
                case SeekOrigin.Current:
                    this.Position += offset;
                    result = this.Position;
                    break;
                default:
                    this.Position = this.Length + offset;
                    result = this.Position;
                    break;
            }

            return result;
        }
        public override void Write(byte[] buffer, int position, int length)
        {
            throw new NotImplementedException();
        }
        public override void SetLength(long value)
        {
            this.totalSize = value;
        }
        public override void Flush()
        {
            throw new NotImplementedException();
        }
    }
}
