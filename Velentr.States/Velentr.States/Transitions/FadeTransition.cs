using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Velentr.States.Transitions
{
    public class FadeTransition : Transition
    {
        private Color _fadeColor;

        public FadeTransition(Color fadeColor, TimeSpan lifespan, TransitionType type, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch = null, bool beginAndEndSpriteBatch = false) : base(lifespan, type, graphicsDevice, spriteBatch, beginAndEndSpriteBatch)
        {
            _fadeColor = fadeColor;
        }

        public Color FadeColor
        {
            get => _fadeColor;
            set => _fadeColor = value;
        }

        protected override void DrawInternal()
        {
            _spriteBatch.Draw(_baseTexture, new Rectangle(0, 0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height), _fadeColor * ValueF);
        }
    }
}
