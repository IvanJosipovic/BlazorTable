using Microsoft.AspNetCore.Components;
using System;
using System.Linq.Expressions;

namespace BlazorTable
{
    public partial class Column<TableItem> : IColumn<TableItem>
    {
        [CascadingParameter(Name = "Table")]
        public ITable<TableItem> Table { get; set; }

        private string _title;

        [Parameter]
        public string Title
        {
            get { return _title ?? Field.GetPropertyMemberInfo()?.Name; }
            set { _title = value; }
        }

        [Parameter]
        public string Width { get; set; }

        [Parameter]
        public bool Sortable { get; set; }

        [Parameter]
        public bool Filterable { get; set; }

        [Parameter]
        public RenderFragment<TableItem> Template { get; set; }

        [Parameter]
        public RenderFragment<TableItem> EditTemplate { get; set; }

        [Parameter]
        public RenderFragment<Column<TableItem>> CustomIFilters { get; set; }

        [Parameter]
        public Expression<Func<TableItem, object>> Field { get; set; }

        [Parameter]
        public Align Align { get; set; }

        /// <summary>
        /// Set the format for values if no template
        /// </summary>
        [Parameter]
        public string Format { get; set; }

        public Expression<Func<TableItem, bool>> Filter { get; set; }

        public bool SortColumn { get; set; }

        public bool SortDescending { get; set; }

        public bool FilterOpen { get; private set; }

        public Type Type { get; private set; }

        public ElementReference FilterRef { get; set; }

        public IFilter<TableItem> FilterControl { get; set; }

        public void Dispose()
        {
            Table.RemoveColumn(this);
        }

        protected override void OnInitialized()
        {
            Table.AddColumn(this);
        }

        protected override void OnParametersSet()
        {
            if ((Sortable && Field == null) || (Filterable && Field == null))
            {
                throw new InvalidOperationException($"Column {Title} Property parameter is null");
            }

            if (Title == null && Field == null)
            {
                throw new InvalidOperationException("A Column has both Title and Property parameters null");
            }

            Type = Field?.GetPropertyMemberInfo().GetMemberUnderlyingType();
        }

        public void ToggleFilter()
        {
            FilterOpen = !FilterOpen;
            Table.Refresh();
        }

        public void SortBy()
        {
            if (Sortable)
            {
                if (SortColumn)
                {
                    SortDescending = !SortDescending;
                }

                Table.Columns.ForEach(x => x.SortColumn = false);

                SortColumn = true;

                Table.Update();
            }
        }

        /// <summary>
        /// Render a default value if no template
        /// </summary>
        /// <param name="data">data row</param>
        /// <returns></returns>
        public string Render(TableItem data)
        {
            if (data == null || Field ==null) return string.Empty;
            var compiled = Field.Compile();
            var value = compiled.Invoke(data);
            if (string.IsNullOrEmpty(Format))
                return value?.ToString();

            if (value == null) return string.Empty;
            return string.Format($"{{0:{Format}}}", value);
        }
    }
}