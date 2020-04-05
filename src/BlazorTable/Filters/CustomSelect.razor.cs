using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace BlazorTable
{
    public partial class CustomSelect<TableItem> : IFilter<TableItem>, ICustomSelect
    {
        [CascadingParameter(Name = "Column")]
        public IColumn<TableItem> Column { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        private List<KeyValuePair<string, object>> Items = new List<KeyValuePair<string, object>>();

        private CustomSelectCondition Condition { get; set; }

        private object FilterValue { get; set; }

        protected override void OnInitialized()
        {
            Column.FilterControl = this;

            if (Column.Filter?.Body is BinaryExpression binaryExpression)
            {
                if (binaryExpression.NodeType == ExpressionType.AndAlso)
                {
                    switch (binaryExpression.Right.NodeType)
                    {
                        case ExpressionType.Equal:
                            Condition = CustomSelectCondition.IsEqualTo;
                            break;
                        case ExpressionType.NotEqual:
                            Condition = CustomSelectCondition.IsNotEqualTo;
                            break;
                    }
                }
                else
                {
                    if (binaryExpression.NodeType == ExpressionType.Equal)
                    {
                        Condition = CustomSelectCondition.IsNull;
                    }
                    else if (binaryExpression.NodeType == ExpressionType.NotEqual)
                    {
                        Condition = CustomSelectCondition.IsNotNull;
                    }
                }

                if (binaryExpression.Right is BinaryExpression binaryExpression2
                    && binaryExpression2.Right is ConstantExpression constantExpression)
                {
                    FilterValue = constantExpression.Value;
                }
            }
        }

        public Expression<Func<TableItem, bool>> GetFilter()
        {
            return Condition switch
            {
                CustomSelectCondition.IsEqualTo =>
                    Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Expression.NotEqual(Column.Field.Body, Expression.Constant(null)),
                            Expression.Equal(
                                Expression.Convert(Column.Field.Body, Column.Type.GetNonNullableType()),
                                Expression.Constant(Convert.ChangeType(FilterValue, Column.Type.GetNonNullableType(), CultureInfo.InvariantCulture)))),
                        Column.Field.Parameters),

                CustomSelectCondition.IsNotEqualTo => Expression.Lambda<Func<TableItem, bool>>(
                    Expression.AndAlso(
                        Expression.NotEqual(Column.Field.Body, Expression.Constant(null)),
                        Expression.NotEqual(
                            Expression.Convert(Column.Field.Body, Column.Type.GetNonNullableType()),
                            Expression.Constant(Convert.ChangeType(FilterValue, Column.Type.GetNonNullableType(), CultureInfo.InvariantCulture)))),
                    Column.Field.Parameters),

                CustomSelectCondition.IsNull =>
                    Expression.Lambda<Func<TableItem, bool>>(
                        Expression.Equal(Column.Field.Body, Expression.Constant(null)),
                    Column.Field.Parameters),

                CustomSelectCondition.IsNotNull =>
                    Expression.Lambda<Func<TableItem, bool>>(
                        Expression.NotEqual(Column.Field.Body, Expression.Constant(null)),
                    Column.Field.Parameters),

                _ => throw new ArgumentException(Condition + " is not defined!"),
            };
        }

        public void AddSelect(string key, object value)
        {
            Items.Add(new KeyValuePair<string, object>(key, value));

            if (FilterValue == null)
            {
                FilterValue = Items.FirstOrDefault().Value;
            }

            StateHasChanged();
        }

        public enum CustomSelectCondition
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
