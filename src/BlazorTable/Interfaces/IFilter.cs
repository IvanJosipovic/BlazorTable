using BlazorTable.Components.ServerSide;
using System;
using System.Linq.Expressions;

namespace BlazorTable
{
    /// <summary>
    /// Filter Component Interface
    /// </summary>
    /// <typeparam name="TableItem"></typeparam>
    public interface IFilter<TableItem>
    {
        /// <summary>
        /// Get Filter Expression
        /// </summary>
        /// <returns></returns>
        Expression<Func<TableItem, bool>> GetFilter();

        /// <summary>
        /// Get filter as string object
        /// </summary>
        /// <returns></returns>
        FilterString GetFilterString();
    }
}
