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
    class forkStream : Stream
    {
        protected long totalSize;
        protected long position;
        protected volumeStream inner;
        public List<hfsPlusForkData.HFSPlusExtentRecord> fork;
        public override bool CanRead
        {
            get { return true; }
        }
        public override bool CanWrite
        {
            get { return false; }
        }
        public override bool CanSeek
        {
            get { return true; }
        }
        public override long Length
        {
            get { return this.totalSize; }
        }
        public override long Position
        {
            get
            {
                return this.position;
            }
            set
            {
                this.position = value;
            }
        }
        public enum forkType
        {
            data = 0,
            resource = 1
        }

        public forkStream(volumeStream inner, HFSPlusFile theFork,  forkType type)
        {
            this.inner = inner;

            switch(type)
            {
                case forkType.data:
                    SetLength((long)theFork.dataLogicalSize);
                    this.fork = theFork.fileContent.dataExtents;
                    break;
                case forkType.resource:
                    SetLength((long)theFork.rsrcLogicalSize);
                    this.fork = theFork.fileContent.resourceExtents;
                    break;
            }

        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = 0;

            while (count > 0)
            {
                // find the extent to read from
                int forkEntry = 0;

                try
                {
                    forkEntry = findExtent();
                }
                catch
                {
                    // if the extent doesn't exist, no more bytes can be read
                    return bytesRead;
                }


                // find the position within the extent
                // the program must subtract the lengths of each extent until the point where doing so would 
                // take the positionInExtent value below zero; at this point it should stop
                long positionInExtent = this.Position;
                int extentcounter = 0;
                while (extentcounter <= forkEntry)
                {
                    if (positionInExtent - fork[extentcounter].blockCount * inner.volume.blockSize >= 0)
                    {
                        positionInExtent -= fork[extentcounter].blockCount * inner.volume.blockSize;
                    }
                    extentcounter++;
                }

                // set the position of the inner stream
                inner.Seek((long)fork[forkEntry].startBlock * inner.volume.blockSize + positionInExtent, SeekOrigin.Begin);

                long bytesRemainingInExtent = (fork[forkEntry].blockCount * inner.volume.blockSize) - positionInExtent;

                // read to the end of the count or to the end of the extent
                if (count > bytesRemainingInExtent)
                {
                    int sizeToRead = (int)bytesRemainingInExtent;
                    inner.Read(buffer, offset, sizeToRead);
                    offset += sizeToRead;
                    count -= sizeToRead;

                    bytesRead = sizeToRead;
                }
                else
                {
                    inner.Read(buffer, offset, count);
                    bytesRead = count;
                    count = 0;
                }

                // update the position of current stream
                this.Position += bytesRead;
            }

            return bytesRead;
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
        public override void SetLength(long value)
        {
            this.totalSize = value;
        }
        private int findExtent()
        {
            long findPosition = 0;
            int i = 0;
            foreach (hfsPlusForkData.HFSPlusExtentRecord extent in fork)
            {
                findPosition += extent.blockCount * inner.volume.blockSize;
                if (findPosition > this.Position)
                {
                    return i;
                }
                i++;
            }

            throw new System.IndexOutOfRangeException("This position does not exist within the known extents of the file.");
        }
    }
}
