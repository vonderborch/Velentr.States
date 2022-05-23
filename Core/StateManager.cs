using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Velentr.Collections.Collections;
using Velentr.States.States;

namespace Velentr.States
{
    /// <summary>
    /// A manager.
    /// </summary>
    ///
    /// <seealso cref="IDisposable"/>
    public class StateManager : IDisposable
    {
        /// <summary>
        /// The states.
        /// </summary>
        public Dictionary<string, State> States;

        /// <summary>
        /// The history.
        /// </summary>
        private HistoryCollection<string> _history;

        /// <summary>
        /// Gets or sets the last game time.
        /// </summary>
        ///
        /// <value>
        /// The last game time.
        /// </value>
        public GameTime LastGameTime { get; internal set; }

        /// <summary>
        /// Gets or sets the maximum previous states to keep.
        /// </summary>
        ///
        /// <value>
        /// The maximum previous states to keep.
        /// </value>
        public int MaxPreviousStatesToKeep { get; set; }

        /// <summary>
        /// The current state.
        /// </summary>
        private string _currentState;

        /// <summary>
        /// Constructor.
        /// </summary>
        ///
        /// <param name="game">                         The game. </param>
        /// <param name="previousStateHistoryToKeep">   (Optional) The previous state history to keep. </param>
        public StateManager(Game game, int previousStateHistoryToKeep = 2)
        {
            States = new Dictionary<string, State>();
            Game = game;
            _history = new HistoryCollection<string>(previousStateHistoryToKeep);
        }

        /// <summary>
        /// The game.
        /// </summary>
        public Game Game;

        /// <summary>
        /// Gets the current state name.
        /// </summary>
        ///
        /// <value>
        /// The name of the current state.
        /// </value>
        public string CurrentStateName => _currentState;

        /// <summary>
        /// Gets the name of the last state.
        /// </summary>
        ///
        /// <value>
        /// The name of the last state.
        /// </value>
        public string PreviousStateName => _history.Count > 0 ? _history.NewestItem : string.Empty;

        /// <summary>
        /// Gets the state of the previous.
        /// </summary>
        ///
        /// <value>
        /// The previous state.
        /// </value>
        public State PreviousState => _history.Count > 0 ? States[_history.OldestItem] : null;

        /// <summary>
        /// Gets the current state.
        /// </summary>
        ///
        /// <value>
        /// The current state.
        /// </value>
        public State CurrentState => _currentState == null ? null : States[_currentState];

        /// <summary>
        /// Registers the state.
        /// </summary>
        ///
        /// <param name="name">             The name. </param>
        /// <param name="state">            The state. </param>
        /// <param name="autoInitialize">   (Optional) True to automatically initialize. </param>
        /// <param name="autoLoad">         (Optional) True to automatically load. </param>
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

        /// <summary>
        /// Removes the state described by state.
        /// </summary>
        ///
        /// <exception cref="ArgumentOutOfRangeException">  Thrown when one or more arguments are outside
        ///                                                 the required range. </exception>
        /// <exception cref="ArgumentException">            Thrown when one or more arguments have
        ///                                                 unsupported or illegal values. </exception>
        ///
        /// <param name="state">    The state. </param>
        public void RemoveState(string state)
        {
            if (!States.ContainsKey(state))
            {
                throw new ArgumentOutOfRangeException(nameof(state), $"The state [{state}] does not exist!");
            }

            if (_currentState == state)
            {
                throw new ArgumentException("Cannot remove the active state!");
            }

            States[state].Unload();
            States[state].Dispose();
        }

        /// <summary>
        /// Gets state names.
        /// </summary>
        ///
        /// <returns>
        /// The state names.
        /// </returns>
        public List<string> GetStateNames()
        {
            return new List<string>(States.Keys);
        }

        /// <summary>
        /// Gets the states.
        /// </summary>
        ///
        /// <returns>
        /// The states.
        /// </returns>
        public List<State> GetStates()
        {
            return new List<State>(States.Values);
        }

        /// <summary>
        /// Initializes this object.
        /// </summary>
        public void Initialize()
        {
            foreach (var state in States)
            {
                state.Value.Initialize();
            }
        }

        /// <summary>
        /// Change state.
        /// </summary>
        ///
        /// <exception cref="ArgumentOutOfRangeException">  Thrown when one or more arguments are outside
        ///                                                 the required range. </exception>
        /// <exception cref="ArgumentException">            Thrown when one or more arguments have
        ///                                                 unsupported or illegal values. </exception>
        ///
        /// <param name="newState"> State of the new. </param>
        public void ChangeState(string newState)
        {
            if (!States.ContainsKey(newState))
            {
                throw new ArgumentOutOfRangeException(nameof(newState), $"The state [{newState}] does not exist!");
            }

            if (States[newState].IsDisposed)
            {
                throw new ArgumentException("Cannot change to an already disposed state!");
            }

            _history.AddItem(_currentState);
            _currentState = newState;

            CurrentState.Load();

            if (PreviousState?.TransitionOut == null)
            {
                CurrentState.Status = CurrentState.TransitionIn == null ? Status.Active : Status.TransitionIn;
                if (PreviousState != null)
                {
                    PreviousState.Status = Status.Inactive;
                }

                if (CurrentState.Status == Status.TransitionIn)
                {
                    CurrentState.TransitionIn?.StartTransition(LastGameTime);
                }
            }
            else
            {
                CurrentState.Status = Status.Inactive;
                if (PreviousState != null)
                {
                    PreviousState.Status = Status.TransitionOut;
                    PreviousState.TransitionOut?.StartTransition(LastGameTime);
                }
            }

            if (PreviousState?.Status == Status.Inactive)
            {
                PreviousState?.Unload();
            }
        }

        /// <summary>
        /// Updates the given gameTime.
        /// </summary>
        ///
        /// <exception cref="Exception">    Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="gameTime"> The game time. </param>
        public void Update(GameTime gameTime)
        {
            if (PreviousState != null && PreviousState.Status == Status.TransitionOut)
            {
                PreviousState.UpdateInternal(gameTime);
                if (PreviousState.Status == Status.Inactive)
                {
                    PreviousState.Unload();
                    CurrentState.Status = CurrentState.TransitionIn == null ? Status.Active : Status.TransitionIn;
                    if (CurrentState.Status == Status.TransitionIn)
                    {
                        CurrentState.TransitionIn?.StartTransition(gameTime);
                    }
                }
            }

            CurrentState?.UpdateInternal(gameTime);

            LastGameTime = gameTime;
        }

        /// <summary>
        /// Draws the given game time.
        /// </summary>
        ///
        /// <exception cref="Exception">    Thrown when an exception error condition occurs. </exception>
        ///
        /// <param name="gameTime"> The game time. </param>
        public void Draw(GameTime gameTime)
        {
            if (PreviousState != null && PreviousState.Status == Status.TransitionOut)
            {
                PreviousState.DrawInternal(gameTime);
            }
            CurrentState?.DrawInternal(gameTime);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ///
        /// <seealso cref="IDisposable.Dispose()"/>
        public void Dispose()
        {
            foreach (var state in States)
            {
                state.Value.Dispose();
            }
        }
    }
}