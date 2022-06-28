﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BlazorTable
{
    /// <summary>
    /// BlazorTable Interface
    /// </summary>
    /// <typeparam name="TableItem"></typeparam>
    public interface ITable<TableItem> : ITable
    {
        /// <summary>
        /// List of All Available Columns
        /// </summary>
        List<IColumn<TableItem>> Columns { get; }

        /// <summary>
        /// Adds a Column to the Table
        /// </summary>
        /// <param name="column"></param>
        void AddColumn(IColumn<TableItem> column);

        /// <summary>
        /// Removes a Column from the Table
        /// </summary>
        /// <param name="column"></param>
        void RemoveColumn(IColumn<TableItem> column);

        /// <summary>
        /// IQueryable data source to display in the table
        /// </summary>
        IQueryable<TableItem> ItemsQueryable { get; set; }

        /// <summary>
        /// Collection to display in the table
        /// </summary>
        IEnumerable<TableItem> Items { get; set; }

        /// <summary>
        /// Collection of filtered items
        /// </summary>
        IEnumerable<TableItem> FilteredItems { get; }

        /// <summary>
        /// Action performed when the row is clicked
        /// </summary>
        Action<TableItem> RowClickAction { get; set; }

        /// <summary>
        /// Collection of selected items
        /// </summary>
        List<TableItem> SelectedItems { get; }

		/// <summary>
		/// Name of the default column.
		/// </summary>
		string DefaultSortColumnField { get; }
		
		bool SortDescending { get; }

        /// <summary>
        /// Set the SetDetailTemplate for the table
        /// </summary>
        /// <param name="template"></param>
        void SetDetailTemplate(DetailTemplate<TableItem> template);

        /// <summary>
        /// Add custom row to table
        /// </summary>
        /// <param name="customRow">Custom row to add</param>
        void AddCustomRow(CustomRow<TableItem> customRow);

		/// <summary>
		/// Sets the sort column field.
		/// </summary>
		/// <param name="sortColumnField"></param>
		void SetSortColumnField(string sortColumnField);

		/// <summary>
		/// Sets the sort order for the set column field..
		/// </summary>
		/// <param name="sortColumnField">True if sorted descending.</param>
		void SetSortOrder(bool sortDescending = false);
	}
}
