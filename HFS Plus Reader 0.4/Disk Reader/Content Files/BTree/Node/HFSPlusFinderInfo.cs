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
    public static class HFSPlusFinderInfo
    {
        public enum finderFlags
        {
            kIsOnDesk = 0x0001,             /* Files and folders (System 6) */
            kColor = 0x000E,                /* Files and folders */
            kIsShared = 0x0040,             /* Files only (Applications only) If */
            /* clear, the application needs */
            /* to write to its resource fork, */
            /* and therefore cannot be shared */
            /* on a server */
            kHasNoINITs = 0x0080,           /* Files only (Extensions/Control */
            /* Panels only) */
            /* This file contains no INIT resource */
            kHasBeenInited = 0x0100,        /* Files only.  Clear if the file */
            /* contains desktop database resources */
            /* ('BNDL', 'FREF', 'open', 'kind'...) */
            /* that have not been added yet.  Set */
            /* only by the Finder. */
            /* Reserved for folders */
            kHasCustomIcon = 0x0400,        /* Files and folders */
            kIsStationery = 0x0800,         /* Files only */
            kNameLocked = 0x1000,           /* Files and folders */
            kHasBundle = 0x2000,            /* Files only */
            kIsInvisible = 0x4000,          /* Files and folders */
            kIsAlias = 0x8000               /* Files only */
        }
        public enum extendedFlags
        {
            kExtendedFlagsAreInvalid = 0x8000,      /* The other extended flags */
            /* should be ignored */
            kExtendedFlagHasCustomBadge = 0x0100,   /* The file or folder has a */
            /* badge resource */
            kExtendedFlagHasRoutingInfo = 0x0004    /* The file contains routing */
            /* info resource */
        }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public struct point
        {
            public short v { get; set; }
            public short h { get; set; }
        }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public struct rect
        {
            public short top { get; set; }
            public short left { get; set; }
            public short bottom { get; set; }
            public short right { get; set; }
        }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public struct fileInformation
        {
            public uint fileType { get; set; }
            public uint fileCreator { get; set; }            
            public bool isOnDesk { get; set; }
            public bool color { get; set; }
            public bool isShared { get; set; }
            public bool hasNoINITs { get; set; }
            public bool hasBeenInited { get; set; }
            public bool hasCustomIcon { get; set; }
            public bool isStationery { get; set; }
            public bool nameLocked { get; set; }
            public bool hasBundle { get; set; }
            public bool isInvisible { get; set; }
            public bool isAlias { get; set; }
            public point location { get; set; }
            public ushort reserved { get; set; }
        }
        public struct extendedFileInfo
        {
            public short[] reserved { get; set; }
            public bool ignoreOtherFlags { get; set; }
            public bool hasCustomBadge { get; set; }
            public bool hasRoutingInfo { get; set; }
            public short reserved2 { get; set; }
            public int putAwayFolderID { get; set; }
        }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public struct folderInfo
        {
            public rect windowBounds { get; set; }
            public bool isOnDesk { get; set; }
            public bool color { get; set; }
            public bool isShared { get; set; }
            public bool hasNoINITs { get; set; }
            public bool hasBeenInited { get; set; }
            public bool hasCustomIcon { get; set; }
            public bool isStationery { get; set; }
            public bool nameLocked { get; set; }
            public bool hasBundle { get; set; }
            public bool isInvisible { get; set; }
            public bool isAlias { get; set; }
            public point location { get; set; }
            public ushort reserved { get; set; }
        }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public struct extendedFolderInfo
        {
            public point scrollPosition { get; set; }
            public int reserved { get; set; }
            public bool ignoreOtherFlags { get; set; }
            public bool hasCustomBadge { get; set; }
            public bool hasRoutingInfo { get; set; }
            public short reserved2 { get; set; }
            public int putAwayFolderID { get; set; }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public static extendedFolderInfo getFolderFinderInfo(ref byte[] rawInfo)
        {
            extendedFolderInfo info = new extendedFolderInfo();

            point scrollPosition = new point();

            scrollPosition.v = dataOperations.convToLE(BitConverter.ToInt16(rawInfo, 0));
            scrollPosition.h = dataOperations.convToLE(BitConverter.ToInt16(rawInfo, 2));

            info.scrollPosition = scrollPosition;
            info.reserved = dataOperations.convToLE(BitConverter.ToInt32(rawInfo, 4));
            ushort extendedFinderFlags = dataOperations.convToLE(BitConverter.ToUInt16(rawInfo, 8));
            info.ignoreOtherFlags = (extendedFlags.kExtendedFlagsAreInvalid & (extendedFlags)extendedFinderFlags) == extendedFlags.kExtendedFlagsAreInvalid;
            info.hasCustomBadge = (extendedFlags.kExtendedFlagHasCustomBadge & (extendedFlags)extendedFinderFlags) == extendedFlags.kExtendedFlagHasCustomBadge;
            info.hasRoutingInfo = (extendedFlags.kExtendedFlagHasRoutingInfo & (extendedFlags)extendedFinderFlags) == extendedFlags.kExtendedFlagHasRoutingInfo;

            info.reserved2 = dataOperations.convToLE(BitConverter.ToInt16(rawInfo, 10));
            info.putAwayFolderID = dataOperations.convToLE(BitConverter.ToInt32(rawInfo, 12));

            return info;
        }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public static folderInfo getFolderUserInfo(ref byte[] rawInfo)
        {
            folderInfo info = new folderInfo();

            rect windowBounds = new rect();
            point location = new point();

            windowBounds.top = dataOperations.convToLE(BitConverter.ToInt16(rawInfo, 0));
            windowBounds.left = dataOperations.convToLE(BitConverter.ToInt16(rawInfo, 2));
            windowBounds.bottom = dataOperations.convToLE(BitConverter.ToInt16(rawInfo, 4));
            windowBounds.right = dataOperations.convToLE(BitConverter.ToInt16(rawInfo, 6));

            info.windowBounds = windowBounds;

            ushort ff = dataOperations.convToLE(BitConverter.ToUInt16(rawInfo, 8));
            info.isOnDesk=(finderFlags.kIsOnDesk & (finderFlags)ff) == finderFlags.kIsOnDesk;
            info.color = (finderFlags.kColor & (finderFlags)ff)==finderFlags.kColor;
            info.isShared=(finderFlags.kIsShared & (finderFlags)ff)==finderFlags.kIsShared;
            info.hasNoINITs=(finderFlags.kHasNoINITs & (finderFlags)ff)==finderFlags.kHasNoINITs;
            info.hasBeenInited=(finderFlags.kHasBeenInited & (finderFlags)ff)==finderFlags.kHasBeenInited;
            info.hasCustomIcon=(finderFlags.kHasCustomIcon & (finderFlags)ff)==finderFlags.kHasCustomIcon;
            info.isStationery=(finderFlags.kIsStationery & (finderFlags)ff)==finderFlags.kIsStationery;
            info.nameLocked=(finderFlags.kNameLocked & (finderFlags)ff)==finderFlags.kNameLocked;
            info.hasBundle=(finderFlags.kHasBundle & (finderFlags)ff)==finderFlags.kHasBundle;
            info.isInvisible=(finderFlags.kIsInvisible & (finderFlags)ff)==finderFlags.kIsInvisible;
            info.isAlias=(finderFlags.kIsAlias & (finderFlags)ff)==finderFlags.kIsAlias;

            location.v = dataOperations.convToLE(BitConverter.ToInt16(rawInfo, 10));
            location.h = dataOperations.convToLE(BitConverter.ToInt16(rawInfo, 12));
            info.location = location;
            info.reserved = dataOperations.convToLE(BitConverter.ToUInt16(rawInfo, 14));

            return info;
        }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public static fileInformation getFileUserInfo(ref byte[] rawInfo)
        {
            fileInformation info = new fileInformation();
            point location = new point();

            info.fileType = dataOperations.convToLE(BitConverter.ToUInt32(rawInfo, 0));
            info.fileCreator = dataOperations.convToLE(BitConverter.ToUInt32(rawInfo, 4)); 
            
            ushort ff = dataOperations.convToLE(BitConverter.ToUInt16(rawInfo, 8));
            info.isOnDesk = (finderFlags.kIsOnDesk & (finderFlags)ff) == finderFlags.kIsOnDesk;
            info.color = (finderFlags.kColor & (finderFlags)ff) == finderFlags.kColor;
            info.isShared = (finderFlags.kIsShared & (finderFlags)ff) == finderFlags.kIsShared;
            info.hasNoINITs = (finderFlags.kHasNoINITs & (finderFlags)ff) == finderFlags.kHasNoINITs;
            info.hasBeenInited = (finderFlags.kHasBeenInited & (finderFlags)ff) == finderFlags.kHasBeenInited;
            info.hasCustomIcon = (finderFlags.kHasCustomIcon & (finderFlags)ff) == finderFlags.kHasCustomIcon;
            info.isStationery = (finderFlags.kIsStationery & (finderFlags)ff) == finderFlags.kIsStationery;
            info.nameLocked = (finderFlags.kNameLocked & (finderFlags)ff) == finderFlags.kNameLocked;
            info.hasBundle = (finderFlags.kHasBundle & (finderFlags)ff) == finderFlags.kHasBundle;
            info.isInvisible = (finderFlags.kIsInvisible & (finderFlags)ff) == finderFlags.kIsInvisible;
            info.isAlias = (finderFlags.kIsAlias & (finderFlags)ff) == finderFlags.kIsAlias;

            location.v = dataOperations.convToLE(BitConverter.ToInt16(rawInfo, 10));
            location.h = dataOperations.convToLE(BitConverter.ToInt16(rawInfo, 12));

            info.location = location;
            info.reserved = dataOperations.convToLE(BitConverter.ToUInt16(rawInfo, 14));

            return info;
        }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public static extendedFileInfo getFileFinderInfo(ref byte[] rawInfo)
        {
            extendedFileInfo info = new extendedFileInfo();
            info.reserved = new short[4];

            Array.Copy(rawInfo, info.reserved, 4); 
            ushort extendedFinderFlags = dataOperations.convToLE(BitConverter.ToUInt16(rawInfo, 8));
            info.ignoreOtherFlags = (extendedFlags.kExtendedFlagsAreInvalid & (extendedFlags)extendedFinderFlags) == extendedFlags.kExtendedFlagsAreInvalid;
            info.hasCustomBadge = (extendedFlags.kExtendedFlagHasCustomBadge & (extendedFlags)extendedFinderFlags) == extendedFlags.kExtendedFlagHasCustomBadge;
            info.hasRoutingInfo = (extendedFlags.kExtendedFlagHasRoutingInfo & (extendedFlags)extendedFinderFlags) == extendedFlags.kExtendedFlagHasRoutingInfo;

            info.reserved2 = dataOperations.convToLE(BitConverter.ToInt16(rawInfo, 6));
            info.putAwayFolderID = dataOperations.convToLE(BitConverter.ToInt32(rawInfo, 8));

            return info;
        }
    }
}
