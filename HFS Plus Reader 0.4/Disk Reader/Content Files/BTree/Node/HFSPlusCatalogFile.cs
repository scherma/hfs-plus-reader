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
    [TypeConverter(typeof(ExpandableObjectConverter))]
    class HFSPlusCatalogFile : HFSPlusCatalogRecord
    {
        enum fileFlags
        {
            kHFSFileLockedBit = 0x0000,
            kHFSFileLockedMask = 0x0001,
            kHFSThreadExistsBit = 0x0001,
            kHFSThreadExistsMask = 0x0002
        }

        public bool fileLockedBit { get; set; }
        public bool fileLockedMask { get; set; }
        public bool threadExistsBit { get; set; }
        public bool threadExistsMask { get; set; }
        public uint reserved1 { get; set; }
        public uint fileID { get; set; }
        public DateTime createDate { get; set; }
        public DateTime contentModDate { get; set; }
        public DateTime attributeModDate { get; set; }
        public DateTime accessDate { get; set; }
        public DateTime backupDate { get; set; }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public HFSPlusPermissions permissions { get; set; }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public HFSPlusFinderInfo.fileInformation userInfo { get; set; }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public HFSPlusFinderInfo.extendedFileInfo finderInfo { get; set; }
        public uint textEncoding { get; set; }
        public uint reserved2 { get; set; }
        public string path { get; set; }

        public dataOperations.hashValues hashes { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public hfsPlusForkData dataFork { get; set; }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public hfsPlusForkData resourceFork { get; set; }
        
        public HFSPlusCatalogFile(ref byte[] rawData) : base(ref rawData)
        {
            ushort flags = dataOperations.convToLE(BitConverter.ToUInt16(rawData, 2));

            this.fileLockedBit = (fileFlags.kHFSFileLockedBit & (fileFlags)flags) == fileFlags.kHFSFileLockedBit;
            this.fileLockedMask = (fileFlags.kHFSFileLockedMask & (fileFlags)flags) == fileFlags.kHFSFileLockedMask;
            this.threadExistsBit = (fileFlags.kHFSThreadExistsBit & (fileFlags)flags) == fileFlags.kHFSThreadExistsBit;
            this.threadExistsMask = (fileFlags.kHFSThreadExistsMask & (fileFlags)flags) == fileFlags.kHFSThreadExistsMask;
            this.reserved1 = dataOperations.convToLE(BitConverter.ToUInt32(rawData, 4));
            this.fileID = dataOperations.convToLE(BitConverter.ToUInt32(rawData, 8));
            this.createDate = HFSPlus.FromHFSPlusTime(dataOperations.convToLE(BitConverter.ToUInt32(rawData, 12)));
            this.contentModDate = HFSPlus.FromHFSPlusTime(dataOperations.convToLE(BitConverter.ToUInt32(rawData, 16)));
            this.attributeModDate = HFSPlus.FromHFSPlusTime(dataOperations.convToLE(BitConverter.ToUInt32(rawData, 20)));
            this.accessDate = HFSPlus.FromHFSPlusTime(dataOperations.convToLE(BitConverter.ToUInt32(rawData, 24)));
            this.backupDate = HFSPlus.FromHFSPlusTime(dataOperations.convToLE(BitConverter.ToUInt32(rawData, 28)));

            byte[] filePermissions = new byte[16];
            Array.Copy(rawData, 32, filePermissions, 0, 16);
            byte[] fileUserInfo = new byte[16];
            Array.Copy(rawData, 48, fileUserInfo, 0, 16);
            byte[] fileFinderInfo = new byte[16];
            Array.Copy(rawData, 64, fileFinderInfo, 0, 16);

            this.userInfo = HFSPlusFinderInfo.getFileUserInfo(ref fileUserInfo);
            this.finderInfo = HFSPlusFinderInfo.getFileFinderInfo(ref fileFinderInfo);
            this.permissions = getHFSPlusPermissions(ref filePermissions);

            this.textEncoding = dataOperations.convToLE(BitConverter.ToUInt32(rawData, 80));
            this.reserved2 = dataOperations.convToLE(BitConverter.ToUInt32(rawData, 84));

            this.dataFork = new hfsPlusForkData(ref rawData, 88);
            this.resourceFork = new hfsPlusForkData(ref rawData, 168);
        }

        public HFSPlusCatalogFile()
        {
        }

    }
}
