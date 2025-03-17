using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Linq;
using Cysharp.Threading.Tasks;

public class UniTaskEvent
{
    private event Func<UniTask> InstanceHandlers;

    public void AddListener(Func<UniTask> handler) => InstanceHandlers += handler;
    public void RemoveListener(Func<UniTask> handler) => InstanceHandlers -= handler;
    public void Clear() => InstanceHandlers = null;

    public async UniTask InvokeAsync()
    {
        if (InstanceHandlers != null)
        {
            var handlers = InstanceHandlers.GetInvocationList().Cast<Func<UniTask>>().ToList(); // Create a copy of the invocation list
            foreach (Func<UniTask> handler in handlers)
            {
                await handler();
            }
        }
    }
}

public class UniTaskEvent<T>
{
    private event Func<T, UniTask> InstanceHandlers;

    public void AddListener(Func<T, UniTask> handler) => InstanceHandlers += handler;
    public void RemoveListener(Func<T, UniTask> handler) => InstanceHandlers -= handler;
    public void Clear() => InstanceHandlers = null;

    public async UniTask InvokeAsync(T arg)
    {
        if (InstanceHandlers != null)
        {
            var handlers = InstanceHandlers.GetInvocationList().Cast<Func<T, UniTask>>().ToList(); // Create a copy of the invocation list
            foreach (Func<T, UniTask> handler in handlers)
            {
                await handler(arg);
            }
        }
    }
}

public class UniTaskEvent<T1, T2>
{
    private event Func<T1, T2, UniTask> InstanceHandlers;

    public void AddListener(Func<T1, T2, UniTask> handler) => InstanceHandlers += handler;
    public void RemoveListener(Func<T1, T2, UniTask> handler) => InstanceHandlers -= handler;
    public void Clear() => InstanceHandlers = null;

    public async UniTask InvokeAsync(T1 arg1, T2 arg2)
    {
        if (InstanceHandlers != null)
        {
            var handlers = InstanceHandlers.GetInvocationList().Cast<Func<T1, T2, UniTask>>().ToList();
            foreach (Func<T1, T2, UniTask> handler in handlers)
            {
                await handler(arg1, arg2);
            }
        }
    }
}