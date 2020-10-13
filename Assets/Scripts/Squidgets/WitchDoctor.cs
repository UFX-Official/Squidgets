using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchDoctor : Squidget
{
    public WitchDoctor()
    {
        name = "Witch Doctor";
        rarity = SquidgetRarity.Rare;

        maxHealth = 100;  health = maxHealth;
        squishFactor = 0.5f;
        bounceFactor = 0.5f;
        frictionFactor = 1.0f;
        jumpFactor = 1.0f;
        defenseFactor = 1.0f;
    }
}
