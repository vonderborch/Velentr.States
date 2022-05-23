using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Velentr.Font;
using Velentr.States;
using Velentr.States.States;
using Velentr.States.Transitions;

namespace CoreDev
{
    public class TestState : State
    {
        private string _subState;
        private string _fontName = "MontserratRegular-RpK6l.otf";
        private FontManager _fontManager;
        private Font _font;
        private Color _color;

        public TestState(string name, Color color, StateManager manager, GraphicsDevice graphicsDevice, Transition inTransition = null, Transition outTransition = null, SpriteBatch spriteBatch = null, bool beginAndEndSpriteBatch = false, bool autoAddToManager = false, bool autoInitialize = false, bool autoLoad = false) : base(name, manager, graphicsDevice, inTransition, outTransition, spriteBatch, beginAndEndSpriteBatch, autoAddToManager, autoInitialize, autoLoad)
        {
            _subState = "Class initialized";
            _color = color;
        }

        public override void Initialize()
        {
            // initialize stuffs here!
            _subState = "Initialize";
            _fontManager = new FontManager(this._graphicsDevice);
            _font = _fontManager.GetFont(_fontName, 32);
        }

        public override void ActualDispose()
        {
            _font.Dispose();
            _fontManager.Dispose();
        }

        public override void Load()
        {
            // load stuffs here!
            _subState = "Loading";
        }

        public override void Unload()
        {
            // unload stuffs here!
            _subState = "Unloading";
        }

        public override void Update(GameTime gameTime)
        {
            // update stuffs here!
            _subState = "Updating things";
        }

        public override void TransitionInUpdate(GameTime gameTime)
        {
            // update stuffs here!
            _subState = "Updating things when transitioning in";
        }

        public override void TransitionOutUpdate(GameTime gameTime)
        {
            // update stuffs here!
            _subState = "Updating things when transitioning out";
        }

        protected override void TransitionInDraw(GameTime gameTime)
        {
            // draw stuffs here!
            this._spriteBatch.DrawString(_font, "I can also draw things when transitioning in!", new Vector2(0, 240), _color);
            this._spriteBatch.DrawString(_font, $"Sub-State: {_subState}", new Vector2(0, 360), _color);
        }

        protected override void TransitionOutDraw(GameTime gameTime)
        {
            // draw stuffs here!
            this._spriteBatch.DrawString(_font, "I can also draw things when transitioning out!", new Vector2(0, 240), _color);
            this._spriteBatch.DrawString(_font, $"Sub-State: {_subState}", new Vector2(0, 360), _color);
        }

        protected override void Draw(GameTime gameTime)
        {
            // draw stuffs here!
            this._spriteBatch.DrawString(_font, "I can also draw things!", new Vector2(0, 240), _color);
            this._spriteBatch.DrawString(_font, $"Sub-State: {_subState}", new Vector2(0, 360), _color);
        }
    }
}