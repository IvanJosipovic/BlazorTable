using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BlazorTable
{
    public partial class NumberFilter<TableItem> : IFilter<TableItem>
    {
        [CascadingParameter(Name = "FilterManager")] public IFilterManager<TableItem> FilterManager { get; set; }

        [Inject] public ILogger<NumberFilter<TableItem>> Logger { get; set; }

        private NumberCondition Condition { get; set; }

        private int FilterValue { get; set; }

        public List<Type> FilterTypes => new List<Type>() { typeof(short), typeof(int), typeof(long) };

        protected override void OnInitialized()
        {
            if (FilterTypes.Contains(FilterManager.Column.Type))
            {
                FilterManager.Filter = this;
            }
        }

        public void ApplyFilter()
        {
            switch (Condition)
            {
                case NumberCondition.IsEqualTo:
                    FilterManager.Column.Filter = Utillities.CallMethodTypeObj(FilterManager.Column.Property, FilterManager.Column.Type, nameof(int.Equals), FilterManager.Column.Type, FilterValue);
                    break;
                case NumberCondition.IsNotEqualTo:
                    FilterManager.Column.Filter = Utillities.CallMethodTypeObj(FilterManager.Column.Property, FilterManager.Column.Type, nameof(int.Equals), FilterManager.Column.Type, FilterValue).Not();
                    break;
                case NumberCondition.IsGreaterThanOrEqualTo:
                    break;
                case NumberCondition.IsGreaterThan:
                    break;
                case NumberCondition.IsLessThanOrEqualTo:
                    break;
                case NumberCondition.IsLessThan:
                    break;
                case NumberCondition.IsNull:
                    break;
                case NumberCondition.IsNotNull:
                    break;
                default:
                    throw new ArgumentException(Condition + " is not defined!");
            }
        }
    }

    public enum NumberCondition
    {
        [Description("Is equal to")]
        IsEqualTo,

        [Description("Is not equal to")]
        IsNotEqualTo,

        [Description("Is greater than or equal to")]
        IsGreaterThanOrEqualTo,

        [Description("Is greater than")]
        IsGreaterThan,

        [Description("Is less than or equal to")]
        IsLessThanOrEqualTo,

        [Description("Is less than")]
        IsLessThan,

        [Description("Is null")]
        IsNull,

        [Description("Is not null")]
        IsNotNull
    }
}
