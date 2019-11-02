using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace BlazorTable
{
    public interface ITable<TableItem>
    {
        /// <summary>
        /// Page Size
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// Current Page Number
        /// </summary>
        int PageNumber { get; }

        /// <summary>
        /// Total Count of Item
        /// </summary>
        int TotalCount { get; }

        /// <summary>
        /// Is Table in Edit mode
        /// </summary>
        bool IsEditMode { get; }

        /// <summary>
        /// Go to First Page
        /// </summary>
        void FirstPage();

        /// <summary>
        /// Go to Next Page
        /// </summary>
        void NextPage();

        /// <summary>
        /// Go to Previous Page
        /// </summary>
        void PreviousPage();

        /// <summary>
        /// Go to Last Page
        /// </summary>
        void LastPage();

        /// <summary>
        /// Redraws the Table using EditTemplate instead of Template
        /// </summary>
        void ToggleEditMode();

        /// <summary>
        /// List of All Available Columns
        /// </summary>
        List<IColumn<TableItem>> Columns { get; }

        /// <summary>
        /// Table Element CSS
        /// </summary>
        string TableClass { get; set; }

        /// <summary>
        /// Table Body CSS
        /// </summary>
        string TableBodyClass { get; set; }

        /// <summary>
        /// Table Head CSS
        /// </summary>
        string TableHeadClass { get; set; }

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
        /// Redraws Table without Getting Data
        /// </summary>
        void Refresh();

        /// <summary>
        /// Gets Data and redraws the Table
        /// </summary>
        void Update();
    }
}