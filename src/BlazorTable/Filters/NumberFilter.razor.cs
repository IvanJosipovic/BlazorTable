using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace BlazorTable
{
    public partial class NumberFilter<TableItem> : IFilter<TableItem>
    {
        [CascadingParameter(Name = "FilterManager")] public IFilterManager<TableItem> FilterManager { get; set; }

        [Inject] public ILogger<NumberFilter<TableItem>> Logger { get; set; }

        private NumberCondition Condition { get; set; }

        private string FilterValue { get; set; }

        protected override void OnInitialized()
        {
            if (FilterManager.Column.Type.IsNumeric())
            {
                FilterManager.Filter = this;

                if (FilterManager.Column.Filter != null)
                {
                    if (FilterManager.Column.Filter.Body is BinaryExpression binaryExpression)
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

                        if (binaryExpression.Right is BinaryExpression binaryExpression2)
                        {
                            if (binaryExpression2.Right is ConstantExpression constantExpression)
                            {
                                FilterValue = constantExpression.Value.ToString();
                            }
                        }
                    }
                }
            }
        }

        public void ApplyFilter()
        {
            switch (Condition)
            {
                case NumberCondition.IsEqualTo:
                    FilterManager.Column.Filter = Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Expression.NotEqual(FilterManager.Column.Property.Body, Expression.Constant(null)),
                            Expression.Equal(
                                Expression.Convert(FilterManager.Column.Property.Body, FilterManager.Column.Type.GetNonNullableType()),
                                Expression.Constant(Convert.ChangeType(FilterValue, FilterManager.Column.Type.GetNonNullableType())))),
                        FilterManager.Column.Property.Parameters);
                    break;
                case NumberCondition.IsNotEqualTo:
                    FilterManager.Column.Filter = Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Expression.NotEqual(FilterManager.Column.Property.Body, Expression.Constant(null)),
                            Expression.NotEqual(
                                Expression.Convert(FilterManager.Column.Property.Body, FilterManager.Column.Type.GetNonNullableType()),
                                Expression.Constant(Convert.ChangeType(FilterValue, FilterManager.Column.Type.GetNonNullableType())))),
                        FilterManager.Column.Property.Parameters);
                    break;
                case NumberCondition.IsGreaterThanOrEqualTo:
                    FilterManager.Column.Filter = Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Expression.NotEqual(FilterManager.Column.Property.Body, Expression.Constant(null)),
                            Expression.GreaterThanOrEqual(
                                Expression.Convert(FilterManager.Column.Property.Body, FilterManager.Column.Type.GetNonNullableType()),
                                Expression.Constant(Convert.ChangeType(FilterValue, FilterManager.Column.Type.GetNonNullableType())))),
                        FilterManager.Column.Property.Parameters);
                    break;
                case NumberCondition.IsGreaterThan:
                    FilterManager.Column.Filter = Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Expression.NotEqual(FilterManager.Column.Property.Body, Expression.Constant(null)),
                            Expression.GreaterThan(
                                Expression.Convert(FilterManager.Column.Property.Body, FilterManager.Column.Type.GetNonNullableType()),
                                Expression.Constant(Convert.ChangeType(FilterValue, FilterManager.Column.Type.GetNonNullableType())))),
                        FilterManager.Column.Property.Parameters);
                    break;
                case NumberCondition.IsLessThanOrEqualTo:
                    FilterManager.Column.Filter = Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Expression.NotEqual(FilterManager.Column.Property.Body, Expression.Constant(null)),
                            Expression.LessThanOrEqual(
                                Expression.Convert(FilterManager.Column.Property.Body, FilterManager.Column.Type.GetNonNullableType()),
                                Expression.Constant(Convert.ChangeType(FilterValue, FilterManager.Column.Type.GetNonNullableType())))),
                        FilterManager.Column.Property.Parameters);
                    break;
                case NumberCondition.IsLessThan:
                    FilterManager.Column.Filter = Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Expression.NotEqual(FilterManager.Column.Property.Body, Expression.Constant(null)),
                            Expression.LessThan(
                                Expression.Convert(FilterManager.Column.Property.Body, FilterManager.Column.Type.GetNonNullableType()),
                                Expression.Constant(Convert.ChangeType(FilterValue, FilterManager.Column.Type.GetNonNullableType())))),
                        FilterManager.Column.Property.Parameters);
                    break;
                case NumberCondition.IsNull:
                    FilterManager.Column.Filter = Expression.Lambda<Func<TableItem, bool>>(
                            Expression.Equal(FilterManager.Column.Property.Body, Expression.Constant(null)),
                        FilterManager.Column.Property.Parameters);
                    break;
                case NumberCondition.IsNotNull:
                    FilterManager.Column.Filter = Expression.Lambda<Func<TableItem, bool>>(
                            Expression.NotEqual(FilterManager.Column.Property.Body, Expression.Constant(null)),
                        FilterManager.Column.Property.Parameters);
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
