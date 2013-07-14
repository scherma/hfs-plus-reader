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

namespace Disk_Reader 
{
    abstract class absHFSPlusBTree
    {
        public enum BTreeType
        {
            kHFSBTreeType = 0,      // control file
            kUserBTreeType = 128,      // user btree type starts from 128
            kReservedBTreeType = 255
        }

        public headerNode header { get; set; }
        protected forkStream fs;
        protected HFSPlusFile extents;
        protected bool variableKeys;
        public ushort nodeSize;
        public byte[] treeMap;
        bool isRawDataComplete = false;

        public absHFSPlusBTree(HFSPlusFile knownExtents, volumeStream hfsp)
        {
            extents = knownExtents;

            // grab a bunch of information to ensure the header node is captured
            byte[] firstBlock = new byte[hfsp.volume.blockSize];

            this.fs = new forkStream(hfsp, knownExtents, forkStream.forkType.data);

            fs.Read(firstBlock, 0, firstBlock.Count());

            // nodeSize is byte 30 of header record which comes immediately after 14 byte descriptor
            this.nodeSize = dataOperations.convToLE(BitConverter.ToUInt16(firstBlock, 32)); 

            byte[] headerData = new byte[nodeSize];
            headerData = getNodeData(0, nodeSize);
            header = new headerNode(ref headerData);

            // check whether all of the data extents are known
            long treeSize = header.headerInfo.totalNodes * header.headerInfo.nodeSize;
            if (fs.Length >= treeSize && fs.Length > 0)
            {
                isRawDataComplete = true;

                buildMap(fs);
            }
        }

        protected byte[] getNodeData(uint nodeNumber, ushort nodeSize)
        {
            byte[] nodeData = new byte[this.nodeSize];

            fs.Seek(nodeNumber * nodeSize, SeekOrigin.Begin);
            fs.Read(nodeData, 0, this.nodeSize);

            return nodeData;
        }
        protected absNode.nodeType getNodeType(uint nodeNumber)
        {
            absNode.nodeType result;

            long nodePosition = nodeNumber * this.nodeSize;

            fs.Seek(nodePosition + 8, SeekOrigin.Begin);

            result = (absNode.nodeType)fs.ReadByte();

            return result;
        }
        protected byte[] buildMap(forkStream fs)
        {
            List<byte[]> mapContent = new List<byte[]>();

            mapContent.Add(header.map.bitmapComponent);
            uint fLink = this.header.BTNodeDescriptor.fLink;

            uint mapSize = (uint)header.map.bitmapComponent.Length;

            // if fLink > 0, there are more map nodes with map data to be read
            while (fLink > 0)
            {
                byte[] nodeRawData = new byte[this.nodeSize];
                fs.Seek(fLink * this.nodeSize, System.IO.SeekOrigin.Begin);
                fs.Read(nodeRawData, 0, this.nodeSize);
                mapNode currentMap = new mapNode(ref nodeRawData);
                mapContent.Add(currentMap.bitmapComponent);

                mapSize += (uint)currentMap.bitmapComponent.Length;

                fLink = currentMap.BTNodeDescriptor.fLink;
            }

            byte[] mapData = new byte[mapSize];

            int position = 0;
            foreach (byte[] component in mapContent)
            {
                Array.Copy(component, 0, mapData, position, component.Length);
                position += component.Length;
            }

            return mapData;
        }

    }
}
