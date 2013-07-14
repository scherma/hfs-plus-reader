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
    class ComponentStream : Stream
    {
        StreamReader inner;
        public FileInfo F;
        string ImageFile;
        long position;
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
            get { return inner.BaseStream.Length; }
        }

        public ComponentStream(string filepath)
        {
            FileInfo f = new FileInfo(filepath);

            this.F = f;
            this.ImageFile = filepath;
            inner = new StreamReader(File.OpenRead(ImageFile));
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var result = inner.BaseStream.Read(buffer, offset, count);
            this.position = inner.BaseStream.Position;
            return result;
        }
        public override long Seek(long offset, SeekOrigin origin)
        {
            var result = inner.BaseStream.Seek(offset, origin);
            this.position = inner.BaseStream.Position;
            return result;
        }
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
                    throw new ArgumentOutOfRangeException(value.ToString() + " too large or too small to be used as a seek position.");
                }
            }
        }
        public override void Write(byte[] buffer, int position, int length)
        {
            throw new NotImplementedException();
        }
        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }
        public override void Flush()
        {
            throw new NotImplementedException();
        }
    }
}
