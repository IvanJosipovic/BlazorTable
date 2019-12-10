using Microsoft.AspNetCore.Components;

namespace BlazorTable
{
    public partial class Pager<TableItem>
    {
        [CascadingParameter(Name = "Table")]
        public ITable<TableItem> Table { get; set; }

        [Parameter]
        public bool AlwaysShow { get; set; }

        [Parameter]
        public bool ShowPageNumber { get; set; }

        [Parameter]
        public bool ShowTotalCount { get; set; }
    }
}
