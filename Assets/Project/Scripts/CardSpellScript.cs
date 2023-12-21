using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSpellScript : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (!GameManagerScript.Instance.IsPlayerTurn)
            return;

        CardControllerScript spell = eventData.pointerDrag.GetComponent<CardControllerScript>(),
                             target = GetComponent<CardControllerScript>();

        if (spell &&
            spell.Card.IsSpell &&
            spell.IsPlayerCard &&
            target.Card.IsPlay &&
            GameManagerScript.Instance.CurrentGame.Player.Mana >= spell.Card.MnCst)
        {
            var spellCard = (SpellCard)spell.Card;

            if ((spellCard.SpellTarget == SpellCard.SpellTargetType.PLAYER_CARD_TARGET && target.IsPlayerCard) ||
                (spellCard.SpellTarget == SpellCard.SpellTargetType.ENEMY_CARD_TARGET && !target.IsPlayerCard) ||
                spellCard.SpellTarget == SpellCard.SpellTargetType.NO_TARGET)
            {
                GameManagerScript.Instance.ReduceMana(true, spell.Card.MnCst);
                spell.UseSpell(target);
                GameManagerScript.Instance.CheckCardsForMana();
            }
        }
    }
}
