using BlazorTable.Localization;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq.Expressions;

namespace BlazorTable
{
    public partial class StringFilter<TableItem> : IFilter<TableItem>
    {
        [CascadingParameter(Name = "Column")]
        public IColumn<TableItem> Column { get; set; }

        [Inject]
        Microsoft.Extensions.Localization.IStringLocalizer<BlazorTable.Localization.Localization> Localization { get; set; }

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
                return method switch
                {
                    nameof(string.IndexOf) => StringCondition.DoesNotContain,
                    nameof(string.Equals) => StringCondition.IsNotEqualTo,
                    nameof(string.IsNullOrEmpty) => StringCondition.IsNotNulOrEmpty,
                    _ => throw new InvalidOperationException("Shouldn't be here"),
                };
            }

            return method switch
            {
                nameof(string.IndexOf) => StringCondition.Contains,
                nameof(string.StartsWith) => StringCondition.StartsWith,
                nameof(string.EndsWith) => StringCondition.EndsWith,
                nameof(string.Equals) => StringCondition.IsEqualTo,
                nameof(string.IsNullOrEmpty) => StringCondition.IsNullOrEmpty,
                _ => throw new InvalidOperationException("Shouldn't be here"),
            };
        }

        public Expression<Func<TableItem, bool>> GetFilter()
        {
            FilterText = FilterText?.Trim();

            if (Condition != StringCondition.IsNullOrEmpty && Condition != StringCondition.IsNotNulOrEmpty && string.IsNullOrEmpty(FilterText))
            {
                return null;
            }

            return Condition switch
            {
                StringCondition.Contains =>
                    Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Column.Field.Body.CreateNullChecks(),
                            Expression.GreaterThanOrEqual(
                                Expression.Call(
                                    Expression.Call(Column.Field.Body, "ToString", Type.EmptyTypes),
                                    typeof(string).GetMethod(nameof(string.IndexOf), new[] { typeof(string), typeof(StringComparison) }),
                                    new[] { Expression.Constant(FilterText), Expression.Constant(StringComparison.OrdinalIgnoreCase) }),
                                Expression.Constant(0))),
                        Column.Field.Parameters),

                StringCondition.DoesNotContain =>
                    Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Column.Field.Body.CreateNullChecks(),
                            Expression.LessThanOrEqual(
                                Expression.Call(
                                    Expression.Call(Column.Field.Body, "ToString", Type.EmptyTypes),
                                    typeof(string).GetMethod(nameof(string.IndexOf), new[] { typeof(string), typeof(StringComparison) }),
                                    new[] { Expression.Constant(FilterText), Expression.Constant(StringComparison.OrdinalIgnoreCase) }),
                                Expression.Constant(-1))),
                        Column.Field.Parameters),

                StringCondition.StartsWith =>
                    Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Column.Field.Body.CreateNullChecks(),
                            Expression.Call(
                                Expression.Call(Column.Field.Body, "ToString", Type.EmptyTypes),
                                typeof(string).GetMethod(nameof(string.StartsWith), new[] { typeof(string), typeof(StringComparison) }),
                                new[] { Expression.Constant(FilterText), Expression.Constant(StringComparison.OrdinalIgnoreCase) })),
                        Column.Field.Parameters),

                StringCondition.EndsWith =>
                    Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Column.Field.Body.CreateNullChecks(),
                            Expression.Call(
                                Expression.Call(Column.Field.Body, "ToString", Type.EmptyTypes),
                                typeof(string).GetMethod(nameof(string.EndsWith), new[] { typeof(string), typeof(StringComparison) }),
                                new[] { Expression.Constant(FilterText), Expression.Constant(StringComparison.OrdinalIgnoreCase) })),
                        Column.Field.Parameters),

                StringCondition.IsEqualTo =>
                    Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Column.Field.Body.CreateNullChecks(),
                            Expression.Call(
                                Expression.Call(Column.Field.Body, "ToString", Type.EmptyTypes),
                                typeof(string).GetMethod(nameof(string.Equals), new[] { typeof(string), typeof(StringComparison) }),
                                new[] { Expression.Constant(FilterText), Expression.Constant(StringComparison.OrdinalIgnoreCase) })),
                        Column.Field.Parameters),

                StringCondition.IsNotEqualTo =>
                    Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Column.Field.Body.CreateNullChecks(),
                            Expression.Not(
                                Expression.Call(
                                    Expression.Call(Column.Field.Body, "ToString", Type.EmptyTypes),
                                    typeof(string).GetMethod(nameof(string.Equals), new[] { typeof(string), typeof(StringComparison) }),
                                    new[] { Expression.Constant(FilterText), Expression.Constant(StringComparison.OrdinalIgnoreCase) }))),
                        Column.Field.Parameters),

                StringCondition.IsNullOrEmpty =>
                    Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Column.Field.Body.CreateNullChecks(true),
                            Expression.Call(
                                typeof(string).GetMethod(nameof(string.IsNullOrEmpty), new[] { typeof(string) }),
                            Expression.Call(Column.Field.Body, "ToString", Type.EmptyTypes))),
                        Column.Field.Parameters),

                StringCondition.IsNotNulOrEmpty =>
                    Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Column.Field.Body.CreateNullChecks(true),
                            Expression.Not(
                                Expression.Call(
                                    typeof(string).GetMethod(nameof(string.IsNullOrEmpty), new[] { typeof(string) }),
                            Expression.Call(Column.Field.Body, "ToString", Type.EmptyTypes)))),
                        Column.Field.Parameters),

                _ => throw new ArgumentException(Condition + " is not defined!"),
            };
        }
    }

    public enum StringCondition
    {
        [LocalizedDescription("StringConditionContains", typeof(Localization.Localization))]
        Contains,

        [LocalizedDescription("StringConditionDoesNotContain", typeof(Localization.Localization))]
        DoesNotContain,

        [LocalizedDescription("StringConditionStartsWith", typeof(Localization.Localization))]
        StartsWith,

        [LocalizedDescription("StringConditionEndsWith", typeof(Localization.Localization))]
        EndsWith,

        [LocalizedDescription("StringConditionIsEqualTo", typeof(Localization.Localization))]
        IsEqualTo,

        [LocalizedDescription("StringConditionIsNotEqualTo", typeof(Localization.Localization))]
        IsNotEqualTo,

        [LocalizedDescription("StringConditionIsNullOrEmpty", typeof(Localization.Localization))]
        IsNullOrEmpty,

        [LocalizedDescription("StringConditionIsNotNullOrEmpty", typeof(Localization.Localization))]
        IsNotNulOrEmpty
    }
}
