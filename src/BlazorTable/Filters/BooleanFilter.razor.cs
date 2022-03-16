﻿using BlazorTable.Components.ServerSide;
using BlazorTable.Localization;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BlazorTable
{
    public partial class BooleanFilter<TableItem> : IFilter<TableItem>
    {
        [CascadingParameter(Name = "Column")]
        public IColumn<TableItem> Column { get; set; }

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

                if (Column.InitialFilterString != null)
                {
                    Condition = Utilities.ParseEnum<BooleanCondition>(Column.InitialFilterString.Condition);
                    
                    Column.InitialFilterString = null;

                    Column.Filter = GetFilter();
                }

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
            if (Column.InitialFilterString != null)
            {
                Condition = Utilities.ParseEnum<BooleanCondition>(Column.InitialFilterString.Condition);

                Column.InitialFilterString = null;
            }

            return Condition switch
            {
                BooleanCondition.True =>
                    Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Column.Field.Body.CreateNullChecks(),
                            Expression.IsTrue(Expression.Convert(Column.Field.Body, Column.Type.GetNonNullableType()))),
                        Column.Field.Parameters),

                BooleanCondition.False =>
                    Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Column.Field.Body.CreateNullChecks(),
                            Expression.IsFalse(Expression.Convert(Column.Field.Body, Column.Type.GetNonNullableType()))),
                            Column.Field.Parameters),

                BooleanCondition.IsNull =>
                    Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Column.Field.Body.CreateNullChecks(true),
                            Expression.Equal(Column.Field.Body, Expression.Constant(null))),
                        Column.Field.Parameters),

                BooleanCondition.IsNotNull =>
                    Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Column.Field.Body.CreateNullChecks(true),
                            Expression.NotEqual(Column.Field.Body, Expression.Constant(null))),
                        Column.Field.Parameters),

                _ => null,
            };
        }
        public FilterString GetFilterString()
        {
            return new FilterString()
            {
                Field = Column.Field.GetPropertyMemberInfo().Name,
                Condition = Condition.ToString()
            };
        }
    }

    

    public enum BooleanCondition
    {
        [LocalizedDescription("BooleanConditionTrue", typeof(Localization.Localization))]
        True,

        [LocalizedDescription("BooleanConditionFalse", typeof(Localization.Localization))]
        False,

        [LocalizedDescription("BooleanConditionIsNull", typeof(Localization.Localization))]
        IsNull,

        [LocalizedDescription("BooleanConditionIsNotNull", typeof(Localization.Localization))]
        IsNotNull
    }
}
