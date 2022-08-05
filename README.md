# Velentr.States
A library to make managing states in a game or an app easier.

# Installation
There are nuget packages available for Monogame and FNA:
- Monogame [![NuGet version (Velentr.States.Monogame)](https://img.shields.io/nuget/v/Velentr.States.Monogame.svg?style=flat-square)](https://www.nuget.org/packages/Velentr.States.Monogame/): [Velentr.States.Monogame](https://www.nuget.org/packages/Velentr.States.Monogame/)
- FNA [![NuGet version (Velentr.States.FNA)](https://img.shields.io/nuget/v/Velentr.States.FNA.svg?style=flat-square)](https://www.nuget.org/packages/Velentr.States.FNA/): [Velentr.States.FNA](https://www.nuget.org/packages/Velentr.States.FNA/)

## Screenshot:
![Screenshot](https://github.com/vonderborch/Velentr.States/blob/main/example.gif?raw=true)

# Example
```
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Velentr.Font;
using Velentr.Miscellaneous.CodeProfiling;
using Velentr.States;
using Velentr.States.States;
using Velentr.States.Transitions;

namespace CoreDev
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private FpsTracker _frameCounter = new FpsTracker(10);
        private PerformanceTracker _performance = new PerformanceTracker(10, enableFpsTracker: true);
        private string _baseTitle = "Velentr.States.DevEnv";
        private string _decimals = "0.000";

        // Font stuff for rendering text
        private string _fontName = "MontserratRegular-RpK6l.otf";

        private FontManager _fontManager;
        private Font _font;

        // State stuffs
        private StateManager _stateManager;

        // other variables for examples
        private int _ticks = 0;

        private const int TransitionTimeSeconds = 1;
        private const int MaxTicksToSwitch = 60 * 9;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
            _baseTitle = $"{_baseTitle} | FPS: {{0:{_decimals}}} | TPS: {{1:{_decimals}}} | CPU: {{2:{_decimals}}}% | Memory: {{3:{_decimals}}} MB";

            _fontManager = new FontManager(GraphicsDevice);
            _font = _fontManager.GetFont(_fontName, 32);

            _stateManager = new StateManager(this);
            _stateManager.RegisterState("Test State A", new TestState("TestStateA", Color.Red, _stateManager, GraphicsDevice, new FadeTransition(Color.Black, TimeSpan.FromSeconds(TransitionTimeSeconds)), new FadeTransition(Color.Black, TimeSpan.FromSeconds(TransitionTimeSeconds)), _spriteBatch, false, false, false, false));
            _stateManager.RegisterState("Test State B", new TestState("TestStateB", Color.Yellow, _stateManager, GraphicsDevice, new FadeTransition(Color.Black, TimeSpan.FromSeconds(TransitionTimeSeconds)), new FadeTransition(Color.Black, TimeSpan.FromSeconds(TransitionTimeSeconds)), _spriteBatch, false, false, false, false));
            _stateManager.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            _performance.Update(gameTime.ElapsedGameTime);

            _stateManager.Update(gameTime);
            _ticks++;
            if (_ticks > MaxTicksToSwitch)
            {
                _ticks = 0;
                _stateManager.ChangeState(_stateManager.CurrentStateName == "Test State A" ? "Test State B" : "Test State A");
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _frameCounter.Update(gameTime.ElapsedGameTime);
            Window.Title = string.Format(_baseTitle, _frameCounter.AverageFramesPerSecond, _performance.FpsTracker.AverageFramesPerSecond, _performance.CpuTracker.CpuPercent, _performance.MemoryTracker.MemoryUsageMb);

            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            var extraText = "";
            if (_stateManager.PreviousState?.Status == Status.TransitionOut)
            {
                extraText = $" ({_stateManager.PreviousStateName} is transitioning out)";
            }
            _spriteBatch.DrawString(_font, $"Current State: {_stateManager.CurrentStateName}{extraText}", new Vector2(0, 0), Color.Black);
            _spriteBatch.DrawString(_font, $"Previous State: {_stateManager.PreviousStateName}", new Vector2(0, 120), Color.Black);
            _stateManager.Draw(gameTime);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
```

# Future Plans
See list of issues under the Milestones: https://github.com/vonderborch/Velentr.States/milestones