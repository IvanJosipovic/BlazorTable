using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace BlazorTable
{
    public partial class StringFilter<TableItem> : IFilter<TableItem>
    {
        [CascadingParameter(Name = "Column")] public IColumn<TableItem> Column { get; set; }

        [Inject] public ILogger<StringFilter<TableItem>> Logger { get; set; }

        private StringCondition Condition { get; set; }

        private string FilterText { get; set; }

        public Type FilterType => typeof(string);

        protected override void OnInitialized()
        {
            if (Column.Type == typeof(string))
            {
                Column.FilterControl = this;

                if (Column.Filter != null)
                {
                    bool NotCondition = false;

                    Expression method;

                    if (Column.Filter.Body is UnaryExpression unary)
                    {
                        NotCondition = unary.NodeType == ExpressionType.Not;

                        method = unary.Operand;
                    }
                    else
                    {
                        method = Column.Filter.Body;
                    }

                    if (method is MethodCallExpression methodCall)
                    {
                        var MethodName = methodCall.Method.Name;

                        if (methodCall.Arguments[0] != null && methodCall.Arguments[0] is ConstantExpression constantExpression)
                        {
                            FilterText = constantExpression.Value.ToString();
                        }

                        Condition = GetConditionFromMethod(MethodName, NotCondition);
                    }
                }
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

        public Expression<Func<TableItem, bool>> GetFilter()
        {
            FilterText = FilterText?.Trim();
            
            switch (Condition)
            {
                case StringCondition.Contains:
                    return Utillities.CallMethodType(Column.Field, typeof(string), nameof(string.Contains), typeof(string), FilterText);
                case StringCondition.DoesNotContain:
                    return Utillities.CallMethodType(Column.Field, typeof(string), nameof(string.Contains), typeof(string), FilterText).Not();
                case StringCondition.StartsWith:
                    return Utillities.CallMethodType(Column.Field, typeof(string), nameof(string.StartsWith), typeof(string), FilterText);
                case StringCondition.EndsWith:
                    return Utillities.CallMethodType(Column.Field, typeof(string), nameof(string.EndsWith), typeof(string), FilterText);
                case StringCondition.IsEqualTo:
                    return Utillities.CallMethodType(Column.Field, typeof(string), nameof(string.Equals), typeof(string), FilterText);
                case StringCondition.IsNotEqualTo:
                    return Utillities.CallMethodType(Column.Field, typeof(string), nameof(string.Equals), typeof(string), FilterText).Not();
                case StringCondition.IsNullOrEmpty:
                    return Utillities.CallMethodTypeStaticSelf(Column.Field, typeof(string), nameof(string.IsNullOrEmpty), typeof(string));
                case StringCondition.IsNotNulOrEmpty:
                    return Utillities.CallMethodTypeStaticSelf(Column.Field, typeof(string), nameof(string.IsNullOrEmpty), typeof(string)).Not();
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
