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
    class extentsOverflowIndexNode : absIndexOrLeafNode
    {
        public struct extentsOverflowIndexRecord
        {
            public extentsOverflowFile.HFSPlusExtentKey extentKey;
            public uint pointer;
        }

        public List<extentsOverflowIndexRecord> records = new List<extentsOverflowIndexRecord>();

        public extentsOverflowIndexNode(ref byte[] nodeRawData) : base(ref nodeRawData)
        {
            buildRecords();
        }

        private void buildRecords()
        {
            foreach (rawKeyAndRecord rawRecord in this.rawRecords)
            {
                extentsOverflowIndexRecord currentRecord = new extentsOverflowIndexRecord();

                byte[] keyData = rawRecord.keyData;

                currentRecord.extentKey = extentsOverflowFile.buildExtentSearchKey(ref keyData);
                currentRecord.pointer = dataOperations.convToLE(BitConverter.ToUInt32(rawRecord.recordData, 0));

                records.Add(currentRecord);
            }
        }

    }
}
