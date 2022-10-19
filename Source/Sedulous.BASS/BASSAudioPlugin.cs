using Sedulous.Core;

namespace Sedulous.BASS
{
    /// <summary>
    /// Represents an Sedulous plugin which registers BASS as the audio subsystem implementation.
    /// </summary>
    public class BASSAudioPlugin : SedulousPlugin
    {
        /// <inheritdoc/>
        public override void Register(SedulousConfiguration configuration)
        {
            Contract.Require(configuration, nameof(configuration));

            var asm = typeof(BASSAudioPlugin).Assembly;
            configuration.AudioSubsystemAssembly = $"{asm.GetName().Name}, Version={asm.GetName().Version}, Culture=neutral, PublicKeyToken=78da2f4877323311, processorArchitecture=MSIL";

            base.Register(configuration);
        }
    }
}
