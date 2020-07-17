﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using SabreTools.Library.Data;
using SabreTools.Library.DatItems;
using SabreTools.Library.Tools;

namespace SabreTools.Library.DatFiles
{
    /// <summary>
    /// Represents the filtering operations that need to be performed on a set of items, usually a DAT
    /// </summary>
    public class Filter
    {
        #region Private instance variables

        private Dictionary<string, object> Filters { get; set; } = new Dictionary<string, object>()
        {
            #region Machine Filters

            { "Machine.Name", new FilterItem<string>() },
            { "Machine.Comment", new FilterItem<string>() },
            { "Machine.Description", new FilterItem<string>() },
            { "Machine.Year", new FilterItem<string>() },
            { "Machine.Manufacturer", new FilterItem<string>() },
            { "Machine.Publisher", new FilterItem<string>() },
            { "Machine.Category", new FilterItem<string>() },
            { "Machine.RomOf", new FilterItem<string>() },
            { "Machine.CloneOf", new FilterItem<string>() },
            { "Machine.SampleOf", new FilterItem<string>() },
            { "Machine.Supported", new FilterItem<bool?>() { Neutral = null } },
            { "Machine.SourceFile", new FilterItem<string>() },
            { "Machine.Runnable", new FilterItem<bool?>() { Neutral = null } },
            { "Machine.Board", new FilterItem<string>() },
            { "Machine.RebuildTo", new FilterItem<string>() },
            { "Machine.Devices", new FilterItem<string>() }, // TODO: List<string>
            { "Machine.SlotOptions", new FilterItem<string>() }, // TODO: List<string>
            { "Machine.Infos", new FilterItem<string>() }, // TODO: List<KeyValuePair<string, string>>
            { "Machine.MachineType", new FilterItem<MachineType>()  { Positive = Data.MachineType.NULL, Negative = Data.MachineType.NULL } },

            { "IncludeOfInGame", new FilterItem<bool>() { Neutral = false } },

            #endregion

            #region DatItem Filters

            { "DatItem.Type", new FilterItem<string>() },
            { "DatItem.Name", new FilterItem<string>() },
            { "DatItem.PartName", new FilterItem<string>() },
            { "DatItem.PartInterface", new FilterItem<string>() },
            { "DatItem.Features", new FilterItem<string>() }, // TODO: List<KeyValuePair<string, string>>
            { "DatItem.AreaName", new FilterItem<string>() },
            { "DatItem.AreaSize", new FilterItem<long?>() { Positive = null, Negative = null, Neutral = null } },
            { "DatItem.Default", new FilterItem<bool?>() { Neutral = null } },
            { "DatItem.Description", new FilterItem<string>() },
            { "DatItem.Size", new FilterItem<long>() { Positive = -1, Negative = -1, Neutral = -1 } },
            { "DatItem.CRC", new FilterItem<string>() },
            { "DatItem.MD5", new FilterItem<string>() },
#if NET_FRAMEWORK
            { "DatItem.RIPEMD160", new FilterItem<string>() },
#endif
            { "DatItem.SHA1", new FilterItem<string>() },
            { "DatItem.SHA256", new FilterItem<string>() },
            { "DatItem.SHA384", new FilterItem<string>() },
            { "DatItem.SHA512", new FilterItem<string>() },
            { "DatItem.Merge", new FilterItem<string>() },
            { "DatItem.Region", new FilterItem<string>() },
            { "DatItem.Index", new FilterItem<string>() },
            { "DatItem.Writable", new FilterItem<bool?>() { Neutral = null } },
            { "DatItem.Optional", new FilterItem<bool?>() { Neutral = null } },
            { "DatItem.Status", new FilterItem<ItemStatus>() { Positive = ItemStatus.NULL, Negative = ItemStatus.NULL } },
            { "DatItem.Language", new FilterItem<string>() },
            { "DatItem.Date", new FilterItem<string>() },
            { "DatItem.Bios", new FilterItem<string>() },
            { "DatItem.Offset", new FilterItem<string>() },

            #endregion

            #region Manipulation Filters

            { "Clean", new FilterItem<bool>() { Neutral = false } },
            { "DescriptionAsName", new FilterItem<bool>() { Neutral = false } },
            { "InternalSplit", new FilterItem<SplitType>() { Neutral = SplitType.None } },
            { "RemoveUnicode", new FilterItem<bool>() { Neutral = false } },
            { "Root", new FilterItem<string>() { Neutral = null } },
            { "Single", new FilterItem<bool>() { Neutral = false } },
            { "Trim", new FilterItem<bool>() { Neutral = false } },

            #endregion
        };

        #endregion

        #region Pubically facing variables

        #region Machine Filters

        /// <summary>
        /// Include or exclude machine names
        /// </summary>
        public FilterItem<string> MachineName
        {
            get { return Filters["Machine.Name"] as FilterItem<string>; }
            set { Filters["Machine.Name"] = value; }
        }

        /// <summary>
        /// Include or exclude machine comments
        /// </summary>
        public FilterItem<string> Comment
        {
            get { return Filters["Machine.Comment"] as FilterItem<string>; }
            set { Filters["Machine.Comment"] = value; }
        }

        /// <summary>
        /// Include or exclude machine descriptions
        /// </summary>
        public FilterItem<string> MachineDescription
        {
            get { return Filters["Machine.Description"] as FilterItem<string>; }
            set { Filters["Machine.Description"] = value; }
        }

        /// <summary>
        /// Include or exclude machine years
        /// </summary>
        public FilterItem<string> Year
        {
            get { return Filters["Machine.Year"] as FilterItem<string>; }
            set { Filters["Machine.Year"] = value; }
        }

        /// <summary>
        /// Include or exclude machine manufacturers
        /// </summary>
        public FilterItem<string> Manufacturer
        {
            get { return Filters["Machine.Manufacturer"] as FilterItem<string>; }
            set { Filters["Machine.Manufacturer"] = value; }
        }

        /// <summary>
        /// Include or exclude machine publishers
        /// </summary>
        public FilterItem<string> Publisher
        {
            get { return Filters["Machine.Publisher"] as FilterItem<string>; }
            set { Filters["Machine.Publisher"] = value; }
        }

        /// <summary>
        /// Include or exclude machine categories
        /// </summary>
        public FilterItem<string> Category
        {
            get { return Filters["Machine.Category"] as FilterItem<string>; }
            set { Filters["Machine.Category"] = value; }
        }

        /// <summary>
        /// Include or exclude machine romof
        /// </summary>
        public FilterItem<string> RomOf
        {
            get { return Filters["Machine.RomOf"] as FilterItem<string>; }
            set { Filters["Machine.RomOf"] = value; }
        }

        /// <summary>
        /// Include or exclude machine cloneof
        /// </summary>
        public FilterItem<string> CloneOf
        {
            get { return Filters["Machine.CloneOf"] as FilterItem<string>; }
            set { Filters["Machine.CloneOf"] = value; }
        }

        /// <summary>
        /// Include or exclude machine sampleof
        /// </summary>
        public FilterItem<string> SampleOf
        {
            get { return Filters["Machine.SampleOf"] as FilterItem<string>; }
            set { Filters["Machine.SampleOf"] = value; }
        }

        /// <summary>
        /// Include or exclude items with the "Supported" tag
        /// </summary>
        public FilterItem<bool?> Supported
        {
            get { return Filters["Machine.Supported"] as FilterItem<bool?>; }
            set { Filters["Machine.Supported"] = value; }
        }

        /// <summary>
        /// Include or exclude machine source file
        /// </summary>
        public FilterItem<string> SourceFile
        {
            get { return Filters["Machine.SourceFile"] as FilterItem<string>; }
            set { Filters["Machine.SourceFile"] = value; }
        }

        /// <summary>
        /// Include or exclude items with the "Runnable" tag
        /// </summary>
        public FilterItem<bool?> Runnable
        {
            get { return Filters["Machine.Runnable"] as FilterItem<bool?>; }
            set { Filters["Machine.Runnable"] = value; }
        }

        /// <summary>
        /// Include or exclude machine board
        /// </summary>
        public FilterItem<string> Board
        {
            get { return Filters["Machine.Board"] as FilterItem<string>; }
            set { Filters["Machine.Board"] = value; }
        }

        /// <summary>
        /// Include or exclude machine rebuildto
        /// </summary>
        public FilterItem<string> RebuildTo
        {
            get { return Filters["Machine.RebuildTo"] as FilterItem<string>; }
            set { Filters["Machine.RebuildTo"] = value; }
        }

        /// <summary>
        /// Include or exclude machine types
        /// </summary>
        public FilterItem<MachineType> MachineTypes
        {
            get { return Filters["Machine.MachineType"] as FilterItem<MachineType>; }
            set { Filters["Machine.MachineType"] = value; }
        }

        /// <summary>
        /// Include romof and cloneof when filtering machine names
        /// </summary>
        public FilterItem<bool> IncludeOfInGame
        {
            get { return Filters["IncludeOfInGame"] as FilterItem<bool>; }
            set { Filters["IncludeOfInGame"] = value; }
        }

        #endregion

        #region DatItem Filters

        /// <summary>
        /// Include or exclude item types
        /// </summary>
        public FilterItem<string> ItemTypes
        {
            get { return Filters["DatItem.Type"] as FilterItem<string>; }
            set { Filters["DatItem.Type"] = value; }
        }

        /// <summary>
        /// Include or exclude item names
        /// </summary>
        public FilterItem<string> ItemName
        {
            get { return Filters["DatItem.Name"] as FilterItem<string>; }
            set { Filters["DatItem.Name"] = value; }
        }

        /// <summary>
        /// Include or exclude part names
        /// </summary>
        public FilterItem<string> PartName
        {
            get { return Filters["DatItem.PartName"] as FilterItem<string>; }
            set { Filters["DatItem.PartName"] = value; }
        }

        /// <summary>
        /// Include or exclude part interfaces
        /// </summary>
        public FilterItem<string> PartInterface
        {
            get { return Filters["DatItem.PartInterface"] as FilterItem<string>; }
            set { Filters["DatItem.PartInterface"] = value; }
        }

        /// <summary>
        /// Include or exclude area names
        /// </summary>
        public FilterItem<string> AreaName
        {
            get { return Filters["DatItem.AreaName"] as FilterItem<string>; }
            set { Filters["DatItem.AreaName"] = value; }
        }

        /// <summary>
        /// Include or exclude area sizes
        /// </summary>
        public FilterItem<long?> AreaSize
        {
            get { return Filters["DatItem.AreaName"] as FilterItem<long?>; }
            set { Filters["DatItem.AreaName"] = value; }
        }

        /// <summary>
        /// Include or exclude items with the "Default" tag
        /// </summary>
        public FilterItem<bool?> Default
        {
            get { return Filters["DatItem.Default"] as FilterItem<bool?>; }
            set { Filters["DatItem.Default"] = value; }
        }

        /// <summary>
        /// Include or exclude descriptions
        /// </summary>
        public FilterItem<string> Description
        {
            get { return Filters["DatItem.Description"] as FilterItem<string>; }
            set { Filters["DatItem.Description"] = value; }
        }

        /// <summary>
        /// Include or exclude item sizes
        /// </summary>
        /// <remarks>Positive means "Greater than or equal", Negative means "Less than or equal", Neutral means "Equal"</remarks>
        public FilterItem<long> Size
        {
            get { return Filters["DatItem.Size"] as FilterItem<long>; }
            set { Filters["DatItem.Size"] = value; }
        }

        /// <summary>
        /// Include or exclude CRC32 hashes
        /// </summary>
        public FilterItem<string> CRC
        {
            get { return Filters["DatItem.CRC"] as FilterItem<string>; }
            set { Filters["DatItem.CRC"] = value; }
        }

        /// <summary>
        /// Include or exclude MD5 hashes
        /// </summary>
        public FilterItem<string> MD5
        {
            get { return Filters["DatItem.MD5"] as FilterItem<string>; }
            set { Filters["DatItem.MD5"] = value; }
        }

#if NET_FRAMEWORK
        /// <summary>
        /// Include or exclude RIPEMD160 hashes
        /// </summary>
        public FilterItem<string> RIPEMD160
        {
            get { return Filters["DatItem.RIPEMD160"] as FilterItem<string>; }
            set { Filters["DatItem.RIPEMD160"] = value; }
        }
#endif

        /// <summary>
        /// Include or exclude SHA-1 hashes
        /// </summary>
        public FilterItem<string> SHA1
        {
            get { return Filters["DatItem.SHA1"] as FilterItem<string>; }
            set { Filters["DatItem.SHA1"] = value; }
        }

        /// <summary>
        /// Include or exclude SHA-256 hashes
        /// </summary>
        public FilterItem<string> SHA256
        {
            get { return Filters["DatItem.SHA256"] as FilterItem<string>; }
            set { Filters["DatItem.SHA256"] = value; }
        }

        /// <summary>
        /// Include or exclude SHA-384 hashes
        /// </summary>
        public FilterItem<string> SHA384
        {
            get { return Filters["DatItem.SHA384"] as FilterItem<string>; }
            set { Filters["DatItem.SHA384"] = value; }
        }

        /// <summary>
        /// Include or exclude SHA-512 hashes
        /// </summary>
        public FilterItem<string> SHA512
        {
            get { return Filters["DatItem.SHA512"] as FilterItem<string>; }
            set { Filters["DatItem.SHA512"] = value; }
        }

        /// <summary>
        /// Include or exclude merge tags
        /// </summary>
        public FilterItem<string> MergeTag
        {
            get { return Filters["DatItem.Merge"] as FilterItem<string>; }
            set { Filters["DatItem.Merge"] = value; }
        }

        /// <summary>
        /// Include or exclude regions
        /// </summary>
        public FilterItem<string> Region
        {
            get { return Filters["DatItem.Region"] as FilterItem<string>; }
            set { Filters["DatItem.Region"] = value; }
        }

        /// <summary>
        /// Include or exclude indexes
        /// </summary>
        public FilterItem<string> Index
        {
            get { return Filters["DatItem.Index"] as FilterItem<string>; }
            set { Filters["DatItem.Index"] = value; }
        }

        /// <summary>
        /// Include or exclude items with the "Writable" tag
        /// </summary>
        public FilterItem<bool?> Writable
        {
            get { return Filters["DatItem.Writable"] as FilterItem<bool?>; }
            set { Filters["DatItem.Writable"] = value; }
        }

        /// <summary>
        /// Include or exclude items with the "Writable" tag
        /// </summary>
        public FilterItem<bool?> Optional
        {
            get { return Filters["DatItem.Optional"] as FilterItem<bool?>; }
            set { Filters["DatItem.Optional"] = value; }
        }

        /// <summary>
        /// Include or exclude item statuses
        /// </summary>
        public FilterItem<ItemStatus> Status
        {
            get { return Filters["DatItem.Status"] as FilterItem<ItemStatus>; }
            set { Filters["DatItem.Status"] = value; }
        }

        /// <summary>
        /// Include or exclude languages
        /// </summary>
        public FilterItem<string> Language
        {
            get { return Filters["DatItem.Language"] as FilterItem<string>; }
            set { Filters["DatItem.Language"] = value; }
        }

        /// <summary>
        /// Include or exclude dates
        /// </summary>
        public FilterItem<string> Date
        {
            get { return Filters["DatItem.Date"] as FilterItem<string>; }
            set { Filters["DatItem.Date"] = value; }
        }

        /// <summary>
        /// Include or exclude bioses
        /// </summary>
        public FilterItem<string> Bios
        {
            get { return Filters["DatItem.Bios"] as FilterItem<string>; }
            set { Filters["DatItem.Bios"] = value; }
        }

        /// <summary>
        /// Include or exclude offsets
        /// </summary>
        public FilterItem<string> Offset
        {
            get { return Filters["DatItem.Offset"] as FilterItem<string>; }
            set { Filters["DatItem.Offset"] = value; }
        }

        #endregion

        #region Manipulation Filters

        /// <summary>
        /// Clean all names to WoD standards
        /// </summary>
        public FilterItem<bool> Clean
        {
            get { return Filters["Clean"] as FilterItem<bool>; }
            set { Filters["Clean"] = value; }
        }

        /// <summary>
        /// Set Machine Description from Machine Name
        /// </summary>
        public FilterItem<bool> DescriptionAsName
        {
            get { return Filters["DescriptionAsName"] as FilterItem<bool>; }
            set { Filters["DescriptionAsName"] = value; }
        }

        /// <summary>
        /// Internally split a DatFile
        /// </summary>
        public FilterItem<SplitType> InternalSplit
        {
            get { return Filters["InternalSplit"] as FilterItem<SplitType>; }
            set { Filters["InternalSplit"] = value; }
        }

        /// <summary>
        /// Remove all unicode characters
        /// </summary>
        public FilterItem<bool> RemoveUnicode
        {
            get { return Filters["RemoveUnicode"] as FilterItem<bool>; }
            set { Filters["RemoveUnicode"] = value; }
        }

        /// <summary>
        /// Include root directory when determing trim sizes
        /// </summary>
        public FilterItem<string> Root
        {
            get { return Filters["Root"] as FilterItem<string>; }
            set { Filters["Root"] = value; }
        }

        /// <summary>
        /// Change all machine names to "!"
        /// </summary>
        public FilterItem<bool> Single
        {
            get { return Filters["Single"] as FilterItem<bool>; }
            set { Filters["Single"] = value; }
        }

        /// <summary>
        /// Trim total machine and item name to not exceed NTFS limits
        /// </summary>
        public FilterItem<bool> Trim
        {
            get { return Filters["Trim"] as FilterItem<bool>; }
            set { Filters["Trim"] = value; }
        }

        #endregion

        #endregion // Pubically facing variables

        #region Instance methods

        /// <summary>
        /// Filter a DatFile using the inputs
        /// </summary>
        /// <param name="datFile">DatFile to filter</param>
        /// <param name="useTags">True if DatFile tags override splitting, false otherwise</param>
        /// <returns>True if the DatFile was filtered, false on error</returns>
        public bool FilterDatFile(DatFile datFile, bool useTags)
        {
            try
            {
                // Loop over every key in the dictionary
                List<string> keys = datFile.Keys;
                foreach (string key in keys)
                {
                    // For every item in the current key
                    List<DatItem> items = datFile[key];
                    List<DatItem> newitems = new List<DatItem>();
                    foreach (DatItem item in items)
                    {
                        // If the rom passes the filter, include it
                        if (ItemPasses(item))
                        {
                            // If we're stripping unicode characters, do so from all relevant things
                            if (this.RemoveUnicode.Neutral)
                            {
                                item.Name = Sanitizer.RemoveUnicodeCharacters(item.Name);
                                item.MachineName = Sanitizer.RemoveUnicodeCharacters(item.MachineName);
                                item.MachineDescription = Sanitizer.RemoveUnicodeCharacters(item.MachineDescription);
                            }

                            // If we're in cleaning mode, do so from all relevant things
                            if (this.Clean.Neutral)
                            {
                                item.MachineName = Sanitizer.CleanGameName(item.MachineName);
                                item.MachineDescription = Sanitizer.CleanGameName(item.MachineDescription);
                            }

                            // If we are in single game mode, rename all games
                            if (this.Single.Neutral)
                                item.MachineName = "!";

                            // If we are in NTFS trim mode, trim the game name
                            if (this.Trim.Neutral)
                            {
                                // Windows max name length is 260
                                int usableLength = 260 - item.MachineName.Length - this.Root.Neutral.Length;
                                if (item.Name.Length > usableLength)
                                {
                                    string ext = Path.GetExtension(item.Name);
                                    item.Name = item.Name.Substring(0, usableLength - ext.Length);
                                    item.Name += ext;
                                }
                            }

                            // Lock the list and add the item back
                            lock (newitems)
                            {
                                newitems.Add(item);
                            }
                        }
                    }

                    datFile.Remove(key);
                    datFile.AddRange(key, newitems);
                }

                // Process description to machine name
                if (this.DescriptionAsName.Neutral)
                    MachineDescriptionToName(datFile);

                // If we are using tags from the DAT, set the proper input for split type unless overridden
                if (useTags && this.InternalSplit.Neutral == SplitType.None)
                    this.InternalSplit.Neutral = datFile.DatHeader.ForceMerging.AsSplitType();

                // Run internal splitting
                ProcessSplitType(datFile, this.InternalSplit.Neutral);

                // We remove any blanks, if we aren't supposed to have any
                if (!datFile.DatHeader.KeepEmptyGames)
                {
                    foreach (string key in datFile.Keys)
                    {
                        List<DatItem> items = datFile[key];
                        List<DatItem> newitems = items.Where(i => i.ItemType != ItemType.Blank).ToList();

                        datFile.Remove(key);
                        datFile.AddRange(key, newitems);
                    }
                }

                // If we are removing scene dates, do that now
                if (datFile.DatHeader.SceneDateStrip)
                    StripSceneDatesFromItems(datFile);

                // Run the one rom per game logic, if required
                if (datFile.DatHeader.OneRom)
                    OneRomPerGame(datFile);
            }
            catch (Exception ex)
            {
                Globals.Logger.Error(ex.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check to see if a DatItem passes the filter
        /// </summary>
        /// <param name="item">DatItem to check</param>
        /// <returns>True if the file passed the filter, false otherwise</returns>
        public bool ItemPasses(DatItem item)
        {
            // If the item is null, we automatically fail it
            if (item == null)
                return false;

            #region Machine Filters

            // Filter on machine name
            bool? machineNameFound = this.MachineName.MatchesPositiveSet(item.MachineName);
            if (this.IncludeOfInGame.Neutral)
            {
                machineNameFound |= (this.MachineName.MatchesPositiveSet(item.CloneOf) == true);
                machineNameFound |= (this.MachineName.MatchesPositiveSet(item.RomOf) == true);
            }
            if (machineNameFound == false)
                return false;

            machineNameFound = this.MachineName.MatchesNegativeSet(item.MachineName);
            if (this.IncludeOfInGame.Neutral)
            {
                machineNameFound |= (this.MachineName.MatchesNegativeSet(item.CloneOf) == true);
                machineNameFound |= (this.MachineName.MatchesNegativeSet(item.RomOf) == true);
            }
            if (machineNameFound == false)
                return false;

            // Filter on comment
            if (this.Comment.MatchesPositiveSet(item.Comment) == false)
                return false;
            if (this.Comment.MatchesNegativeSet(item.Comment) == true)
                return false;

            // Filter on machine description
            if (this.MachineDescription.MatchesPositiveSet(item.MachineDescription) == false)
                return false;
            if (this.MachineDescription.MatchesNegativeSet(item.MachineDescription) == true)
                return false;

            // Filter on year
            if (this.Year.MatchesPositiveSet(item.Year) == false)
                return false;
            if (this.Year.MatchesNegativeSet(item.Year) == true)
                return false;

            // Filter on manufacturer
            if (this.Manufacturer.MatchesPositiveSet(item.Manufacturer) == false)
                return false;
            if (this.Manufacturer.MatchesNegativeSet(item.Manufacturer) == true)
                return false;

            // Filter on publisher
            if (this.Publisher.MatchesPositiveSet(item.Publisher) == false)
                return false;
            if (this.Publisher.MatchesNegativeSet(item.Publisher) == true)
                return false;

            // Filter on category
            if (this.Category.MatchesPositiveSet(item.Category) == false)
                return false;
            if (this.Category.MatchesNegativeSet(item.Category) == true)
                return false;

            // Filter on romof
            if (this.RomOf.MatchesPositiveSet(item.RomOf) == false)
                return false;
            if (this.RomOf.MatchesNegativeSet(item.RomOf) == true)
                return false;

            // Filter on cloneof
            if (this.CloneOf.MatchesPositiveSet(item.CloneOf) == false)
                return false;
            if (this.CloneOf.MatchesNegativeSet(item.CloneOf) == true)
                return false;

            // Filter on sampleof
            if (this.SampleOf.MatchesPositiveSet(item.SampleOf) == false)
                return false;
            if (this.SampleOf.MatchesNegativeSet(item.SampleOf) == true)
                return false;

            // Filter on supported
            if (this.Supported.MatchesNeutral(null, item.Supported) == false)
                return false;

            // Filter on source file
            if (this.SourceFile.MatchesPositiveSet(item.SourceFile) == false)
                return false;
            if (this.SourceFile.MatchesNegativeSet(item.SourceFile) == true)
                return false;

            // Filter on runnable
            if (this.Runnable.MatchesNeutral(null, item.Runnable) == false)
                return false;

            // Filter on board
            if (this.Board.MatchesPositiveSet(item.Board) == false)
                return false;
            if (this.Board.MatchesNegativeSet(item.Board) == true)
                return false;

            // Filter on rebuildto
            if (this.RebuildTo.MatchesPositiveSet(item.RebuildTo) == false)
                return false;
            if (this.RebuildTo.MatchesNegativeSet(item.RebuildTo) == true)
                return false;

            // Filter on machine type
            if (this.MachineTypes.MatchesPositive(MachineType.NULL, item.MachineType) == false)
                return false;
            if (this.MachineTypes.MatchesNegative(MachineType.NULL, item.MachineType) == true)
                return false;

            #endregion

            #region DatItem Filters

            // Filter on item type
            if (this.ItemTypes.PositiveSet.Count == 0 && this.ItemTypes.NegativeSet.Count == 0
                && item.ItemType != ItemType.Rom && item.ItemType != ItemType.Disk && item.ItemType != ItemType.Blank)
                return false;
            if (this.ItemTypes.MatchesPositiveSet(item.ItemType.ToString()) == false)
                return false;
            if (this.ItemTypes.MatchesNegativeSet(item.ItemType.ToString()) == true)
                return false;

            // Filter on item name
            if (this.ItemName.MatchesPositiveSet(item.Name) == false)
                return false;
            if (this.ItemName.MatchesNegativeSet(item.Name) == true)
                return false;

            // Filter on part name
            if (this.PartName.MatchesPositiveSet(item.PartName) == false)
                return false;
            if (this.PartName.MatchesNegativeSet(item.PartName) == true)
                return false;

            // Filter on part interface
            if (this.PartInterface.MatchesPositiveSet(item.PartInterface) == false)
                return false;
            if (this.PartInterface.MatchesNegativeSet(item.PartInterface) == true)
                return false;

            // Filter on area name
            if (this.AreaName.MatchesPositiveSet(item.AreaName) == false)
                return false;
            if (this.AreaName.MatchesNegativeSet(item.AreaName) == true)
                return false;

            // Filter on area size
            if (this.AreaSize.MatchesNeutral(null, item.AreaSize) == false)
                return false;
            else if (this.AreaSize.MatchesPositive(null, item.AreaSize) == false)
                return false;
            else if (this.AreaSize.MatchesNegative(null, item.AreaSize) == false)
                return false;

            // Take care of item-specific differences
            switch (item.ItemType)
            {
                case ItemType.Archive:
                    // Archive has no special fields
                    break;

                case ItemType.BiosSet:
                    BiosSet biosSet = (BiosSet)item;

                    // Filter on description
                    if (this.Description.MatchesNeutral(null, biosSet.Description) == false)
                        return false;

                    // Filter on default
                    if (this.Default.MatchesNeutral(null, biosSet.Default) == false)
                        return false;

                    break;

                case ItemType.Blank:
                    // Blank has no special fields
                    break;

                case ItemType.Disk:
                    Disk disk = (Disk)item;

                    // Filter on MD5
                    if (this.MD5.MatchesPositiveSet(disk.MD5) == false)
                        return false;
                    if (this.MD5.MatchesNegativeSet(disk.MD5) == true)
                        return false;

#if NET_FRAMEWORK
                    // Filter on RIPEMD160
                    if (this.RIPEMD160.MatchesPositiveSet(disk.RIPEMD160) == false)
                        return false;
                    if (this.RIPEMD160.MatchesNegativeSet(disk.RIPEMD160) == true)
                        return false;
#endif

                    // Filter on SHA-1
                    if (this.SHA1.MatchesPositiveSet(disk.SHA1) == false)
                        return false;
                    if (this.SHA1.MatchesNegativeSet(disk.SHA1) == true)
                        return false;

                    // Filter on SHA-256
                    if (this.SHA256.MatchesPositiveSet(disk.SHA256) == false)
                        return false;
                    if (this.SHA256.MatchesNegativeSet(disk.SHA256) == true)
                        return false;

                    // Filter on SHA-384
                    if (this.SHA384.MatchesPositiveSet(disk.SHA384) == false)
                        return false;
                    if (this.SHA384.MatchesNegativeSet(disk.SHA384) == true)
                        return false;

                    // Filter on SHA-512
                    if (this.SHA512.MatchesPositiveSet(disk.SHA512) == false)
                        return false;
                    if (this.SHA512.MatchesNegativeSet(disk.SHA512) == true)
                        return false;

                    // Filter on merge tag
                    if (this.MergeTag.MatchesPositiveSet(disk.MergeTag) == false)
                        return false;
                    if (this.MergeTag.MatchesNegativeSet(disk.MergeTag) == true)
                        return false;

                    // Filter on region
                    if (this.Region.MatchesPositiveSet(disk.Region) == false)
                        return false;
                    if (this.Region.MatchesNegativeSet(disk.Region) == true)
                        return false;

                    // Filter on index
                    if (this.Index.MatchesPositiveSet(disk.Index) == false)
                        return false;
                    if (this.Index.MatchesNegativeSet(disk.Index) == true)
                        return false;

                    // Filter on writable
                    if (this.Writable.MatchesNeutral(null, disk.Writable) == false)
                        return false;

                    // Filter on status
                    if (this.Status.MatchesPositive(ItemStatus.NULL, disk.ItemStatus) == false)
                        return false;
                    if (this.Status.MatchesNegative(ItemStatus.NULL, disk.ItemStatus) == true)
                        return false;

                    // Filter on optional
                    if (this.Optional.MatchesNeutral(null, disk.Optional) == false)
                        return false;

                    break;

                case ItemType.Release:
                    Release release = (Release)item;

                    // Filter on region
                    if (this.Region.MatchesPositiveSet(release.Region) == false)
                        return false;
                    if (this.Region.MatchesNegativeSet(release.Region) == true)
                        return false;

                    // Filter on language
                    if (this.Language.MatchesPositiveSet(release.Language) == false)
                        return false;
                    if (this.Language.MatchesNegativeSet(release.Language) == true)
                        return false;

                    // Filter on date
                    if (this.Date.MatchesPositiveSet(release.Date) == false)
                        return false;
                    if (this.Date.MatchesNegativeSet(release.Date) == true)
                        return false;

                    // Filter on default
                    if (this.Default.MatchesNeutral(null, release.Default) == false)
                        return false;

                    break;

                case ItemType.Rom:
                    Rom rom = (Rom)item;

                    // Filter on bios
                    if (this.Bios.MatchesPositiveSet(rom.Bios) == false)
                        return false;
                    if (this.Bios.MatchesNegativeSet(rom.Bios) == true)
                        return false;

                    // Filter on rom size
                    if (this.Size.MatchesNeutral(-1, rom.Size) == false)
                        return false;
                    else if (this.Size.MatchesPositive(-1, rom.Size) == false)
                        return false;
                    else if (this.Size.MatchesNegative(-1, rom.Size) == false)
                        return false;

                    // Filter on CRC
                    if (this.CRC.MatchesPositiveSet(rom.CRC) == false)
                        return false;
                    if (this.CRC.MatchesNegativeSet(rom.CRC) == true)
                        return false;

                    // Filter on MD5
                    if (this.MD5.MatchesPositiveSet(rom.MD5) == false)
                        return false;
                    if (this.MD5.MatchesNegativeSet(rom.MD5) == true)
                        return false;

#if NET_FRAMEWORK
                    // Filter on RIPEMD160
                    if (this.RIPEMD160.MatchesPositiveSet(rom.RIPEMD160) == false)
                        return false;
                    if (this.RIPEMD160.MatchesNegativeSet(rom.RIPEMD160) == true)
                        return false;
#endif

                    // Filter on SHA-1
                    if (this.SHA1.MatchesPositiveSet(rom.SHA1) == false)
                        return false;
                    if (this.SHA1.MatchesNegativeSet(rom.SHA1) == true)
                        return false;

                    // Filter on SHA-256
                    if (this.SHA256.MatchesPositiveSet(rom.SHA256) == false)
                        return false;
                    if (this.SHA256.MatchesNegativeSet(rom.SHA256) == true)
                        return false;

                    // Filter on SHA-384
                    if (this.SHA384.MatchesPositiveSet(rom.SHA384) == false)
                        return false;
                    if (this.SHA384.MatchesNegativeSet(rom.SHA384) == true)
                        return false;

                    // Filter on SHA-512
                    if (this.SHA512.MatchesPositiveSet(rom.SHA512) == false)
                        return false;
                    if (this.SHA512.MatchesNegativeSet(rom.SHA512) == true)
                        return false;

                    // Filter on merge tag
                    if (this.MergeTag.MatchesPositiveSet(rom.MergeTag) == false)
                        return false;
                    if (this.MergeTag.MatchesNegativeSet(rom.MergeTag) == true)
                        return false;

                    // Filter on region
                    if (this.Region.MatchesPositiveSet(rom.Region) == false)
                        return false;
                    if (this.Region.MatchesNegativeSet(rom.Region) == true)
                        return false;

                    // Filter on offset
                    if (this.Offset.MatchesPositiveSet(rom.Offset) == false)
                        return false;
                    if (this.Offset.MatchesNegativeSet(rom.Offset) == true)
                        return false;

                    // Filter on date
                    if (this.Date.MatchesPositiveSet(rom.Date) == false)
                        return false;
                    if (this.Date.MatchesNegativeSet(rom.Date) == true)
                        return false;

                    // Filter on status
                    if (this.Status.MatchesPositive(ItemStatus.NULL, rom.ItemStatus) == false)
                        return false;
                    if (this.Status.MatchesNegative(ItemStatus.NULL, rom.ItemStatus) == true)
                        return false;

                    // Filter on optional
                    if (this.Optional.MatchesNeutral(null, rom.Optional) == false)
                        return false;

                    break;

                case ItemType.Sample:
                    // Sample has no special fields
                    break;
            }

            #endregion

            return true;
        }

        #region Internal Splitting/Merging

        /// <summary>
        /// Process items according to SplitType
        /// </summary>
        /// <param name="datFile">DatFile to filter</param>
        /// <param name="splitType">SplitType to implement</param>
        private void ProcessSplitType(DatFile datFile, SplitType splitType)
        {
            // Now we pre-process the DAT with the splitting/merging mode
            switch (splitType)
            {
                case SplitType.None:
                    // No-op
                    break;
                case SplitType.DeviceNonMerged:
                    CreateDeviceNonMergedSets(datFile, DedupeType.None);
                    break;
                case SplitType.FullNonMerged:
                    CreateFullyNonMergedSets(datFile, DedupeType.None);
                    break;
                case SplitType.NonMerged:
                    CreateNonMergedSets(datFile, DedupeType.None);
                    break;
                case SplitType.Merged:
                    CreateMergedSets(datFile, DedupeType.None);
                    break;
                case SplitType.Split:
                    CreateSplitSets(datFile, DedupeType.None);
                    break;
            }
        }

        /// <summary>
        /// Use cdevice_ref tags to get full non-merged sets and remove parenting tags
        /// </summary>
        /// <param name="datFile">DatFile to filter</param>
        /// <param name="mergeroms">Dedupe type to be used</param>
        private void CreateDeviceNonMergedSets(DatFile datFile, DedupeType mergeroms)
        {
            Globals.Logger.User("Creating device non-merged sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            datFile.BucketBy(BucketedBy.Game, mergeroms, norename: true);

            // Now we want to loop through all of the games and set the correct information
            while (AddRomsFromDevices(datFile, false, false)) ;
            while (AddRomsFromDevices(datFile, true, false)) ;

            // Then, remove the romof and cloneof tags so it's not picked up by the manager
            RemoveTagsFromChild(datFile);
        }

        /// <summary>
        /// Use cloneof tags to create non-merged sets and remove the tags plus using the device_ref tags to get full sets
        /// </summary>
        /// <param name="datFile">DatFile to filter</param>
        /// <param name="mergeroms">Dedupe type to be used</param>
        private void CreateFullyNonMergedSets(DatFile datFile, DedupeType mergeroms)
        {
            Globals.Logger.User("Creating fully non-merged sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            datFile.BucketBy(BucketedBy.Game, mergeroms, norename: true);

            // Now we want to loop through all of the games and set the correct information
            while (AddRomsFromDevices(datFile, true, true)) ;
            AddRomsFromDevices(datFile, false, true);
            AddRomsFromParent(datFile);

            // Now that we have looped through the cloneof tags, we loop through the romof tags
            AddRomsFromBios(datFile);

            // Then, remove the romof and cloneof tags so it's not picked up by the manager
            RemoveTagsFromChild(datFile);
        }

        /// <summary>
        /// Use cloneof tags to create merged sets and remove the tags
        /// </summary>
        /// <param name="datFile">DatFile to filter</param>
        /// <param name="mergeroms">Dedupe type to be used</param>
        private void CreateMergedSets(DatFile datFile, DedupeType mergeroms)
        {
            Globals.Logger.User("Creating merged sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            datFile.BucketBy(BucketedBy.Game, mergeroms, norename: true);

            // Now we want to loop through all of the games and set the correct information
            AddRomsFromChildren(datFile);

            // Now that we have looped through the cloneof tags, we loop through the romof tags
            RemoveBiosRomsFromChild(datFile, false);
            RemoveBiosRomsFromChild(datFile, true);

            // Finally, remove the romof and cloneof tags so it's not picked up by the manager
            RemoveTagsFromChild(datFile);
        }

        /// <summary>
        /// Use cloneof tags to create non-merged sets and remove the tags
        /// </summary>
        /// <param name="datFile">DatFile to filter</param>
        /// <param name="mergeroms">Dedupe type to be used</param>
        private void CreateNonMergedSets(DatFile datFile, DedupeType mergeroms)
        {
            Globals.Logger.User("Creating non-merged sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            datFile.BucketBy(BucketedBy.Game, mergeroms, norename: true);

            // Now we want to loop through all of the games and set the correct information
            AddRomsFromParent(datFile);

            // Now that we have looped through the cloneof tags, we loop through the romof tags
            RemoveBiosRomsFromChild(datFile, false);
            RemoveBiosRomsFromChild(datFile, true);

            // Finally, remove the romof and cloneof tags so it's not picked up by the manager
            RemoveTagsFromChild(datFile);
        }

        /// <summary>
        /// Use cloneof and romof tags to create split sets and remove the tags
        /// </summary>
        /// <param name="datFile">DatFile to filter</param>
        /// <param name="mergeroms">Dedupe type to be used</param>
        private void CreateSplitSets(DatFile datFile, DedupeType mergeroms)
        {
            Globals.Logger.User("Creating split sets from the DAT");

            // For sake of ease, the first thing we want to do is bucket by game
            datFile.BucketBy(BucketedBy.Game, mergeroms, norename: true);

            // Now we want to loop through all of the games and set the correct information
            RemoveRomsFromChild(datFile);

            // Now that we have looped through the cloneof tags, we loop through the romof tags
            RemoveBiosRomsFromChild(datFile, false);
            RemoveBiosRomsFromChild(datFile, true);

            // Finally, remove the romof and cloneof tags so it's not picked up by the manager
            RemoveTagsFromChild(datFile);
        }

        /// <summary>
        /// Use romof tags to add roms to the children
        /// </summary>
        /// <param name="datFile">DatFile to filter</param>
        private void AddRomsFromBios(DatFile datFile)
        {
            List<string> games = datFile.Keys;
            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                if (datFile[game].Count == 0)
                    continue;

                // Determine if the game has a parent or not
                string parent = null;
                if (!string.IsNullOrWhiteSpace(datFile[game][0].RomOf))
                    parent = datFile[game][0].RomOf;

                // If the parent doesnt exist, we want to continue
                if (string.IsNullOrWhiteSpace(parent))
                    continue;

                // If the parent doesn't have any items, we want to continue
                if (datFile[parent].Count == 0)
                    continue;

                // If the parent exists and has items, we copy the items from the parent to the current game
                DatItem copyFrom = datFile[game][0];
                List<DatItem> parentItems = datFile[parent];
                foreach (DatItem item in parentItems)
                {
                    DatItem datItem = (DatItem)item.Clone();
                    datItem.CopyMachineInformation(copyFrom);
                    if (datFile[game].Where(i => i.Name == datItem.Name).Count() == 0 && !datFile[game].Contains(datItem))
                        datFile.Add(game, datItem);
                }
            }
        }

        /// <summary>
        /// Use device_ref and optionally slotoption tags to add roms to the children
        /// </summary>
        /// <param name="datFile">DatFile to filter</param>
        /// <param name="dev">True if only child device sets are touched, false for non-device sets (default)</param>
        /// <param name="slotoptions">True if slotoptions tags are used as well, false otherwise</param>
        private bool AddRomsFromDevices(DatFile datFile, bool dev = false, bool slotoptions = false)
        {
            bool foundnew = false;
            List<string> games = datFile.Keys;
            foreach (string game in games)
            {
                // If the game doesn't have items, we continue
                if (datFile[game] == null || datFile[game].Count == 0)
                    continue;

                // If the game (is/is not) a bios, we want to continue
                if (dev ^ (datFile[game][0].MachineType.HasFlag(Data.MachineType.Device)))
                    continue;

                // If the game has no devices, we continue
                if (datFile[game][0].Devices == null
                    || datFile[game][0].Devices.Count == 0
                    || (slotoptions && datFile[game][0].SlotOptions == null)
                    || (slotoptions && datFile[game][0].SlotOptions.Count == 0))
                {
                    continue;
                }

                // Determine if the game has any devices or not
                List<string> devices = datFile[game][0].Devices;
                List<string> newdevs = new List<string>();
                foreach (string device in devices)
                {
                    // If the device doesn't exist then we continue
                    if (datFile[device].Count == 0)
                        continue;

                    // Otherwise, copy the items from the device to the current game
                    DatItem copyFrom = datFile[game][0];
                    List<DatItem> devItems = datFile[device];
                    foreach (DatItem item in devItems)
                    {
                        DatItem datItem = (DatItem)item.Clone();
                        newdevs.AddRange(datItem.Devices ?? new List<string>());
                        datItem.CopyMachineInformation(copyFrom);
                        if (datFile[game].Where(i => i.Name.ToLowerInvariant() == datItem.Name.ToLowerInvariant()).Count() == 0)
                        {
                            foundnew = true;
                            datFile.Add(game, datItem);
                        }
                    }
                }

                // Now that every device is accounted for, add the new list of devices, if they don't already exist
                foreach (string device in newdevs)
                {
                    if (!datFile[game][0].Devices.Contains(device))
                        datFile[game][0].Devices.Add(device);
                }

                // If we're checking slotoptions too
                if (slotoptions)
                {
                    // Determine if the game has any slotoptions or not
                    List<string> slotopts = datFile[game][0].SlotOptions;
                    List<string> newslotopts = new List<string>();
                    foreach (string slotopt in slotopts)
                    {
                        // If the slotoption doesn't exist then we continue
                        if (datFile[slotopt].Count == 0)
                            continue;

                        // Otherwise, copy the items from the slotoption to the current game
                        DatItem copyFrom = datFile[game][0];
                        List<DatItem> slotItems = datFile[slotopt];
                        foreach (DatItem item in slotItems)
                        {
                            DatItem datItem = (DatItem)item.Clone();
                            newslotopts.AddRange(datItem.SlotOptions ?? new List<string>());
                            datItem.CopyMachineInformation(copyFrom);
                            if (datFile[game].Where(i => i.Name.ToLowerInvariant() == datItem.Name.ToLowerInvariant()).Count() == 0)
                            {
                                foundnew = true;
                                datFile.Add(game, datItem);
                            }
                        }
                    }

                    // Now that every slotoption is accounted for, add the new list of slotoptions, if they don't already exist
                    foreach (string slotopt in newslotopts)
                    {
                        if (!datFile[game][0].SlotOptions.Contains(slotopt))
                            datFile[game][0].SlotOptions.Add(slotopt);
                    }
                }
            }

            return foundnew;
        }

        /// <summary>
        /// Use cloneof tags to add roms to the children, setting the new romof tag in the process
        /// </summary>
        /// <param name="datFile">DatFile to filter</param>
        private void AddRomsFromParent(DatFile datFile)
        {
            List<string> games = datFile.Keys;
            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                if (datFile[game].Count == 0)
                    continue;

                // Determine if the game has a parent or not
                string parent = null;
                if (!string.IsNullOrWhiteSpace(datFile[game][0].CloneOf))
                    parent = datFile[game][0].CloneOf;

                // If the parent doesnt exist, we want to continue
                if (string.IsNullOrWhiteSpace(parent))
                    continue;

                // If the parent doesn't have any items, we want to continue
                if (datFile[parent].Count == 0)
                    continue;

                // If the parent exists and has items, we copy the items from the parent to the current game
                DatItem copyFrom = datFile[game][0];
                List<DatItem> parentItems = datFile[parent];
                foreach (DatItem item in parentItems)
                {
                    DatItem datItem = (DatItem)item.Clone();
                    datItem.CopyMachineInformation(copyFrom);
                    if (datFile[game].Where(i => i.Name.ToLowerInvariant() == datItem.Name.ToLowerInvariant()).Count() == 0
                        && !datFile[game].Contains(datItem))
                    {
                        datFile.Add(game, datItem);
                    }
                }

                // Now we want to get the parent romof tag and put it in each of the items
                List<DatItem> items = datFile[game];
                string romof = datFile[parent][0].RomOf;
                foreach (DatItem item in items)
                {
                    item.RomOf = romof;
                }
            }
        }

        /// <summary>
        /// Use cloneof tags to add roms to the parents, removing the child sets in the process
        /// </summary>
        /// <param name="datFile">DatFile to filter</param>
        private void AddRomsFromChildren(DatFile datFile)
        {
            List<string> games = datFile.Keys;
            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                if (datFile[game].Count == 0)
                    continue;

                // Determine if the game has a parent or not
                string parent = null;
                if (!string.IsNullOrWhiteSpace(datFile[game][0].CloneOf))
                    parent = datFile[game][0].CloneOf;

                // If there is no parent, then we continue
                if (string.IsNullOrWhiteSpace(parent))
                    continue;

                // Otherwise, move the items from the current game to a subfolder of the parent game
                DatItem copyFrom = datFile[parent].Count == 0 ? new Rom { MachineName = parent, MachineDescription = parent } : datFile[parent][0];
                List<DatItem> items = datFile[game];
                foreach (DatItem item in items)
                {
                    // If the disk doesn't have a valid merge tag OR the merged file doesn't exist in the parent, then add it
                    if (item.ItemType == Data.ItemType.Disk && (((Disk)item).MergeTag == null || !datFile[parent].Select(i => i.Name).Contains(((Disk)item).MergeTag)))
                    {
                        item.CopyMachineInformation(copyFrom);
                        datFile.Add(parent, item);
                    }

                    // Otherwise, if the parent doesn't already contain the non-disk (or a merge-equivalent), add it
                    else if (item.ItemType != Data.ItemType.Disk && !datFile[parent].Contains(item))
                    {
                        // Rename the child so it's in a subfolder
                        item.Name = $"{item.MachineName}\\{item.Name}";

                        // Update the machine to be the new parent
                        item.CopyMachineInformation(copyFrom);

                        // Add the rom to the parent set
                        datFile.Add(parent, item);
                    }
                }

                // Then, remove the old game so it's not picked up by the writer
                datFile.Remove(game);
            }
        }

        /// <summary>
        /// Remove all BIOS and device sets
        /// </summary>
        /// <param name="datFile">DatFile to filter</param>
        private void RemoveBiosAndDeviceSets(DatFile datFile)
        {
            List<string> games = datFile.Keys;
            foreach (string game in games)
            {
                if (datFile[game].Count > 0
                    && (datFile[game][0].MachineType.HasFlag(Data.MachineType.Bios)
                        || datFile[game][0].MachineType.HasFlag(Data.MachineType.Device)))
                {
                    datFile.Remove(game);
                }
            }
        }

        /// <summary>
        /// Use romof tags to remove bios roms from children
        /// </summary>
        /// <param name="datFile">DatFile to filter</param>
        /// <param name="bios">True if only child Bios sets are touched, false for non-bios sets (default)</param>
        private void RemoveBiosRomsFromChild(DatFile datFile, bool bios = false)
        {
            // Loop through the romof tags
            List<string> games = datFile.Keys;
            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                if (datFile[game].Count == 0)
                    continue;

                // If the game (is/is not) a bios, we want to continue
                if (bios ^ datFile[game][0].MachineType.HasFlag(Data.MachineType.Bios))
                    continue;

                // Determine if the game has a parent or not
                string parent = null;
                if (!string.IsNullOrWhiteSpace(datFile[game][0].RomOf))
                    parent = datFile[game][0].RomOf;

                // If the parent doesnt exist, we want to continue
                if (string.IsNullOrWhiteSpace(parent))
                    continue;

                // If the parent doesn't have any items, we want to continue
                if (datFile[parent].Count == 0)
                    continue;

                // If the parent exists and has items, we remove the items that are in the parent from the current game
                List<DatItem> parentItems = datFile[parent];
                foreach (DatItem item in parentItems)
                {
                    DatItem datItem = (DatItem)item.Clone();
                    while (datFile[game].Contains(datItem))
                    {
                        datFile.Remove(game, datItem);
                    }
                }
            }
        }

        /// <summary>
        /// Use cloneof tags to remove roms from the children
        /// </summary>
        /// <param name="datFile">DatFile to filter</param>
        private void RemoveRomsFromChild(DatFile datFile)
        {
            List<string> games = datFile.Keys;
            foreach (string game in games)
            {
                // If the game has no items in it, we want to continue
                if (datFile[game].Count == 0)
                    continue;

                // Determine if the game has a parent or not
                string parent = null;
                if (!string.IsNullOrWhiteSpace(datFile[game][0].CloneOf))
                    parent = datFile[game][0].CloneOf;

                // If the parent doesnt exist, we want to continue
                if (string.IsNullOrWhiteSpace(parent))
                    continue;

                // If the parent doesn't have any items, we want to continue
                if (datFile[parent].Count == 0)
                    continue;

                // If the parent exists and has items, we remove the parent items from the current game
                List<DatItem> parentItems = datFile[parent];
                foreach (DatItem item in parentItems)
                {
                    DatItem datItem = (DatItem)item.Clone();
                    while (datFile[game].Contains(datItem))
                    {
                        datFile.Remove(game, datItem);
                    }
                }

                // Now we want to get the parent romof tag and put it in each of the remaining items
                List<DatItem> items = datFile[game];
                string romof = datFile[parent][0].RomOf;
                foreach (DatItem item in items)
                {
                    item.RomOf = romof;
                }
            }
        }

        /// <summary>
        /// Remove all romof and cloneof tags from all games
        /// </summary>
        /// <param name="datFile">DatFile to filter</param>
        private void RemoveTagsFromChild(DatFile datFile)
        {
            List<string> games = datFile.Keys;
            foreach (string game in games)
            {
                List<DatItem> items = datFile[game];
                foreach (DatItem item in items)
                {
                    item.CloneOf = null;
                    item.RomOf = null;
                }
            }
        }

        #endregion

        #region Manipulation

        /// <summary>
        /// Use game descriptions as names in the DAT, updating cloneof/romof/sampleof
        /// </summary>
        /// <param name="datFile">DatFile to filter</param>
        private void MachineDescriptionToName(DatFile datFile)
        {
            try
            {
                // First we want to get a mapping for all games to description
                ConcurrentDictionary<string, string> mapping = new ConcurrentDictionary<string, string>();
                List<string> keys = datFile.Keys;
                Parallel.ForEach(keys, Globals.ParallelOptions, key =>
                {
                    List<DatItem> items = datFile[key];
                    foreach (DatItem item in items)
                    {
                        // If the key mapping doesn't exist, add it
                        mapping.TryAdd(item.MachineName, item.MachineDescription.Replace('/', '_').Replace("\"", "''").Replace(":", " -"));
                    }
                });

                // Now we loop through every item and update accordingly
                keys = datFile.Keys;
                Parallel.ForEach(keys, Globals.ParallelOptions, key =>
                {
                    List<DatItem> items = datFile[key];
                    List<DatItem> newItems = new List<DatItem>();
                    foreach (DatItem item in items)
                    {
                        // Update machine name
                        if (!string.IsNullOrWhiteSpace(item.MachineName) && mapping.ContainsKey(item.MachineName))
                            item.MachineName = mapping[item.MachineName];

                        // Update cloneof
                        if (!string.IsNullOrWhiteSpace(item.CloneOf) && mapping.ContainsKey(item.CloneOf))
                            item.CloneOf = mapping[item.CloneOf];

                        // Update romof
                        if (!string.IsNullOrWhiteSpace(item.RomOf) && mapping.ContainsKey(item.RomOf))
                            item.RomOf = mapping[item.RomOf];

                        // Update sampleof
                        if (!string.IsNullOrWhiteSpace(item.SampleOf) && mapping.ContainsKey(item.SampleOf))
                            item.SampleOf = mapping[item.SampleOf];

                        // Add the new item to the output list
                        newItems.Add(item);
                    }

                    // Replace the old list of roms with the new one
                    datFile.Remove(key);
                    datFile.AddRange(key, newItems);
                });
            }
            catch (Exception ex)
            {
                Globals.Logger.Warning(ex.ToString());
            }
        }

        /// <summary>
        /// Ensure that all roms are in their own game (or at least try to ensure)
        /// </summary>
        /// <param name="datFile">DatFile to filter</param>
        /// TODO: This is incorrect for the actual 1G1R logic... this is actually just silly
        private void OneRomPerGame(DatFile datFile)
        {
            // For each rom, we want to update the game to be "<game name>/<rom name>"
            Parallel.ForEach(datFile.Keys, Globals.ParallelOptions, key =>
            {
                List<DatItem> items = datFile[key];
                for (int i = 0; i < items.Count; i++)
                {
                    string[] splitname = items[i].Name.Split('.');
                    items[i].MachineName += $"/{string.Join(".", splitname.Take(splitname.Length > 1 ? splitname.Length - 1 : 1))}";
                }
            });
        }

        /// <summary>
        /// Strip the dates from the beginning of scene-style set names
        /// </summary>
        /// <param name="datFile">DatFile to filter</param>
        private void StripSceneDatesFromItems(DatFile datFile)
        {
            // Output the logging statement
            Globals.Logger.User("Stripping scene-style dates");

            // Set the regex pattern to use
            string pattern = @"([0-9]{2}\.[0-9]{2}\.[0-9]{2}-)(.*?-.*?)";

            // Now process all of the roms
            List<string> keys = datFile.Keys;
            Parallel.ForEach(keys, Globals.ParallelOptions, key =>
            {
                List<DatItem> items = datFile[key];
                for (int j = 0; j < items.Count; j++)
                {
                    DatItem item = items[j];
                    if (Regex.IsMatch(item.MachineName, pattern))
                        item.MachineName = Regex.Replace(item.MachineName, pattern, "$2");

                    if (Regex.IsMatch(item.MachineDescription, pattern))
                        item.MachineDescription = Regex.Replace(item.MachineDescription, pattern, "$2");

                    items[j] = item;
                }

                datFile.Remove(key);
                datFile.AddRange(key, items);
            });
        }

        #endregion

        #endregion // Instance Methods
    }
}
