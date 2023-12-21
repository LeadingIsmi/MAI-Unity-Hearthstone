using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card
{
    public enum AbilityType
    {
        NO_Ability,
        INSTANT_ACTIVE,
        DOUBLE_ATTACK,
        SHIELD,
        PROVOCATION,
        REGENERATION,
        COUNTER_ATTACK
    }

    public List<AbilityType> Abilities;

    public Sprite Image;
    public string Name;
    public string Info;
    public int Atc, HP, MnCst;
    public bool CanAttack;
    public bool IsPlay;
    public bool IsAlive
    {
        get
        {
            return HP > 0;
        }
    }
    public bool IsSpell;

    public int TimesDealedDamage;
    public bool HasAbility
    {
        get
        {
            return Abilities.Count > 0;
        }
    }
    public bool IsProvocation
    {
        get
        {
            return Abilities.Exists(x=> x == AbilityType.PROVOCATION);
        }
    } 

    public Card(string name, string image, string info, int atc, int hp, int mc, 
                AbilityType abilityType = 0)
    {      
        Name = name;
        Image = Resources.Load<Sprite>(image);
        Info = info;
        Atc = atc;
        HP = hp;
        MnCst = mc;
        CanAttack = false;
        IsPlay = false;

        Abilities = new List<AbilityType>();
        if (abilityType != 0)
            Abilities.Add(abilityType);

        TimesDealedDamage = 0;
    }

    public Card(Card card)
    {
        Name = card.Name;
        Image = card.Image;
        Info = card.Info;
        Atc = card.Atc;
        HP = card.HP;
        MnCst = card.MnCst;
        CanAttack = false;
        IsPlay = false;

        Abilities = new List<AbilityType>(card.Abilities);

        TimesDealedDamage = 0;
    }

    public void GetDamage(int dmg)
    {
        if (dmg > 0)
        {
            if (Abilities.Exists(x => x == AbilityType.SHIELD))
                Abilities.Remove(AbilityType.SHIELD);
            else
                HP -= dmg;
        }
    }

    public Card GetCopy() 
    {
        return new Card(this);
    }
}

public class SpellCard : Card
{
    public enum SpellType
    {
        NO_SPELL,
        HEAL_PLAYER_FIELD_CARDS,
        DAMAGE_ENEMY_FIELD_CARDS,
        HEAL_PLAYER_HERO,
        DAMAGE_ENEMY_HERO,
        HEAL_PLAYER_CARD,
        DAMAGE_ENEMY_CARD,
        SHIELD_PLAYER_CARD,
        PROVOCATION_PLAYER_CARD,
        BUFF_PLAYER_CARD_DAMAGE,
        DEBUFF_ENEMY_CARD_DAMAGE
    }

    public enum SpellTargetType
    {
        NO_TARGET,
        PLAYER_CARD_TARGET,
        ENEMY_CARD_TARGET
    }

    public SpellType Spell;
    public SpellTargetType SpellTarget;
    public int SpellValue;

    public SpellCard(string name, string image, string info, int mc,
                SpellType spellType = 0, int spellValue = 0,
                SpellTargetType spellTargetType = 0) : base(name, image, info, 0, 0, mc)
    {
        IsSpell = true;
        Spell = spellType;
        SpellTarget = spellTargetType;
        SpellValue = spellValue;
    }

    public SpellCard(SpellCard card) : base(card)
    {
        IsSpell = true;
        Spell = card.Spell;
        SpellTarget = card.SpellTarget;
        SpellValue = card.SpellValue;
    }

    public new SpellCard GetCopy()
    {
        return new SpellCard(this);
    }
}

public static class CardManager
{
    public static List<Card> AllCards = new List<Card>();
}

public class CardManagerScript : MonoBehaviour
{
    public void Awake()
    {
            CardManager.AllCards.Add(new Card("roflan", "Sprites/Cards/roflan", "", 5, 5, 5));
            CardManager.AllCards.Add(new Card("penguin", "Sprites/Cards/penguin", "", 4, 5, 4));
            CardManager.AllCards.Add(new Card("buldiga", "Sprites/Cards/buldiga", "", 4, 2, 3));
            CardManager.AllCards.Add(new Card("vo", "Sprites/Cards/vo", "", 6, 1, 3));
            CardManager.AllCards.Add(new Card("sad pepe", "Sprites/Cards/sad_pepe", "", 7, 4, 5));
            CardManager.AllCards.Add(new Card("ded", "Sprites/Cards/ded", "", 10, 10, 10));

            CardManager.AllCards.Add(new Card("wojak", "Sprites/Cards/wojak", "PROVOCATION", 2, 2, 2, Card.AbilityType.PROVOCATION));
            CardManager.AllCards.Add(new Card("ok", "Sprites/Cards/ok", "REGENERATION: 2", 4, 5, 6, Card.AbilityType.REGENERATION));
            CardManager.AllCards.Add(new Card("troll face", "Sprites/Cards/troll_face", "DOUBLE ATTACK", 2, 4, 3, Card.AbilityType.DOUBLE_ATTACK));
            CardManager.AllCards.Add(new Card("shlepa", "Sprites/Cards/shlepa", "INSTANT ACTIVE", 6, 1, 4, Card.AbilityType.INSTANT_ACTIVE));
            CardManager.AllCards.Add(new Card("like a boss", "Sprites/Cards/like_a_boss", "SHIELD", 6, 1, 4, Card.AbilityType.SHIELD));
            CardManager.AllCards.Add(new Card("kid", "Sprites/Cards/kid", "COUNTER ATTACK", 5, 5, 6, Card.AbilityType.COUNTER_ATTACK));

            CardManager.AllCards.Add(new SpellCard("vjuh", "Sprites/Cards/vjuh", "HEAL PLAYER FIELD CARDS: 2", 4,
                                              SpellCard.SpellType.HEAL_PLAYER_FIELD_CARDS, 2, SpellCard.SpellTargetType.NO_TARGET));
            CardManager.AllCards.Add(new SpellCard("tornado", "Sprites/Cards/tornado", "DAMAGE ENEMY FIELD CARDS: 2", 4,
                                              SpellCard.SpellType.DAMAGE_ENEMY_FIELD_CARDS, 2, SpellCard.SpellTargetType.NO_TARGET));
            CardManager.AllCards.Add(new SpellCard("mercy", "Sprites/Cards/mercy", "HEAL PLAYER HERO: 4", 2,
                                              SpellCard.SpellType.HEAL_PLAYER_HERO, 4, SpellCard.SpellTargetType.NO_TARGET));
            CardManager.AllCards.Add(new SpellCard("mr incredible", "Sprites/Cards/mr incredible", "DAMAGE ENEMY HERO: 4", 2,
                                              SpellCard.SpellType.DAMAGE_ENEMY_HERO, 4, SpellCard.SpellTargetType.NO_TARGET));
            CardManager.AllCards.Add(new SpellCard("defo", "Sprites/Cards/defo", "HEAL PLAYER CARD: 3", 2,
                                              SpellCard.SpellType.HEAL_PLAYER_CARD, 3, SpellCard.SpellTargetType.PLAYER_CARD_TARGET));
            CardManager.AllCards.Add(new SpellCard("chego nadelal", "Sprites/Cards/chego nadelal", "DAMAGE ENEMY CARD: 3", 2,
                                              SpellCard.SpellType.DAMAGE_ENEMY_CARD, 3, SpellCard.SpellTargetType.ENEMY_CARD_TARGET));
            CardManager.AllCards.Add(new SpellCard("diamond set", "Sprites/Cards/diamond set", "SHIELD PLAYER CARD", 2,
                                              SpellCard.SpellType.SHIELD_PLAYER_CARD, 0, SpellCard.SpellTargetType.PLAYER_CARD_TARGET));
            CardManager.AllCards.Add(new SpellCard("coincidence", "Sprites/Cards/coincidence", "PROVOCATION PLAYER CARD", 1,
                                              SpellCard.SpellType.PROVOCATION_PLAYER_CARD, 0, SpellCard.SpellTargetType.PLAYER_CARD_TARGET));
            CardManager.AllCards.Add(new SpellCard("shawarma", "Sprites/Cards/shawarma", "BUFF PLAYER CARD DAMAGE: 4", 2,
                                              SpellCard.SpellType.BUFF_PLAYER_CARD_DAMAGE, 4, SpellCard.SpellTargetType.PLAYER_CARD_TARGET));
            CardManager.AllCards.Add(new SpellCard("vanga", "Sprites/Cards/vanga", "DEBUFF ENEMY CARD DAMAGE: 4", 2,
                                              SpellCard.SpellType.DEBUFF_ENEMY_CARD_DAMAGE, 4, SpellCard.SpellTargetType.ENEMY_CARD_TARGET));
    }
}
