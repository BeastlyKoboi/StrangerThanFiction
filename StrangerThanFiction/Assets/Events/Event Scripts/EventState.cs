using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventState
{
    public EventState()
    {
    }
}

public class EventStateInt : EventState
{
    public int value;

    public EventStateInt(int value)
    {
        this.value = value;
    }
}