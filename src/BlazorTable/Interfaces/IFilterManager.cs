using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace BlazorTable
{
    public interface IFilterManager<TableItem>
    {
        IColumn<TableItem> Column { get; }

        IFilter<TableItem> Filter { get; set; }
    }
}