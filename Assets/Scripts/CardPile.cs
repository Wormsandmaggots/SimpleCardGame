using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class CardPile : MonoBehaviour
{
    private List<Card> _cards = new List<Card>();
    private List<CardValue> _values = new List<CardValue>();

    public void AddToPile(Card card)
    {
        _cards.Add(card);
        _values.Add(card.Value);
        
        Transform cardTransform = card.transform;

        cardTransform.SetParent(transform);
        cardTransform.localPosition = Vector3.zero;
        cardTransform.GetChild(0).localPosition = Vector3.zero;
    }

    public IEnumerator Resolve()
    {
        int index = _cards.Count - 1;
        for (int i = index; i >= 0; i--)
        {
            Task t1 = ResolveOne(i);
            yield return new WaitUntil(() => t1.IsCompleted);
        }
        
        CardGameManager.Instance.AfterResolve();
    }

    private async Task ResolveOne(int index)
    {
        Rigidbody2D rb = _cards[index].GetComponent<Rigidbody2D>();

        Task t1;
        
        if(_cards[index].Hand == CardGameManager.Instance.PlayerHand)
            t1 = _cards[index].ResolveThis(_cards[index].Hand,CardGameManager.Instance.FishHand,_cards);
        else
            t1 = _cards[index].ResolveThis(_cards[index].Hand,CardGameManager.Instance.PlayerHand,_cards);

        await t1;

        if (_cards[index].ShouldWait)
        {
            await Task.Run(() => Thread.Sleep(500));
        }

        rb.velocity = GetRandomVelocity() * Random.Range(5f,8f);

        await Task.Run(() => Thread.Sleep(500));

        rb.velocity = Vector2.zero;
        _cards.RemoveAt(index);
        _values.RemoveAt(index);
    }

    private Vector2 GetRandomVelocity()
    {
        Vector2 vec = new Vector2(Random.Range(-1f,1f),Random.Range(-1f,1f));

        return vec.normalized;
    }

    public int GetCardsAmount()
    {
        return _cards.Count;
    }
}
