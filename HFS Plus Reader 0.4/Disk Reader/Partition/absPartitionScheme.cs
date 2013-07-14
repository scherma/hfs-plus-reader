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
    abstract class absPartitionScheme
    {
        public absImageStream i;
        public List<entry> entries;

        public struct entry
        {
            private ulong partStartLBAVal;
            private ulong partLengthVal;

            public ulong partStart { get { return partStartLBAVal; } set { partStartLBAVal = value; } }
            public ulong partLength { get { return partLengthVal; } set { partLengthVal = value; } }
        }

        public absPartitionScheme(absImageStream fileset)
        {   
            i = fileset;
        }

    }
}
