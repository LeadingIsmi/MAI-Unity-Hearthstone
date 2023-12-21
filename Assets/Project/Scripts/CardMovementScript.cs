using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class CardMovementScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler //интерфейсы
{
    public CardControllerScript CardController;
    Camera MainCamera;
    Vector3 offset;
    public Transform DefaultParent, DefaultTempCardParent;
    GameObject TempCardGO;
    public bool IsDraggable;

    void Awake()
    {
        MainCamera = Camera.allCameras[0];  //инциализация камеры
        TempCardGO = GameObject.Find("TempCardGO");
    }

    public void OnBeginDrag(PointerEventData eventData) //выполняется единожды, как возьмется объект 
    {
        AudioControllerScript.Instance.PlayChoiceCard();

        offset = transform.position - MainCamera.ScreenToWorldPoint(eventData.position);

        DefaultParent = DefaultTempCardParent = transform.parent;

        IsDraggable = GameManagerScript.Instance.IsPlayerTurn &&
                      (
                      (DefaultParent.GetComponent<DropPlaceScript>().Type == FieldType.HAND &&
                       GameManagerScript.Instance.CurrentGame.Player.Mana >= CardController.Card.MnCst) ||
                      (DefaultParent.GetComponent<DropPlaceScript>().Type == FieldType.FIELD &&
                       CardController.Card.CanAttack)
                      );

        if (!IsDraggable)
            return;

        if (CardController.Card.IsSpell || CardController.Card.CanAttack)
            GameManagerScript.Instance.ShowTargetsLight(CardController, true);

        TempCardGO.transform.SetParent(DefaultParent);
        TempCardGO.transform.SetSiblingIndex(transform.GetSiblingIndex());

        transform.SetParent(DefaultParent.parent);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData) //выполняется каждый кадр, пока перемещается объект 
    {
        if (!IsDraggable)
            return;

        Vector3 newPos = MainCamera.ScreenToWorldPoint(eventData.position); //текущая координата мыши
        transform.position = newPos + offset; //текущая позиция карты

        if (!CardController.Card.IsSpell)
        {
            if (TempCardGO.transform.parent != DefaultTempCardParent)
                TempCardGO.transform.SetParent(DefaultTempCardParent);

            if (DefaultParent.GetComponent<DropPlaceScript>().Type != FieldType.FIELD)
                CheckPosition();
        }
    }

    public void OnEndDrag(PointerEventData eventData) //выполняется единожды, как отпустится объект
    {
        if (!IsDraggable)
            return;

        GameManagerScript.Instance.ShowTargetsLight(CardController, false);

        transform.SetParent(DefaultParent);
        GetComponent<CanvasGroup>().blocksRaycasts = true;

        transform.SetSiblingIndex(TempCardGO.transform.GetSiblingIndex());
        TempCardGO.transform.SetParent(GameObject.Find("Canvas").transform);
        TempCardGO.transform.localPosition = new Vector3(2101, 0);
    }

    void CheckPosition()
    {
        int newIndex = DefaultTempCardParent.childCount;

        for (int i = 0; i < DefaultTempCardParent.childCount; i++)
        {
            if (transform.position.x < DefaultTempCardParent.GetChild(i).position.x)
            {
                newIndex = i;

                if (TempCardGO.transform.GetSiblingIndex() < newIndex)
                    newIndex--;

                break;
            }
        }

        TempCardGO.transform.SetSiblingIndex(newIndex);
    }

    public void MoveToField(Transform field)
    {
        transform.SetParent(GameObject.Find("BackGround").transform);
        transform.DOMove(field.position, .5f);       
    }

    public void MoveToTarget(Transform target)
    {
        StartCoroutine(MoveToTargetCor(target));
    }

    IEnumerator MoveToTargetCor(Transform target)
    {
        Vector3 pos = transform.position;
        Transform parent = transform.parent;
        int i = transform.GetSiblingIndex();

        if (transform.parent.GetComponent<HorizontalLayoutGroup>())
            transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = false;

        transform.SetParent(GameObject.Find("BackGround").transform);
        transform.DOMove(target.position, .5f);
        yield return new WaitForSeconds(.25f);

        transform.DOMove(pos, .5f);
        yield return new WaitForSeconds(.25f);

        transform.SetParent(parent);
        transform.SetSiblingIndex(i);

        if (transform.parent.GetComponent<HorizontalLayoutGroup>())
            transform.parent.GetComponent<HorizontalLayoutGroup>().enabled = true;
    }
}
