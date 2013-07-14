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
    abstract class absVolume
    {
        public struct volumeHeader { }


        public uint blockSize;
        public absPartitionScheme.entry partEntry;
        public absImageStream ais;
        public long volumeStart;
        public long volumeLength;

        public absVolume(absImageStream fileSet, GPTScheme.entry partition)
        {
            GPTScheme.entry partEntry = partition;
            ais = fileSet;

            blockSize = 512;
            volumeStart = (long)partition.partStartLBA * ais.sectorSize;
            volumeLength = (long)partition.partLength * ais.sectorSize;
        }
        
    }
}
