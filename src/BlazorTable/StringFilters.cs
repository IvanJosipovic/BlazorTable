using System.ComponentModel;

namespace BlazorTable
{
    public enum StringFilters
    {
        [Description("Contains")]
        Contains,

        [Description("Does not contain")]
        Does_not_contain,

        [Description("Starts with")]
        Starts_with,

        [Description("Ends with")]
        Ends_with,

        [Description("Is equal to")]
        Is_equal_to,

        [Description("Is not equal to")]
        Is_not_equal_to,

        [Description("Is null or empty")]
        Is_null_or_empty,

        [Description("Is not null or empty")]
        Is_not_null_or_empty
    }
}