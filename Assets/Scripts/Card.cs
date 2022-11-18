using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Card : MonoBehaviour
{
    private CardHand _hand;
    private float _highlightMultiplier;
    private bool _isInHand;
    private CardValue _value;
    [SerializeField] private SpriteRenderer _backGround;
    [SerializeField] private SpriteRenderer _display;
    private bool _unUsable;
    private bool _shouldWait;
    private Task _unUsableTask;

    private void Start()
    {
        _hand = GetComponentInParent<CardHand>(true);
        
        _backGround.material.SetInt("_ShowOutline",0);
    }

    private void OnMouseEnter()
    {
        if(_isInHand && _hand.CanPlay)
            transform.position += Vector3.up * _highlightMultiplier;
    }

    private void OnMouseExit()
    {
        if(_isInHand && _hand.CanPlay)
            transform.position -= Vector3.up * _highlightMultiplier;
    }

    private void OnMouseDown()
    {
        if(_isInHand && _hand.CanPlay)
            _hand.PlayCard(this);
    }

    public async Task ResolveThis(CardHand owner, CardHand enemy,List<Card> cards)
    {
        Task t1;

        if (_unUsable)
        {
            t1 = _unUsableTask;
        }
        else
            t1 = _value.ResolveThis(owner, enemy, cards);

        await t1;
    }

    public void SetSortingOrder(int backGroundOrder)
    {
        _backGround.sortingOrder = backGroundOrder;
        _display.sortingOrder = backGroundOrder + 1;
    }

    public CardValue Value
    {
        get => _value;
        set
        {
            _value = value;
            _display.sprite = value.CardSprite;
        }
    }

    public CardHand Hand => _hand;

    public bool IsInHand
    {
        get => _isInHand;
        set => _isInHand = value;
    }

    public float HighlightMultiplier
    {
        get => _highlightMultiplier;
        set => _highlightMultiplier = value;
    }

    public bool UnUsable
    {
        get => _unUsable;
        set
        {
            _shouldWait = value;
            _unUsable = value;
        }
    }

    public SpriteRenderer BackGround => _backGround;

    public bool ShouldWait
    {
        get => _shouldWait;
        set => _shouldWait = value;
    }

    public Task UnUsableTask
    {
        get => _unUsableTask;
        set => _unUsableTask = value;
    }
}
