using LinqKit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BlazorTable.Components.ServerSide;
using BlazorTable.Interfaces;

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
        /// Table Footer Class
        /// </summary>
        [Parameter]
        public string TableFooterClass { get; set; } = "text-white bg-secondary";

        /// <summary>
        /// Expression to set Row Class
        /// </summary>
        [Parameter]
        public Func<TableItem, string> TableRowClass { get; set; }

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
        /// Service to use to query data server side
        /// </summary>
        [Parameter]
        public IDataLoader<TableItem> DataLoader { get; set; }

        /// <summary>
        /// Search all columns for the specified string, supports spaces as a delimiter
        /// </summary>
        [Parameter]
        public string GlobalSearch { get; set; }

        [Inject]
        private ILogger<ITable<TableItem>> Logger { get; set; }

        [Inject]
        IStringLocalizer<Localization.Localization> Localization { get; set; }

        /// <summary>
        /// Ref to visibility menu icon for popover display
        /// </summary>
        private ElementReference VisibilityMenuIconRef { get; set; }

        /// <summary>
        /// True if visibility menu is open otherwise false
        /// </summary>
        private bool VisibilityMenuOpen { get; set; }

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

        /// <summary>
        /// Custom Rows
        /// </summary>
        private List<CustomRow<TableItem>> CustomRows { get; set; } = new List<CustomRow<TableItem>>();

        protected override async Task OnParametersSetAsync()
        {
            await UpdateAsync().ConfigureAwait(false);
        }

        private IEnumerable<TableItem> GetData()
        {
            if (Items == null && ItemsQueryable == null)
            {
                return Items;
            }
            if (Items != null)
            {
                ItemsQueryable = Items.AsQueryable();
            }

            foreach (var item in Columns)
            {
                if (item.Filter != null)
                {
                    ItemsQueryable = ItemsQueryable.Where(item.Filter);
                }
            }

            if (DataLoader != null)
            {
                return ItemsQueryable.ToList();
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
                ItemsQueryable = sortColumn.SortDescending ?
                    ItemsQueryable.OrderByDescending(sortColumn.Field) :
                    ItemsQueryable.OrderBy(sortColumn.Field);
            }

            // if the current page is filtered out, we should go back to a page that exists
            if (PageNumber > TotalPages)
            {
                PageNumber = TotalPages - 1;
            }

            // if PageSize is zero, we return all rows and no paging
            return PageSize <= 0 ? ItemsQueryable.ToList() : ItemsQueryable.Skip(PageNumber * PageSize).Take(PageSize).ToList();
        }

        private Dictionary<int, bool> detailsViewOpen = new Dictionary<int, bool>();

        /// <summary>
        /// Open/Close detail view in specified row.
        /// </summary>
        /// <param name="row">number of row to toggle detail view</param>
        /// <param name="open">true for openening detail view, false for closing detail view</param>
        public void ToggleDetailView(int row, bool open)
        {
            if (!detailsViewOpen.ContainsKey(row))
                throw new KeyNotFoundException("Specified row could not be found in the currently rendered part of the table.");

            detailsViewOpen[row] = open;
        }

        /// <summary>
        /// Open/Close all detail views.
        /// </summary>
        /// <param name="open">true for openening detail view, false for closing detail view</param>
        public void ToggleAllDetailsView(bool open)
        {
            int[] rows = new int[detailsViewOpen.Keys.Count];
            detailsViewOpen.Keys.CopyTo(rows, 0);
            foreach (int row in rows)
            {
                detailsViewOpen[row] = open;
            }
        }

        /// <summary>
        /// Gets Data and redraws the Table
        /// </summary>
        public async Task UpdateAsync()
        {
            await LoadServerSideDataAsync().ConfigureAwait(false);
            FilteredItems = GetData();
            Refresh();
        }

        private async Task LoadServerSideDataAsync()
        {
            if (DataLoader != null)
            {
                var sortColumn = Columns.Find(x => x.SortColumn);
                var sortExpression = new StringBuilder();
                if (sortColumn != null)
                {
                    sortExpression
                        .Append(sortColumn.Field.GetPropertyMemberInfo()?.Name)
                        .Append(' ')
                        .Append(sortColumn.SortDescending ? "desc" : "asc");
                }

                var result = await DataLoader.LoadDataAsync(new FilterData
                {
                    Top = PageSize,
                    Skip = PageNumber * PageSize,
                    Query = GlobalSearch,
                    OrderBy = sortExpression.ToString()
                }).ConfigureAwait(false);
                Items = result.Records;
                TotalCount = result.Total.GetValueOrDefault(1);
            }
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
        public async Task FirstPageAsync()
        {
            if (PageNumber != 0)
            {
                PageNumber = 0;
                detailsViewOpen.Clear();
                await UpdateAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Go to Next Page
        /// </summary>
        public async Task NextPageAsync()
        {
            if (PageNumber + 1 < TotalPages)
            {
                PageNumber++;
                detailsViewOpen.Clear();
                await UpdateAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Go to Previous Page
        /// </summary>
        public async Task PreviousPageAsync()
        {
            if (PageNumber > 0)
            {
                PageNumber--;
                detailsViewOpen.Clear();
                await UpdateAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Go to Last Page
        /// </summary>
        public async Task LastPageAsync()
        {
            PageNumber = TotalPages - 1;
            detailsViewOpen.Clear();
            await UpdateAsync().ConfigureAwait(false);
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
            InvokeAsync(StateHasChanged);
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
            if (DragSource != null)
            {
                int index = Columns.FindIndex(a => a == column);

                Columns.Remove(DragSource);
                Columns.Insert(index, DragSource);
                DragSource = null;

                StateHasChanged();
            }
        }

        /// <summary>
        /// Return row class for item if expression is specified
        /// </summary>
        /// <param name="item">TableItem to return for</param>
        /// <returns></returns>
        private string RowClass(TableItem item)
        {
            return TableRowClass?.Invoke(item);
        }

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
                }
                else if (_selectionType == SelectionType.Single && SelectedItems.Count > 1)
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

        /// <summary>
        /// Add custom row to current table
        /// </summary>
        /// <param name="customRow">custom row to add</param>
        public void AddCustomRow(CustomRow<TableItem> customRow)
        {
            CustomRows.Add(customRow);
        }

        private Expression<Func<TableItem, bool>> GlobalSearchQuery(string value)
        {
            Expression<Func<TableItem, bool>> expression = null;

            foreach (string keyword in value.Trim().Split(" "))
            {
                Expression<Func<TableItem, bool>> tmp = null;

                foreach (var column in Columns.Where(x => x.Field != null))
                {
                    var newQuery = Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            column.Field.Body.CreateNullChecks(),
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

        /// <summary>
        /// Show or hide table footer. Hide by default.
        /// </summary>
        [Parameter]
        public bool ShowFooter { get; set; }

        /// <summary>
        /// Set Table Page Size
        /// </summary>
        /// <param name="pageSize"></param>
        public async Task SetPageSizeAsync(int pageSize)
        {
            PageSize = pageSize;
            await UpdateAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Show table child content at the top of the table.
        /// </summary>
        [Parameter]
        public bool ShowChildContentAtTop { get; set; }

    }
}
