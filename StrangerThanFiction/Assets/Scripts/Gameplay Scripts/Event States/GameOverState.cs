using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverState 
{
    public bool hasPlayerWon = false;
    public int unusedInk = 0;
    public int bindingPower = 0;
    public int bindingDamage = 0;

    public GameOverState(bool hasPlayerWon, int unusedInk, int bindingPower, int bindingDamage)
    {
        this.hasPlayerWon = hasPlayerWon;
        this.unusedInk = unusedInk;
        this.bindingPower = bindingPower;
        this.bindingDamage = bindingDamage;
    }
}
