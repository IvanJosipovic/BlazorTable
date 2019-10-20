using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace BlazorTable
{
    public partial class FilterManager<TableItem> : IFilterManager<TableItem>
    {
        [CascadingParameter(Name = "Table")] public ITable<TableItem> Table { get; set; }

        [CascadingParameter(Name = "Column")] public IColumn<TableItem> Column { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        [Inject] public ILogger<FilterManager<TableItem>> Logger { get; set; }

        public IFilter<TableItem> Filter { get; set; }

        protected override void OnInitialized()
        {
            Column.FilterManager = this;
        }

        private void ApplyFilter()
        {
            if (Table == null)
            {
                Logger.LogInformation("Table is Null!");
                return;
            }

            Column.ToggleFilter();

            Filter.ApplyFilter();

            Table.Update();
        }

        private void ClearFilter()
        {
            if (Table == null)
            {
                Logger.LogInformation("Table is Null!");
                return;
            }

            Column.ToggleFilter();

            Column.Filter = null;

            Table.Update();
        }
    }
}
