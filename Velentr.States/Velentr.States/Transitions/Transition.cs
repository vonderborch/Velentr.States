using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Velentr.States.Transitions
{
    public abstract class Transition : IDisposable
    {
        protected GraphicsDevice _graphicsDevice;
        protected SpriteBatch _spriteBatch;
        protected bool _beginAndEndSpriteBatch;
        protected bool disposeSpriteBatch;
        protected double _value;
        protected TimeSpan _transitionStartTime;
        protected Texture2D _baseTexture;

        public Transition(TimeSpan lifespan, TransitionType type, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch = null, bool beginAndEndSpriteBatch = false)
        {
            Lifespan = lifespan;
            TransitionType = type;

            _graphicsDevice = graphicsDevice;
            disposeSpriteBatch = spriteBatch != null;
            _spriteBatch = disposeSpriteBatch
                ? new SpriteBatch(graphicsDevice)
                : spriteBatch;
            _beginAndEndSpriteBatch = beginAndEndSpriteBatch;

            TransitionIsDone = false;

            _baseTexture = new Texture2D(graphicsDevice, 1, 1);
            _baseTexture.SetData(new Color[] { Color.White });
        }

        public double Value => _value;

        public TimeSpan TransitionStartTime => _transitionStartTime;

        public float ValueF => (float)_value;

        public TimeSpan Lifespan { get; }

        public TransitionType TransitionType { get; }

        public bool TransitionIsDone { get; private set; }

        public void StartTransition(GameTime gameTime)
        {
            _transitionStartTime = gameTime.TotalGameTime;
            TransitionIsDone = false;
        }

        public virtual void Update(GameTime gameTime)
        {
            var timeDifference = gameTime.TotalGameTime - _transitionStartTime;

            switch (TransitionType)
            {
                case TransitionType.In:
                    _value = 1 - timeDifference.Ticks / Lifespan.Ticks;
                    break;
                case TransitionType.Out:
                    _value = timeDifference.Ticks / Lifespan.Ticks;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("TransitionType must be set to TransitionType.In or TransitionType.Out!");
            }

            if (timeDifference >= Lifespan)
            {
                TransitionIsDone = true;
            }
        }

        public void Draw()
        {
            if (_beginAndEndSpriteBatch)
            {
                _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            }

            DrawInternal();

            if (_beginAndEndSpriteBatch)
            {
                _spriteBatch.End();
            }
        }

        protected abstract void DrawInternal();

        public virtual void Dispose()
        {
            if (disposeSpriteBatch)
            {
                _spriteBatch.Dispose();
            }
        }
    }
}
