using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;


public abstract class GameEventAsyncBase : ScriptableObject
{
    private event Func<UniTask> listeners;

    public void AddListener(Func<UniTask> handler) => listeners += handler;
    public void RemoveListener(Func<UniTask> handler) => listeners -= handler;
    public void Clear() => listeners = null;

    public async UniTask InvokeAsync()
    {
        if (listeners != null)
        {
            List<Func<UniTask>> handlers = listeners.GetInvocationList().Cast<Func<UniTask>>().ToList(); // Create a copy of the invocation list
            foreach (Func<UniTask> handler in handlers)
            {
                await handler();
            }
        }
    }

}


public abstract class GameEventAsyncBase<T> : ScriptableObject
{
    private event Func<T, UniTask> listeners;

    public void AddListener(Func<T, UniTask> handler) => listeners += handler;
    public void RemoveListener(Func<T, UniTask> handler) => listeners -= handler;
    public void Clear() => listeners = null;

    public async UniTask InvokeAsync(T arg)
    {
        if (listeners != null)
        {
            List<Func<T, UniTask>> handlers = listeners.GetInvocationList().Cast<Func<T, UniTask>>().ToList(); // Create a copy of the invocation list
            foreach (Func<T, UniTask> handler in handlers)
            {
                await handler(arg);
            }
        }
    }

}

[CreateAssetMenu(menuName = "Game Event Async/EventState")]
public class GameEventAsync : GameEventAsyncBase<EventState> { }

