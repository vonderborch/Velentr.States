using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Velentr.States.Transitions
{
    /// <summary>
    /// A transition.
    /// </summary>
    ///
    /// <seealso cref="IDisposable"/>
    public abstract class Transition : IDisposable
    {
        /// <summary>
        /// The graphics device.
        /// </summary>
        protected GraphicsDevice _graphicsDevice;

        /// <summary>
        /// The sprite batch.
        /// </summary>
        protected SpriteBatch _spriteBatch;

        /// <summary>
        /// True to begin and end sprite batch.
        /// </summary>
        protected bool _beginAndEndSpriteBatch;

        /// <summary>
        /// True to dispose sprite batch.
        /// </summary>
        protected bool disposeSpriteBatch;

        /// <summary>
        /// The value.
        /// </summary>
        protected double _value;

        /// <summary>
        /// The transition start time.
        /// </summary>
        protected TimeSpan _transitionStartTime;

        /// <summary>
        /// The base texture.
        /// </summary>
        protected Texture2D _baseTexture;

        /// <summary>
        /// Constructor.
        /// </summary>
        ///
        /// <param name="lifespan">         The lifespan. </param>
        /// <param name="transitionType">   (Optional) Type of the transition. </param>
        public Transition(TimeSpan lifespan, BuiltInTransitionType transitionType = BuiltInTransitionType.Custom)
        {
            Lifespan = lifespan;
            TransitionIsDone = false;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        ///
        /// <value>
        /// The value.
        /// </value>
        public double Value => _value;

        /// <summary>
        /// Gets the transition start time.
        /// </summary>
        ///
        /// <value>
        /// The transition start time.
        /// </value>
        public TimeSpan TransitionStartTime => _transitionStartTime;

        /// <summary>
        /// Gets the value f.
        /// </summary>
        ///
        /// <value>
        /// The value f.
        /// </value>
        public float ValueF => (float)_value;

        /// <summary>
        /// Gets the lifespan.
        /// </summary>
        ///
        /// <value>
        /// The lifespan.
        /// </value>
        public TimeSpan Lifespan { get; }

        /// <summary>
        /// Gets the type of the transition.
        /// </summary>
        ///
        /// <value>
        /// The type of the transition.
        /// </value>
        public TransitionMode TransitionMode { get; internal set; }

        /// <summary>
        /// Gets or sets the type of the transition.
        /// </summary>
        ///
        /// <value>
        /// The type of the transition.
        /// </value>
        public BuiltInTransitionType TransitionType { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether the transition is done.
        /// </summary>
        ///
        /// <value>
        /// True if transition is done, false if not.
        /// </value>
        public bool TransitionIsDone { get; private set; }

        /// <summary>
        /// Starts a transition.
        /// </summary>
        ///
        /// <param name="gameTime"> The game time. </param>
        public void StartTransition(GameTime gameTime)
        {
            _transitionStartTime = gameTime.TotalGameTime;
            TransitionIsDone = false;
        }

        /// <summary>
        /// Setups the transition.
        /// </summary>
        ///
        /// <param name="transitionMode">   Type of the transition. </param>
        /// <param name="graphicsDevice">   The graphics device. </param>
        /// <param name="spriteBatch">      The sprite batch. </param>
        internal void Setup(TransitionMode transitionMode, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            TransitionMode = transitionMode;
            _graphicsDevice = graphicsDevice;
            _spriteBatch = spriteBatch;
            _baseTexture = new Texture2D(_graphicsDevice, 1, 1);
            _baseTexture.SetData(new Color[] { Color.White });
        }

        /// <summary>
        /// Updates the given gameTime.
        /// </summary>
        ///
        /// <exception cref="ArgumentOutOfRangeException">  Thrown when one or more arguments are outside
        ///                                                 the required range. </exception>
        ///
        /// <param name="gameTime"> The game time. </param>
        public virtual void Update(GameTime gameTime)
        {
            var timeDifference = gameTime.TotalGameTime - _transitionStartTime;

            switch (TransitionMode)
            {
                case TransitionMode.In:
                    _value = 1 - timeDifference.Ticks / (double)Lifespan.Ticks;
                    break;

                case TransitionMode.Out:
                    _value = timeDifference.Ticks / (double)Lifespan.Ticks;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("TransitionMode must be set to TransitionMode.In or TransitionMode.Out!");
            }

            if (timeDifference >= Lifespan)
            {
                TransitionIsDone = true;
            }
        }

        /// <summary>
        /// Draws this object.
        /// </summary>
        public abstract void Draw();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ///
        /// <seealso cref="IDisposable.Dispose()"/>
        public virtual void Dispose()
        {
            if (disposeSpriteBatch)
            {
                _spriteBatch.Dispose();
            }
        }
    }
}