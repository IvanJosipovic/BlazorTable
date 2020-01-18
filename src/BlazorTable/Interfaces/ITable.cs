using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BlazorTable
{
    /// <summary>
    /// BlazorTable Interface
    /// </summary>
    public interface ITable
    {
        /// <summary>
        /// Page Size
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// Allow Columns to be reordered
        /// </summary>
        bool ColumnReorder { get; set; }

        /// <summary>
        /// Current Page Number
        /// </summary>
        int PageNumber { get; }

        /// <summary>
        /// Total Count of Item
        /// </summary>
        int TotalCount { get; }

        /// <summary>
        /// Total Pages
        /// </summary>
        public int TotalPages { get; }

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
        /// Redraws Table without Getting Data
        /// </summary>
        void Refresh();

        /// <summary>
        /// Gets Data and redraws the Table
        /// </summary>
        void Update();

        /// <summary>
        /// Set the EmptyDataTemplate for the table
        /// </summary>
        /// <param name="template"></param>
        void SetEmptyDataTemplate(EmptyDataTemplate template);

        /// <summary>
        /// Set the LoadingDataTemplate for the table
        /// </summary>
        /// <param name="template"></param>
        void SetLoadingDataTemplate(LoadingDataTemplate template);
    }
}