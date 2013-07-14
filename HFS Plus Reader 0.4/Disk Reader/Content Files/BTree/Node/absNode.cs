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

namespace Disk_Reader
{
    class absNode
    {
        public descriptor BTNodeDescriptor;
        protected ushort[] offsets;

        public enum nodeType
        {
            header = 1,
            map = 2,
            index = 0,
            leaf = 255
        }

        public struct descriptor
        {
            public uint fLink { get; set; }
            public uint bLink { get; set; }
            public sbyte kind { get; set; }
            public byte height { get; set; }
            public ushort numRecords { get; set; }
            public ushort reserved { get; set; }
        }

        public byte[] nodeData { get; protected set; }

        public absNode(ref byte[] nodeRawData)
        {
            nodeData = nodeRawData;
            getDescriptor(nodeData);
            getOffsets(nodeData, this.BTNodeDescriptor.numRecords);
        }

        private void getDescriptor(byte[] nodeRawData)
        {
            BTNodeDescriptor.fLink = dataOperations.convToLE(BitConverter.ToUInt32(nodeRawData, 0));
            BTNodeDescriptor.bLink = dataOperations.convToLE(BitConverter.ToUInt32(nodeRawData, 4));
            BTNodeDescriptor.kind = (sbyte)nodeRawData[8];
            BTNodeDescriptor.height = nodeRawData[9];
            BTNodeDescriptor.numRecords = dataOperations.convToLE(BitConverter.ToUInt16(nodeRawData, 10));
            BTNodeDescriptor.reserved = dataOperations.convToLE(BitConverter.ToUInt16(nodeRawData, 12));
        }
        private void getOffsets(byte[] nodeRawData, int numRecords)
        {
            offsets = new ushort[numRecords + 1];

            for (int i = 0; i <= numRecords; i++)
            {
                int thisOffset = nodeRawData.Length - ((i + 1) * 2);

                offsets[i] = dataOperations.convToLE(BitConverter.ToUInt16(nodeRawData, thisOffset));
            }
        }
        protected byte[] getRecordData(int recordNo)
        {
            int recordSize = this.offsets[recordNo + 1] - this.offsets[recordNo];

            byte[] recordData = new byte[recordSize];
            Array.Copy(this.nodeData, this.offsets[0], recordData, 0, recordSize);

            return recordData;
        }
    }
}
