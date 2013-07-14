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
    class catalogLeafNode : absIndexOrLeafNode
    {
        public enum recordType {
            kHFSFolderRecord            = 0x01,
            kHFSFileRecord              = 0x02,
            kHFSFolderThreadRecord      = 0x03,
            kHFSFileThreadRecord        = 0x04
        }
        public struct HFSPlusCatalogThread
        {
            public catalogFile.HFSPlusCatalogKey key;
            public recordType type;
            public short reserved;
            public uint parentID;
            public byte[] nodeName;
        }

        public List<HFSPlusCatalogFile> fileRecords = new List<HFSPlusCatalogFile>();
        public List<HFSPlusCatalogFolder> folderRecords = new List<HFSPlusCatalogFolder>();
        public List<HFSPlusCatalogThread> threadRecords = new List<HFSPlusCatalogThread>();

        public catalogLeafNode(ref byte[] nodeRawData)
            : base(ref nodeRawData)
        {
            getRecords();
        }

        private void getRecords()
        {
            foreach(rawKeyAndRecord record in this.rawRecords)
            {
                short thisRecordType = dataOperations.convToLE(BitConverter.ToInt16(record.recordData, 0));

                catalogFile.HFSPlusCatalogKey key = new catalogFile.HFSPlusCatalogKey();

                key.keyLength = record.keyLength;
                key.parentID = dataOperations.convToLE(BitConverter.ToUInt32(record.keyData, 0));
                byte[] nodeName = new byte[record.keyLength - 6];
                Array.Copy(record.keyData, 6, nodeName, 0, record.keyLength - 6);
                key.nodeName = nodeName;

                byte[] rawData = record.recordData;

                switch ((recordType)thisRecordType)
                {
                    case recordType.kHFSFileRecord:
                        HFSPlusCatalogFile fileRecord = new HFSPlusCatalogFile(ref rawData);

                        fileRecord.key = key;

                        fileRecords.Add(fileRecord);

                        break;
                    case recordType.kHFSFolderRecord:
                        HFSPlusCatalogFolder folderRecord = new HFSPlusCatalogFolder(ref rawData);

                        folderRecord.key = key;

                        folderRecords.Add(folderRecord);

                        break;
                    case recordType.kHFSFileThreadRecord: case recordType.kHFSFolderThreadRecord:
                        HFSPlusCatalogThread threadRecord = new HFSPlusCatalogThread();

                        threadRecord.key = key;
                        threadRecord.type = (recordType)thisRecordType;
                        
                        threadRecord.reserved = dataOperations.convToLE(BitConverter.ToInt16(rawData, 2));
                        threadRecord.parentID = dataOperations.convToLE(BitConverter.ToUInt32(rawData, 4));

                        threadRecord.nodeName = new byte[rawData.Length - 8];

                        Array.Copy(rawData, 8, threadRecord.nodeName, 0, rawData.Length - 8);

                        threadRecords.Add(threadRecord);

                        break;
                }
            }
        }
    }
}
