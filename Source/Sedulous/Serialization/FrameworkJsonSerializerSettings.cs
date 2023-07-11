using Sedulous.Core;

namespace Sedulous
{
    /// <summary>
    /// Represents a standard set of JSON serializer settings for loading Sedulous objects.
    /// </summary>
    public class FrameworkJsonSerializerSettings : CoreJsonSerializerSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameworkJsonSerializerSettings"/> class.
        /// </summary>
        public FrameworkJsonSerializerSettings()
        { }

        /// <summary>
        /// Gets the singleton instance of the <see cref="FrameworkJsonSerializerSettings"/> class.
        /// </summary>
        public new static FrameworkJsonSerializerSettings Instance { get; } = new FrameworkJsonSerializerSettings();

        /// <inheritdoc/>
        protected override void SetContractResolver()
        {
            ContractResolver = new FrameworkJsonContractResolver();
        }

        /// <inheritdoc/>
        protected override void SetConverters()
        {
            Converters.Add(new FrameworkJsonConverter());
        }
    }
}
