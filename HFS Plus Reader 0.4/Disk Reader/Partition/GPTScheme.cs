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
using System.IO;
using System.ComponentModel;

namespace Disk_Reader
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    class GPTScheme : absPartitionScheme
    {
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public struct GPTHeader
        {
            private int revisionVal;
            private int headerSizeVal;
            private uint headerCheckVal;
            private long mainHeaderVal;
            private long backupHeaderVal;
            private long firstLBAVal;
            private long lastLBAVal;
            private Guid diskGUIDVal;
            private long tableStartVal;
            private uint tableCheckVal;

            public int revision { get { return revisionVal; } set { revisionVal = value; } }
            public int headersize { get { return headerSizeVal; } set { headerSizeVal = value; } }
            public uint headercheck { get { return headerCheckVal; } set { headerCheckVal = value; } }
            public long mainheader { get { return mainHeaderVal; } set { mainHeaderVal = value; } }
            public long backupheader { get { return backupHeaderVal; } set { backupHeaderVal = value; } }
            public long firstlba { get { return firstLBAVal; } set { firstLBAVal = value; } }
            public long lastlba { get { return lastLBAVal; } set { lastLBAVal = value; } }
            public Guid diskguid { get { return diskGUIDVal; } set { diskGUIDVal = value; } }
            public long tablestart { get { return tableStartVal; } set { tableStartVal = value; } }
            public uint tablecheck { get { return tableCheckVal; } set { tableCheckVal = value; } }
        }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        new public struct entry
        {
            private Guid partTypeGUIDVal;
            private Guid partGUIDVal;
            private int partitionNoVal;
            private byte[] attributesVal;
            private string nameVal;
            private long partStartLBAVal;
            private long partEndLBAVal;
            private long partLengthVal;

            public Guid parttype_guid { get { return partTypeGUIDVal; } set { partTypeGUIDVal = value; } }
            public Guid part_guid { get { return partGUIDVal; } set { partGUIDVal = value; } }
            public int partitionNo { get { return partitionNoVal; } set { partitionNoVal = value; } }
            public byte[] attributes { get { return attributesVal; } set { attributesVal = new byte[8];
                attributesVal = value; } }
            public string name { get { return nameVal; } set { nameVal = value; } }
            public long partStartLBA { get { return partStartLBAVal; } set { partStartLBAVal = value; } }
            public long partEndLBA { get { return partEndLBAVal; } set { partEndLBAVal = value; } }
            public long partLength { get { return partLengthVal; } set { partLengthVal = value; } }
        }
        public enum partitionType
        {
            unknown = 0,
            FAT12 = 1,
            FAT16 = 2,
            FAT32 = 3,
            HFSPlus = 4,
            HFSPlusCS = 5
        }

        public int numberofentries { get; set; }
        public int sizeofentry { get; set; }
        public long tablestart { get; set; }
        public int tablelength { get; set; }
        public GPTHeader header  { get; set; }
        public GPTHeader backupHeader { get; set; }
        public bool headerMatchesCRC { get; set; }
        public bool tableMatchesCRC { get; set; }
        new public List<entry> entries { get; set; }
        public List<entry> backupEntries { get; set; }

        public bool headerFound { get; set; }
        public bool backupFound { get; set; }
        public bool backupMatchesCRC { get; set; }
        public bool backupTableMatchesCRC { get; set; }
        public bool protectiveMBRExists { get; set; }
        

        public GPTScheme(absImageStream fileset) : base(fileset)
        {
            entries = new List<GPTScheme.entry>();
            backupEntries = new List<GPTScheme.entry>();
            headerMatchesCRC = false;
            tableMatchesCRC = false;
            headerFound = false;
            backupFound = false;
            backupMatchesCRC = false;
            backupTableMatchesCRC = false;
            protectiveMBRExists = false;
            header = new GPTHeader();
            backupHeader = new GPTHeader();
            runChecks();
        }
        private GPTHeader getHeader(byte[] buffer, absImageStream fileset, GPTHeader theHeader)
        {
            byte[] findguid = new byte[16];
            Array.Copy(buffer, 0x38, findguid, 0, 16);

            theHeader.revision = BitConverter.ToInt32(buffer, 0x08);
            theHeader.headersize = BitConverter.ToInt32(buffer, 0x0C);
            theHeader.headercheck = BitConverter.ToUInt32(buffer, 0x10);
            theHeader.mainheader = BitConverter.ToInt64(buffer, 0x18);
            theHeader.backupheader = BitConverter.ToInt64(buffer, 0x20);
            theHeader.firstlba = BitConverter.ToInt64(buffer, 0x28);
            theHeader.lastlba = BitConverter.ToInt64(buffer, 0x30);
            theHeader.diskguid = new Guid(findguid);
            theHeader.tablestart = BitConverter.ToInt32(buffer, 0x48);
            numberofentries = BitConverter.ToInt32(buffer, 0x50);
            sizeofentry = BitConverter.ToInt32(buffer, 0x54);
            theHeader.tablecheck = BitConverter.ToUInt32(buffer, 0x58);
            
            tablelength = numberofentries * sizeofentry;

            return theHeader;
        }
        private void runChecks()
        {
            // look at the position for the initial header first
            byte[] buffer = new byte[i.sectorSize];

            i.Seek(i.sectorSize, SeekOrigin.Begin);
            i.Read(buffer, 0, buffer.Length);

            int headersize = BitConverter.ToInt32(buffer, 0x0C);
            uint headercheck = BitConverter.ToUInt32(buffer, 0x10);

            // checksum of header is calculated with the checksum field set to zero
            Array.Copy(new byte[] { 0, 0, 0, 0 }, 0x0, buffer, 0x10, 4);

            // reserved field is not included
            byte[] headerbuffer = new byte[headersize];
            Array.Copy(buffer, headerbuffer, headerbuffer.Length);

            Crc32 headercrc = new Crc32();
            headercrc.ComputeHash(headerbuffer);
            if (headercrc.CrcValue == headercheck) 
            {
                this.headerFound = true;
                // if the header is valid, add the header
                this.headerMatchesCRC = true;

                buffer = new byte[i.sectorSize];

                i.Seek(i.sectorSize, SeekOrigin.Begin);
                i.Read(buffer, 0, i.sectorSize);
                this.header = getHeader(buffer, i, this.header);
                this.tablestart = header.tablestart;

                // make sure the table CRC matches as well
                byte[] rawTable = new byte[tablelength];
                i.Seek(header.tablestart * i.sectorSize, SeekOrigin.Begin);
                i.Read(rawTable, 0, tablelength);

                Crc32 tablecrc = new Crc32();
                tablecrc.ComputeHash(rawTable);
                if (tablecrc.CrcValue == header.tablecheck)
                {
                    // if the table is valid, add the entries
                    this.tableMatchesCRC = true;
                    this.entries = buildTable(rawTable);
                }
            }

            // then do the same for the backup header
            buffer = new byte[i.sectorSize];
            if (this.headerMatchesCRC)
            {
                // if the original header is valid, use the value from it to find the backup header
                i.Seek(this.header.backupheader * i.sectorSize, SeekOrigin.Begin);
            }
            else
            {
                // if it is not valid, look 512 bytes from the end of the image
                i.Seek(-i.sectorSize, SeekOrigin.End);
            }
            i.Read(buffer, 0, buffer.Length);

            int backupheadersize = BitConverter.ToInt32(buffer, 0x0C);
            uint backupheadercheck = BitConverter.ToUInt32(buffer, 0x10);

            // checksum of header is calculated with the checksum field set to zero
            Array.Copy(new byte[] { 0, 0, 0, 0 }, 0x0, buffer, 0x10, 4);

            // reserved field is not included
            byte[] backupheaderbuffer = new byte[headersize];
            Array.Copy(buffer, backupheaderbuffer, backupheaderbuffer.Length);

            Crc32 backupheadercrc = new Crc32();
            backupheadercrc.ComputeHash(backupheaderbuffer);
            if (backupheadercrc.CrcValue == backupheadercheck)
            {
                // if the header is valid, add the header
                this.backupMatchesCRC = true;
                this.backupFound = true;

                buffer = new byte[i.sectorSize];

                // go back 512 bytes because previous read incremented position by 512
                i.Seek(-i.sectorSize, SeekOrigin.Current);
                i.Read(buffer, 0, buffer.Length);
                this.backupHeader = getHeader(buffer, i, this.backupHeader);

                // make sure the table CRC matches as well
                byte[] rawTable = new byte[tablelength];
                i.Seek(backupHeader.tablestart * i.sectorSize, SeekOrigin.Begin);
                i.Read(rawTable, 0, tablelength);

                Crc32 tablecrc = new Crc32();
                tablecrc.ComputeHash(rawTable);
                if (tablecrc.CrcValue == backupHeader.tablecheck)
                {
                    // if the table is valid, add the entries
                    this.backupTableMatchesCRC = true;

                    this.backupEntries = buildTable(rawTable);
                }
            }

            i.Seek(0, SeekOrigin.Begin);
            i.Read(buffer, 0, i.sectorSize);
            if (buffer[buffer.Length - 2] == 0x55 && buffer[buffer.Length - 1] == 0xAA)
            {
                this.protectiveMBRExists = true;
            }
        }
        private List<entry> buildTable(byte[] rawTable)
        {
            List<entry> tableEntries = new List<entry>();
            // loop through all entries
            for (int j = 0; j < numberofentries; j++)
            {
                byte[] findguid = new byte[16];
                byte[] currentEntry = new byte[sizeofentry];

                Array.Copy(rawTable, j * sizeofentry, currentEntry, 0, sizeofentry);

                Array.Copy(currentEntry, findguid, 16);
                Guid entryTypeGUID = new Guid(findguid);

                // if an entry has a zero GUID, it is unused; ignore it
                if (entryTypeGUID != Guid.Parse("{00000000-0000-0000-0000-000000000000}"))
                {
                    entry thisEntry = new entry();
                    thisEntry.attributes = new byte[8];

                    Array.Copy(currentEntry, 16, findguid, 0, 16);
                    Guid entryPartGUID = new Guid(findguid);
                    byte[] namestr = new byte[72];

                    Array.Copy(currentEntry, 54, namestr, 0, 72);

                    thisEntry.parttype_guid = entryTypeGUID;
                    thisEntry.part_guid = entryPartGUID;
                    thisEntry.partitionNo = j;
                    thisEntry.partStartLBA = BitConverter.ToInt64(currentEntry, 32);
                    thisEntry.partEndLBA = BitConverter.ToInt64(currentEntry, 40);
                    thisEntry.partLength = thisEntry.partEndLBA - thisEntry.partStartLBA;
                    Array.Copy(currentEntry, 48, thisEntry.attributes, 0, 8);

                    string name = System.Text.Encoding.Unicode.GetString(namestr);
                    thisEntry.name = name.Trim('\0');

                    tableEntries.Add(thisEntry);
                }
            }

            return tableEntries;
        }
        public partitionType findPartitionType(GPTScheme.entry entry)
        {
            partitionType entryType = partitionType.unknown;

            byte[] HFSsigbytes = new byte[2];

            i.Seek(entry.partStartLBA * i.sectorSize + 1024, SeekOrigin.Begin);
            i.Read(HFSsigbytes, 0, 2);

            string HFSsig = System.Text.Encoding.UTF8.GetString(HFSsigbytes);

            if (HFSsig == "H+")
            {
                entryType = partitionType.HFSPlus;
            }
            else if (HFSsig == "HX")
            {
                entryType = partitionType.HFSPlusCS;
            }

            return entryType;
        }
        public List<entry> getValidTable()
        {
            if (this.headerMatchesCRC && this.tableMatchesCRC)
            {
                return this.entries;
            }
            else if (this.backupMatchesCRC && this.backupTableMatchesCRC)
            {
                return this.backupEntries;
            }
            else
            {
                throw new Exception("No valid partition tables present.");
            }
        }
    }
}
