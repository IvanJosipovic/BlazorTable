﻿using Microsoft.AspNetCore.Components;
using System;
using System.Linq.Expressions;

namespace BlazorTable
{
    /// <summary>
    /// Table Column
    /// </summary>
    /// <typeparam name="TableItem"></typeparam>
    public interface IColumn<TableItem>
    {
        /// <summary>
        /// Parent Table
        /// </summary>
        ITable<TableItem> Table { get; set; }

        /// <summary>
        /// Title (Optional, will use Field Name if null)
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Width auto|value|initial|inherit
        /// </summary>
        string Width { get; set; }

        /// <summary>
        /// Column can be sorted
        /// </summary>
        bool Sortable { get; set; }

        /// <summary>
        /// Column can be filtered
        /// </summary>
        bool Filterable { get; set; }

        /// <summary>
        /// Set the format for values if no template
        /// </summary>
        string Format { get; set; }

        /// <summary>
        /// Filter Panel is open
        /// </summary>
        bool FilterOpen { get; }

        /// <summary>
        /// Opens/Closes the Filter Panel
        /// </summary>
        void ToggleFilter();

        /// <summary>
        /// Sort by this column
        /// </summary>
        void SortBy();

        /// <summary>
        /// Column Data Type
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Field which this column is for<br />
        /// Required when Sortable = true<br />
        /// Required when Filterable = true
        /// </summary>
        Expression<Func<TableItem, object>> Field { get; set; }

        /// <summary>
        /// Filter expression
        /// </summary>
        Expression<Func<TableItem, bool>> Filter { get; set; }

        /// <summary>
        /// Edit Mode Item Template
        /// </summary>
        RenderFragment<TableItem> EditTemplate { get; set; }

        /// <summary>
        /// Normal Item Template
        /// </summary>
        RenderFragment<TableItem> Template { get; set; }

        /// <summary>
        /// Currently applied Filter Control
        /// </summary>
        IFilter<TableItem> FilterControl { get; set; }

        /// <summary>
        /// Place custom controls which implement IFilter
        /// </summary>
        RenderFragment<IColumn<TableItem>> CustomIFilters { get; set; }

        /// <summary>
        /// True if this is the current Sort Column
        /// </summary>
        bool SortColumn { get; set; }

        /// <summary>
        /// Direction of sorting
        /// </summary>
        bool SortDescending { get; set; }

        /// <summary>
        /// Horizontal alignment
        /// </summary>
        Align Align { get; set; }

        /// <summary>
        /// Filter Icon Element
        /// </summary>
        ElementReference FilterRef { get; set; }

        /// <summary>
        /// Column CSS Class
        /// </summary>
        string Class { get; set; }

        /// <summary>
        /// True if this is the default Sort Column
        /// </summary>
        bool? DefaultSortColumn { get; set; }

        /// <summary>
        /// Direction of default sorting
        /// </summary>
        bool? DefaultSortDescending { get; set; }

        /// <summary>
        /// Default render if no Template specified
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        string Render(TableItem item);
    }
}
