using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;

namespace BlazorTable
{
    public partial class Table<TableItem> : ITable<TableItem>
    {
        [Parameter] public string TableClass { get; set; } = "table table-striped table-bordered table-hover table-sm";
        [Parameter] public string TableHeadClass { get; set; } = "thead-light text-dark";

        [Parameter] public int PageSize { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public IEnumerable<TableItem> Items { get; set; }

        private IEnumerable<TableItem> TempItems { get; set; }

        public List<IColumn<TableItem>> Columns { get; } = new List<IColumn<TableItem>>();

        public IColumn<TableItem> SortColumn { get; set; }

        public bool SortDescending { get; private set; }

        public int PageNumber { get; private set; }

        public int TotalCount { get; private set; }

        public bool IsEditMode { get; private set; }

        protected override void OnParametersSet()
        {
            Update();
        }

        protected override void OnInitialized()
        {
            Update();
        }

        private IEnumerable<TableItem> GetData()
        {
            if (Items != null)
            {
                var query = Items.AsQueryable();

                TotalCount = Items.Count();

                if (SortColumn != null)
                {
                    if (SortDescending)
                    {
                        query = query.OrderByDescending(SortColumn.Property);
                    }
                    else
                    {
                        query = query.OrderBy(SortColumn.Property);
                    }
                }

                foreach (var item in Columns)
                {
                    if (item.Filter != null)
                    {
                        query = query.Where(item.Filter);
                    }
                }

                return query.Skip(PageNumber * PageSize).Take(PageSize).ToList();
            }

            return Items;
        }

        public void Update()
        {
            TempItems = GetData();
            StateHasChanged();
        }

        public void AddColumn(IColumn<TableItem> column)
        {
            Columns.Add(column);
            StateHasChanged();
        }

        public void RemoveColumn(IColumn<TableItem> column)
        {
            Columns.Remove(column);
            StateHasChanged();
        }

        public void FirstPage()
        {
            PageNumber = 0;
            Update();
        }

        public void NextPage()
        {
            if (PageNumber < TotalCount / PageSize)
            {
                PageNumber++;
                Update();
            }
        }

        public void PreviousPage()
        {
            if (PageNumber >= 1)
            {
                PageNumber--;
                Update();
            }
        }

        public void LastPage()
        {
            PageNumber = TotalCount / PageSize;
            Update();
        }

        public void SortBy(IColumn<TableItem> column)
        {
            if (column.Sortable)
            {
                if (SortColumn != column)
                {
                    SortColumn = column;
                    SortDescending = false;
                }
                else
                {
                    SortDescending = !SortDescending;
                }

                PageNumber = 0;

                Update();
            }
        }

        public void ToggleEditMode()
        {
            IsEditMode = !IsEditMode;
            StateHasChanged();
        }

        public void Refresh()
        {
            StateHasChanged();
        }
    }
}
