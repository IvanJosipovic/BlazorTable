﻿using LinqKit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace BlazorTable
{
    public static class Utillities
    {
        public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }

        public static IDictionary<T, V> OrEmptyIfNull<T, V>(this IDictionary<T, V> source)
        {
            return source ?? new Dictionary<T, V>();
        }

        public static bool IsNumeric(this Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                case TypeCode.Object:
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return Nullable.GetUnderlyingType(type).IsNumeric();
                    }
                    return false;
                default:
                    return false;
            }
        }

        public static Type GetNonNullableType(this Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }

        public static string GetDescription<T>(this T e) where T : IConvertible
        {
            if (e is Enum)
            {
                Type type = e.GetType();
                Array values = Enum.GetValues(type);

                foreach (int val in values)
                {
                    if (val == e.ToInt32(CultureInfo.InvariantCulture))
                    {
                        var memInfo = type.GetMember(type.GetEnumName(val));

                        if (memInfo[0]
                            .GetCustomAttributes(typeof(DescriptionAttribute), false)
                            .FirstOrDefault() is DescriptionAttribute descriptionAttribute)
                        {
                            return descriptionAttribute.Description;
                        }
                    }
                }
            }

            return null; // could also return string.Empty
        }

        public static Type GetMemberUnderlyingType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                default:
                    throw new ArgumentException("MemberInfo must be if type FieldInfo, PropertyInfo or EventInfo", nameof(member));
            }
        }

        public static MemberInfo GetPropertyMemberInfo<T>(this Expression<Func<T, object>> expression)
        {
            if (expression == null)
            {
                return null;
            }

            if (!(expression.Body is MemberExpression body))
            {
                UnaryExpression ubody = (UnaryExpression)expression.Body;
                body = ubody.Operand as MemberExpression;
            }

            return body?.Member;
        }

        public static string ToDescriptionString(this Enum val)
        {
            var attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }

        /// <summary>
        /// Recursively walks up the tree and adds null checks
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> AddNullChecks<T>(this Expression<Func<T, bool>> expression)
        {
            var parents = new Queue<MemberExpression>();
            Expression<Func<T, bool>> tempExpression = expression;

            if (expression?.Body is BinaryExpression binary)
            {
                if (binary.Left is MemberExpression member)
                {
                    // From here we're looking at parents
                    Recurse(member.Expression);
                }
                else if (expression?.Body is BinaryExpression binary2)
                {
                    if (binary2.Left is BinaryExpression binary3 && binary3.Left is MemberExpression member2)
                    {
                        // From here we're looking at parents
                        Recurse(member2.Expression);
                    }
                }
            }

            while (parents.Count > 0)
            {
                var nullCheck = Expression.NotEqual(parents.Dequeue(), Expression.Constant(null));

                var newQuery = Expression.Lambda<Func<T, bool>>(nullCheck, expression.Parameters);

                tempExpression = newQuery.And(tempExpression);
            }

            return tempExpression;

            void Recurse(object expression)
            {
                if (expression is MemberExpression member)
                {
                    parents.Enqueue(member);
                    Recurse(member.Expression);
                }
            }
        }
    }
}
