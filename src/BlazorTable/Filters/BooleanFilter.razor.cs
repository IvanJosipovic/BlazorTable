using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace BlazorTable
{
    public partial class BooleanFilter<TableItem> : IFilter<TableItem>
    {
        [CascadingParameter(Name = "Column")] public IColumn<TableItem> Column { get; set; }

        [Inject] public ILogger<NumberFilter<TableItem>> Logger { get; set; }

        private BooleanCondition Condition { get; set; }

        public List<Type> FilterTypes => new List<Type>()
        {
            typeof(bool)
        };

        protected override void OnInitialized()
        {
            if (FilterTypes.Contains(Column.Type.GetNonNullableType()))
            {
                Column.FilterControl = this;

                if (Column.Filter != null)
                {
                    var nodeType = Column.Filter.Body.NodeType;

                    if (Column.Filter.Body is BinaryExpression binaryExpression
                        && binaryExpression.NodeType == ExpressionType.AndAlso)
                    {
                        nodeType = binaryExpression.Right.NodeType;
                    }

                    switch (nodeType)
                    {
                        case ExpressionType.IsTrue:
                            Condition = BooleanCondition.True;
                            break;
                        case ExpressionType.IsFalse:
                            Condition = BooleanCondition.False;
                            break;
                        case ExpressionType.Equal:
                            Condition = BooleanCondition.IsNull;
                            break;
                        case ExpressionType.NotEqual:
                            Condition = BooleanCondition.IsNotNull;
                            break;
                    }
                }
            }
        }

        public Expression<Func<TableItem, bool>> GetFilter()
        {
            switch (Condition)
            {
                case BooleanCondition.True:
                    return Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Expression.NotEqual(Column.Field.Body, Expression.Constant(null)),
                            Expression.IsTrue(Expression.Convert(Column.Field.Body, Column.Type.GetNonNullableType()))),
                        Column.Field.Parameters);

                case BooleanCondition.False:
                    return Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Expression.NotEqual(Column.Field.Body, Expression.Constant(null)),
                            Expression.IsFalse(Expression.Convert(Column.Field.Body, Column.Type.GetNonNullableType()))),
                        Column.Field.Parameters);

                case BooleanCondition.IsNull:
                    return Expression.Lambda<Func<TableItem, bool>>(
                        Expression.Equal(Column.Field.Body, Expression.Constant(null)),
                        Column.Field.Parameters);

                case BooleanCondition.IsNotNull:
                    return Expression.Lambda<Func<TableItem, bool>>(
                        Expression.NotEqual(Column.Field.Body, Expression.Constant(null)),
                        Column.Field.Parameters);
            }

            return null;
        }
    }

    public enum BooleanCondition
    {
        [Description("True")]
        True,

        [Description("False")]
        False,

        [Description("Is null")]
        IsNull,

        [Description("Is not null")]
        IsNotNull
    }
}
