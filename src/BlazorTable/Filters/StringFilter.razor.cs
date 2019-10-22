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

                if (FilterManager.Column.Filter != null)
                {
                    bool NotCondition = false;

                    Expression method;

                    if (FilterManager.Column.Filter.Body is UnaryExpression unary)
                    {
                        NotCondition = unary.NodeType == ExpressionType.Not;

                        method = unary.Operand;
                    }
                    else
                    {
                        method = FilterManager.Column.Filter.Body;
                    }

                    var methodCall = (MethodCallExpression)method;

                    var MethodName = methodCall.Method.Name;

                    if (methodCall.Arguments[0] != null && methodCall.Arguments[0] is ConstantExpression)
                    {
                        var filterText = ((ConstantExpression)(methodCall.Arguments[0]));

                        FilterText = filterText.Value.ToString();
                    }

                    Condition = GetConditionFromMethod(MethodName, NotCondition);
                }
            }
        }

        public void ApplyFilter()
        {
            FilterText = FilterText?.Trim();

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
                    FilterManager.Column.Filter = Utillities.CallMethodTypeStaticSelf(FilterManager.Column.Property, typeof(string), nameof(string.IsNullOrEmpty), typeof(string));
                    break;
                case StringCondition.IsNotNulOrEmpty:
                    FilterManager.Column.Filter = Utillities.CallMethodTypeStaticSelf(FilterManager.Column.Property, typeof(string), nameof(string.IsNullOrEmpty), typeof(string)).Not();
                    break;
                default:
                    throw new ArgumentException(Condition + " is not defined!");
            }
        }

        private StringCondition GetConditionFromMethod(string method, bool not)
        {
            if (method == nameof(string.Contains) && !not)
            {
                return StringCondition.Contains;
            }
            else if (method == nameof(string.Contains) && not)
            {
                return StringCondition.DoesNotContain;
            }
            else if (method == nameof(string.StartsWith) && !not)
            {
                return StringCondition.StartsWith;
            }
            else if (method == nameof(string.EndsWith) && !not)
            {
                return StringCondition.EndsWith;
            }
            else if (method == nameof(string.Equals) && !not)
            {
                return StringCondition.IsEqualTo;
            }
            else if (method == nameof(string.Equals) && not)
            {
                return StringCondition.IsNotEqualTo;
            }
            else if (method == nameof(string.IsNullOrEmpty) && !not)
            {
                return StringCondition.IsNullOrEmpty;
            }
            else if (method == nameof(string.IsNullOrEmpty) && not)
            {
                return StringCondition.IsNullOrEmpty;
            }

            throw new InvalidOperationException("Shouldn't be here");
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
