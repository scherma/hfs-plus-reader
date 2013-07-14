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
    abstract class HFSPlusCatalogRecord
    {
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public catalogFile.HFSPlusCatalogKey key { get; set; }
        public catalogLeafNode.recordType type { get; set; }
        public int partitionAssoc;

        public HFSPlusCatalogRecord(ref byte[] rawData)
        {
            short thisRecordType = dataOperations.convToLE(BitConverter.ToInt16(rawData, 0));
            this.type = (catalogLeafNode.recordType)thisRecordType;
        }
        public HFSPlusCatalogRecord()
        {
        }

        public enum filemodeFlags
        {
            otherRead = 0x0004,
            otherWrite = 0x0002,
            otherExecute = 0x0001,
            groupRead = 0x0020,
            groupWrite = 0x0010,
            groupExecute = 0x0008,
            ownerRead = 0x0100,
            ownerWrite = 0x0080,
            ownerExecute = 0x0040,
            userIDOnExecute = 0x0800,
            groupIDOnExecute = 0x0400,
            stickyBit = 0x0200,
            namedPipe = 0x1000,
            charSpecial = 0x2000,
            directory = 0x4000,
            blockSpecial = 0x6000,
            regular = 0xA000,
            symbolicLink = 0xC000,
            whiteout = 0xE000
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public struct file_Mode
        {
            public rwx other { get; set; }
            public rwx group { get; set; }
            public rwx owner { get; set; }
            public bool userIDOnExecute { get; set; }
            public bool groupIDOnExecute { get; set; }
            public bool stickyBit { get; set; }
            public bool namedPipe { get; set; }
            public bool charSpecial { get; set; }
            public bool directory { get; set; }
            public bool blockSpecial { get; set; }
            public bool regular { get; set; }
            public bool symbolicLink { get; set; }
            public bool whiteout { get; set; }
        }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public struct rwx
        {
            public bool read { get; set; }
            public bool write { get; set; }
            public bool execute { get; set; }
        }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public struct HFSPlusPermissions
        {
            public uint ownerID { get; set; }
            public uint groupID { get; set; }
            public byte adminFlags { get; set; } // this can be expanded further
            public byte ownerFlags { get; set; } // this can be expanded further
            public file_Mode fileMode { get; set; }
            public uint special { get; set; }

            public specialType type { get; set; }

            public enum specialType
            {
                reserved = 0,
                iNodeNum = 1,
                linkCount = 2,
                rawDevice = 3
            }
        }

        protected HFSPlusPermissions getHFSPlusPermissions(ref byte[] rawInfo)
        {
            HFSPlusPermissions result = new HFSPlusPermissions();

            result.ownerID = dataOperations.convToLE(BitConverter.ToUInt32(rawInfo, 0));
            result.groupID = dataOperations.convToLE(BitConverter.ToUInt32(rawInfo, 4));
            result.adminFlags = rawInfo[8];
            result.ownerFlags = rawInfo[9];
            result.fileMode = getFileMode(ref rawInfo);
            result.special = dataOperations.convToLE(BitConverter.ToUInt32(rawInfo, 12));

            return result;
        }
        private bool Is(filemodeFlags current, filemodeFlags value)
        {
            return (current & value) == value;
        }
        private file_Mode getFileMode(ref byte[] rawPermissions)
        {
            file_Mode result = new file_Mode();

            ushort fm = dataOperations.convToLE(BitConverter.ToUInt16(rawPermissions, 10));

            rwx other = new rwx();
            rwx group = new rwx();
            rwx owner = new rwx();

            other.read = Is((filemodeFlags)fm, filemodeFlags.otherRead);
            other.write = Is((filemodeFlags)fm, filemodeFlags.otherWrite);
            other.execute = Is((filemodeFlags)fm, filemodeFlags.otherExecute);
            group.read = Is((filemodeFlags)fm, filemodeFlags.groupRead);
            group.write = Is((filemodeFlags)fm, filemodeFlags.groupWrite);
            group.execute = Is((filemodeFlags)fm, filemodeFlags.groupExecute);
            owner.read = Is((filemodeFlags)fm, filemodeFlags.ownerRead);
            owner.write = Is((filemodeFlags)fm, filemodeFlags.ownerWrite);
            owner.execute = Is((filemodeFlags)fm, filemodeFlags.ownerExecute);

            result.other = other;
            result.group = group;
            result.owner = owner;
            result.userIDOnExecute = Is((filemodeFlags)fm, filemodeFlags.userIDOnExecute);
            result.groupIDOnExecute = Is((filemodeFlags)fm, filemodeFlags.groupIDOnExecute);
            result.stickyBit = Is((filemodeFlags)fm, filemodeFlags.stickyBit);
            result.namedPipe = Is((filemodeFlags)fm, filemodeFlags.namedPipe);
            result.charSpecial = Is((filemodeFlags)fm, filemodeFlags.charSpecial);
            result.directory = Is((filemodeFlags)fm, filemodeFlags.directory);
            result.blockSpecial = Is((filemodeFlags)fm, filemodeFlags.blockSpecial);
            result.regular = Is((filemodeFlags)fm, filemodeFlags.regular);
            result.symbolicLink = Is((filemodeFlags)fm, filemodeFlags.symbolicLink);
            result.whiteout = Is((filemodeFlags)fm, filemodeFlags.whiteout);

            return result;
        }
    }
}
