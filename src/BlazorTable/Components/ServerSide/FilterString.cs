namespace BlazorTable.Components.ServerSide
{

    public class FilterString
    {
        /// <summary>
        /// Field name
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// it should be a enum converted to string according with field type. Available enums are StringCondition, NumberCondition, EnumCondition, CustomSelectCondition and BooleanCondition
        /// </summary>
        public string Condition { get; set; }

        /// <summary>
        /// Value to filter
        /// </summary>
        public string FilterValue { get; set; }
    }
}
