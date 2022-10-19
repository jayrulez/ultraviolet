using Sedulous.Core;

namespace Sedulous.FMOD
{
    /// <summary>
    /// Represents an Sedulous plugin which registers FMOD as the audio subsystem implementation.
    /// </summary>
    public class FMODAudioPlugin : SedulousPlugin
    {
        /// <inheritdoc/>
        public override void Register(SedulousConfiguration configuration)
        {
            Contract.Require(configuration, nameof(configuration));

            var asm = typeof(FMODAudioPlugin).Assembly;
            configuration.AudioSubsystemAssembly = $"{asm.GetName().Name}, Version={asm.GetName().Version}, Culture=neutral, PublicKeyToken=78da2f4877323311, processorArchitecture=MSIL";

            base.Register(configuration);
        }
    }
}
