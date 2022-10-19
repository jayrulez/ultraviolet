using Sedulous.Graphics;

namespace Sedulous.OpenGL.Graphics
{
    /// <summary>
    /// Represents the block of effect parameters which are used by <see cref="OpenGLSkinnedEffect"/>.
    /// </summary>
    internal class OpenGLSkinnedEffectParameterBlock : OpenGLBasicEffectParameterBlock
    {
        public EffectParameter Bones { get; set; }
    }
}
