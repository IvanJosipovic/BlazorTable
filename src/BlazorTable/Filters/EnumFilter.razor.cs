using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;

namespace BlazorTable
{
    public partial class EnumFilter<TableItem> : IFilter<TableItem>
    {
        [CascadingParameter(Name = "Column")] public IColumn<TableItem> Column { get; set; }

        [Inject] public ILogger<NumberFilter<TableItem>> Logger { get; set; }

        private EnumCondition Condition { get; set; }

        private object FilterValue { get; set; }

        protected override void OnInitialized()
        {
            if (Column.Type.GetNonNullableType().IsEnum)
            {
                Column.FilterControl = this;

                FilterValue = Enum.GetValues(Column.Type.GetNonNullableType()).GetValue(0);

                if (Column.Filter?.Body is BinaryExpression binaryExpression)
                {
                    if (binaryExpression.NodeType == ExpressionType.AndAlso)
                    {
                        switch (binaryExpression.Right.NodeType)
                        {
                            case ExpressionType.Equal:
                                Condition = EnumCondition.IsEqualTo;
                                break;
                            case ExpressionType.NotEqual:
                                Condition = EnumCondition.IsNotEqualTo;
                                break;
                        }
                    }
                    else
                    {
                        if (binaryExpression.NodeType == ExpressionType.Equal)
                        {
                            Condition = EnumCondition.IsNull;
                        }
                        else if (binaryExpression.NodeType == ExpressionType.NotEqual)
                        {
                            Condition = EnumCondition.IsNotNull;
                        }
                    }

                    if (binaryExpression.Right is BinaryExpression binaryExpression2
                        && binaryExpression2.Right is ConstantExpression constantExpression)
                    {
                        FilterValue = constantExpression.Value;
                    }
                }
            }
        }

        public Expression<Func<TableItem, bool>> GetFilter()
        {
            switch (Condition)
            {
                case EnumCondition.IsEqualTo:
                    return Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Expression.NotEqual(Column.Field.Body, Expression.Constant(null)),
                            Expression.Equal(
                                Expression.Convert(Column.Field.Body, Column.Type.GetNonNullableType()),
                                Expression.Constant(Convert.ChangeType(FilterValue, Column.Type.GetNonNullableType(), CultureInfo.InvariantCulture)))),
                        Column.Field.Parameters);

                case EnumCondition.IsNotEqualTo:
                    return Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Expression.NotEqual(Column.Field.Body, Expression.Constant(null)),
                            Expression.NotEqual(
                                Expression.Convert(Column.Field.Body, Column.Type.GetNonNullableType()),
                                Expression.Constant(Convert.ChangeType(FilterValue, Column.Type.GetNonNullableType(), CultureInfo.InvariantCulture)))),
                        Column.Field.Parameters);

                case EnumCondition.IsNull:
                    return Expression.Lambda<Func<TableItem, bool>>(
                            Expression.Equal(Column.Field.Body, Expression.Constant(null)),
                        Column.Field.Parameters);

                case EnumCondition.IsNotNull:
                    return Expression.Lambda<Func<TableItem, bool>>(
                            Expression.NotEqual(Column.Field.Body, Expression.Constant(null)),
                        Column.Field.Parameters);
                default:
                    throw new ArgumentException(Condition + " is not defined!");
            }
        }

        public enum EnumCondition
        {
            [Description("Is Equal To")]
            IsEqualTo,

            [Description("Is Not Equal To")]
            IsNotEqualTo,

            [Description("Is null")]
            IsNull,

            [Description("Is not null")]
            IsNotNull
        }
    }
}
