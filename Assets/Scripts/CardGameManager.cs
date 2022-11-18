using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CardGameManager : MonoBehaviour
{
    [SerializeField] private CardHand _playerHand;
    [SerializeField] private CardHand _fishHand;
    [SerializeField] private CardPile _cardPile;
    [SerializeField] private Card _cardPrefab;
    [SerializeField] private float _speed;
    [SerializeField] private List<CardValue> _cards;
    [SerializeField] private SpriteRenderer _table;
    [SerializeField] private float _showMultiplier = 0.01f;
    [SerializeField] private TextMeshProUGUI _resultText;
    [SerializeField] private Button _playAgainButton;
    [SerializeField] private KeyCode _confirmCardKey;
    private static CardGameManager _instance;
    private int _currentCard;
    private int _matAttributeID;
    private Coroutine _changeCardCoroutine;

    private Coroutine _resolveCoroutine;

    public static Action<Card> PlayCard;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        PlayCard += PlayCardFromHand;
    }

    private void OnDisable()
    {
        PlayCard -= PlayCardFromHand;
    }

    private void Start()
    {
        _resultText.enabled = false;
        _playAgainButton.gameObject.SetActive(false);
        _resultText.text = String.Empty;
        StartCoroutine(ShowEverything());
    }

    private void Update()
    {
        if (_playerHand.GetCardAmount() <= 0 || !_playerHand.CanPlay)
            return;

        if (Input.GetKeyDown(_confirmCardKey))
        {
            _playerHand.CardsInHand[_currentCard].BackGround.material.SetInt("_ShowOutline",0);
            _playerHand.PlayCard(_playerHand.CardsInHand[_currentCard]);
            _currentCard = 0;
        }
            
        
        if(_changeCardCoroutine == null)
            _changeCardCoroutine = StartCoroutine(ChangeActiveCard());
    }

    private IEnumerator ChangeActiveCard()
    {
        int att = Shader.PropertyToID("_ShowOutline");
        _playerHand.CardsInHand[_currentCard].BackGround.material.SetInt(att,0);
        _currentCard += (int)Input.GetAxisRaw("Horizontal");

        if (_currentCard < 0)
            _currentCard = _playerHand.GetCardAmount() - 1;
        else if (_currentCard >= _playerHand.GetCardAmount())
            _currentCard = 0;
        
        _playerHand.CardsInHand[_currentCard].BackGround.material.SetInt(att,1);
        
        yield return new WaitForSeconds(0.1f);

        _changeCardCoroutine = null;
    }

    private IEnumerator ShowEverything()
    {
        Material mat = _table.material;
        int index = Shader.PropertyToID("_Fade");
        mat.SetFloat(index,0);
        while (mat.GetFloat(index) < 1)
        {
            mat.SetFloat(index,mat.GetFloat(index) + _showMultiplier);

            yield return null;
        }
        
        for (int i = 0; i < 4; i++)
        {
            _playerHand.AddCardToHand(Instantiate(_cardPrefab,_playerHand.transform));

            yield return new WaitForSeconds(0.2f);
            
            _fishHand.AddCardToHand(Instantiate(_cardPrefab,_fishHand.transform));
            
            yield return new WaitForSeconds(0.2f);

        }
        
        _playerHand.CanPlay = true;
    }

    private void PlayCardFromHand(Card card)
    {
        StartCoroutine(Play(card));
    }

    public CardValue GetRandomCard()
    {
        return _cards[Random.Range(0, _cards.Count)];
    }

    private IEnumerator Play(Card card)
    {
        bool lastPlay = _playerHand.CanPlay;
        
        _playerHand.CanPlay = false;
        _fishHand.CanPlay = false;
        
        Transform cardTransform = card.transform;
        Transform pileTransform = _cardPile.transform;

        cardTransform = cardTransform.GetChild(0);
        
        card.SetSortingOrder(_cardPile.GetCardsAmount() * 2);
        
        while (Vector2.Distance(cardTransform.position,pileTransform.position) > 0.01f)
        {
            cardTransform.position = Vector2.MoveTowards(cardTransform.position,
                                pileTransform.position,
                                Time.deltaTime * _speed);

            yield return null;
        }
        
        _cardPile.AddToPile(card);
        
        if (_fishHand.GetCardAmount() + _playerHand.GetCardAmount() == 0)
        {
            TryResolve();
        }

        _playerHand.CanPlay = !lastPlay;

        if (_playerHand.CanPlay == false)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
            _fishHand.PlayCard(_fishHand.CardsInHand[Random.Range(0,_fishHand.GetCardAmount())]);
        }
    }
    
    private void TryResolve()
    {
        StartCoroutine(_cardPile.Resolve());
    }

    public void AfterResolve()
    {
        if (_fishHand.Hp < _playerHand.Hp)
        {
            StartCoroutine(ShowText("You win"));
        }
        else
        {
            StartCoroutine(ShowText("You lose"));
        }
    }

    private IEnumerator ShowText(string text)
    {
        _resultText.enabled = true;
        
        for (int i = 0; i < text.Length; i++)
        {
            _resultText.text += text[i];

            yield return new WaitForSeconds(0.1f);
        }

        _playAgainButton.gameObject.SetActive(true);
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(0);
    }

    public static CardGameManager Instance => _instance;

    public CardHand PlayerHand => _playerHand;
    public CardHand FishHand => _fishHand;
}
