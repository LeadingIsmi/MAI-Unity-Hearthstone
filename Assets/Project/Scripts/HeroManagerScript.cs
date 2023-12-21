using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroManagerScript 
{
    public int HP, Mana, ManaPool;
    const int MAX_MP = 10;

    public HeroManagerScript()
    {
        HP = 50;
        Mana = ManaPool = 2;
    }

    public void RestoreRoundMana()
    {
        Mana = ManaPool;
    }

    public void IncreaseManaPool()
    {
        ManaPool = Mathf.Clamp(ManaPool + 1, 0, MAX_MP);
    }

    public void GetDamage(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, int.MaxValue);
    }
}