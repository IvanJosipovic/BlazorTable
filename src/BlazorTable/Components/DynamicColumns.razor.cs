using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace BlazorTable
{
    /// <summary>
    /// BlazorTable Dynamic Columns
    /// </summary>
    /// <typeparam name="TableItem"></typeparam>
    public partial class DynamicColumns<TableItem>
    {
        /// <summary>
        /// Parent Table
        /// </summary>
        [CascadingParameter(Name = "Table")]
        public ITable<TableItem> Table { get; set; }

        /// <summary>
        /// Column can be sorted
        /// </summary>
        [Parameter]
        public bool Sortable { get; set; }

        /// <summary>
        /// Column can be filtered
        /// </summary>
        [Parameter]
        public bool Filterable { get; set; }

        /// <summary>
        /// Horizontal alignment
        /// </summary>
        [Parameter]
        public Align Align { get; set; }

        /// <summary>
        /// Column CSS Class
        /// </summary>
        [Parameter]
        public string Class { get; set; }

        [Parameter]
        public Type Type { get; set; }

        private static Expression<Func<TModel, T>> GenerateMemberExpression<TModel, T>(PropertyInfo propertyInfo)
        {
            var entityParam = Expression.Parameter(typeof(TModel), "x");

            Expression columnExpr = null;
            
            if (propertyInfo.ReflectedType.Name == "JObject")
            {
                var assembly = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.ManifestModule.Name == "Newtonsoft.Json.dll").First();

                var type = propertyInfo.ReflectedType;// assembly.GetType("Newtonsoft.Json.Linq.JToken");
                var exttype = assembly.GetType("Newtonsoft.Json.Linq.Extensions");

                columnExpr = Expression.Call(exttype.GetMethod("Value", new[] { typeof(IEnumerable<>).MakeGenericType(type) }).MakeGenericMethod(new[] {type}),
                                 Expression.Property(
                                    Expression.Call(entityParam, "Property", null, Expression.Constant(propertyInfo.Name)),
                                    "Value")
                            );
            }
            else
            {
                columnExpr = Expression.Property(entityParam, propertyInfo);

                if (propertyInfo.PropertyType != typeof(T))
                    columnExpr = Expression.Convert(columnExpr, typeof(T));
            }

            return Expression.Lambda<Func<TModel, T>>(columnExpr, entityParam);
        }

        private string RenderProperty(TableItem data, PropertyInfo property, Func<TableItem, object> func = null)
        {
            if (property.ReflectedType.Name == "JObject")
            {
                return "";// func.Invoke(data)?.ToString();
            }

            object rawData = property.GetValue(data);

            if (rawData == null)
                return "";

            if (rawData.GetType().IsEnum)
            {
                Type enumType = property.GetValue(data).GetType();

                MemberInfo[] memberInfo = enumType.GetMember(rawData.ToString());
                if (memberInfo != null && memberInfo.Length > 0)
                {
                    object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                    if (attrs != null && attrs.Length > 0)
                    {
                        //Pull out the description value
                        return ((DescriptionAttribute)attrs[0]).Description;
                    }
                }
            }

            return rawData.ToString();
        }
    }
}