using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControllerScript : MonoBehaviour
{
    public void MakeTurn()
    {
        StartCoroutine(EnemyTurn(GameManagerScript.Instance.EnemyHandCards));
    }

    public IEnumerator EnemyTurn(List<CardControllerScript> cards)
    {
        yield return new WaitForSeconds(0.25f);
        int count = cards.Count;

        for (int i = 0; i < count; i++)
        {
            if (GameManagerScript.Instance.EnemyFieldCards.Count > 6 ||
                GameManagerScript.Instance.CurrentGame.Enemy.Mana == 0 ||
                GameManagerScript.Instance.EnemyHandCards.Count == 0)
                break;

            List<CardControllerScript> cardList = cards.FindAll(x => GameManagerScript.Instance.CurrentGame.Enemy.Mana >= x.Card.MnCst);

            if (cardList.Count == 0)
                break;

            int j = Random.Range(0, cardList.Count);

            if (cardList[j].Card.IsSpell)
            {
                CastCardSpell(cardList[j]);
                yield return new WaitForSeconds(1f);
            }
            else
            {
                cardList[j].Info.ShowCardInfo();
                cardList[j].GetComponent<CardMovementScript>().MoveToField(GameManagerScript.Instance.EnemyField);                
                AudioControllerScript.Instance.PlayDrawCard();
                yield return new WaitForSeconds(1f);
                cardList[j].transform.SetParent(GameManagerScript.Instance.EnemyField);
                cardList[j].OnCast();
            }
        }

        yield return new WaitForSeconds(0.25f);

        while (GameManagerScript.Instance.EnemyFieldCards.Exists(x => x.Card.CanAttack))
        {
            var activeCard = GameManagerScript.Instance.EnemyFieldCards.FindAll(x => x.Card.CanAttack)[0];
            bool hasProvocation = GameManagerScript.Instance.PlayerFieldCards.Exists(x => x.Card.IsProvocation);

            if (hasProvocation ||
                Random.Range(0, 1) == 0 &&
                GameManagerScript.Instance.PlayerFieldCards.Count > 0)
            {
                CardControllerScript enemy;

                if (hasProvocation)
                    enemy = GameManagerScript.Instance.PlayerFieldCards.Find(x => x.Card.IsProvocation);
                else
                    enemy = GameManagerScript.Instance.PlayerFieldCards[Random.Range(0, GameManagerScript.Instance.PlayerFieldCards.Count)];

                Debug.Log(activeCard.Card.Name + " (" + activeCard.Card.Atc + "; " + activeCard.Card.HP + ") " +
                          " ---> " + enemy.Card.Name + " (" + enemy.Card.Atc + "; " + enemy.Card.HP + ") ");

                activeCard.Movement.MoveToTarget(enemy.transform);
                AudioControllerScript.Instance.PlayHitCard();
                yield return new WaitForSeconds(1);
                GameManagerScript.Instance.FightCards(enemy, activeCard);
            }
            else
            {
                if (GameManagerScript.Instance.CurrentGame.Player.HP == 0)
                    yield break;

                Debug.Log(activeCard.Card.Name + " (" + activeCard.Card.Atc + "; " + activeCard.Card.HP + ") " + "Attacked the hero");

                activeCard.GetComponent<CardMovementScript>().MoveToTarget(GameManagerScript.Instance.PlayerHero.transform);
                AudioControllerScript.Instance.PlayHitCard();
                yield return new WaitForSeconds(1);
                GameManagerScript.Instance.DamageHero(activeCard, false);
            }

            yield return new WaitForSeconds(0.25f);          
        }

        GameManagerScript.Instance.ChangeTurn();
    }

    void CastCardSpell(CardControllerScript card)
    {
        switch (((SpellCard)card.Card).SpellTarget)
        {
            case SpellCard.SpellTargetType.NO_TARGET:

                switch (((SpellCard)card.Card).Spell)
                {
                    case SpellCard.SpellType.HEAL_PLAYER_FIELD_CARDS:
                        if (GameManagerScript.Instance.EnemyFieldCards.Count > 1)
                            StartCoroutine(SpellCardMovement(card));

                        break;

                    case SpellCard.SpellType.DAMAGE_ENEMY_FIELD_CARDS:
                        if (GameManagerScript.Instance.PlayerFieldCards.Count > 1)
                            StartCoroutine(SpellCardMovement(card));

                        break;

                    case SpellCard.SpellType.HEAL_PLAYER_HERO:
                        StartCoroutine(SpellCardMovement(card));

                        break;

                    case SpellCard.SpellType.DAMAGE_ENEMY_HERO:
                        StartCoroutine(SpellCardMovement(card));

                        break;
                }

                break;

            case SpellCard.SpellTargetType.PLAYER_CARD_TARGET:
                if (GameManagerScript.Instance.EnemyFieldCards.Count > 0)
                    StartCoroutine(SpellCardMovement(card, 
                        GameManagerScript.Instance.EnemyFieldCards[Random.Range(0, GameManagerScript.Instance.EnemyFieldCards.Count)]));

                break;

            case SpellCard.SpellTargetType.ENEMY_CARD_TARGET:
                if (GameManagerScript.Instance.PlayerFieldCards.Count > 0)
                    StartCoroutine(SpellCardMovement(card,
                        GameManagerScript.Instance.PlayerFieldCards[Random.Range(0, GameManagerScript.Instance.PlayerFieldCards.Count)]));

                break;
        }
    }

    IEnumerator SpellCardMovement(CardControllerScript spell, CardControllerScript target = null)
    {
        if (((SpellCard)spell.Card).SpellTarget == SpellCard.SpellTargetType.NO_TARGET)
        {
            spell.Info.ShowCardInfo();
            spell.GetComponent<CardMovementScript>().MoveToField(GameManagerScript.Instance.EnemyField);
            yield return new WaitForSeconds(1f);

            spell.OnCast();
        }
        else
        {
            spell.Info.ShowCardInfo();
            spell.GetComponent<CardMovementScript>().MoveToTarget(target.transform);
            yield return new WaitForSeconds(1f);

            GameManagerScript.Instance.ReduceMana(false, spell.Card.MnCst);
            GameManagerScript.Instance.EnemyHandCards.Remove(spell);
            GameManagerScript.Instance.EnemyFieldCards.Add(spell);

            spell.Card.IsPlay = true;        
            spell.UseSpell(target);
        }

        string targetStr = target == null ? "no target " : target.Card.Name;
        Debug.Log("AI spell cast: " + (spell.Card).Name + " target: " + targetStr);
    }
}
