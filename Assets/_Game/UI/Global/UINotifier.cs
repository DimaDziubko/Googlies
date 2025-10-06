using System;
using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.UI.Common.Scripts;
using _Game.UI.Pin.Scripts;
using Sirenix.OdinInspector;

namespace _Game.UI.Global
{
    public class UINotifier : IUINotifier, IDisposable
    {
        [ShowInInspector]
        private IGameScreenListener _compositeListener;

        [ShowInInspector]
        private readonly Dictionary<Type, IGameScreen> _gameScreens = new();
        
        [ShowInInspector]
        private readonly Dictionary<IGameScreen, PinView> _screenPins = new();

        private IMyLogger _logger;

        public UINotifier(IMyLogger logger)
        {
            _logger = logger;
        }

        public void RegisterScreen<TScreen>(TScreen screen, IGameScreenEvents<TScreen> screenEvents) where TScreen : IGameScreen
        {
            var screenType = typeof(TScreen);

            if (!_gameScreens.ContainsKey(screenType))
            {
                _gameScreens.Add(screenType, screen);
                AttachToScreenEvents(screenEvents);
                _logger.Log($"SCREEN REGISTERED {screenType}", DebugStatus.Success);
            }
            else
            {
                _logger.Log($"Screen of type {screenType} is already registered.", DebugStatus.Warning);
            }
        }

        public void UnregisterScreen<TScreen>(IGameScreenEvents<TScreen> screenEvents) where TScreen : IGameScreen
        {
            var screenType = typeof(TScreen);

            if (_gameScreens.ContainsKey(screenType))
            {
                _gameScreens.Remove(screenType);
                DetachFromScreenEvents(screenEvents);
            }
            else
            {
                _logger.Log($"Screen of type {screenType} is not registered.", DebugStatus.Warning);
            }
        }

        public void RegisterPin(Type screenType, PinView pin)
        {
            if (_gameScreens.TryGetValue(screenType, out var screen) && !_screenPins.ContainsKey(_gameScreens[screenType]))
            {
                _screenPins.Add(screen, pin);
                _logger.Log($"PIN REGISTERED {screenType}", DebugStatus.Success);
                
                UpdatePinState(_gameScreens[screenType]);
            }
            else
            {
                _logger.Log($"Screen lost, pin registration failed {screenType}", DebugStatus.Fault);
            }
        }

        public void UnregisterPin(Type screenType)
        {
            if (_gameScreens.ContainsKey(screenType) && _screenPins.ContainsKey(_gameScreens[screenType]))
            {
                _screenPins.Remove(_gameScreens[screenType]);
                _logger.Log($"PIN UNREGISTERED {screenType}", DebugStatus.Warning);
            }
        }
        
        public void Register(IGameScreenListener listener)
        {
            if (_compositeListener == null)
                _compositeListener = listener;
        }

        public void Unregister(IGameScreenListener listener)
        {
            if (_compositeListener == listener)
                _compositeListener = null;
        }
        
        private void AttachToScreenEvents<TScreen>(IGameScreenEvents<TScreen> screenEvents) where TScreen : IGameScreen
        {
            screenEvents.ScreenOpened += NotifyScreenOpened;
            screenEvents.InfoChanged += NotifyInfoChanged;
            screenEvents.RequiresAttention += NotifyRequiresAttention;
            screenEvents.ScreenClosed += NotifyScreenClosed;
            screenEvents.ScreenDisposed += NotifyScreenDisposed;
            screenEvents.ActiveChanged += NotifyActiveChanged;
        }

        private void DetachFromScreenEvents<TScreen>(IGameScreenEvents<TScreen> screenEvents) where TScreen : IGameScreen
        {
            screenEvents.ScreenOpened -= NotifyScreenOpened;
            screenEvents.InfoChanged -= NotifyInfoChanged;
            screenEvents.RequiresAttention -= NotifyRequiresAttention;
            screenEvents.ScreenClosed -= NotifyScreenClosed;
            screenEvents.ScreenDisposed -= NotifyScreenDisposed;
            screenEvents.ActiveChanged -= NotifyActiveChanged;
        }

        private void NotifyActiveChanged<TScreen>(TScreen screen, bool isActive) where TScreen : IGameScreen
        {
            if (_compositeListener is IGameScreenListener<TScreen> typedListener)
            {
                typedListener.OnScreenActiveChanged(screen, isActive);
            }
            
            if (screen is IGameScreenWithInfo withInfo && 
                _compositeListener is IGameScreenListener<IGameScreenWithInfo> withInfoListener)
            {
                withInfoListener.OnScreenActiveChanged(withInfo, isActive);
            }
        }

        private void NotifyRequiresAttention<TScreen>(TScreen screen) where TScreen : IGameScreen
        {
            if (_compositeListener is IGameScreenListener<TScreen> typedListener)
            {
                typedListener.OnRequiresAttention(screen);
            }
            
            UpdatePinState(screen);
        }

        private void NotifyInfoChanged<TScreen>(TScreen screen) where TScreen : IGameScreen
        {
            if (_compositeListener is IGameScreenListener<TScreen> typedListener)
            {
                typedListener.OnInfoChanged(screen);
            }

            if (screen is IGameScreenWithInfo withInfo && 
                _compositeListener is IGameScreenListener<IGameScreenWithInfo> withInfoListener)
            {
                withInfoListener.OnInfoChanged(withInfo);
            }
        }

        private void NotifyScreenOpened<TScreen>(TScreen screen) where TScreen : IGameScreen
        {
            _logger.Log($"SCREEN OPENED {typeof(TScreen)}", DebugStatus.Warning);
            
            if (_compositeListener is IGameScreenListener<TScreen> typedListener)
            {
                typedListener.OnScreenOpened(screen);
            }

            if (screen is IGameScreenWithInfo withInfo && 
                _compositeListener is IGameScreenListener<IGameScreenWithInfo> withInfoListener)
            {
                withInfoListener.OnScreenOpened(withInfo);
            }
            
            UpdatePinState(screen);
        }

        private void NotifyScreenClosed<TScreen>(TScreen screen) where TScreen : IGameScreen
        {
            if (_compositeListener is IGameScreenListener<TScreen> typedListener)
            {
                typedListener.OnScreenClosed(screen);
            }
            
            if (screen is IGameScreenWithInfo withInfo && 
                _compositeListener is IGameScreenListener<IGameScreenWithInfo> withInfoListener)
            {
                withInfoListener.OnScreenClosed(withInfo);
            }
        }

        private void NotifyScreenDisposed<TScreen>(TScreen screen) where TScreen : IGameScreen
        {
            if (_compositeListener is IGameScreenListener<TScreen> typedListener)
            {
                typedListener.OnScreenDisposed(screen);
            }
            
            if (screen is IGameScreenWithInfo withInfo && 
                _compositeListener is IGameScreenListener<IGameScreenWithInfo> withInfoListener)
            {
                withInfoListener.OnScreenDisposed(withInfo);
            }
        }

        private void UpdatePinState(IGameScreen screen)
        {
            if (_screenPins.TryGetValue(screen, out var pin))
            {
                pin.SetActive(!screen.IsReviewed && screen.NeedAttention);
                
                _logger.Log($"PIN UPDATED {screen.GetType()}", DebugStatus.Success);
            }
        }

        void IDisposable.Dispose()
        {
            _screenPins.Clear();
            _gameScreens.Clear();
        }
    }
}