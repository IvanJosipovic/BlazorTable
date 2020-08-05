using LinqKit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace BlazorTable
{
    internal static class Utilities
    {
        /// <summary>
        /// Calculates Sum or Average of a column base on given field name.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="member"></param>
        /// <param name="aggregateType"></param>
        /// <returns></returns>
        public static object Aggregate(this IQueryable source, string member, AggregateType aggregateType)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (member == null) throw new ArgumentNullException(nameof(member));

            // The most common variant of Queryable.Sum() expects a lambda.
            // Since we just have a string to a property, we need to create a
            // lambda from the string in order to pass it to the sum method.

            // Lets create a ((TSource s) => s.Price ). First up, the parameter "s":
            ParameterExpression parameter = Expression.Parameter(source.ElementType, "s");

            // Followed by accessing the Price property of "s" (s.Price):
            PropertyInfo property = source.ElementType.GetProperty(member, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (property == null) return source;

            MemberExpression getter = Expression.MakeMemberAccess(parameter, property);

            // And finally, we create a lambda from that. First specifying on what
            // to execute when the lambda is called, and finally the parameters of the lambda.
            Expression selector = Expression.Lambda(getter, parameter);

            try
            {
                // There are a lot of Queryable.Sum() overloads with different
                // return types  (double, int, decimal, double?, int?, etc...).
                // We're going to find one that matches the type of our property.
                MethodInfo aggregateMethod = typeof(Queryable).GetMethods().First(
                    m => m.Name == aggregateType.ToString()
                         && m.ReturnType == property.PropertyType
                         && m.IsGenericMethod);

                // Now that we have the correct method, we need to know how to call the method.
                // Note that the Queryable.Sum<TSource>(source, selector) has a generic type,
                // which we haven't resolved yet. Good thing is that we can use copy the one from
                // our initial source expression.
                var genericAggregateMethod = aggregateMethod.MakeGenericMethod(new[] { source.ElementType });

                // TSource, source and selector are now all resolved. We now know how to call
                // the sum-method. We're not going to call it here, we just express how we're going
                // call it.
                var callExpression = Expression.Call(
                    null,
                    genericAggregateMethod,
                    new[] { source.Expression, Expression.Quote(selector) });

                // Pass it down to the query provider. This can be a simple LinqToObject-datasource,
                // but also a more complex datasource (such as LinqToSql). Anyway, it knows what to
                // do.
                return source.Provider.Execute(callExpression);
            }
            catch (Exception)
            {
                throw new InvalidOperationException($"The {aggregateType} aggregation cannot be used for {member} field. The {member} field must be in numeric data type to perform this operation.");
            }
        }

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
        /// <param name="skipFinalMember"></param>
        /// <returns></returns>
        public static BinaryExpression CreateNullChecks(this Expression expression, bool skipFinalMember = false)
        {
            var parents = new Stack<BinaryExpression>();

            BinaryExpression newExpression = null;

            if (expression is UnaryExpression unary)
            {
                expression = unary.Operand;
            }

            MemberExpression temp = expression as MemberExpression;

            while (temp is MemberExpression member)
            {
                try
                {
                    var nullCheck = Expression.NotEqual(temp, Expression.Constant(null));
                    parents.Push(nullCheck);
                }
                catch (InvalidOperationException){}

                temp = member.Expression as MemberExpression;
            }

            while (parents.Count > 0)
            {
                if (skipFinalMember && parents.Count == 1 && newExpression != null)
                    break;
                else if (newExpression == null)
                    newExpression = parents.Pop();
                else
                    newExpression = Expression.AndAlso(newExpression, parents.Pop());
            }

            if (newExpression == null)
            {
                return Expression.Equal(Expression.Constant(true), Expression.Constant(true));
            }

            return newExpression;
        }
    }
}
