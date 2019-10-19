using Microsoft.AspNetCore.Components;
using System;
using System.Linq.Expressions;

namespace BlazorTable
{
    public interface IColumn<TableItem>
    {
        string Title { get; set; }

        string Width { get; set; }

        bool Sortable { get; set; }

        bool Filterable { get; set; }

        bool FilterOpen { get; }

        void ToggleFilter();

        Expression<Func<TableItem, object>> Property { get; set; }

        Expression<Func<TableItem, bool>> Filter { get; set; }

        RenderFragment<TableItem> EditorTemplate { get; set; }

        RenderFragment<TableItem> Template { get; set; }
    }
}
