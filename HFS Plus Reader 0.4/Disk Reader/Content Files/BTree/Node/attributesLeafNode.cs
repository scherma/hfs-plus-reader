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
    class attributesLeafNode : absIndexOrLeafNode
    {
        public enum type 
        {
            kHFSPlusAttrInlineData  = 0x10,
            kHFSPlusAttrForkData    = 0x20,
            kHFSPlusAttrExtents     = 0x30
        }
        public struct HFSPlusAttrForkData
        {
            public attributesFile.HFSPlusAttrKey key { get; set; }
            public type recordType { get; set; }
            public uint reserved { get; set; }
            public hfsPlusForkData theFork { get; set; }
            public int partitionAssoc;
        }
        public struct HFSPlusAttrExtents
        {
            public attributesFile.HFSPlusAttrKey key { get; set; }
            public type recordType { get; set; }
            public uint reserved { get; set; }
            public hfsPlusForkData.HFSPlusExtentRecord[] extents { get; set; }
        }
        public struct HFSPlusAttrInlineData
        {
            // will have to work this out from xattr.c file
            public attributesFile.HFSPlusAttrKey key { get; set; }
            public type recordType { get; set; }
            public byte[] otherData { get; set; }
            public int partitionAssoc;
        }
        public struct attributesDataForFile
        {
            public List<HFSPlusAttrForkData> forks;
            public List<HFSPlusAttrInlineData> inline;
        }

        public type recordType { get; set; }

        public List<HFSPlusAttrForkData> forkDataRecords;
        public List<HFSPlusAttrExtents> extentsRecords;
        public List<HFSPlusAttrInlineData> inlineRecords;

        public attributesLeafNode(ref byte[] nodeRawData) : base(ref nodeRawData)
        {
            getRecords();
        }

        private void getRecords()
        {
            forkDataRecords = new List<HFSPlusAttrForkData>();
            extentsRecords = new List<HFSPlusAttrExtents>();
            inlineRecords = new List<HFSPlusAttrInlineData>();

            foreach (rawKeyAndRecord raw in this.rawRecords)
            {
                type recordType = (type)dataOperations.convToLE(BitConverter.ToUInt32(raw.recordData, 0));

                switch (recordType)
                {
                    case type.kHFSPlusAttrExtents:
                        HFSPlusAttrExtents exrecord = new HFSPlusAttrExtents();

                        exrecord.key = attributesFile.buildAttrTrialKey(raw);
                        exrecord.recordType = type.kHFSPlusAttrExtents;
                        exrecord.reserved = dataOperations.convToLE(BitConverter.ToUInt32(raw.recordData, 4));

                        byte[] extentsData = new byte[raw.recordData.Length - 8];
                        Array.Copy(raw.recordData, 8, extentsData, 0, raw.recordData.Length - 8);
                        exrecord.extents = new hfsPlusForkData.HFSPlusExtentRecord[extentsData.Length / 8];

                        for (int i = 0; i < extentsData.Length / 8; i++)
                        {
                            exrecord.extents[i].startBlock = dataOperations.convToLE(BitConverter.ToUInt32(extentsData, i * 8));
                            exrecord.extents[i].blockCount = dataOperations.convToLE(BitConverter.ToUInt32(extentsData, (i * 8) + 4));
                        }

                        extentsRecords.Add(exrecord);
                        break;
                    case type.kHFSPlusAttrForkData:
                        HFSPlusAttrForkData fdrecord = new HFSPlusAttrForkData();

                        fdrecord.key = attributesFile.buildAttrTrialKey(raw);
                        fdrecord.recordType = type.kHFSPlusAttrForkData;
                        fdrecord.reserved = dataOperations.convToLE(BitConverter.ToUInt32(raw.recordData, 4));

                        byte[] fdrecordData = raw.recordData;

                        fdrecord.theFork = new hfsPlusForkData(ref fdrecordData, 8);

                        forkDataRecords.Add(fdrecord);

                        break;
                    case type.kHFSPlusAttrInlineData:
                        HFSPlusAttrInlineData attrRecord = new HFSPlusAttrInlineData();

                        attrRecord.key = attributesFile.buildAttrTrialKey(raw);
                        attrRecord.recordType = type.kHFSPlusAttrInlineData;
                        attrRecord.otherData = new byte[raw.recordData.Length - 4];
                        Array.Copy(raw.recordData, 4, attrRecord.otherData, 0, attrRecord.otherData.Length);

                        inlineRecords.Add(attrRecord);
                        break;
                }
            }
        }
    }
}
