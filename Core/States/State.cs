using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Velentr.States.Transitions;

namespace Velentr.States.States
{
    /// <summary>
    /// A state.
    /// </summary>
    public abstract class State : IDisposable
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
        /// Gets or sets a value indicating whether this object is disposed.
        /// </summary>
        ///
        /// <value>
        /// True if this object is disposed, false if not.
        /// </value>
        internal bool IsDisposed { get; private set; } = false;

        /// <summary>
        /// Constructor.
        /// </summary>
        ///
        /// <param name="name">                     The name. </param>
        /// <param name="manager">                  The manager. </param>
        /// <param name="graphicsDevice">           The graphics device. </param>
        /// <param name="inTransition">             (Optional) The in transition. </param>
        /// <param name="outTransition">            (Optional) The out transition. </param>
        /// <param name="spriteBatch">              (Optional) The sprite batch. </param>
        /// <param name="beginAndEndSpriteBatch">   (Optional) True to begin and end sprite batch. </param>
        /// <param name="autoAddToManager">         (Optional) True to automatically add to manager. </param>
        /// <param name="autoInitialize">           (Optional) True to automatically initialize. </param>
        /// <param name="autoLoad">                 (Optional) True to automatically load. </param>
        public State(string name, StateManager manager, GraphicsDevice graphicsDevice, Transition inTransition = null, Transition outTransition = null, SpriteBatch spriteBatch = null, bool beginAndEndSpriteBatch = false, bool autoAddToManager = false, bool autoInitialize = false, bool autoLoad = false)
        {
            Name = name;
            Manager = manager;
            TransitionIn = inTransition;
            TransitionOut = outTransition;

            _graphicsDevice = graphicsDevice;
            disposeSpriteBatch = spriteBatch == null;
            _spriteBatch = disposeSpriteBatch
                ? new SpriteBatch(graphicsDevice)
                : spriteBatch;
            _beginAndEndSpriteBatch = beginAndEndSpriteBatch;

            TransitionIn?.Setup(TransitionMode.In, graphicsDevice, _spriteBatch);
            TransitionOut?.Setup(TransitionMode.Out, graphicsDevice, _spriteBatch);

            if (autoAddToManager)
            {
                Manager.RegisterState(name, this, autoInitialize, autoLoad);
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        ///
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        ///
        /// <value>
        /// The status.
        /// </value>
        public Status Status { get; internal set; }

        /// <summary>
        /// The transition in.
        /// </summary>
        public Transition TransitionIn;

        /// <summary>
        /// The transition out.
        /// </summary>
        public Transition TransitionOut;

        /// <summary>
        /// Gets or sets the manager.
        /// </summary>
        ///
        /// <value>
        /// The manager.
        /// </value>
        public static StateManager Manager { get; internal set; }

        /// <summary>
        /// Gets the game.
        /// </summary>
        ///
        /// <value>
        /// The game.
        /// </value>
        public Game Game => Manager.Game;

        /// <summary>
        /// Initializes this object.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Loads this object.
        /// </summary>
        public abstract void Load();

        /// <summary>
        /// Unloads this object.
        /// </summary>
        public abstract void Unload();

        /// <summary>
        /// Updates the internal described by gameTime.
        /// </summary>
        ///
        /// <param name="gameTime"> The game time. </param>
        public void UpdateInternal(GameTime gameTime)
        {
            switch (Status)
            {
                case Status.TransitionIn:
                    TransitionInUpdate(gameTime);
                    if (TransitionIn != null)
                    {
                        TransitionIn.Update(gameTime);
                        if (TransitionIn.TransitionIsDone)
                        {
                            Status = Status.Active;
                        }
                    }
                    else
                    {
                        Status = Status.Active;
                    }
                    break;

                case Status.Active:
                    Update(gameTime);
                    break;

                case Status.TransitionOut:
                    TransitionOutUpdate(gameTime);
                    if (TransitionOut != null)
                    {
                        TransitionOut.Update(gameTime);
                        if (TransitionOut.TransitionIsDone)
                        {
                            Status = Status.Inactive;
                        }
                    }
                    else
                    {
                        Status = Status.Inactive;
                    }
                    break;

                case Status.Inactive:
                    break;
            }
        }

        /// <summary>
        /// Updates the given gameTime.
        /// </summary>
        ///
        /// <param name="gameTime"> The game time. </param>
        public abstract void Update(GameTime gameTime);

        /// <summary>
        /// Transition in update.
        /// </summary>
        ///
        /// <param name="gameTime"> The game time. </param>
        public abstract void TransitionInUpdate(GameTime gameTime);

        /// <summary>
        /// Transition out update.
        /// </summary>
        ///
        /// <param name="gameTime"> The game time. </param>
        public abstract void TransitionOutUpdate(GameTime gameTime);

        /// <summary>
        /// Transition in draw.
        /// </summary>
        ///
        /// <param name="gameTime"> The game time. </param>
        protected abstract void TransitionInDraw(GameTime gameTime);

        /// <summary>
        /// Transition out draw.
        /// </summary>
        ///
        /// <param name="gameTime"> The game time. </param>
        protected abstract void TransitionOutDraw(GameTime gameTime);

        /// <summary>
        /// Draws the given game time.
        /// </summary>
        ///
        /// <param name="gameTime"> The game time. </param>
        protected abstract void Draw(GameTime gameTime);

        /// <summary>
        /// Draw internal.
        /// </summary>
        ///
        /// <param name="gameTime"> The game time. </param>
        public void DrawInternal(GameTime gameTime)
        {
            if (_beginAndEndSpriteBatch)
            {
                _spriteBatch.Begin();
            }

            switch (Status)
            {
                case Status.TransitionIn:
                    TransitionInDraw(gameTime);
                    TransitionIn?.Draw();
                    break;

                case Status.Active:
                    Draw(gameTime);
                    break;

                case Status.TransitionOut:
                    TransitionOutDraw(gameTime);
                    TransitionOut?.Draw();
                    break;

                case Status.Inactive:
                    break;
            }

            if (_beginAndEndSpriteBatch)
            {
                _spriteBatch.End();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            IsDisposed = true;
            Manager = null;
            if (disposeSpriteBatch)
            {
                _spriteBatch.Dispose();
            }
            ActualDispose();
        }

        /// <summary>
        /// Actual dispose.
        /// </summary>
        public virtual void ActualDispose()
        {
        }
    }
}