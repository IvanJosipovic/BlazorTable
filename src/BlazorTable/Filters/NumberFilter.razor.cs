﻿using BlazorTable.Components.ServerSide;
using BlazorTable.Localization;
using Microsoft.AspNetCore.Components;
using System;
using System.Globalization;
using System.Linq.Expressions;

namespace BlazorTable
{
    public partial class NumberFilter<TableItem> : IFilter<TableItem>
    {
        [CascadingParameter(Name = "Column")]
        public IColumn<TableItem> Column { get; set; }

        [Inject]
        Microsoft.Extensions.Localization.IStringLocalizer<BlazorTable.Localization.Localization> Localization { get; set; }

        private NumberCondition Condition { get; set; }

        private string FilterValue { get; set; }

        protected override void OnInitialized()
        {
            if (Column.Type.IsNumeric() && !Column.Type.GetNonNullableType().IsEnum)
            {
                Column.FilterControl = this;

                if (Column.Filter?.Body is BinaryExpression binaryExpression
                    && binaryExpression.Right is BinaryExpression logicalBinary
                    && logicalBinary.Right is ConstantExpression constant)
                {
                    switch (binaryExpression.Right.NodeType)
                    {
                        case ExpressionType.Equal:
                            Condition = constant.Value == null ? NumberCondition.IsNull : NumberCondition.IsEqualTo;
                            break;
                        case ExpressionType.NotEqual:
                            Condition = constant.Value == null ? NumberCondition.IsNotNull : NumberCondition.IsNotEqualTo;
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

                    if (constant.Value != null)
                    {
                        FilterValue = constant.Value.ToString();
                    }
                }
            }
        }

        public Expression<Func<TableItem, bool>> GetFilter()
        {
            if (Condition != NumberCondition.IsNull && Condition != NumberCondition.IsNotNull && string.IsNullOrEmpty(FilterValue))
            {
                return null;
            }

            return Condition switch
            {
                NumberCondition.IsEqualTo =>
                    Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Column.Field.Body.CreateNullChecks(),
                            Expression.Equal(
                                Expression.Convert(Column.Field.Body, Column.Type.GetNonNullableType()),
                                Expression.Constant(Convert.ChangeType(FilterValue, Column.Type.GetNonNullableType(), CultureInfo.InvariantCulture)))),
                        Column.Field.Parameters),

                NumberCondition.IsNotEqualTo =>
                    Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Column.Field.Body.CreateNullChecks(),
                            Expression.NotEqual(
                                Expression.Convert(Column.Field.Body, Column.Type.GetNonNullableType()),
                                Expression.Constant(Convert.ChangeType(FilterValue, Column.Type.GetNonNullableType(), CultureInfo.InvariantCulture)))),
                        Column.Field.Parameters),

                NumberCondition.IsGreaterThanOrEqualTo =>
                    Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Column.Field.Body.CreateNullChecks(),
                            Expression.GreaterThanOrEqual(
                                Expression.Convert(Column.Field.Body, Column.Type.GetNonNullableType()),
                                Expression.Constant(Convert.ChangeType(FilterValue, Column.Type.GetNonNullableType(), CultureInfo.InvariantCulture)))),
                        Column.Field.Parameters),

                NumberCondition.IsGreaterThan =>
                    Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Column.Field.Body.CreateNullChecks(),
                            Expression.GreaterThan(
                                Expression.Convert(Column.Field.Body, Column.Type.GetNonNullableType()),
                                Expression.Constant(Convert.ChangeType(FilterValue, Column.Type.GetNonNullableType(), CultureInfo.InvariantCulture)))),
                        Column.Field.Parameters),

                NumberCondition.IsLessThanOrEqualTo =>
                    Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Column.Field.Body.CreateNullChecks(),
                            Expression.LessThanOrEqual(
                                Expression.Convert(Column.Field.Body, Column.Type.GetNonNullableType()),
                                Expression.Constant(Convert.ChangeType(FilterValue, Column.Type.GetNonNullableType(), CultureInfo.InvariantCulture)))),
                        Column.Field.Parameters),

                NumberCondition.IsLessThan =>
                    Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Column.Field.Body.CreateNullChecks(),
                            Expression.LessThan(
                                Expression.Convert(Column.Field.Body, Column.Type.GetNonNullableType()),
                                Expression.Constant(Convert.ChangeType(FilterValue, Column.Type.GetNonNullableType(), CultureInfo.InvariantCulture)))),
                        Column.Field.Parameters),

                NumberCondition.IsNull =>
                    Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Column.Field.Body.CreateNullChecks(true),
                            Expression.Equal(Column.Field.Body, Expression.Constant(null))),
                        Column.Field.Parameters),

                NumberCondition.IsNotNull =>
                    Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Column.Field.Body.CreateNullChecks(true),
                            Expression.NotEqual(Column.Field.Body, Expression.Constant(null))),
                        Column.Field.Parameters),

                _ => throw new ArgumentException(Condition + " is not defined!"),
            };
        }

        public FilterString GetFilterString()
        {

            return new FilterString()
            {
                Field = Column.Field.GetPropertyMemberInfo().Name,
                Condition = Condition.ToString(),
                FilterValue = FilterValue
            };
        }
    }

    public enum NumberCondition
    {
        [LocalizedDescription("NumberConditionIsEqualTo", typeof(Localization.Localization))]
        IsEqualTo,

        [LocalizedDescription("NumberConditionIsnotEqualTo", typeof(Localization.Localization))]
        IsNotEqualTo,

        [LocalizedDescription("NumberConditionIsGreaterThanOrEqualTo", typeof(Localization.Localization))]
        IsGreaterThanOrEqualTo,

        [LocalizedDescription("NumberConditionIsGreaterThan", typeof(Localization.Localization))]
        IsGreaterThan,

        [LocalizedDescription("NumberConditionIsLessThanOrEqualTo", typeof(Localization.Localization))]
        IsLessThanOrEqualTo,

        [LocalizedDescription("NumberConditionIsLessThan", typeof(Localization.Localization))]
        IsLessThan,

        [LocalizedDescription("NumberConditionIsNull", typeof(Localization.Localization))]
        IsNull,

        [LocalizedDescription("NumberConditionIsNotNull", typeof(Localization.Localization))]
        IsNotNull
    }
}
