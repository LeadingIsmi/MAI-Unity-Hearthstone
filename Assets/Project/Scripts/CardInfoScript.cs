using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardInfoScript : MonoBehaviour
{
    public CardControllerScript CardController;
    public Image Image;
    public Image BGAttack, BGHitPoint;
    public TextMeshProUGUI Name, Info, Attack, HitPoint, ManaCost;
    public GameObject HideImage, BLImage;
    public Color NormalCol, TargetCol, SpellTargetCol;

    public void HideCardInfo()
    {
        HideImage.SetActive(true);
    }

    public void ShowCardInfo()
    {
        HideImage.SetActive(false);

        Image.sprite = CardController.Card.Image;
        Image.preserveAspect = true;
        Name.text = CardController.Card.Name;
        Info.text = CardController.Card.Info;

        if (CardController.Card.IsSpell)
        {
            BGAttack.gameObject.SetActive(false);
            BGHitPoint.gameObject.SetActive(false);
            Attack.gameObject.SetActive(false);
            HitPoint.gameObject.SetActive(false);
        }

        RefreshData();
    }

    public void RefreshData()
    {
        Attack.text = CardController.Card.Atc.ToString();
        HitPoint.text = CardController.Card.HP.ToString();
        ManaCost.text = CardController.Card.MnCst.ToString();
    }

    public void AttackLightCard(bool backlight)
    {
        if (CardController.Card.Atc > 0)
            BLImage.SetActive(backlight);
    }

    public void ManaLightCard(int currentMana)
    {
        GetComponent<CanvasGroup>().alpha = currentMana >= CardController.Card.MnCst ?
            1 : .5f;
    }

    public void TargetLightCard(bool highlight)
    {
        GetComponent<Image>().color = highlight ?
                                      TargetCol :
                                      NormalCol;
    }

    public void SpellTargetLightCard(bool highlight)
    {
        GetComponent<Image>().color = highlight ?
                                      SpellTargetCol :
                                      NormalCol;
    }


}
