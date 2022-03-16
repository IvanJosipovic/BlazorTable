using BlazorTable.Localization;
using Microsoft.AspNetCore.Components;
using System;
using System.Globalization;
using System.Linq.Expressions;
using Microsoft.Extensions.Localization;
using BlazorTable.Components.ServerSide;

namespace BlazorTable
{
    public partial class EnumFilter<TableItem> : IFilter<TableItem>
    {
        [CascadingParameter(Name = "Column")]
        public IColumn<TableItem> Column { get; set; }

        [Inject]
        IStringLocalizer<Localization.Localization> Localization { get; set; }

        private EnumCondition Condition { get; set; }

        private object FilterValue { get; set; }

        protected override void OnInitialized()
        {
            if (Column.Type.GetNonNullableType().IsEnum)
            {
                Column.FilterControl = this;

                if (Column.InitialFilterString != null)
                {
                    Condition = Utilities.ParseEnum<EnumCondition>(Column.InitialFilterString.Condition);
                    FilterValue = Column.InitialFilterString.FilterValue;
                    Column.InitialFilterString = null;

                    Column.Filter = GetFilter();
                }

                if (Column.Filter?.Body is BinaryExpression binaryExpression
                    && binaryExpression.Right is BinaryExpression logicalBinary
                    && logicalBinary.Right is ConstantExpression constant)
                {
                    switch (binaryExpression.Right.NodeType)
                    {
                        case ExpressionType.Equal:
                            Condition = constant.Value == null ? EnumCondition.IsNull : EnumCondition.IsEqualTo;
                            break;
                        case ExpressionType.NotEqual:
                            Condition = constant.Value == null ? EnumCondition.IsNotNull : EnumCondition.IsNotEqualTo;
                            break;
                    }

                    FilterValue = constant.Value;
                }

                if (FilterValue == null)
                {
                    FilterValue = Enum.GetValues(Column.Type.GetNonNullableType()).GetValue(0);
                }
            }
        }

        public Expression<Func<TableItem, bool>> GetFilter()
        {
            if (Column.InitialFilterString != null)
            {
                Condition = Utilities.ParseEnum<EnumCondition>(Column.InitialFilterString.Condition);
                FilterValue = Column.InitialFilterString.FilterValue;
                Column.InitialFilterString = null;
            }

            return Condition switch
            {
                EnumCondition.IsEqualTo =>
                    Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Column.Field.Body.CreateNullChecks(),
                            Expression.Equal(
                                Expression.Convert(Column.Field.Body, Column.Type.GetNonNullableType()),
                                Expression.Constant(Convert.ChangeType(FilterValue, Column.Type.GetNonNullableType(), CultureInfo.InvariantCulture)))),
                        Column.Field.Parameters),

                EnumCondition.IsNotEqualTo =>
                    Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Column.Field.Body.CreateNullChecks(),
                            Expression.NotEqual(
                                Expression.Convert(Column.Field.Body, Column.Type.GetNonNullableType()),
                                Expression.Constant(Convert.ChangeType(FilterValue, Column.Type.GetNonNullableType(), CultureInfo.InvariantCulture)))),
                        Column.Field.Parameters),

                EnumCondition.IsNull =>
                    Expression.Lambda<Func<TableItem, bool>>(
                        Expression.AndAlso(
                            Column.Field.Body.CreateNullChecks(true),
                            Expression.Equal(Column.Field.Body, Expression.Constant(null))),
                        Column.Field.Parameters),

                EnumCondition.IsNotNull =>
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
                FilterValue = FilterValue.ToString()
            }; 
        }

        public enum EnumCondition
        {
            [LocalizedDescription("EnumConditionIsEqualTo", typeof(Localization.Localization))]
            IsEqualTo,

            [LocalizedDescription("EnumConditionIsNotEqualTo", typeof(Localization.Localization))]
            IsNotEqualTo,

            [LocalizedDescription("EnumConditionIsNull", typeof(Localization.Localization))]
            IsNull,

            [LocalizedDescription("EnumConditionIsNotNull", typeof(Localization.Localization))]
            IsNotNull
        }
    }
}
