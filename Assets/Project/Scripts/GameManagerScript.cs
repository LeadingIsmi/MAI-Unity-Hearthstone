using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Game
{
    public HeroManagerScript Player, Enemy;
    public List<Card> PlayerDeck, EnemyDeck;
    
    public Game()
    {
        PlayerDeck = GiveDeckCard();
        EnemyDeck = GiveDeckCard();

        Player = new HeroManagerScript();
        Enemy = new HeroManagerScript();
    }

    List<Card> GiveDeckCard()
    {
        List<Card> list = new List<Card>();

        list.Add(CardManager.AllCards[10].GetCopy());

        for (int i = 0; i < 30; i++)
        {
            var card = CardManager.AllCards[Random.Range(0, CardManager.AllCards.Count)];

            if(card.IsSpell)
                list.Add(((SpellCard)card).GetCopy());
            else
                list.Add(card.GetCopy()); 
        }
            
        return list;
    }
}

public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript Instance;

    public Game CurrentGame;
    public Transform PlayerHand, PlayerField, EnemyHand, EnemyField;
    public GameObject CardPref;
    public int Turn, ReadyTime;
    public float TurnTime;
    public HeroAttackedScript EnemyHero, PlayerHero;
    public AIControllerScript EnemyAI;

    public List<CardControllerScript> PlayerHandCards = new List<CardControllerScript>(),
                                      EnemyHandCards = new List<CardControllerScript>(),
                                      PlayerFieldCards = new List<CardControllerScript>(),
                                      EnemyFieldCards = new List<CardControllerScript>();                                    

    public bool IsPlayerTurn
    {
        get
        {
            return Turn % 2 == 0;
        }
    }

    public void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    void Start()
    {
        StartWindow();
    }

    public void StartWindow()
    {
        UIControllerScript.Instance.ShowStartWindow();
        AudioControllerScript.Instance.PlayStartWindowAudio();
        CleanGame();
    }

    public void RestartGame()
    {
        AudioControllerScript.Instance.PlaySettings();
        CleanGame();
        StartGame();
    }

    public void MenuGame()
    {
        AudioControllerScript.Instance.PlaySettings();
        UIControllerScript.Instance.ShowMenu();
    }

    public void ContinueGame()
    {
        AudioControllerScript.Instance.PlaySettings();
        UIControllerScript.Instance.ShowStartGame();
        CheckResult();
    }

    public void QuitGame()
    {
        AudioControllerScript.Instance.PlaySettings();
        CleanGame();
        Process.GetCurrentProcess().Kill();
        //Application.Quit();
    }

    public void StartGame()
    {
        CurrentGame = new Game();
        ReadyTime = 20;

        Turn = 1;
        TurnTime = 30;

        GiveHandCards(CurrentGame.PlayerDeck, PlayerHand);
        GiveHandCards(CurrentGame.EnemyDeck, EnemyHand);

        AudioControllerScript.Instance.PlayBGAudio();
        UIControllerScript.Instance.UpdateHeroHPAndMana();
        UIControllerScript.Instance.UpdateTurnAndCountCards();
        UIControllerScript.Instance.UpdateTurnTime(TurnTime);
        UIControllerScript.Instance.UpdateWickTime(TurnTime);
        UIControllerScript.Instance.ShowFirstCard();

        StartCoroutine(GiveFirstCardsFunc());
    }

    public void StartTurn()
    {
        StopCoroutine(GiveFirstCardsFunc());

        AudioControllerScript.Instance.PlayChangeTurn();
        UIControllerScript.Instance.ShowStartGame();
        StartCoroutine(TurnFunc());
    }

    void GiveHandCards(List<Card> deck, Transform hand)
    {
        for (int i = 0; i < 4; i++)
            GiveCardToHand(deck, hand);
    }

    void GiveCardToHand(List<Card> deck, Transform hand)
    {
        if (deck.Count == 0)
            return;

        CreateCardPref(deck[0], hand);
        deck.RemoveAt(0);
    }

    public void ReplaceStartingCards()
    {
        AudioControllerScript.Instance.PlaySettings();

        List<Card> replacedCards = new List<Card>();

        foreach (var card in PlayerHandCards)
        {
            replacedCards.Add(card.Card);
            Destroy(card.gameObject);
        }

        PlayerHandCards.Clear();

        CurrentGame.PlayerDeck.AddRange(replacedCards);
        GiveHandCards(CurrentGame.PlayerDeck, PlayerHand);
        UIControllerScript.Instance.ReplaceBtn.interactable = false;
    }

    void CreateCardPref(Card card, Transform hand)
    {
        GameObject cardGO = Instantiate(CardPref, hand, false);
        CardControllerScript cardC = cardGO.GetComponent<CardControllerScript>();

        cardC.Init(card, hand == PlayerHand);

        if (cardC.IsPlayerCard)
            PlayerHandCards.Add(cardC);
        else
            EnemyHandCards.Add(cardC);
    }

    IEnumerator GiveFirstCardsFunc()
    {    
        UIControllerScript.Instance.UpdateReadyTime(ReadyTime);

        while (ReadyTime-- > 0)
        {

            UIControllerScript.Instance.UpdateReadyTime(ReadyTime);
            yield return new WaitForSeconds(1);
        }

        StartTurn();
    }

    IEnumerator TurnFunc()
    {
        UIControllerScript.Instance.UpdateTurnTime(TurnTime);
        UIControllerScript.Instance.UpdateWickTime(TurnTime);

        foreach (var card in PlayerFieldCards)
            card.Info.AttackLightCard(false);

        CheckCardsForMana();

        if (IsPlayerTurn)
        {
            UIControllerScript.Instance.ShowStartTurn();
            yield return new WaitForSeconds(1.5f);
            UIControllerScript.Instance.CloseStartTurn();

            foreach (var card in PlayerFieldCards)
            {
                if (card.Card.Atc > 0)
                    card.Card.CanAttack = true;
                card.Info.AttackLightCard(true);
                card.Ability.OnNewTurn();
            }

            while (TurnTime-- > 0)
            {
                UIControllerScript.Instance.UpdateWickTime(TurnTime);
                UIControllerScript.Instance.UpdateTurnTime(TurnTime);
                yield return new WaitForSeconds(1);
            }

            ChangeTurn();
        }
        else
        {
            UIControllerScript.Instance.ShowStartTurn();
            yield return new WaitForSeconds(1.5f);
            UIControllerScript.Instance.CloseStartTurn();

            foreach (var card in EnemyFieldCards)
            {
                if (card.Card.Atc > 0)
                    card.Card.CanAttack = true;
                card.Ability.OnNewTurn();
            }

            EnemyAI.MakeTurn();

            while (TurnTime-- > 0)
            {
                UIControllerScript.Instance.UpdateWickTime(TurnTime);
                UIControllerScript.Instance.UpdateTurnTime(TurnTime);
                yield return new WaitForSeconds(1);
            }

            ChangeTurn();
        }       
    }

    public void ChangeTurn()
    {
        StopAllCoroutines();

        TurnTime = 30;
        Turn++;

        UIControllerScript.Instance.DisableTurnBtn();
        AudioControllerScript.Instance.PlayChangeTurn();

        if (IsPlayerTurn)
        {
            if (PlayerHandCards.Count < 10)
                GiveCardToHand(CurrentGame.PlayerDeck, PlayerHand);

            CurrentGame.Player.IncreaseManaPool();
            CurrentGame.Player.RestoreRoundMana();

            UIControllerScript.Instance.UpdateHeroHPAndMana();
        }
        else
        {
            if(EnemyHandCards.Count < 10)
            {
                for (int i = 0; i < Random.Range(1, 3); i++)
                    GiveCardToHand(CurrentGame.EnemyDeck, EnemyHand);
            }

            CurrentGame.Enemy.IncreaseManaPool();
            CurrentGame.Enemy.RestoreRoundMana();

            UIControllerScript.Instance.UpdateHeroHPAndMana();
        }

        UIControllerScript.Instance.UpdateTurnAndCountCards();

        StartCoroutine(TurnFunc());
    }

    public void FightCards(CardControllerScript attacker, CardControllerScript defender)    
    {
        defender.Card.GetDamage(attacker.Card.Atc);
        attacker.OnDamageDeal();
        defender.OnTakeDamage(attacker);

        attacker.Card.GetDamage(defender.Card.Atc);
        defender.OnDamageDeal();
        attacker.OnTakeDamage(defender);

        attacker.CheckForAlive();
        defender.CheckForAlive();
    }

    public void ReduceMana(bool playerMana, int manacost)
    {
        if (playerMana)
            CurrentGame.Player.Mana -= manacost;
        else
            CurrentGame.Enemy.Mana -= manacost;

        UIControllerScript.Instance.UpdateHeroHPAndMana();
    }

    public void DamageHero(CardControllerScript card, bool isEnemyAttacked)
    {
        if (isEnemyAttacked)
            CurrentGame.Enemy.GetDamage(card.Card.Atc);
        else
            CurrentGame.Player.GetDamage(card.Card.Atc);

        UIControllerScript.Instance.UpdateHeroHPAndMana();
        card.OnDamageDeal();
        CheckResult();
    }

    public void CheckResult()
    {
        if (CurrentGame.Enemy.HP == 0 || CurrentGame.Player.HP == 0)
        {
            StopCoroutine(EnemyAI.EnemyTurn(EnemyHandCards));
            StopCoroutine(EnemyAI.EnemyTurn(EnemyFieldCards));
            StopAllCoroutines();
            UIControllerScript.Instance.ShowResult();
        } 
    }

    public void CheckCardsForMana()
    {
        foreach (var card in PlayerHandCards)
            card.Info.ManaLightCard(CurrentGame.Player.Mana);
    }

    public void ShowTargetsLight(CardControllerScript attacker, bool highlight)
    {
        List<CardControllerScript> targets = new List<CardControllerScript>();

        if (attacker.Card.IsSpell)
        {
            var spellCard = (SpellCard)attacker.Card;

            if (spellCard.SpellTarget == SpellCard.SpellTargetType.NO_TARGET)
                targets = new List<CardControllerScript>();
            else if (spellCard.SpellTarget == SpellCard.SpellTargetType.PLAYER_CARD_TARGET)
                targets = PlayerFieldCards;
            else if (spellCard.SpellTarget == SpellCard.SpellTargetType.ENEMY_CARD_TARGET)
                targets = EnemyFieldCards;
        }
        else
        {
            if (EnemyFieldCards.Exists(x => x.Card.IsProvocation))
                targets = EnemyFieldCards.FindAll(x => x.Card.IsProvocation);
            else
            {
                targets = EnemyFieldCards;
                EnemyHero.TargetLightCard(highlight);
            }
        }

        foreach (var card in targets)
        {
            if(attacker.Card.IsSpell)
                card.Info.SpellTargetLightCard(highlight);
            else
                card.Info.TargetLightCard(highlight);
        }
    }

    public void CleanGame()
    {
        StopAllCoroutines();
        StopCoroutine(EnemyAI.EnemyTurn(EnemyHandCards));
        StopCoroutine(EnemyAI.EnemyTurn(EnemyFieldCards));

        foreach (var card in PlayerHandCards)
            Destroy(card.gameObject);
        foreach (var card in PlayerFieldCards)
            Destroy(card.gameObject);
        foreach (var card in EnemyHandCards)
            Destroy(card.gameObject);
        foreach (var card in EnemyFieldCards)
            Destroy(card.gameObject);

        PlayerHandCards.Clear();
        PlayerFieldCards.Clear();
        EnemyHandCards.Clear();
        EnemyFieldCards.Clear();       
    }

}


