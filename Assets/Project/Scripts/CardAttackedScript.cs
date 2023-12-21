using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardAttackedScript : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (!GameManagerScript.Instance.IsPlayerTurn)
            return;

        CardControllerScript attacker = eventData.pointerDrag.GetComponent<CardControllerScript>(),
                             defender = GetComponent<CardControllerScript>();

        if (attacker &&
            attacker.Card.CanAttack &&
            defender.Card.IsPlay)
        {
            if (GameManagerScript.Instance.EnemyFieldCards.Exists(x => x.Card.IsProvocation) &&
                !defender.Card.IsProvocation)
                return;

            AudioControllerScript.Instance.PlayHitCard();
            GameManagerScript.Instance.FightCards(attacker, defender);
        }
    }
}
