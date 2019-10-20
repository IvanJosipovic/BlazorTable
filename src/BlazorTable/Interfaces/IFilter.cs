using System;

namespace BlazorTable
{
    public interface IFilter<TableItem>
    {
        void ApplyFilter();
    }
}
