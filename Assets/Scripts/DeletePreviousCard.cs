using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(menuName = "DeleteCard")]
public class DeletePreviousCard : CardValue
{
    public override async Task ResolveThis(CardHand owner, CardHand enemy, List<Card> cards)
    {
        if (cards.Count - 2 < 0)
            return;

        Card toDelete = cards[cards.Count - 2];

        toDelete.UnUsable = true;
        toDelete.UnUsableTask = Unusable(toDelete);

        await Task.Run(() => Thread.Sleep(_delay));
    }

    private async Task Unusable(Card card)
    {
        Transform tr = card.transform;
        GameObject eff = Instantiate(_effectObject, tr.position, quaternion.identity, tr);
        eff.GetComponent<Animator>().SetTrigger("Show");
        
        await Task.Run(() => Thread.Sleep(_delay));
    }
}
