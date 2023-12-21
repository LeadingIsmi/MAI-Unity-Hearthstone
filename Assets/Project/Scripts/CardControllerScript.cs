using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardControllerScript : MonoBehaviour
{
    public Card Card;
    public bool IsPlayerCard;
    public CardInfoScript Info;
    public CardMovementScript Movement;
    public CardAbilityScript Ability;
    GameManagerScript gameManager;

    public void Init(Card card, bool isPlayerCard)
    {
        Card = card;
        gameManager = GameManagerScript.Instance;
        IsPlayerCard = isPlayerCard;

        if (IsPlayerCard)
        {
            Info.ShowCardInfo();
            GetComponent<CardAttackedScript>().enabled = false;
        }
        else
            Info.HideCardInfo();
    }

    public void OnCast()
    {
        if (Card.IsSpell && ((SpellCard)Card).SpellTarget != SpellCard.SpellTargetType.NO_TARGET)
            return;      

        if (IsPlayerCard)
        {
            gameManager.PlayerHandCards.Remove(this);
            gameManager.PlayerFieldCards.Add(this);
            gameManager.ReduceMana(true, Card.MnCst);
            gameManager.CheckCardsForMana();
        }
        else
        {
            Info.ShowCardInfo();
            gameManager.EnemyHandCards.Remove(this);
            gameManager.EnemyFieldCards.Add(this);
            gameManager.ReduceMana(false, Card.MnCst);
        }

        Card.IsPlay = true;

        if (Card.HasAbility)
            Ability.OnCast();

        if (Card.IsSpell)
            UseSpell(null);

        UIControllerScript.Instance.UpdateHeroHPAndMana();
    }

    public void OnTakeDamage(CardControllerScript attacker = null)
    {
        CheckForAlive();
        Ability.OnDamageTake(attacker);
    }

    public void OnDamageDeal()
    {
        Card.TimesDealedDamage++;
        Card.CanAttack = false;
        Info.AttackLightCard(false);

        if (Card.HasAbility)
            Ability.OnDamageDeal();
    }
    
    public void UseSpell(CardControllerScript target)
    {
        var spellCard = (SpellCard)Card;
        bool mustDestroyed = true;
        switch (spellCard.Spell)
        {
            case SpellCard.SpellType.BUFF_PLAYER_CARD_DAMAGE:

                AudioControllerScript.Instance.PlayBuffCard();

                target.Card.Atc += spellCard.SpellValue;

                if(target.Card.TimesDealedDamage < 1 && !target.Card.IsPlay)
                {
                    target.Card.CanAttack = true;
                    target.Info.AttackLightCard(true);
                }

                break;

            case SpellCard.SpellType.DAMAGE_ENEMY_CARD:

                AudioControllerScript.Instance.PlayHitSpellCard();

                GiveDamageTo(target, spellCard.SpellValue);

                break;

            case SpellCard.SpellType.DAMAGE_ENEMY_FIELD_CARDS:

                AudioControllerScript.Instance.PlayHitSpellCard();

                var enemyCards = IsPlayerCard ?
                                  new List<CardControllerScript>(gameManager.EnemyFieldCards) :
                                  new List<CardControllerScript>(gameManager.PlayerFieldCards);

                foreach (var card in enemyCards)
                    GiveDamageTo(card, spellCard.SpellValue);

                break;

            case SpellCard.SpellType.DAMAGE_ENEMY_HERO:

                AudioControllerScript.Instance.PlayHitSpellCard();

                if (IsPlayerCard)
                    gameManager.CurrentGame.Enemy.HP -= spellCard.SpellValue;
                else
                    gameManager.CurrentGame.Player.HP -= spellCard.SpellValue;

                UIControllerScript.Instance.UpdateHeroHPAndMana();
                gameManager.CheckResult();

                break;

            case SpellCard.SpellType.DEBUFF_ENEMY_CARD_DAMAGE:

                AudioControllerScript.Instance.PlayBuffCard();

                target.Card.Atc = Mathf.Clamp(target.Card.Atc - spellCard.SpellValue, 0, int.MaxValue);

                break;

            case SpellCard.SpellType.HEAL_PLAYER_CARD:

                AudioControllerScript.Instance.PlayBuffCard();

                target.Card.HP += spellCard.SpellValue;

                break;

            case SpellCard.SpellType.HEAL_PLAYER_FIELD_CARDS:

                AudioControllerScript.Instance.PlayBuffCard();

                var playerCards = IsPlayerCard ?
                                  gameManager.PlayerFieldCards :
                                  gameManager.EnemyFieldCards;

                foreach (var card in playerCards)
                {
                    card.Card.HP += spellCard.SpellValue;
                    card.Info.RefreshData();
                }

                break;

            case SpellCard.SpellType.HEAL_PLAYER_HERO:

                AudioControllerScript.Instance.PlayBuffCard();

                if (IsPlayerCard)
                    gameManager.CurrentGame.Player.HP += spellCard.SpellValue;
                else
                    gameManager.CurrentGame.Enemy.HP += spellCard.SpellValue;

                UIControllerScript.Instance.UpdateHeroHPAndMana();
                gameManager.CheckResult();

                break;

            case SpellCard.SpellType.PROVOCATION_PLAYER_CARD:

                AudioControllerScript.Instance.PlayBuffCard();

                if (!target.Card.Abilities.Exists(x => x == Card.AbilityType.PROVOCATION))
                    target.Card.Abilities.Add(Card.AbilityType.PROVOCATION);
                /*
                else
                {
                    if (target.IsPlayerCard)
                    {
                        mustDestroyed = false;
                        gameManager.PlayerHandCards.Add(target);
                        gameManager.CurrentGame.Player.Mana += spellCard.MnCst;
                    }
                    else
                    {
                        mustDestroyed = false;
                        gameManager.EnemyHandCards.Add(target);
                        gameManager.CurrentGame.Enemy.Mana += spellCard.MnCst;
                    }
                }
                */
                break;

            case SpellCard.SpellType.SHIELD_PLAYER_CARD:

                AudioControllerScript.Instance.PlayBuffCard();

                if (!target.Card.Abilities.Exists(x => x == Card.AbilityType.SHIELD))
                    target.Card.Abilities.Add(Card.AbilityType.SHIELD);
                else
                {
                    /*
                    if (target.IsPlayerCard)
                    {
                        mustDestroyed = false;
                        gameManager.PlayerHandCards.Add(target);
                        gameManager.CurrentGame.Player.Mana += spellCard.MnCst;
                    }
                    else
                    {
                        mustDestroyed = false;
                        gameManager.EnemyHandCards.Add(target);
                        gameManager.CurrentGame.Enemy.Mana += spellCard.MnCst;
                    }
                    */
                }

                break;
        }
        
        if (target != null)
        {
            target.Ability.OnCast();
            target.CheckForAlive();
        }

        if (mustDestroyed)
            DestroyCard();
    }
    
    public void GiveDamageTo(CardControllerScript card, int damage)
    {
        card.Card.GetDamage(damage);
        card.CheckForAlive();
        card.OnTakeDamage();
    }

    public void CheckForAlive()
    {
        if (Card.IsAlive)
            Info.RefreshData();
        else
            DestroyCard();
    }

    public void DestroyCard()
    {
        Movement.OnEndDrag(null);

        RemoveCardFromList(gameManager.EnemyFieldCards);
        RemoveCardFromList(gameManager.PlayerFieldCards);
        RemoveCardFromList(gameManager.EnemyHandCards);
        RemoveCardFromList(gameManager.PlayerHandCards);

        Destroy(gameObject);
    }

    void RemoveCardFromList(List<CardControllerScript> list)
    {
        if (list.Exists(x => x == this))
            list.Remove(this);
    }    
}
