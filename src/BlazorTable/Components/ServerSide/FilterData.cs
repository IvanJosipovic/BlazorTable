namespace BlazorTable.Components.ServerSide
{
    public class FilterData
    {
        public string OrderBy { get; set; }

        public string Query { get; set; }

        public int? Top { get; set; }

        public int? Skip { get; set; }
    }

}
