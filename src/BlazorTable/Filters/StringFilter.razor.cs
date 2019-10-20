using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace BlazorTable
{
    public partial class StringFilter<TableItem> : IFilter<TableItem>
    {
        [CascadingParameter(Name = "FilterManager")] public IFilterManager<TableItem> FilterManager { get; set; }

        [Inject] public ILogger<StringFilter<TableItem>> Logger { get; set; }

        private StringCondition Condition { get; set; }

        private string FilterText { get; set; }

        public Type FilterType => typeof(string);

        protected override void OnInitialized()
        {
            if (FilterManager.Column.Type == FilterType)
            {
                FilterManager.Filter = this;
            }
        }

        public void ApplyFilter()
        {
            if (string.IsNullOrEmpty(FilterText))
            {
                Logger.LogInformation("Filter Text is Null!");
                return;
            }

            FilterText = FilterText.Trim();

            switch (Condition)
            {
                case StringCondition.Contains:
                    FilterManager.Column.Filter = Utillities.CallMethodType(FilterManager.Column.Property, typeof(string), nameof(string.Contains), typeof(string), FilterText);
                    break;
                case StringCondition.DoesNotContain:
                    FilterManager.Column.Filter = Utillities.CallMethodType(FilterManager.Column.Property, typeof(string), nameof(string.Contains), typeof(string), FilterText).Not();
                    break;
                case StringCondition.StartsWith:
                    FilterManager.Column.Filter = Utillities.CallMethodType(FilterManager.Column.Property, typeof(string), nameof(string.StartsWith), typeof(string), FilterText);
                    break;
                case StringCondition.EndsWith:
                    FilterManager.Column.Filter = Utillities.CallMethodType(FilterManager.Column.Property, typeof(string), nameof(string.EndsWith), typeof(string), FilterText);
                    break;
                case StringCondition.IsEqualTo:
                    FilterManager.Column.Filter = Utillities.CallMethodType(FilterManager.Column.Property, typeof(string), nameof(string.Equals), typeof(string), FilterText);
                    break;
                case StringCondition.IsNotEqualTo:
                    FilterManager.Column.Filter = Utillities.CallMethodType(FilterManager.Column.Property, typeof(string), nameof(string.Equals), typeof(string), FilterText).Not();
                    break;
                case StringCondition.IsNullOrEmpty:
                    FilterManager.Column.Filter = Utillities.CallMethodType(FilterManager.Column.Property, typeof(string), nameof(string.IsNullOrEmpty), typeof(string), FilterText);
                    break;
                case StringCondition.IsNotNulOrEmpty:
                    FilterManager.Column.Filter = Utillities.CallMethodType(FilterManager.Column.Property, typeof(string), nameof(string.IsNullOrEmpty), typeof(string), FilterText).Not();
                    break;
                default:
                    throw new ArgumentException(Condition + " is not defined!");
            }
        }
    }

    public enum StringCondition
    {
        [Description("Contains")]
        Contains,

        [Description("Does not contain")]
        DoesNotContain,

        [Description("Starts with")]
        StartsWith,

        [Description("Ends with")]
        EndsWith,

        [Description("Is equal to")]
        IsEqualTo,

        [Description("Is not equal to")]
        IsNotEqualTo,

        [Description("Is null or empty")]
        IsNullOrEmpty,

        [Description("Is not null or empty")]
        IsNotNulOrEmpty
    }
}
