using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        /// <summary>
        /// Show Pages Numbers
        /// </summary>
        [Parameter]
        public bool ShowPageNumberSelector { get; set; }

        /// <summary>
        /// Show Pages Number selector input
        /// </summary>
        [Parameter]
        public bool ShowPageNumberInput { get; set; }

        [Inject]
        IStringLocalizer<Localization.Localization> Localization { get; set; }

        private async Task SetPageSizeAsync(ChangeEventArgs args)
        {
            if (int.TryParse(args.Value.ToString(), out int result))
            {
                await Table.SetPageSizeAsync(result).ConfigureAwait(false);
            }
        }

        private int inputPage { get; set; } = 1;

        private async Task SetPageInput(KeyboardEventArgs e)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                await GoToPage().ConfigureAwait(false);
            }
        }

        private async Task GoToPage() 
        {
            if (inputPage > Table.TotalPages)
            {
                inputPage = Table.TotalPages;
            }

            if (inputPage < 1)
            {
                inputPage = 1;
            }

            await Table.GoToPageAsync(inputPage - 1).ConfigureAwait(false);
        }
    }
}
