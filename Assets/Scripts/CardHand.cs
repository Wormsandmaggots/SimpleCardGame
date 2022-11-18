using System.Collections.Generic;
using UnityEngine;

public class CardHand : MonoBehaviour
{
    private List<Card> _cardsInHand = new List<Card>();
    private float _angle = 15;
    [SerializeField] private float _highlightMultiplier = 0.1f;
    private bool _canPlay;
    private int _hp = 10;

    public void AddCardToHand(Card card)
    {
        _cardsInHand.Add(card);
        UpdateHand();

        card.Value = CardGameManager.Instance.GetRandomCard();
        card.IsInHand = true;
        card.HighlightMultiplier = _highlightMultiplier;

        for (int i = 0; i < _cardsInHand.Count; i++)
        {
            _cardsInHand[i].SetSortingOrder(i * 2);
        }
    }

    private void UpdateHand()
    {
        float currentAngle = (_cardsInHand.Count - 1) * _angle;
        foreach (Card cardInHand in _cardsInHand)
        {
            cardInHand.transform.eulerAngles = new Vector3(0, 0, currentAngle);
            currentAngle -= _angle * 2;
        }
    }

    public void UpdateHp(int value)
    {
        _hp += value;
    }

    public void PlayCard(Card card)
    {
        card.IsInHand = false;
        _cardsInHand.Remove(card);
        UpdateHand();
        CardGameManager.PlayCard?.Invoke(card);
    }

    public int GetCardAmount()
    {
        return _cardsInHand.Count;
    }

    public List<Card> CardsInHand => _cardsInHand;

    public bool CanPlay
    {
        get => _canPlay;
        set => _canPlay = value;
    }

    public int Hp => _hp;
}
