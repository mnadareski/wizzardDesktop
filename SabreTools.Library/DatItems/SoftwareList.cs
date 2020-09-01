﻿using System.Collections.Generic;
using System.Linq;

using SabreTools.Library.Filtering;
using SabreTools.Library.Tools;
using Newtonsoft.Json;

namespace SabreTools.Library.DatItems
{
    /// <summary>
    /// Represents which SoftwareList(s) is associated with a set
    /// </summary>
    [JsonObject("softwarelist")]
    public class SoftwareList : DatItem
    {
        #region Fields

        [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public SoftwareListStatus Status { get; set; }

        [JsonProperty("filter", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Filter { get; set; }

        #endregion

        #region Accessors

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="mappings">Mappings dictionary</param>
        public override void SetFields(Dictionary<Field, string> mappings)
        {
            // Set base fields
            base.SetFields(mappings);

            // Handle SoftwareList-specific fields
            if (mappings.Keys.Contains(Field.DatItem_SoftwareListStatus))
                Status = mappings[Field.DatItem_Default].AsSoftwareListStatus();

            if (mappings.Keys.Contains(Field.DatItem_Filter))
                Filter = mappings[Field.DatItem_Filter];
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty SoftwareList object
        /// </summary>
        public SoftwareList()
        {
            Name = string.Empty;
            ItemType = ItemType.SoftwareList;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new SoftwareList()
            {
                Name = this.Name,
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                AltName = this.AltName,
                AltTitle = this.AltTitle,

                Original = this.Original,
                OpenMSXSubType = this.OpenMSXSubType,
                OpenMSXType = this.OpenMSXType,
                Remark = this.Remark,
                Boot = this.Boot,

                Part = this.Part,
                Features = this.Features,
                AreaName = this.AreaName,
                AreaSize = this.AreaSize,
                AreaWidth = this.AreaWidth,
                AreaEndianness = this.AreaEndianness,
                Value = this.Value,
                LoadFlag = this.LoadFlag,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Status = this.Status,
                Filter = this.Filter,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a sample, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a SoftwareList
            SoftwareList newOther = other as SoftwareList;

            // If the SoftwareList information matches
            return (Name == newOther.Name
                && Status == newOther.Status
                && Filter == newOther.Filter);
        }

        #endregion

        #region Filtering

        /// <summary>
        /// Check to see if a DatItem passes the filter
        /// </summary>
        /// <param name="filter">Filter to check against</param>
        /// <returns>True if the item passed the filter, false otherwise</returns>
        public override bool PassesFilter(Filter filter)
        {
            // Check common fields first
            if (!base.PassesFilter(filter))
                return false;

            // Filter on status
            if (filter.DatItem_SoftwareListStatus.MatchesPositive(SoftwareListStatus.NULL, Status) == false)
                return false;
            if (filter.DatItem_SoftwareListStatus.MatchesNegative(SoftwareListStatus.NULL, Status) == true)
                return false;

            // Filter on filter
            if (filter.DatItem_Filter.MatchesPositiveSet(Filter) == false)
                return false;
            if (filter.DatItem_Filter.MatchesNegativeSet(Filter) == true)
                return false;

            return true;
        }

        /// <summary>
        /// Remove fields from the DatItem
        /// </summary>
        /// <param name="fields">List of Fields to remove</param>
        public override void RemoveFields(List<Field> fields)
        {
            // Remove common fields first
            base.RemoveFields(fields);

            // Remove the fields
            if (fields.Contains(Field.DatItem_SoftwareListStatus))
                Status = SoftwareListStatus.NULL;

            if (fields.Contains(Field.DatItem_Filter))
                Filter = null;
        }

        #endregion

        #region Sorting and Merging

        /// <summary>
        /// Replace fields from another item
        /// </summary>
        /// <param name="item">DatItem to pull new information from</param>
        /// <param name="fields">List of Fields representing what should be updated</param>
        public override void ReplaceFields(DatItem item, List<Field> fields)
        {
            // Replace common fields first
            base.ReplaceFields(item, fields);

            // If we don't have a SoftwareList to replace from, ignore specific fields
            if (item.ItemType != ItemType.SoftwareList)
                return;

            // Cast for easier access
            SoftwareList newItem = item as SoftwareList;

            // Replace the fields
            if (fields.Contains(Field.DatItem_SoftwareListStatus))
                Status = newItem.Status;

            if (fields.Contains(Field.DatItem_Filter))
                Filter = newItem.Filter;
        }

        #endregion
    }
}