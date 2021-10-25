namespace BlazorTable
{
    /// <summary>
    /// Options to configure BlazorTable
    /// </summary>
    public class BlazorTableOptions
    {
        internal bool IsBootstrap5 { get; private set; }

        /// <summary>
        /// Use Bootstrap version 5 instead of the default version 4
        /// </summary>
        public BlazorTableOptions UseBootstrap5()
        {
            IsBootstrap5 = true;
            return this;
        }
    }
}