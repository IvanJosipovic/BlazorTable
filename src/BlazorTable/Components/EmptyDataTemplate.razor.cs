using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorTable
{
    /// <summary>
    /// Child content for empty dataset
    /// </summary>
    /// <typeparam name="TableItem"></typeparam>
    public partial class EmptyDataTemplate<TableItem>
    {
        /// <summary>
        /// Parent table
        /// </summary>
        [CascadingParameter(Name = "Table")]
        public ITable<TableItem> Table { get; set; }

        /// <summary>
        /// Content to show
        /// </summary>
        [Parameter]
        public RenderFragment Template { get; set; }

        /// <summary>
        /// When initialized, tell table of this item
        /// </summary>
        protected override void OnInitialized()
        {
            Table.SetEmptyDataTemplate(this);
        }

    }
}
