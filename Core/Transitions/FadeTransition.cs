using Microsoft.Xna.Framework;
using System;

namespace Velentr.States.Transitions
{
    /// <summary>
    /// A fade transition.
    /// </summary>
    ///
    /// <seealso cref="Transition"/>
    public class FadeTransition : Transition
    {
        /// <summary>
        /// The fade color.
        /// </summary>
        private Color _fadeColor;

        /// <summary>
        /// Constructor.
        /// </summary>
        ///
        /// <param name="fadeColor"> The color of the fade. </param>
        /// <param name="lifespan">  The lifespan. </param>
        public FadeTransition(Color fadeColor, TimeSpan lifespan) : base(lifespan, BuiltInTransitionType.Fade)
        {
            _fadeColor = fadeColor;
        }

        /// <summary>
        /// Draws this object.
        /// </summary>
        public override void Draw()
        {
            _spriteBatch.Draw(_baseTexture, new Rectangle(0, 0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height), _fadeColor * ValueF);
        }
    }
}