﻿using Microsoft.AspNetCore.Components;

namespace BlazorTable
{
    /// <summary>
    /// Child content for null dataset
    /// </summary>
    /// <typeparam name="TableItem"></typeparam>
    public partial class LoadingDataTemplate
    {
        /// <summary>
        /// Parent table
        /// </summary>
        [CascadingParameter(Name = "Table")]
        public ITable Table { get; set; }

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
            Table.SetLoadingDataTemplate(this);
        }
    }
}
