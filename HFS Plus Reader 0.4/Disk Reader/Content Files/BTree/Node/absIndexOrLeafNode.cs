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
    abstract class absIndexOrLeafNode : absNode
    {
        public struct rawKeyAndRecord
        {
            private ushort keyLengthVal;
            private byte[] keyDataVal;
            private byte[] recordDataVal;

            public ushort keyLength { get { return keyLengthVal; } set { keyLengthVal = value; } }
            public byte[] keyData { get { return keyDataVal; } set { keyDataVal = value; } }
            public byte[] recordData { get { return recordDataVal; } set { recordDataVal = value; } }
        }

        public rawKeyAndRecord[] rawRecords;

        public absIndexOrLeafNode(ref byte[] nodeRawData) : base(ref nodeRawData)
        {
            buildRawRecords(nodeData);
        }

        private void buildRawRecords(byte[] nodeData)
        {
            this.rawRecords = new rawKeyAndRecord[this.BTNodeDescriptor.numRecords];

            for (int i = 0; i < this.BTNodeDescriptor.numRecords; i++)
            {
                this.rawRecords[i].keyLength = dataOperations.convToLE(BitConverter.ToUInt16(nodeData, offsets[i]));
                this.rawRecords[i].keyData = new byte[this.rawRecords[i].keyLength];
                Array.Copy(nodeData, offsets[i] + 2, rawRecords[i].keyData, 0, rawRecords[i].keyLength);

                
                int recordDataLength = offsets[i + 1] - offsets[i] - rawRecords[i].keyLength - 2;
                this.rawRecords[i].recordData = new byte[recordDataLength];
                Array.Copy(nodeData, offsets[i] + rawRecords[i].keyLength + 2, rawRecords[i].recordData, 0, recordDataLength);
            }
        }
    }
}
