using Microsoft.AspNetCore.Components;
using System;

namespace BlazorTable
{
    /// <summary>
    /// A custom row template
    /// </summary>
    /// <typeparam name="TableItem"></typeparam>
    public partial class CustomRow<TableItem> : ComponentBase
    {
        /// <summary>
        /// Parent Table
        /// </summary>
        [CascadingParameter(Name = "Table")]
        public ITable<TableItem> Table { get; set; }

        /// <summary>
        /// Function that defines if the custom row is active for the TableItem inserted as parameter
        /// </summary>
        [Parameter]
        public Func<TableItem, bool> IsActiveForItem { get; set; }

        /// <summary>
        /// Content to show
        /// </summary>
        [Parameter]
        public RenderFragment<TableItem> ChildContent { get; set; }

        /// <summary>
        /// When initialized, add custom row to table
        /// </summary>
        protected override void OnInitialized()
        {
            Table.AddCustomRow(this);
        }
    }
}
