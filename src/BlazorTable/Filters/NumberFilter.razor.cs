using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;

namespace BlazorTable
{
    public partial class NumberFilter<TableItem> : IFilter<TableItem>
    {
        [CascadingParameter(Name = "Column")] public IColumn<TableItem> Column { get; set; }

        [Inject] public ILogger<NumberFilter<TableItem>> Logger { get; set; }

        private NumberCondition Condition { get; set; }

        private string FilterValue { get; set; }

        protected override void OnInitialized()
        {
            if (Column.Type.IsNumeric() && !Column.Type.GetNonNullableType().IsEnum)
            {
                Column.FilterControl = this;

                if (Column.Filter?.Body is BinaryExpression binaryExpression)
                {
                    if (binaryExpression.NodeType == ExpressionType.AndAlso)
                    {
                        switch (binaryExpression.Right.NodeType)
                        {
                            case ExpressionType.Equal:
                                Condition = NumberCondition.IsEqualTo;
                                break;
                            case ExpressionType.NotEqual:
                                Condition = NumberCondition.IsNotEqualTo;
                                break;
                            case ExpressionType.GreaterThanOrEqual:
                                Condition = NumberCondition.IsGreaterThanOrEqualTo;
                                break;
                            case ExpressionType.GreaterThan:
                                Condition = NumberCondition.IsGreaterThan;
                                break;
                            case ExpressionType.LessThanOrEqual:
                                Condition = NumberCondition.IsLessThanOrEqualTo;
                                break;
                            case ExpressionType.LessThan:
                                Condition = NumberCondition.IsLessThan;
                                break;
                        }
                    }
                    else
                    {
                        if (binaryExpression.NodeType == ExpressionType.Equal)
                        {
                            Condition = NumberCondition.IsNull;
                        }
                        else if (binaryExpression.NodeType == ExpressionType.NotEqual)
                        {
                            Condition = NumberCondition.IsNotNull;
                        }
                    }

                    if (binaryExpression.Right is BinaryExpression binaryExpression2
                        && binaryExpression2.Right is ConstantExpression constantExpression)
                    {
                        FilterValue = constantExpression.Value.ToString();
                    }
                }
            }
        }

        public Expression<Func<TableItem, bool>> GetFilter()
        {
            switch (Condition)
            {
                case NumberCondition.IsEqualTo:
                    return Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Expression.NotEqual(Column.Field.Body, Expression.Constant(null)),
                            Expression.Equal(
                                Expression.Convert(Column.Field.Body, Column.Type.GetNonNullableType()),
                                Expression.Constant(Convert.ChangeType(FilterValue, Column.Type.GetNonNullableType(), CultureInfo.InvariantCulture)))),
                        Column.Field.Parameters);

                case NumberCondition.IsNotEqualTo:
                    return Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Expression.NotEqual(Column.Field.Body, Expression.Constant(null)),
                            Expression.NotEqual(
                                Expression.Convert(Column.Field.Body, Column.Type.GetNonNullableType()),
                                Expression.Constant(Convert.ChangeType(FilterValue, Column.Type.GetNonNullableType(), CultureInfo.InvariantCulture)))),
                        Column.Field.Parameters);

                case NumberCondition.IsGreaterThanOrEqualTo:
                    return Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Expression.NotEqual(Column.Field.Body, Expression.Constant(null)),
                            Expression.GreaterThanOrEqual(
                                Expression.Convert(Column.Field.Body, Column.Type.GetNonNullableType()),
                                Expression.Constant(Convert.ChangeType(FilterValue, Column.Type.GetNonNullableType(), CultureInfo.InvariantCulture)))),
                        Column.Field.Parameters);

                case NumberCondition.IsGreaterThan:
                    return Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Expression.NotEqual(Column.Field.Body, Expression.Constant(null)),
                            Expression.GreaterThan(
                                Expression.Convert(Column.Field.Body, Column.Type.GetNonNullableType()),
                                Expression.Constant(Convert.ChangeType(FilterValue, Column.Type.GetNonNullableType(), CultureInfo.InvariantCulture)))),
                        Column.Field.Parameters);

                case NumberCondition.IsLessThanOrEqualTo:
                    return Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Expression.NotEqual(Column.Field.Body, Expression.Constant(null)),
                            Expression.LessThanOrEqual(
                                Expression.Convert(Column.Field.Body, Column.Type.GetNonNullableType()),
                                Expression.Constant(Convert.ChangeType(FilterValue, Column.Type.GetNonNullableType(), CultureInfo.InvariantCulture)))),
                        Column.Field.Parameters);

                case NumberCondition.IsLessThan:
                    return Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Expression.NotEqual(Column.Field.Body, Expression.Constant(null)),
                            Expression.LessThan(
                                Expression.Convert(Column.Field.Body, Column.Type.GetNonNullableType()),
                                Expression.Constant(Convert.ChangeType(FilterValue, Column.Type.GetNonNullableType(), CultureInfo.InvariantCulture)))),
                        Column.Field.Parameters);

                case NumberCondition.IsNull:
                    return Expression.Lambda<Func<TableItem, bool>>(
                            Expression.Equal(Column.Field.Body, Expression.Constant(null)),
                        Column.Field.Parameters);

                case NumberCondition.IsNotNull:
                    return Expression.Lambda<Func<TableItem, bool>>(
                            Expression.NotEqual(Column.Field.Body, Expression.Constant(null)),
                        Column.Field.Parameters);
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
