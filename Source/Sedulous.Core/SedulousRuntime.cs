namespace Sedulous.Core
{
    /// <summary>
    /// Represents the .NET runtime implementations which are supported by the Sedulous Framework.
    /// </summary>
    public enum SedulousRuntime
    {
        /// <summary>
        /// The Microsoft CLR.
        /// </summary>
        CLR,

        /// <summary>
        /// The .NET Core runtime.
        /// </summary>
        CoreCLR,

        /// <summary>
        /// The Mono runtime.
        /// </summary>
        Mono,
    }
}
