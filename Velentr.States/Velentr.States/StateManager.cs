
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Velentr.Collections.Collections;
using Velentr.States.States;

namespace Velentr.States
{
    /// <summary>
    ///     A manager.
    /// </summary>
    public class StateManager : IDisposable
    {
        public Dictionary<string, State> States;

        private List<string> _previousStates;

        private HistoryCollection<string> _history;

        public int MaxPreviousStatesToKeep { get; set; }

        private string _currentState;

        public StateManager(Game game)
        {
            States = new Dictionary<string, State>();
            Game = game;
            _previousStates = new List<string>();
        }

        public Game Game;

        public string CurrentStateName => _currentState;

        public State CurrentState => _currentState == null ? null : States[_currentState];

        public void RegisterState(string name, State state, bool autoInitialize = false, bool autoLoad = false)
        {
            States[name] = state;
            if (States.Count == 1)
            {
                _currentState = name;
            }

            if (autoInitialize)
            {
                States[name].Initialize();
            }

            if (autoLoad)
            {
                States[name].Load();
            }
        }

        public void RemoveState(string state)
        {
            if (!States.ContainsKey(state))
            {
                throw new ArgumentOutOfRangeException(nameof(state), $"The state [{state}] does not exist!");
            }

            if (_currentState == state)
            {
                States[state].Exit();
                
                if (States.Count > 1)
                {
                    var states = GetStateNames();
                    var nextState = states[states.FindIndex(x => x != state)];

                    States[nextState].Load();
                    _currentState = nextState;
                }
            }

            States[state].Exit();
            States[state].Dispose();
        }

        public List<string> GetStateNames()
        {
            return new List<string>(States.Keys);
        }

        public List<State> GetStates()
        {
            return new List<State>(States.Values);
        }

        public void Initialize()
        {
            foreach (var state in States)
            {
                state.Value.Initialize();
            }
        }

        public void ChangeState(string newState)
        {
            if (!States.ContainsKey(newState))
            {
                throw new ArgumentOutOfRangeException(nameof(newState), $"The state [{newState}] does not exist!");
            }

            States[newState].Load();
            States[_currentState].Exit();
            _currentState = newState;
        }

        public void Update(GameTime gameTime)
        {
            CurrentState?.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            CurrentState?.Draw(gameTime);
        }

        public void Dispose()
        {
            foreach (var state in States)
            {
                state.Value.Dispose();
            }
        }
    }
}