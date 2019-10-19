using Microsoft.AspNetCore.Components;

namespace BlazorTable
{
    public partial class Pager<TableItem>
    {
        [CascadingParameter(Name = "Table")]
        public ITable<TableItem> Table { get; set; }

        [Parameter]
        public bool AlwaysShow { get; set; }

        private long TotalPages { get; set; }

        protected override void OnParametersSet()
        {
            TotalPages = Table.TotalCount / Table.PageSize;
        }
    }
}
