using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAbilityScript : MonoBehaviour
{
    public CardControllerScript CardController;
    public GameObject Shield, Provocation;

    public void OnCast()
    {
        foreach (var ability in CardController.Card.Abilities)
        {
            switch (ability)
            {
                case Card.AbilityType.INSTANT_ACTIVE:
                    CardController.Card.CanAttack = true;

                    if (CardController.IsPlayerCard)
                        CardController.Info.AttackLightCard(true);
                    break;

                case Card.AbilityType.SHIELD:
                    Shield.SetActive(true);
                    break;

                case Card.AbilityType.PROVOCATION:
                    Provocation.SetActive(true);
                    break;
            }
        }
    }

    public void OnDamageDeal()
    {
        foreach (var ability in CardController.Card.Abilities)
        {
            switch (ability)
            {
                case Card.AbilityType.DOUBLE_ATTACK:
                    if (CardController.Card.TimesDealedDamage == 1)
                    {
                        CardController.Card.CanAttack = true;
                        if (CardController.IsPlayerCard)
                            CardController.Info.AttackLightCard(true);
                    }

                    break;
            }
        }
    }

    public void OnDamageTake(CardControllerScript attacker = null)
    {
        Shield.SetActive(false);

        foreach (var ability in CardController.Card.Abilities)
        {
            switch (ability)
            {
                case Card.AbilityType.SHIELD:
                    Shield.SetActive(true);
                    break;

                case Card.AbilityType.COUNTER_ATTACK:
                    if(attacker != null)
                        attacker.Card.GetDamage(CardController.Card.Atc);
                    break;
            }
        }
    }

    public void OnNewTurn()
    {
        CardController.Card.TimesDealedDamage = 0;

        foreach (var ability in CardController.Card.Abilities)
        {
            switch (ability)
            {
                case Card.AbilityType.REGENERATION:
                    CardController.Card.HP += 2;
                    CardController.Info.RefreshData();
                    break;
            }
        }
    }
}
