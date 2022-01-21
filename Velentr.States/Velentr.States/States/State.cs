using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Velentr.States.Transitions;

namespace Velentr.States.States
{
    public abstract class State : IDisposable
    {
        protected GraphicsDevice _graphicsDevice;
        protected SpriteBatch _spriteBatch;
        protected bool _beginAndEndSpriteBatch;
        protected bool disposeSpriteBatch;

        public State(string name, StateManager manager, GraphicsDevice graphicsDevice, Transition inTransition = null, Transition outTransition = null, SpriteBatch spriteBatch = null, bool beginAndEndSpriteBatch = false, bool autoAddToManager = false, bool autoInitialize = false, bool autoLoad = false)
        {
            Name = name;
            Manager = manager;
            TransitionIn = inTransition;
            TransitionOut = outTransition;

            _graphicsDevice = graphicsDevice;
            disposeSpriteBatch = spriteBatch != null;
            _spriteBatch = disposeSpriteBatch
                ? new SpriteBatch(graphicsDevice)
                : spriteBatch;
            _beginAndEndSpriteBatch = beginAndEndSpriteBatch;

            if (autoAddToManager)
            {
                Manager.RegisterState(name, this, autoInitialize, autoLoad);
            }
        }

        public string Name { get; private set; }

        public StateStatus Status { get; private set; }

        public Transition TransitionIn;

        public Transition TransitionOut;

        public static StateManager Manager { get; internal set; }

        public Game Game => Manager.Game;

        public abstract void Initialize();

        public abstract void Load();

        public abstract void Exit();

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(GameTime gameTime);

        public void Dispose()
        {
            Manager = null;
            if (disposeSpriteBatch)
            {
                _spriteBatch.Dispose();
            }
            DisposeState();
        }

        public abstract void DisposeState();
    }
}
