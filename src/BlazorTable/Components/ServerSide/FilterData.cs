using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BlazorTable.Components.ServerSide
{
    public class FilterData<TableItem>
    {
        public string OrderBy { get; set; }

        public string Query { get; set; }

        public int? Top { get; set; }

        public int? Skip { get; set; }

        public List<Expression<Func<TableItem, bool>>> Filters { get; set; }

        public List<FilterString> FilterStrings { get; set; }
    }

}
