﻿using System.Threading.Tasks;

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
        /// Total Count of Items
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
        /// Go to First Page Async
        /// </summary>
        Task FirstPageAsync();

        /// <summary>
        /// Go to Next Page
        /// </summary>
        Task NextPageAsync();

        /// <summary>
        /// Go to Previous Page
        /// </summary>
        Task PreviousPageAsync();

        /// <summary>
        /// Go to Last Page
        /// </summary>
        Task LastPageAsync();

        /// <summary>
        /// Go to Specific Page Async
        /// </summary>
        Task GoToPageAsync(int pageNumber);

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
        /// <param name="updateServerData">false if it is not needed to update server data</param>
        /// <returns></returns>
        Task UpdateAsync(bool updateServerData = true);
        /// <summary>
        /// Open/Close detail view in specified row.
        /// </summary>
        /// <param name="row">number of row to toggle detail view</param>
        /// <param name="open">true for openening detail view, false for closing detail view</param>
        void ToggleDetailView(int row, bool open);

        /// <summary>
        /// Open/Close all detail views.
        /// </summary>
        /// <param name="open">true for openening detail view, false for closing detail view</param>
        void ToggleAllDetailsView(bool open);

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


        /// <summary>
        /// Select Type: None, Single or Multiple
        /// </summary>
        public SelectionType SelectionType { get; set; }

        /// <summary>
        /// Search all columns for the specified string, supports spaces as a delimiter
        /// </summary>
        string GlobalSearch { get; set; }

        /// <summary>
        /// Shows Search Bar above the table
        /// </summary>
        bool ShowSearchBar { get; set; }

        /// <summary>
        /// Show or hide table footer. Hide by default.
        /// </summary>
        bool ShowFooter { get; set; }
      
        /// <summary>
        /// Set Table Page Size
        /// </summary>
        /// <param name="pageSize"></param>
        Task SetPageSizeAsync(int pageSize);
    }
}
