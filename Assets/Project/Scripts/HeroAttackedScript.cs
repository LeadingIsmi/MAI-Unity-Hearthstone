using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HeroAttackedScript : MonoBehaviour, IDropHandler
{
    public enum HeroType
    {
        ENEMY,
        PLAYER
    }

    public HeroType Type;
    public Color NormalCol, TargetCol;

    public void OnDrop(PointerEventData eventData)
    {
        if (!GameManagerScript.Instance.IsPlayerTurn)
            return;

        CardControllerScript card = eventData.pointerDrag.GetComponent<CardControllerScript>();

        if (card &&
            card.Card.CanAttack &&
            Type == HeroType.ENEMY &&
            !GameManagerScript.Instance.EnemyFieldCards.Exists(x => x.Card.IsProvocation)
            )
        {
            AudioControllerScript.Instance.PlayHitCard();
            GameManagerScript.Instance.DamageHero(card, true);
        }
    }

    public void TargetLightCard(bool highlight)
    {
        GetComponent<Image>().color = highlight ?
                                      TargetCol :
                                      NormalCol;
    }
}
