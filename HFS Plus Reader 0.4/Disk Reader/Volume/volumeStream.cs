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
    class volumeStream : Stream
    {
        protected long totalSize;
        protected long position;
        protected Stream inner;
        public absVolume volume;
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


        public volumeStream( absVolume volume)
        {
            this.inner = volume.ais;
            SetLength(volume.volumeLength);
            this.volume = volume;
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
            return inner.Read(buffer, offset, count);
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

            inner.Seek(this.Position + volume.volumeStart, SeekOrigin.Begin);

            return result;
        }
        public override void SetLength(long value)
        {
            this.totalSize = value;
        }
    }
}
