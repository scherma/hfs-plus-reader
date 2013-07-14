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
using System.IO;

namespace Disk_Reader
{
    class hfsPlusForkData
    {
        public struct forkData
        {
            public ulong logicalSize { get; set; }
            public uint clumpSize { get; set; }
            public uint totalBlocks { get; set; }
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public List<HFSPlusExtentRecord> extents { get; set; }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public struct HFSPlusExtentRecord
        {
            private uint startBlockVal;
            private uint blockCountVal;

            public uint startBlock { get { return startBlockVal; } set { startBlockVal = value; } }
            public uint blockCount { get { return blockCountVal; } set { blockCountVal = value; } }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public forkData forkDataValues { get; set; }

        public hfsPlusForkData(ref byte[] rawData, int start)
        {
            forkData theFork = new forkData();
            theFork.extents = new List<HFSPlusExtentRecord>();

            byte[] forkBytes = new byte[80];
            Array.Copy(rawData, start, forkBytes, 0, 80);
            
            theFork.logicalSize = dataOperations.convToLE(BitConverter.ToUInt64(forkBytes, 0));
            theFork.clumpSize = dataOperations.convToLE(BitConverter.ToUInt32(forkBytes, 8));
            theFork.totalBlocks = dataOperations.convToLE(BitConverter.ToUInt32(forkBytes, 12));


            start = 16;

            for (int i = 0; i < 8; i++)
            {
                HFSPlusExtentRecord extent = new HFSPlusExtentRecord();

                extent.startBlock = dataOperations.convToLE(BitConverter.ToUInt32(forkBytes, start + (i * 8)));
                extent.blockCount = dataOperations.convToLE(BitConverter.ToUInt32(forkBytes, start + ((i * 8) + 4)));

                theFork.extents.Add(extent);
            }

            this.forkDataValues = theFork;
        }
        public hfsPlusForkData()
        {
        }
    }
}
