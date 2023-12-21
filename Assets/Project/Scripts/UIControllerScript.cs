using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIControllerScript : MonoBehaviour
{
    public static UIControllerScript Instance;

    public GameObject StartWindow;
    public GameObject BackGround;

    public GameObject PlayerFirstHand;
    public TextMeshProUGUI ReadyTime;
    public Button ReadyBtn;
    public Button ReplaceBtn;

    public TextMeshProUGUI PlayerMana, EnemyMana;
    public TextMeshProUGUI PlayerHP, EnemyHP;

    public TextMeshProUGUI PlayerCountCards, EnemyCountCards;
    public TextMeshProUGUI Turn;  

    public TextMeshProUGUI TurnTime;
    public Button MenuBtn;
    public Button EndTurnBtn;

    public GameObject MenuObj;
    public Scrollbar VolumeScrollbar;

    public GameObject StartTurnObj;
    public TextMeshProUGUI StartTurnTxt;

    public GameObject ResultObj;
    public TextMeshProUGUI ResultTxt;

    public Image Wick;

    private void Awake()
    {
        if (!Instance)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this);
    }

    public void ShowStartWindow()
    {
        StartWindow.SetActive(true);
        ResultObj.SetActive(false);
        StartTurnObj.SetActive(false);
        MenuObj.SetActive(false);
        BackGround.SetActive(false);
    }

    public void ShowFirstCard()
    {
        //foreach (var card in GameManagerScript.Instance.PlayerHandCards)
        //card.Info.GetComponent<CanvasGroup>().alpha = 0;

        ReplaceBtn.interactable = true;
        BackGround.SetActive(true);
        StartWindow.SetActive(false);
        MenuObj.SetActive(false);
        ResultObj.SetActive(false);
        StartTurnObj.SetActive(false);
        PlayerFirstHand.SetActive(true);      
    }

    public void ShowStartGame()
    {
        EndTurnBtn.interactable = GameManagerScript.Instance.IsPlayerTurn;

        PlayerFirstHand.SetActive(false);
        ResultObj.SetActive(false);
        MenuObj.SetActive(false);
        StartTurnObj.SetActive(false);

        UpdateHeroHPAndMana();
        UpdateTurnAndCountCards();
    }

    public void ShowMenu()
    {
        MenuObj.SetActive(true);     
    }

    public void ShowStartTurn()
    {
        StartTurnObj.SetActive(true);

        if (GameManagerScript.Instance.IsPlayerTurn)
            StartTurnTxt.text = "Your turn";
        else
            StartTurnTxt.text = "Enemy turn";
    }

    public void CloseStartTurn()
    {
        StartTurnObj.SetActive(false);
    }

    public void ShowResult()
    {
        StopAllCoroutines();
        ResultObj.SetActive(true);

        if (GameManagerScript.Instance.CurrentGame.Player.HP == 0)
            ResultTxt.text = "You lose :(";
        else
            ResultTxt.text = "GG WP!";
    }

    public void UpdateTurnAndCountCards()
    {
        PlayerCountCards.text = GameManagerScript.Instance.CurrentGame.PlayerDeck.Count.ToString();
        EnemyCountCards.text = GameManagerScript.Instance.CurrentGame.EnemyDeck.Count.ToString();

        Turn.text = GameManagerScript.Instance.Turn.ToString();
    }

    public void UpdateHeroHPAndMana()
    {
        PlayerMana.text = GameManagerScript.Instance.CurrentGame.Player.Mana.ToString();
        EnemyMana.text = GameManagerScript.Instance.CurrentGame.Enemy.Mana.ToString();

        PlayerHP.text = GameManagerScript.Instance.CurrentGame.Player.HP.ToString();
        EnemyHP.text = GameManagerScript.Instance.CurrentGame.Enemy.HP.ToString();
    }

    public void UpdateWickTime(float time)
    {
        Wick.fillAmount = time / 30f;
    }

    public void UpdateTurnTime(float time)
    {
        TurnTime.text = time.ToString();
    }

    public void UpdateReadyTime(int time)
    {
        ReadyTime.text = time.ToString();
    }

    public void DisableTurnBtn()
    {
        EndTurnBtn.interactable = GameManagerScript.Instance.IsPlayerTurn;
    }
}