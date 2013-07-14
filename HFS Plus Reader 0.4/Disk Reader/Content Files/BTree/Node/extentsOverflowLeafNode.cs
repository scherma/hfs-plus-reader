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
using System.ComponentModel;

namespace Disk_Reader
{
    class extentsOverflowLeafNode : absIndexOrLeafNode
    {

        public struct extentsOverflowLeafRecord
        {
            public extentsOverflowFile.HFSPlusExtentKey key;
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public hfsPlusForkData.HFSPlusExtentRecord[] extents;
        }

        public List<extentsOverflowLeafRecord> records = new List<extentsOverflowLeafRecord>(); 

        public extentsOverflowLeafNode(ref byte[] nodeRawData) : base(ref nodeRawData)
        {
            getRecords();
        }

        private void getRecords()
        {
            foreach (rawKeyAndRecord record in this.rawRecords)
            {

                extentsOverflowFile.HFSPlusExtentKey key = new extentsOverflowFile.HFSPlusExtentKey();
                extentsOverflowLeafRecord leafRecord = new extentsOverflowLeafRecord();
                leafRecord.extents = new hfsPlusForkData.HFSPlusExtentRecord[8];

                key.keyLength = record.keyLength;
                key.type = (extentsOverflowFile.forkType)record.keyData[0];
                key.pad = record.keyData[1];
                key.fileID = dataOperations.convToLE(BitConverter.ToUInt32(record.keyData, 2));
                key.startBlock = dataOperations.convToLE(BitConverter.ToUInt32(record.keyData, 6));

                leafRecord.key = key;

                for (int i = 0; i < 8; i++)
                {
                    hfsPlusForkData.HFSPlusExtentRecord extent = new hfsPlusForkData.HFSPlusExtentRecord();

                    extent.startBlock = dataOperations.convToLE(BitConverter.ToUInt32(record.recordData, i * 8));
                    extent.blockCount = dataOperations.convToLE(BitConverter.ToUInt32(record.recordData, (i * 8) + 4));

                    leafRecord.extents[i] = extent;
                }

                this.records.Add(leafRecord);
            }
        }
    }
}
