using Sedulous;
using Sedulous.Input;

namespace Sandbox3D.Input
{
    /// <summary>
    /// Contains extension methods for the ISedulousInput interface.
    /// </summary>
    public static class IFrameworkInputExtensions
    {
        /// <summary>
        /// Gets the game's input actions.
        /// </summary>
        /// <param name="this">The Sedulous Input subsystem.</param>
        /// <returns>The game's input actions.</returns>
        public static GameInputActions GetActions(this IInputSubsystem @this)
        {
            return actions;
        }

        // The singleton instance that represents the game's collection of input actions.
        // This instance is bound to the lifespan of the Sedulous context that creates it.
        private static readonly FrameworkSingleton<GameInputActions> actions = 
            InputActionCollection.CreateSingleton<GameInputActions>();
    }
}