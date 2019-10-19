using Microsoft.AspNetCore.Components;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace BlazorTable
{
    public partial class Column<TableItem> : IColumn<TableItem>
    {
        [CascadingParameter(Name = "Table")]
        private ITable<TableItem> Table { get; set; }

        private string _title;

        /// <summary>
        /// Title
        /// </summary>
        [Parameter]
        public string Title
        {
            get { return _title ?? Property.GetPropertyMemberInfo()?.Name; }
            set { _title = value; }
        }

        /// <summary>
        /// Width auto|value|initial|inherit
        /// </summary>
        [Parameter]
        public string Width { get; set; }

        /// <summary>
        /// Column can be sorted
        /// </summary>
        [Parameter]
        public bool Sortable { get; set; }

        /// <summary>
        /// Column can be filtered
        /// </summary>
        [Parameter]
        public bool Filterable { get; set; }

        /// <summary>
        /// Contains Filter expression
        /// </summary>
        public Expression<Func<TableItem, bool>> Filter { get; set; }

        /// <summary>
        /// Filter Panel is open
        /// </summary>
        public bool FilterOpen { get; private set; }

        /// <summary>
        /// Normal Item Template
        /// </summary>
        [Parameter]
        public RenderFragment<TableItem> Template { get; set; }

        /// <summary>
        /// Edit Mode Item Template
        /// </summary>
        [Parameter]
        public RenderFragment<TableItem> EditorTemplate { get; set; }

        /// <summary>
        /// Select Which Property To Sort On,
        /// Required when Sort = true
        /// </summary>
        [Parameter]
        public Expression<Func<TableItem, object>> Property { get; set; }

        public void Dispose()
        {
            this.Table.RemoveColumn(this);
        }

        protected override void OnInitialized()
        {
            Table.AddColumn(this);
        }

        protected override void OnParametersSet()
        {
            if ((Sortable && Property == null) || (Filterable && Property == null))
            {
                throw new ArgumentNullException($"Column {Title} Property parameter is null");
            }

            if (Title == null && Property == null)
            {
                throw new ArgumentNullException("A Column has both Title and Property parameters null");
            }
        }

        public void ToggleFilter()
        {
            FilterOpen = !FilterOpen;
            Table.Refresh();
        }
    }
}