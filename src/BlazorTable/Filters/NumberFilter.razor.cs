using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace BlazorTable
{
    public partial class NumberFilter<TableItem> : IFilter<TableItem>
    {
        [CascadingParameter(Name = "FilterManager")] public IFilterManager<TableItem> FilterManager { get; set; }

        [Inject] public ILogger<NumberFilter<TableItem>> Logger { get; set; }

        private NumberConditions condition { get; set; }

        private int filterValue { get; set; }

        public static List<Type> FilterTypes => new List<Type>() { typeof(short), typeof(int), typeof(long) };

        protected override void OnInitialized()
        {
            if (FilterTypes.Contains(FilterManager.Column.Type))
            {
                FilterManager.Filter = this;
            }
        }

        public void ApplyFilter()
        {
            switch (condition)
            {
                case NumberConditions.Is_equal_to:
                    FilterManager.Column.Filter = Utillities.CallMethodTypeObj(FilterManager.Column.Property, FilterManager.Column.Type, nameof(int.Equals), FilterManager.Column.Type, filterValue);
                    break;
                case NumberConditions.Is_not_equal_to:
                    FilterManager.Column.Filter = Utillities.Not(Utillities.CallMethodTypeObj(FilterManager.Column.Property, FilterManager.Column.Type, nameof(int.Equals), FilterManager.Column.Type, filterValue));
                    break;
                case NumberConditions.Is_greater_than_or_equal_to:
                    break;
                case NumberConditions.Is_greater_than:
                    break;
                case NumberConditions.Is_less_than_or_equal_to:
                    break;
                case NumberConditions.Is_less_than:
                    break;
                case NumberConditions.Is_null:
                    break;
                case NumberConditions.Is_not_null:
                    break;
                default:
                    throw new ArgumentException(condition + " is not defined!");
            }
        }
    }

    public enum NumberConditions
    {
        [Description("Is equal to")]
        Is_equal_to,

        [Description("Is not equal to")]
        Is_not_equal_to,

        [Description("Is greater than or equal to")]
        Is_greater_than_or_equal_to,

        [Description("Is greater than")]
        Is_greater_than,

        [Description("Is less than or equal to")]
        Is_less_than_or_equal_to,

        [Description("Is less than")]
        Is_less_than,

        [Description("Is null")]
        Is_null,

        [Description("Is not null")]
        Is_not_null
    }
}
