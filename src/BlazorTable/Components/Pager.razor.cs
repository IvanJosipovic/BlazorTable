using Microsoft.AspNetCore.Components;

namespace BlazorTable
{
    /// <summary>
    /// BlazorTable Pager
    /// </summary>
    /// <typeparam name="TableItem"></typeparam>
    public partial class Pager<TableItem>
    {
        [CascadingParameter(Name = "Table")]
        public ITable<TableItem> Table { get; set; }

        /// <summary>
        /// Always show Pager, otherwise only show if TotalPages > 1
        /// </summary>
        [Parameter]
        public bool AlwaysShow { get; set; }

        /// <summary>
        /// Show current page number
        /// </summary>
        [Parameter]
        public bool ShowPageNumber { get; set; }

        /// <summary>
        /// Show total item count
        /// </summary>
        [Parameter]
        public bool ShowTotalCount { get; set; }
    }
}
