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
    class HFSPlusFile : absFile
    {
        [TypeConverter(typeof(ExpandableObjectConverter))]
        new public struct blocks
        {
            public List<hfsPlusForkData.HFSPlusExtentRecord> dataExtents;
            public List<hfsPlusForkData.HFSPlusExtentRecord> resourceExtents;
        }

        public blocks fileContent;
        public bool allDataBlocksKnown = false;
        public bool allResourceBlocksKnown = false;

        public ulong dataLogicalSize;
        public uint dataClumpSize;
        public ulong rsrcLogicalSize;
        public uint rsrcClumpSize;
        public uint totalDataBlocks;
        public uint knownDataBlocks;
        public uint totalResourceBlocks;
        public uint knownResourceBlocks;
        public HFSPlusCatalogFile catalogTreeEntry;

        public HFSPlusFile(hfsPlusForkData forkEntry, forkStream.forkType type)
        {
            fileContent.dataExtents = new List<hfsPlusForkData.HFSPlusExtentRecord>();
            fileContent.resourceExtents = new List<hfsPlusForkData.HFSPlusExtentRecord>();

            switch (type)
            {
                case forkStream.forkType.data:
                    addDataFork(forkEntry);
                    break;
                case forkStream.forkType.resource:
                    addResourceFork(forkEntry);
                    break;
            }
        }

        public HFSPlusFile(HFSPlusCatalogFile catalogEntry, extentsOverflowFile eofInput)
        {
            fileContent.dataExtents = new List<hfsPlusForkData.HFSPlusExtentRecord>();
            fileContent.resourceExtents = new List<hfsPlusForkData.HFSPlusExtentRecord>();
            
            if(catalogEntry.dataFork != null) addDataFork(catalogEntry.dataFork);
            if(catalogEntry.resourceFork != null) addResourceFork(catalogEntry.resourceFork);
            getAllExtents(eofInput, catalogEntry.fileID);
        }

        private void getKnownDataBlocks()
        {
            uint knownblocks = 0;
            foreach (hfsPlusForkData.HFSPlusExtentRecord extent in fileContent.dataExtents)
            {
                knownblocks += extent.blockCount;
            }

            this.knownDataBlocks = knownblocks;
        }
        private void getKnownResourceBlocks()
        {
            uint knownblocks = 0;
            foreach (hfsPlusForkData.HFSPlusExtentRecord extent in fileContent.resourceExtents)
            {
                knownblocks += extent.blockCount;
            }

            this.knownResourceBlocks = knownblocks;
        }
        public void addDataFork(hfsPlusForkData dataFork)
        {
            int i = 0;
            while (i < 8 && dataFork.forkDataValues.extents[i].blockCount > 0)
            {
                this.fileContent.dataExtents.Add(dataFork.forkDataValues.extents[i]);
                i++;
            }

            this.dataClumpSize = dataFork.forkDataValues.clumpSize;
            this.dataLogicalSize = dataFork.forkDataValues.logicalSize;
            this.totalDataBlocks = dataFork.forkDataValues.totalBlocks;

            getKnownDataBlocks();
            this.allDataBlocksKnown = knownDataBlocks == totalDataBlocks;
        }
        public void addDataExtentsToFork(hfsPlusForkData.HFSPlusExtentRecord anExtent)
        {
            fileContent.dataExtents.Add(anExtent);

            getKnownDataBlocks();
            this.allDataBlocksKnown = knownDataBlocks == totalDataBlocks;
        }
        public void addDataExtentsToFork(hfsPlusForkData.HFSPlusExtentRecord[] extents)
        {
            foreach (hfsPlusForkData.HFSPlusExtentRecord anExtent in extents)
            {
                fileContent.dataExtents.Add(anExtent);
            }

            getKnownDataBlocks();
            this.allDataBlocksKnown = knownDataBlocks == totalDataBlocks;
        }
        public void addResourceFork(hfsPlusForkData resourceFork)
        {
            int i = 0;
            while (resourceFork.forkDataValues.extents[i].blockCount > 0 && i < 8)
            {
                this.fileContent.resourceExtents.Add(resourceFork.forkDataValues.extents[i]);
                i++;
            }

            this.rsrcClumpSize = resourceFork.forkDataValues.clumpSize;
            this.rsrcLogicalSize=resourceFork.forkDataValues.logicalSize;
            this.totalResourceBlocks = resourceFork.forkDataValues.totalBlocks;

            getKnownResourceBlocks();
            this.allResourceBlocksKnown = knownResourceBlocks == totalResourceBlocks;
        }
        public void addResourceExtentsToFork(hfsPlusForkData.HFSPlusExtentRecord anExtent)
        {
            fileContent.resourceExtents.Add(anExtent);

            getKnownResourceBlocks();
            this.allResourceBlocksKnown = knownResourceBlocks == totalDataBlocks;
        }
        public void addResourceExtentsToFork(hfsPlusForkData.HFSPlusExtentRecord[] extents)
        {
            foreach (hfsPlusForkData.HFSPlusExtentRecord anExtent in extents)
            {
                fileContent.resourceExtents.Add(anExtent);
            }

            getKnownResourceBlocks();
            this.allResourceBlocksKnown = knownResourceBlocks == totalDataBlocks;
        }
        private void getAllExtents(extentsOverflowFile eofInput, uint CNID)
        {
            extentsOverflowFile.HFSPlusExtentKey extentKey = new extentsOverflowFile.HFSPlusExtentKey();
            extentsOverflowLeafNode.extentsOverflowLeafRecord record;

            extentKey.fileID = CNID;
            extentKey.type = extentsOverflowFile.forkType.data;

            while (this.knownDataBlocks < this.totalDataBlocks)
            {
                extentKey.startBlock = knownDataBlocks;
                record = eofInput.getExtentRecordWithKey(extentKey);

                int i = 0;
                while (i < 8 && record.extents[i].blockCount > 0)
                {
                    this.fileContent.dataExtents.Add(record.extents[i]);
                    this.knownDataBlocks += record.extents[i].blockCount;
                    i++;
                }
                this.allDataBlocksKnown = knownDataBlocks == totalDataBlocks;
            }

            extentKey.type = extentsOverflowFile.forkType.resource;
            while (this.knownResourceBlocks < this.totalResourceBlocks)
            {
                extentKey.startBlock = knownResourceBlocks;
                record = eofInput.getExtentRecordWithKey(extentKey);
                int i = 0;
                while (record.extents[i].blockCount > 0 && i < 8)
                {
                    this.fileContent.resourceExtents.Add(record.extents[i]);
                    this.knownResourceBlocks += record.extents[i].blockCount;
                    i++;
                }
                this.allResourceBlocksKnown = knownResourceBlocks == totalResourceBlocks;
            }
        }

    }
}
