using Microsoft.AspNetCore.Components;
using System;
using System.Linq.Expressions;

namespace BlazorTable
{
    public interface IColumn<TableItem>
    {
        /// <summary>
        /// Title (Optional, will use Property Name if null)
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
        /// Filter Panel is open
        /// </summary>
        bool FilterOpen { get; }

        /// <summary>
        /// Opens/Closes the Filter Panel
        /// </summary>
        void ToggleFilter();

        /// <summary>
        /// Column Data Type
        /// </summary>
        Type Type { get; }

        IFilterManager<TableItem> FilterManager { get; set; }

        /// <summary>
        /// Property which this column is for<br />
        /// Required when Sortable = true<br />
        /// Required when Filterable = true
        /// </summary>
        Expression<Func<TableItem, object>> Property { get; set; }

        /// <summary>
        /// Filter expression
        /// </summary>
        Expression<Func<TableItem, bool>> Filter { get; set; }

        /// <summary>
        /// Edit Mode Item Template
        /// </summary>
        RenderFragment<TableItem> EditorTemplate { get; set; }

        /// <summary>
        /// Normal Item Template
        /// </summary>
        RenderFragment<TableItem> Template { get; set; }
    }
}
