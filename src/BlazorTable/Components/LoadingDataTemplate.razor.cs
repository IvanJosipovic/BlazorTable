using Microsoft.AspNetCore.Components;

namespace BlazorTable
{
    /// <summary>
    /// Child content for null dataset
    /// </summary>
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
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// When initialized, tell table of this item
        /// </summary>
        protected override void OnInitialized()
        {
            Table.SetLoadingDataTemplate(this);
        }
    }
}
