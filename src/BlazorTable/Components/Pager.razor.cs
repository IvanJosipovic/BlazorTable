using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading;

namespace BlazorTable
{
    /// <summary>
    /// BlazorTable Pager
    /// </summary>
    public partial class Pager
    {
        [CascadingParameter(Name = "Table")]
        public ITable Table { get; set; }

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

        /// <summary>
        /// Page size options
        /// </summary>
        [Parameter]
        public List<int> PageSizes { get; set; } = new List<int>() { 15, 30, 60 };

        /// <summary>
        /// Show Page Size Options
        /// </summary>
        [Parameter]
        public bool ShowPageSizes { get; set; }

        private void SetPageSize(ChangeEventArgs args)
        {
            if (int.TryParse(args.Value.ToString(), out int result))
            {
                Table.SetPageSize(result);
            }
        }
    }
}
