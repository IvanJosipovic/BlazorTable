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

                    Expression method = Column.Filter.Body;

                    if (method is BinaryExpression binary)
                    {
                        method = binary.Right;
                    }

                    if (method is BinaryExpression binary2)
                    {
                        NotCondition = binary2.NodeType == ExpressionType.LessThanOrEqual;
                        method = binary2.Left;
                    }

                    if (method is UnaryExpression unary1)
                    {
                        NotCondition = unary1.NodeType == ExpressionType.Not;
                        method = unary1.Operand;
                    }

                    if (method is MethodCallExpression methodCall)
                    {
                        if (methodCall.Arguments[0] is ConstantExpression constantExpression)
                        {
                            FilterText = constantExpression.Value?.ToString();
                        }

                        Condition = GetConditionFromMethod(methodCall.Method.Name, NotCondition);
                    }
                }
            }
        }

        private StringCondition GetConditionFromMethod(string method, bool not)
        {
            if (not)
            {
                if (method == nameof(string.IndexOf))
                {
                    return StringCondition.DoesNotContain;
                }
                else if (method == nameof(string.Equals))
                {
                    return StringCondition.IsNotEqualTo;
                }
                else if (method == nameof(string.IsNullOrEmpty))
                {
                    return StringCondition.IsNotNulOrEmpty;
                }

                throw new InvalidOperationException("Shouldn't be here");
            }

            if (method == nameof(string.IndexOf))
            {
                return StringCondition.Contains;
            }
            else if (method == nameof(string.StartsWith))
            {
                return StringCondition.StartsWith;
            }
            else if (method == nameof(string.EndsWith))
            {
                return StringCondition.EndsWith;
            }
            else if (method == nameof(string.Equals))
            {
                return StringCondition.IsEqualTo;
            }
            else if (method == nameof(string.IsNullOrEmpty))
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
                    return Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Expression.NotEqual(Column.Field.Body, Expression.Constant(null)),
                            Expression.GreaterThanOrEqual(
                                Expression.Call(
                                    Column.Field.Body,
                                    typeof(string).GetMethod(nameof(string.IndexOf), new[] { typeof(string), typeof(StringComparison) }),
                                    new[] { Expression.Constant(FilterText), Expression.Constant(StringComparison.OrdinalIgnoreCase) }),
                                Expression.Constant(0))),
                        Column.Field.Parameters);

                case StringCondition.DoesNotContain:
                    return Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Expression.NotEqual(Column.Field.Body, Expression.Constant(null)),
                            Expression.LessThanOrEqual(
                                Expression.Call(
                                    Column.Field.Body,
                                    typeof(string).GetMethod(nameof(string.IndexOf), new[] { typeof(string), typeof(StringComparison) }),
                                    new[] { Expression.Constant(FilterText), Expression.Constant(StringComparison.OrdinalIgnoreCase) }),
                                Expression.Constant(-1))),
                        Column.Field.Parameters);

                case StringCondition.StartsWith:
                    return Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Expression.NotEqual(Column.Field.Body, Expression.Constant(null)),
                            Expression.Call(
                                Column.Field.Body,
                                typeof(string).GetMethod(nameof(string.StartsWith), new[] { typeof(string), typeof(StringComparison) }),
                                new[] { Expression.Constant(FilterText), Expression.Constant(StringComparison.OrdinalIgnoreCase) })),
                        Column.Field.Parameters);

                case StringCondition.EndsWith:
                    return Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Expression.NotEqual(Column.Field.Body, Expression.Constant(null)),
                            Expression.Call(
                                Column.Field.Body,
                                typeof(string).GetMethod(nameof(string.EndsWith), new[] { typeof(string), typeof(StringComparison) }),
                                new[] { Expression.Constant(FilterText), Expression.Constant(StringComparison.OrdinalIgnoreCase) })),
                        Column.Field.Parameters);

                case StringCondition.IsEqualTo:
                    return Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Expression.NotEqual(Column.Field.Body, Expression.Constant(null)),
                            Expression.Call(
                                Column.Field.Body,
                                typeof(string).GetMethod(nameof(string.Equals), new[] { typeof(string), typeof(StringComparison) }),
                                new[] { Expression.Constant(FilterText), Expression.Constant(StringComparison.OrdinalIgnoreCase) })),
                        Column.Field.Parameters);

                case StringCondition.IsNotEqualTo:
                    return Expression.Lambda<Func<TableItem, bool>>(
                    Expression.AndAlso(
                        Expression.NotEqual(Column.Field.Body, Expression.Constant(null)),
                        Expression.Not(
                            Expression.Call(
                                Column.Field.Body,
                                typeof(string).GetMethod(nameof(string.Equals), new[] { typeof(string), typeof(StringComparison) }),
                                new[] { Expression.Constant(FilterText), Expression.Constant(StringComparison.OrdinalIgnoreCase) }))),
                    Column.Field.Parameters);

                case StringCondition.IsNullOrEmpty:
                    return Expression.Lambda<Func<TableItem, bool>>(
                        Expression.Call(
                                typeof(string).GetMethod(nameof(string.IsNullOrEmpty), new[] { typeof(string)}),
                            Column.Field.Body),
                        Column.Field.Parameters);

                case StringCondition.IsNotNulOrEmpty:
                    return Expression.Lambda<Func<TableItem, bool>>(
                        Expression.Not(
                            Expression.Call(
                                    typeof(string).GetMethod(nameof(string.IsNullOrEmpty), new[] { typeof(string)}),
                            Column.Field.Body)),
                        Column.Field.Parameters);

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
