using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace BlazorTable
{
    public partial class FilterManager<TableItem>
    {
        [CascadingParameter(Name = "Column")] public IColumn<TableItem> Column { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        [Inject] public ILogger<FilterManager<TableItem>> Logger { get; set; }

        private void ApplyFilter()
        {
            Column.ToggleFilter();
            if (Column.FilterControl != null)
            {
                Column.Filter = Column.FilterControl.GetFilter();
                Column.Table.Update();
                Column.Table.FirstPage();
            } else
            {
                Logger.LogInformation("Filter is null");
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
