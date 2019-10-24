using System;
using System.Linq.Expressions;

namespace BlazorTable
{
    public interface IFilter<TableItem>
    {
        Expression<Func<TableItem, bool>> GetFilter();
    }
}
