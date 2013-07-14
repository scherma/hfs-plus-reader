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
using System.ComponentModel;

namespace Disk_Reader
{
    abstract class absImageStream : Stream
    {

        public enum schemeType
        {
            MBR = 0,
            GPT = 1
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public struct imageProperties
        {
            public string imagepath { get; set; }
            public int sectorSize { get; set; }
            public long componentSize { get; set; }
            public long totalSize { get; set; }
            public string hashMD5 { get; set; }
            public string hashSHA1 { get; set; }
            public schemeType scheme { get; set; }
            public absPartitionScheme entries { get; set; }
        }
        public enum sectorType 
        {
            MBR,
            GPT,
            PartitionSlack,
            VolumeHeader,
            VolumeSlack,
            File,
            Unknown
        }

        protected List<ComponentStream> fileSet = new List<ComponentStream>();
        public ComponentStream bf;
        public Stream inner;
        public int sectorSize;
        public long componentSize;
        public byte[] imageMD5;
        public byte[] imageSHA1;
        public schemeType scheme { get; protected set; }        
        protected long totalSize;
        protected long position;
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

        public absImageStream(string filepath)
        {
            bf = new ComponentStream(filepath);

            this.sectorSize = 512; // default unless identified otherwise
        }

        // uses the naming convention for a given imaging format to build a list of files in the set
        abstract protected void buildSet();
        abstract protected long findEvidenceSize();
        abstract protected void findSectorSize();
        abstract protected void findSchemeType();
        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
        public override void Flush()
        {
            throw new NotImplementedException();
        }
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }
    }
}
