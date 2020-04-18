﻿using LinqKit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BlazorTable
{
    public partial class Table<TableItem> : ITable<TableItem>
    {
        private const int DEFAULT_PAGE_SIZE = 10;

        [Parameter(CaptureUnmatchedValues = true)]
        public IReadOnlyDictionary<string, object> UnknownParameters { get; set; }

        /// <summary>
        /// Table CSS Class (Defaults to Bootstrap 4)
        /// </summary>
        [Parameter]
        public string TableClass { get; set; } = "table table-striped table-bordered table-hover table-sm";

        /// <summary>
        /// Table Head Class (Defaults to Bootstrap 4)
        /// </summary>
        [Parameter]
        public string TableHeadClass { get; set; } = "thead-light text-dark";

        /// <summary>
        /// Table Body Class
        /// </summary>
        [Parameter]
        public string TableBodyClass { get; set; } = "";

        /// <summary>
        /// Expression to set Row Class
        /// </summary>
        [Parameter]
        public Expression<Func<TableItem, string>> TableRowClass { get; set; }

        /// <summary>
        /// Page Size, defaults to 15
        /// </summary>
        [Parameter]
        public int PageSize { get; set; } = DEFAULT_PAGE_SIZE;

        /// <summary>
        /// Allow Columns to be reordered
        /// </summary>
        [Parameter]
        public bool ColumnReorder { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// IQueryable data source to display in the table
        /// </summary>
        [Parameter]
        public IQueryable<TableItem> ItemsQueryable { get; set; }

        /// <summary>
        /// Collection to display in the table
        /// </summary>
        [Parameter]
        public IEnumerable<TableItem> Items { get; set; }

        /// <summary>
        /// Search all columns for the specified string, supports spaces as a delimiter
        /// </summary>
        [Parameter]
        public string GlobalSearch { get; set; }

        [Inject]
        private ILogger<ITable<TableItem>> Logger { get; set; }

        /// <summary>
        /// Collection of filtered items
        /// </summary>
        public IEnumerable<TableItem> FilteredItems { get; private set; }

        /// <summary>
        /// List of All Available Columns
        /// </summary>
        public List<IColumn<TableItem>> Columns { get; } = new List<IColumn<TableItem>>();

        /// <summary>
        /// Current Page Number
        /// </summary>
        public int PageNumber { get; private set; }

        /// <summary>
        /// Total Count of Items
        /// </summary>
        public int TotalCount { get; private set; }

        /// <summary>
        /// Is Table in Edit mode
        /// </summary>
        public bool IsEditMode { get; private set; }

        /// <summary>
        /// Total Pages
        /// </summary>
        public int TotalPages => PageSize <= 0 ? 1 : (TotalCount + PageSize - 1) / PageSize;

        protected override void OnParametersSet()
        {
            Update();
        }

        private IEnumerable<TableItem> GetData()
        {
            if (Items != null || ItemsQueryable != null)
            {
                if (Items != null)
                {
                    ItemsQueryable = Items.AsQueryable();
                }

                foreach (var item in Columns)
                {
                    if (item.Filter != null)
                    {
                        ItemsQueryable = ItemsQueryable.Where(item.Filter.AddNullChecks());
                    }
                }

                // Global Search
                if (!string.IsNullOrEmpty(GlobalSearch))
                {
                    ItemsQueryable = ItemsQueryable.Where(GlobalSearchQuery(GlobalSearch));
                }

                TotalCount = ItemsQueryable.Count();

                var sortColumn = Columns.Find(x => x.SortColumn);

                if (sortColumn != null)
                {
                    if (sortColumn.SortDescending)
                    {
                        ItemsQueryable = ItemsQueryable.OrderByDescending(sortColumn.Field);
                    }
                    else
                    {
                        ItemsQueryable = ItemsQueryable.OrderBy(sortColumn.Field);
                    }
                }

                // if the current page is filtered out, we should go back to a page that exists
                if (PageNumber > TotalPages)
                {
                    PageNumber = TotalPages - 1;
                }

                // if PageSize is zero, we return all rows and no paging
                if (PageSize <= 0)
                    return ItemsQueryable.ToList();
                else
                    return ItemsQueryable.Skip(PageNumber * PageSize).Take(PageSize).ToList();
            }

            return Items;
        }

        private bool[] detailsViewOpen;

        /// <summary>
        /// Gets Data and redraws the Table
        /// </summary>
        public void Update()
        {
            detailsViewOpen = new bool[PageSize];
            FilteredItems = GetData();
            Refresh();
        }

        /// <summary>
        /// Adds a Column to the Table
        /// </summary>
        /// <param name="column"></param>
        public void AddColumn(IColumn<TableItem> column)
        {
            column.Table = this;

            if (column.Type == null)
            {
                column.Type = column.Field?.GetPropertyMemberInfo().GetMemberUnderlyingType();
            }

            Columns.Add(column);
            Refresh();
        }

        /// <summary>
        /// Removes a Column from the Table
        /// </summary>
        /// <param name="column"></param>
        public void RemoveColumn(IColumn<TableItem> column)
        {
            Columns.Remove(column);
            Refresh();
        }

        /// <summary>
        /// Go to First Page
        /// </summary>
        public void FirstPage()
        {
            if (PageNumber != 0)
            {
                PageNumber = 0;
                Update();
            }
        }

        /// <summary>
        /// Go to Next Page
        /// </summary>
        public void NextPage()
        {
            if (PageNumber + 1 < TotalPages)
            {
                PageNumber++;
                Update();
            }
        }

        /// <summary>
        /// Go to Previous Page
        /// </summary>
        public void PreviousPage()
        {
            if (PageNumber > 0)
            {
                PageNumber--;
                Update();
            }
        }

        /// <summary>
        /// Go to Last Page
        /// </summary>
        public void LastPage()
        {
            PageNumber = TotalPages - 1;
            Update();
        }

        /// <summary>
        /// Redraws the Table using EditTemplate instead of Template
        /// </summary>
        public void ToggleEditMode()
        {
            IsEditMode = !IsEditMode;
            StateHasChanged();
        }

        /// <summary>
        /// Redraws Table without Getting Data
        /// </summary>
        public void Refresh()
        {
            StateHasChanged();
        }

        /// <summary>
        /// Save currently dragged column
        /// </summary>
        private IColumn<TableItem> DragSource;

        /// <summary>
        /// Handles the Column Reorder Drag Start and set DragSource
        /// </summary>
        /// <param name="column"></param>
        private void HandleDragStart(IColumn<TableItem> column)
        {
            DragSource = column;
        }

        /// <summary>
        /// Handles Drag Drop and inserts DragSource column before itself
        /// </summary>
        /// <param name="column"></param>
        private void HandleDrop(IColumn<TableItem> column)
        {
            int index = Columns.FindIndex(a => a == column);

            Columns.Remove(DragSource);

            Columns.Insert(index, DragSource);

            StateHasChanged();
        }

        /// <summary>
        /// Return row class for item if expression is specified
        /// </summary>
        /// <param name="item">TableItem to return for</param>
        /// <returns></returns>
        private string RowClass(TableItem item)
        {
            if (TableRowClass == null) return null;

            if (_tableRowClassCompiled == null)
                _tableRowClassCompiled = TableRowClass.Compile();

            return _tableRowClassCompiled.Invoke(item);
        }

        /// <summary>
        /// Save compiled TableRowClass property to avoid repeated Compile() calls
        /// </summary>
        private Func<TableItem, string> _tableRowClassCompiled;

        /// <summary>
        /// Set the template to use for empty data
        /// </summary>
        /// <param name="emptyDataTemplate"></param>
        public void SetEmptyDataTemplate(EmptyDataTemplate emptyDataTemplate)
        {
            _emptyDataTemplate = emptyDataTemplate?.ChildContent;
        }

        private RenderFragment _emptyDataTemplate;
        
        /// <summary>
        /// Set the template to use for loading data
        /// </summary>
        /// <param name="loadingDataTemplate"></param>
        public void SetLoadingDataTemplate(LoadingDataTemplate loadingDataTemplate)
        {
            _loadingDataTemplate = loadingDataTemplate?.ChildContent;
        }

        private RenderFragment _loadingDataTemplate;

        /// <summary>
        /// Set the template to use for detail
        /// </summary>
        /// <param name="detailTemplate"></param>
        public void SetDetailTemplate(DetailTemplate<TableItem> detailTemplate)
        {
            _detailTemplate = detailTemplate?.ChildContent;
        }

        private RenderFragment<TableItem> _detailTemplate;

        private SelectionType _selectionType;

        /// <summary>
        /// Select Type: None, Single or Multiple
        /// </summary>
        [Parameter]
        public SelectionType SelectionType
        {
            get { return _selectionType; }
            set
            {
                _selectionType = value;
                if (_selectionType == SelectionType.None)
                {
                    SelectedItems.Clear();
                } else if (_selectionType == SelectionType.Single && SelectedItems.Count > 1)
                {
                    SelectedItems.RemoveRange(1, SelectedItems.Count - 1);
                }
                StateHasChanged();
            }
        }

        /// <summary>
        /// Contains Selected Items
        /// </summary>
        [Parameter]
        public List<TableItem> SelectedItems { get; set; } = new List<TableItem>();

        /// <summary>
        /// Action performed when the row is clicked.
        /// </summary>
        [Parameter]
        public Action<TableItem> RowClickAction { get; set; }

        /// <summary>
        /// Handles the onclick action for table rows.
        /// This allows the RowClickAction to be optional.
        /// </summary>
        private void OnRowClickHandler(TableItem tableItem)
        {
            try
            {
                RowClickAction?.Invoke(tableItem);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "RowClickAction threw an exception: {0}", ex);
            }

            switch (SelectionType)
            {
                case SelectionType.None:
                    return;
                case SelectionType.Single:
                    SelectedItems.Clear();
                    SelectedItems.Add(tableItem);
                    break;
                case SelectionType.Multiple:
                    if (SelectedItems.Contains(tableItem))
                        SelectedItems.Remove(tableItem);
                    else
                        SelectedItems.Add(tableItem);
                    break;
            }
        }

        private Expression<Func<TableItem, bool>> GlobalSearchQuery(string value)
        {
            Expression<Func<TableItem, bool>> expression = null;

            foreach (string keyword in value.Trim().Split(" "))
            {
                Expression<Func<TableItem, bool>> tmp = null;

                foreach (var column in Columns)
                {
                    var newQuery = Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Expression.NotEqual(column.Field.Body, Expression.Constant(null)),
                            Expression.GreaterThanOrEqual(
                                Expression.Call(
                                    Expression.Call(column.Field.Body, "ToString", Type.EmptyTypes),
                                    typeof(string).GetMethod(nameof(string.IndexOf), new[] { typeof(string), typeof(StringComparison) }),
                                    new[] { Expression.Constant(keyword), Expression.Constant(StringComparison.OrdinalIgnoreCase) }),
                            Expression.Constant(0))),
                            column.Field.Parameters[0]);

                    if (tmp == null)
                        tmp = newQuery;
                    else
                        tmp = tmp.Or(newQuery);
                }

                if (expression == null)
                    expression = tmp;
                else
                    expression = expression.And(tmp);
            }

            return expression;
        }

        /// <summary>
        /// Shows Search Bar above the table
        /// </summary>
        [Parameter]
        public bool ShowSearchBar { get; set; }
    }
}
