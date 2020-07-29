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

            if (Column.Filter?.Body is BinaryExpression binaryExpression
                && binaryExpression.Right is BinaryExpression logicalBinary
                && logicalBinary.Right is ConstantExpression constant)
            {
                switch (logicalBinary.NodeType)
                {
                    case ExpressionType.Equal:
                            Condition = constant.Value == null ? CustomSelectCondition.IsNull : CustomSelectCondition.IsEqualTo;
                            break;
                    case ExpressionType.NotEqual:
                            Condition = constant.Value == null ? CustomSelectCondition.IsNotNull : CustomSelectCondition.IsNotEqualTo;
                            break;
                }

                FilterValue = constant.Value;
            }
        }

        public Expression<Func<TableItem, bool>> GetFilter()
        {
            return Condition switch
            {
                CustomSelectCondition.IsEqualTo =>
                    Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Column.Field.Body.CreateNullChecks(),
                            Expression.Equal(
                                Expression.Convert(Column.Field.Body, Column.Type.GetNonNullableType()),
                                Expression.Constant(Convert.ChangeType(FilterValue, Column.Type.GetNonNullableType(), CultureInfo.InvariantCulture)))),
                        Column.Field.Parameters),

                CustomSelectCondition.IsNotEqualTo => Expression.Lambda<Func<TableItem, bool>>(
                    Expression.AndAlso(
                        Column.Field.Body.CreateNullChecks(),
                        Expression.NotEqual(
                            Expression.Convert(Column.Field.Body, Column.Type.GetNonNullableType()),
                            Expression.Constant(Convert.ChangeType(FilterValue, Column.Type.GetNonNullableType(), CultureInfo.InvariantCulture)))),
                    Column.Field.Parameters),

                CustomSelectCondition.IsNull =>
                    Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Column.Field.Body.CreateNullChecks(true),
                            Expression.Equal(Column.Field.Body, Expression.Constant(null))),
                        Column.Field.Parameters),

                CustomSelectCondition.IsNotNull =>
                    Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Column.Field.Body.CreateNullChecks(true),
                            Expression.NotEqual(Column.Field.Body, Expression.Constant(null))),
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
            [Description("Is equal to")]
            IsEqualTo,

            [Description("Is not equal to")]
            IsNotEqualTo,

            [Description("Is null")]
            IsNull,

            [Description("Is not null")]
            IsNotNull
        }
    }
}
