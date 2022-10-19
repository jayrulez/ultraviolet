using Sedulous.Core;

namespace Sedulous
{
    /// <summary>
    /// Represents a standard set of JSON serializer settings for loading Sedulous objects.
    /// </summary>
    public class SedulousJsonSerializerSettings : CoreJsonSerializerSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SedulousJsonSerializerSettings"/> class.
        /// </summary>
        public SedulousJsonSerializerSettings()
        { }

        /// <summary>
        /// Gets the singleton instance of the <see cref="SedulousJsonSerializerSettings"/> class.
        /// </summary>
        public new static SedulousJsonSerializerSettings Instance { get; } = new SedulousJsonSerializerSettings();

        /// <inheritdoc/>
        protected override void SetContractResolver()
        {
            ContractResolver = new SedulousJsonContractResolver();
        }

        /// <inheritdoc/>
        protected override void SetConverters()
        {
            Converters.Add(new SedulousJsonConverter());
        }
    }
}
