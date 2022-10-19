namespace Sedulous.FMOD
{
    /// <summary>
    /// Initializes factory methods for the FMOD implementation of the audio subsystem.
    /// </summary>
    public sealed class FMODAndroidFactoryInitializer : ISedulousFactoryInitializer
    {
        /// <inheritdoc/>
        public void Initialize(SedulousContext owner, SedulousFactory factory)
        {
            factory.SetFactoryMethod<FMODPlatformSpecificImplementationDetailsFactory>((uv) => new FMODAndroidSpecificImplementationDetails());
        }
    }
}
