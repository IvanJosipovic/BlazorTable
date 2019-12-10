using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorTable
{
    public partial class Table<TableItem> : ITable<TableItem>
    {
        [Parameter(CaptureUnmatchedValues = true)]
        public IDictionary<string, object> UnknownParameters { get; set; }

        [Parameter]
        public string TableClass { get; set; } = "table table-striped table-bordered table-hover table-sm";

        [Parameter]
        public string TableHeadClass { get; set; } = "thead-light text-dark";

        [Parameter]
        public string TableBodyClass { get; set; } = "";

        [Parameter]
        public int PageSize { get; set; }

        [Parameter]
        public bool ColumnReorder { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public IEnumerable<TableItem> Items { get; set; }

        [Inject]
        private ILogger<ITable<TableItem>> Logger { get; set; }

        private IEnumerable<TableItem> TempItems { get; set; }

        public List<IColumn<TableItem>> Columns { get; } = new List<IColumn<TableItem>>();

        public int PageNumber { get; private set; }

        public int TotalCount { get; private set; }

        public bool IsEditMode { get; private set; }

        public int TotalPages => (TotalCount + PageSize - 1) / PageSize;

        protected override void OnParametersSet()
        {
            Update();
        }

        private IEnumerable<TableItem> GetData()
        {
            if (Items != null)
            {
                var query = Items.AsQueryable();

                foreach (var item in Columns)
                {
                    if (item.Filter != null)
                    {
                        query = query.Where(item.Filter);
                    }
                }

                TotalCount = query.Count();

                var sortColumn = Columns.Find(x => x.SortColumn);

                if (sortColumn != null)
                {
                    if (sortColumn.SortDescending)
                    {
                        query = query.OrderByDescending(sortColumn.Field);
                    }
                    else
                    {
                        query = query.OrderBy(sortColumn.Field);
                    }
                }

                return query.Skip(PageNumber * PageSize).Take(PageSize).ToList();
            }

            return Items;
        }

        public void Update()
        {
            TempItems = GetData();
            Refresh();
        }

        public void AddColumn(IColumn<TableItem> column)
        {
            Columns.Add(column);
            Refresh();
        }

        public void RemoveColumn(IColumn<TableItem> column)
        {
            Columns.Remove(column);
            Refresh();
        }

        public void FirstPage()
        {
            if (PageNumber != 0)
            {
                PageNumber = 0;
                Update();
            }
        }

        public void NextPage()
        {
            if (PageNumber + 1 < TotalPages)
            {
                PageNumber++;
                Update();
            }
        }

        public void PreviousPage()
        {
            if (PageNumber > 0)
            {
                PageNumber--;
                Update();
            }
        }

        public void LastPage()
        {
            PageNumber = TotalPages - 1;
            Update();
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

        private int DragSourceId;

        private void HandleDragStart(int index)
        {
            DragSourceId = index;
        }

        private List<ElementReference> filterElementReferences = new List<ElementReference>();

        private void HandleDrop(int index)
        {
            var col = Columns[DragSourceId];

            Columns.RemoveAt(DragSourceId);

            if (DragSourceId + 1 < index)
            {
                index--;
            }

            Columns.Insert(index, col);

            StateHasChanged();
        }
    }
}
