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
    class catalogIndexNode : absIndexOrLeafNode
    {
        public struct catalogIndexRecord
        {
            public catalogFile.HFSPlusCatalogKey catalogKey;
            public uint pointer;
        }

        public List<catalogIndexRecord> records = new List<catalogIndexRecord>();

        public catalogIndexNode(ref byte[] nodeRawData) : base(ref nodeRawData)
        {
            buildRecords();
        }

        private void buildRecords()
        {
            foreach (rawKeyAndRecord rawRecord in this.rawRecords)
            {
                catalogIndexRecord currentRecord = new catalogIndexRecord();

                byte[] keyData = rawRecord.keyData;
                byte[] recordData = rawRecord.recordData;

                currentRecord.catalogKey = catalogFile.buildCatalogTrialKey(rawRecord);

                currentRecord.pointer = dataOperations.convToLE(BitConverter.ToUInt32(recordData, 0));

                records.Add(currentRecord);
            }
        }
    }
}
