using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace BlazorTable
{
    public partial class FilterManager<TableItem>
    {
        [CascadingParameter(Name = "Column")] public IColumn<TableItem> Column { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        [Inject] public ILogger<FilterManager<TableItem>> Logger { get; set; }

        public IFilter<TableItem> Filter { get; set; }

        protected override void OnInitialized()
        {
        }

        private void ApplyFilter()
        {
            Column.ToggleFilter();
            if (Filter != null)
            {
                Column.Filter = Filter.GetFilter();
                Column.Table.Update();
                Column.Table.FirstPage();
            }
        }

        private void ClearFilter()
        {
            Column.ToggleFilter();
            Column.Filter = null;
            Column.Table.Update();
        }
    }
}
