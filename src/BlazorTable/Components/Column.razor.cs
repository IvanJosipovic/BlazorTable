using Microsoft.AspNetCore.Components;
using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace BlazorTable
{
    /// <summary>
    /// Table Column
    /// </summary>
    /// <typeparam name="TableItem"></typeparam>
    public partial class Column<TableItem> : IColumn<TableItem>
    {
        /// <summary>
        /// Parent Table
        /// </summary>
        [CascadingParameter(Name = "Table")]
        public ITable<TableItem> Table { get; set; }

        private string _title;

        /// <summary>
        /// Title (Optional, will use Field Name if null)
        /// </summary>
        [Parameter]
        public string Title
        {
            get { return _title ?? Field.GetPropertyMemberInfo()?.Name; }
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
        /// Normal Item Template
        /// </summary>
        [Parameter]
        public RenderFragment<TableItem> Template { get; set; }

        /// <summary>
        /// Edit Mode Item Template
        /// </summary>
        [Parameter]
        public RenderFragment<TableItem> EditTemplate { get; set; }

        /// <summary>
        /// Set custom Footer column value 
        /// </summary>
        [Parameter]
        public string SetFooterValue { get; set; }

        /// <summary>
        /// Place custom controls which implement IFilter
        /// </summary>
        [Parameter]
        public RenderFragment<IColumn<TableItem>> CustomIFilters { get; set; }

        /// <summary>
        /// Field which this column is for<br />
        /// Required when Sortable = true<br />
        /// Required when Filterable = true
        /// </summary>
        [Parameter]
        public Expression<Func<TableItem, object>> Field { get; set; }

        /// <summary>
        /// Horizontal alignment
        /// </summary>
        [Parameter]
        public Align Align { get; set; }

        /// <summary>
        /// Aggregates table column for the footer. It can only be applied to numerical fields (e.g. int, long decimal, double, etc.).
        /// </summary>
        [Parameter]
        public AggregateType? Aggregate { get; set; }

        /// <summary>
        /// Set the format for values if no template
        /// </summary>
        [Parameter]
        public string Format { get; set; }

        /// <summary>
        /// Column CSS Class
        /// </summary>
        [Parameter]
        public string Class { get; set; }

        /// <summary>
        /// Column Footer CSS Class
        /// </summary>
        [Parameter]
        public string ColumnFooterClass { get; set; }

        /// <summary>
        /// Filter expression
        /// </summary>
        public Expression<Func<TableItem, bool>> Filter { get; set; }

        /// <summary>
        /// True if this is the default Sort Column
        /// </summary>
        [Parameter]
        public bool? DefaultSortColumn { get; set; }

        /// <summary>
        /// Direction of default sorting
        /// </summary>
        [Parameter]
        public bool? DefaultSortDescending { get; set; }

        /// <summary>
        /// True if this is the current Sort Column
        /// </summary>
        public bool SortColumn { get; set; }

        /// <summary>
        /// Direction of sorting
        /// </summary>
        public bool SortDescending { get; set; }

        /// <summary>
        /// Filter Panel is open
        /// </summary>
        public bool FilterOpen { get; private set; }

        /// <summary>
        /// Column Data Type
        /// </summary>
        [Parameter]
        public Type Type { get; set; }

        /// <summary>
        /// Filter Icon Element
        /// </summary>
        public ElementReference FilterRef { get; set; }

        /// <summary>
        /// Currently applied Filter Control
        /// </summary>
        public IFilter<TableItem> FilterControl { get; set; }

        protected override void OnInitialized()
        {
            Table.AddColumn(this);

            if (DefaultSortDescending.HasValue)
            {
                this.SortDescending = DefaultSortDescending.Value;
            }

            if (DefaultSortColumn.HasValue)
            {
                this.SortColumn = DefaultSortColumn.Value;
            }
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

            if (Type == null)
            {
                Type = Field?.GetPropertyMemberInfo().GetMemberUnderlyingType();
            }
        }

        /// <summary>
        /// Opens/Closes the Filter Panel
        /// </summary>
        public void ToggleFilter()
        {
            FilterOpen = !FilterOpen;
            Table.Refresh();
        }

        /// <summary>
        /// Sort by this column
        /// </summary>
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
        /// Returns aggregation of this column for the table footer based on given type: Sum, Average, Count, Min, or Max.
        /// </summary>
        /// <returns>string results</returns>
        public string GetFooterValue()
        {
            if (Table.ItemsQueryable != null &&  Aggregate.HasValue && Table.ShowFooter && !string.IsNullOrEmpty(Field.GetPropertyMemberInfo()?.Name))
            {
                return this.Aggregate.Value switch
                {
                    AggregateType.Count => string.Format(CultureInfo.CurrentCulture, $"{{0:{Format}}}", Table.ItemsQueryable.Count()),
                    AggregateType.Min => string.Format(CultureInfo.CurrentCulture, $"{{0:{Format}}}", Table.ItemsQueryable.AsEnumerable().Min(c => c.GetType().GetProperty(Field.GetPropertyMemberInfo()?.Name).GetValue(c, null))),
                    AggregateType.Max => string.Format(CultureInfo.CurrentCulture, $"{{0:{Format}}}", Table.ItemsQueryable.AsEnumerable().Max(c => c.GetType().GetProperty(Field.GetPropertyMemberInfo()?.Name).GetValue(c, null))),
                    _ => string.Format(CultureInfo.CurrentCulture, $"{{0:{Format}}}", Table.ItemsQueryable.Aggregate(Field.GetPropertyMemberInfo()?.Name, this.Aggregate.Value)),
                };
            }
            return string.Empty;
        }

        /// <summary>
        /// Render a default value if no template
        /// </summary>
        /// <param name="data">data row</param>
        /// <returns></returns>
        public string Render(TableItem data)
        {
            if (data == null || Field == null) return string.Empty;

            if (renderCompiled == null)
                renderCompiled = Field.Compile();

            object value = null;

            try
            {
                value = renderCompiled.Invoke(data);
            }
            catch (NullReferenceException){}

            if (value == null) return string.Empty;

            if (string.IsNullOrEmpty(Format))
                return value.ToString();

            return string.Format(CultureInfo.CurrentCulture, $"{{0:{Format}}}", value);
        }

        /// <summary>
        /// Save compiled renderCompiled property to avoid repeated Compile() calls
        /// </summary>
        private Func<TableItem, object> renderCompiled;
    }
}