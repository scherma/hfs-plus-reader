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
    class imageMap
    {
        public struct mapBlock
        {
            public long location { get; set; }
            public long length { get; set; }
            public string name { get; set; }
            public tileType type { get; set; }
            public byte[] allocationMap { get; set; }
            public int mapSectorsPerBlock { get; set; }
        }

        public enum tileType
        {
            disk_unallocated = 0,
            MBR = 1,
            GPT = 2,
            vol_unallocated = 3,
            vol_metafile = 4,
            vol_file = 5,
            vol_unknown = 6
        }

        public List<mapBlock> partitionblocks = new List<mapBlock>();

        public imageMap(absImageStream ais)
        {
            if (ais.scheme == absImageStream.schemeType.GPT)
            {
                GPTScheme gpts = new GPTScheme(ais);

                mapBlock block = new mapBlock();
                block.location = 0;

                if (gpts.protectiveMBRExists)
                {
                    block.length = 1;
                    block.name = "MBR";
                    block.type = tileType.MBR;

                    partitionblocks.Add(block);
                }

                if (gpts.headerFound)
                {
                    block.location = 1;
                    block.length = 1;
                    block.name = "GPT Header";
                    block.type = tileType.GPT;

                    partitionblocks.Add(block);

                    block.location = gpts.tablestart;
                    block.length = gpts.tablelength / ais.sectorSize;
                    if (block.length < 1) block.length = 1;
                    block.name = "GPT Primary Table";
                    block.type = tileType.GPT;

                    partitionblocks.Add(block);
                }

                if (gpts.backupFound)
                {
                    block.location = gpts.backupHeader.mainheader;
                    block.length = 1;
                    block.name = "Backup GPT Header";
                    block.type = tileType.GPT;

                    partitionblocks.Add(block);

                    block.location = gpts.backupHeader.tablestart;
                    block.length = gpts.tablelength / ais.sectorSize;
                    if (block.length < 1) block.length = 1;
                    block.name = "GPT Backup Table";
                    block.type = tileType.GPT;

                    partitionblocks.Add(block);
                }

                foreach (GPTScheme.entry entry in gpts.entries)
                {
                    block.location = entry.partStartLBA;
                    block.length = entry.partLength;
                    block.name = entry.name;
                    block.type = tileType.vol_unknown;

                    if (gpts.findPartitionType(entry) == GPTScheme.partitionType.HFSPlus)
                    {
                        HFSPlus hfsp = new HFSPlus(ais, entry);

                        block.mapSectorsPerBlock = (int)hfsp.volHead.blockSize / ais.sectorSize;
                        forkStream fs = new forkStream(new volumeStream(hfsp), new HFSPlusFile(hfsp.volHead.allocationFile, forkStream.forkType.data), forkStream.forkType.data);

                        block.allocationMap = new byte[(int)fs.Length];
                        fs.Read(block.allocationMap, 0, (int)fs.Length);
                    }
                    else
                    {
                        block.allocationMap = null;
                    }

                    partitionblocks.Add(block);
                }

            }

            partitionblocks.Sort(CompareBlocksByPosition);

        }

        public mapBlock sectorIsIn(long pos)
        {
            return partitionblocks.FirstOrDefault(p => p.location <= pos && p.location + p.length > pos);
        }

        private static int CompareBlocksByPosition(mapBlock x, mapBlock y)
        {
            if (x.location - y.location > 0) return 1;
            else if (x.location - y.location < 0) return -1;
            else return 0;
        }
    }
}
