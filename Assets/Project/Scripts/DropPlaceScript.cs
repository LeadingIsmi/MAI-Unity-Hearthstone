using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum FieldType
{
    HAND,
    FIELD,
    ENEMY_FIELD,
    ENEMY_HAND
}

public class DropPlaceScript : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler //интерфейсы
{
    public FieldType Type;

    public void OnDrop(PointerEventData eventData) //выполняется, когда объект отпускают
    {    
        if (Type != FieldType.FIELD)
            return;

        CardControllerScript card = eventData.pointerDrag.GetComponent<CardControllerScript>(); //eventData хранит инфу об указателях и объектах, с которыми взаим

        if (card &&
            GameManagerScript.Instance.IsPlayerTurn &&
            GameManagerScript.Instance.CurrentGame.Player.Mana >= card.Card.MnCst &&
            !card.Card.IsPlay)
        {
            if(!card.Card.IsSpell)
            {
                AudioControllerScript.Instance.PlayDrawCard();
                card.Movement.DefaultParent = transform;
            }

            card.OnCast();        
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null ||
            Type == FieldType.ENEMY_FIELD || 
            Type == FieldType.ENEMY_HAND ||
            Type == FieldType.HAND)
            return;

        CardMovementScript card = eventData.pointerDrag.GetComponent<CardMovementScript>();

        if (card)
            card.DefaultTempCardParent = transform;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        CardMovementScript card = eventData.pointerDrag.GetComponent<CardMovementScript>();
        
        if (card && card.DefaultTempCardParent == transform)
            card.DefaultTempCardParent = card.DefaultParent;
    }
}
