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
    class HFSPlusCatalogFolder : HFSPlusCatalogRecord
    {
        public ushort flags { get; set; }
        public uint valence { get; set; }
        public uint folderID { get; set; }
        public DateTime createDate { get; set; }
        public DateTime contentModDate { get; set; }
        public DateTime attributeModDate { get; set; }
        public DateTime accessDate { get; set; }
        public DateTime backupDate { get; set; }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public HFSPlusPermissions permissions { get; set; }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public HFSPlusFinderInfo.folderInfo userInfo { get; set; }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public HFSPlusFinderInfo.extendedFolderInfo finderInfo { get; set; }
        public uint textEncoding { get; set; }
        public uint reserved { get; set; }
        public string path { get; set; }

        public HFSPlusCatalogFolder(ref byte[] rawData)
            : base(ref rawData)
        {
            this.flags = dataOperations.convToLE(BitConverter.ToUInt16(rawData, 2));
            this.valence = dataOperations.convToLE(BitConverter.ToUInt32(rawData, 4));
            this.folderID = dataOperations.convToLE(BitConverter.ToUInt32(rawData, 8));
            this.createDate = HFSPlus.FromHFSPlusTime(dataOperations.convToLE(BitConverter.ToUInt32(rawData, 12)));
            this.contentModDate = HFSPlus.FromHFSPlusTime(dataOperations.convToLE(BitConverter.ToUInt32(rawData, 16)));
            this.attributeModDate = HFSPlus.FromHFSPlusTime(dataOperations.convToLE(BitConverter.ToUInt32(rawData, 20)));
            this.accessDate = HFSPlus.FromHFSPlusTime(dataOperations.convToLE(BitConverter.ToUInt32(rawData, 24)));
            this.backupDate = HFSPlus.FromHFSPlusTime(dataOperations.convToLE(BitConverter.ToUInt32(rawData, 28)));

            byte[] folderPermissions = new byte[16];
            Array.Copy(rawData, 32, folderPermissions, 0, 16);
            byte[] folderUserInfo = new byte[16];
            Array.Copy(rawData, 48, folderUserInfo, 0, 16);
            byte[] folderFinderInfo = new byte[16];
            Array.Copy(rawData, 64, folderFinderInfo, 0, 16);

            this.userInfo = HFSPlusFinderInfo.getFolderUserInfo(ref folderUserInfo);
            this.finderInfo = HFSPlusFinderInfo.getFolderFinderInfo(ref folderFinderInfo);
            this.permissions = getHFSPlusPermissions(ref folderPermissions);

            this.textEncoding = dataOperations.convToLE(BitConverter.ToUInt32(rawData, 80));
            this.reserved = dataOperations.convToLE(BitConverter.ToUInt32(rawData, 84));
        }

        public HFSPlusCatalogFolder()
        {
        }
    }
}
