using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "HealCard")]
public class HealCard : CardValue
{
    public override async Task ResolveThis(CardHand owner, CardHand enemy, List<Card> cards)
    {
        int heal = _value;

        enemy.UpdateHp(heal);

        bool isFish = CardGameManager.Instance.FishHand == owner;

        for (int i = 0; i < _value; i++)
        {
            GameObject effect = Instantiate(_effectObject, owner.transform.position, quaternion.identity, owner.transform);

            effect.transform.position += new Vector3(Random.Range(-1f, 1f),
                isFish ? Random.Range(-1.5f, -1f) : Random.Range(1f,1.5f), 0);
            
            effect.GetComponent<Animator>().SetTrigger("Show");
            
            await Task.Run(() => Thread.Sleep(_delay * 2));
            Destroy(effect);
        }
    }
}
