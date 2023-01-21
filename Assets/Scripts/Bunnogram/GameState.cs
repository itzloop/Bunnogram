using System;
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using UniRx;

namespace Bunnogram
{

    public enum GameStates
    {
        MainMenu,
        InGame
    }
    public class GameState
    {
        public static GameState Instance => _instance ??= new GameState();
        
        private static GameState _instance;
        private Dictionary<string, object> _state;
        private Mutex _lock;

        private GameState()
        {
            _state = new Dictionary<string, object>();
            _lock = new Mutex();
        }

        public void Store<T>(T t, string name)
        {
            _lock.WaitOne();
            _state.Add(name, t);
            _lock.ReleaseMutex();
        }
   
        
        public void Update<T>(T t, string name)
        {
            _lock.WaitOne();
            if (!_state.ContainsKey(name))
            {
                _lock.ReleaseMutex();
                throw new KeyNotFoundException($"item of type {typeof(T)} not found with name: {name}");
            }

            _state[name] = t;
            
            _lock.ReleaseMutex();
        }
        public T Get<T>(string name)
        {
            _lock.WaitOne();
            if (!_state.ContainsKey(name))
            {
                _lock.ReleaseMutex();
                throw new KeyNotFoundException($"item not found with name: {name}");
            }
        
            var item = _state[name];
            _lock.ReleaseMutex();
            if (!(item.GetType() == typeof(T)))
                throw new Exception($"item of type {item.GetType()} with name {name}, is not of type {typeof(T)}");

            return (T)item;
        }

        private T UnsafeGet<T>(string name)
        {
            if (!_state.ContainsKey(name))
                throw new KeyNotFoundException($"item not found with name: {name}");
        
            var item = _state[name];
            if (!(item.GetType() == typeof(T)))
                throw new Exception($"item of type {item.GetType()} with name {name}, is not of type {typeof(T)}");

            return (T)item;
        }
        
        private void UnsafeUpdate<T>(T t, string name)
        {
            if (!_state.ContainsKey(name))
                throw new KeyNotFoundException($"item of type {typeof(T)} not found with name: {name}");

            _state[name] = t;
        }

        private bool Lock()
        {
            return _lock.WaitOne();
        }

        private void Unlock()
        {
            _lock.ReleaseMutex();
        }
        
        
        public static void DecInt(Action<int> callback, string name)
        {
            try
            {
                Instance.Lock();
                var val = Instance.UnsafeGet<int>(name);
                Instance.UnsafeUpdate(val - 1, name);
                Instance.Unlock();
                callback(val - 1);
            }
            catch (Exception e)
            {
                Instance.Unlock();
                throw;
            }
        }
        
        public static void IncInt(Action<int> callback, string name)
        {
            try
            {
                Instance.Lock();
                var val = Instance.UnsafeGet<int>(name);
                Instance.UnsafeUpdate(val - 1, name);
                Instance.Unlock();
                callback(val - 1);
            }
            catch (Exception e)
            {
                Instance.Unlock();
                throw;
            }
        }
        
        public static void ChangeClickMode(ClickMode mode, Action<ClickMode> callback)
        {
            try
            {
                Instance.Lock();
                Instance.UnsafeUpdate(mode, Constants.ClickModeKey);
                Instance.Unlock();
                callback(mode);
            }
            catch (Exception e)
            {
                Instance.Unlock();
                throw;
            }
        }

    }

    public static class GameStateHelper
    {
        public static ReactiveProperty<ClickMode> GetClickMode()
        {
            return GameState.Instance.Get<ReactiveProperty<ClickMode>>(Constants.ClickModeKey);
        }
        
        public static ReactiveProperty<int> GetHints()
        {
            return GameState.Instance.Get<ReactiveProperty<int>>(Constants.HintsCountKey);
        }
        
        public static ReactiveProperty<int> GetHealthPoints()
        {
            return GameState.Instance.Get<ReactiveProperty<int>>(Constants.HealthPointKey);
        }
    }

}