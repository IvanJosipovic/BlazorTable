using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace BlazorTable
{
    internal static class Utillities
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
                        //return IsNumeric(Nullable.GetUnderlyingType(type));
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

        public static Expression<Func<T, bool>> CallMethodType<T>(Expression<Func<T, object>> expression, Type type, string method, Type parameter, object value)
        {
            return CallMethodType(expression, type, method, new[] { parameter }, new[] { value });
        }

        public static Expression<Func<T, bool>> CallMethodType<T>(Expression<Func<T, object>> expression, Type type, string method, Type[] parameters, object[] values)
        {
            MethodInfo methodInfo = type.GetMethod(method, parameters);

            return Expression.Lambda<Func<T, bool>>(
                Expression.Call(
                    expression.Body,
                    methodInfo,
                    values.OrEmptyIfNull().Select(Expression.Constant)),
                expression.Parameters);
        }

        public static Expression<Func<T, bool>> CallMethodTypeStaticSelf<T>(Expression<Func<T, object>> expression, Type type, string method, Type parameter)
        {
            MethodInfo methodInfo = type.GetMethod(method, new[] { parameter });

            var call = Expression.Call(methodInfo, expression.Body);

            return Expression.Lambda<Func<T, bool>>(call, expression.Parameters);
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

        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
        {
            return Expression.Lambda<Func<T, bool>>(Expression.Not(expression.Body), expression.Parameters[0]);
        }
    }
}
