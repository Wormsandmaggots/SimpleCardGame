using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Card")]
public class CardValue : ScriptableObject
{
    public Sprite CardSprite;
    [SerializeField] protected int _value;
    [SerializeField] protected GameObject _effectObject;
    protected int _delay = 500;

    public virtual async Task ResolveThis(CardHand owner, CardHand enemy,List<Card> cards)
    {
        enemy.UpdateHp(-_value);

        bool isFish = CardGameManager.Instance.FishHand == enemy;

        for (int i = 0; i < _value; i++)
        {
            GameObject effect = Instantiate(_effectObject, enemy.transform.position, quaternion.identity, enemy.transform);

            effect.transform.position += new Vector3(Random.Range(-1f, 1f),
                isFish ? Random.Range(-1.5f, -0.5f) : Random.Range(0.5f,1.5f), 0);
            
            await Task.Run(() => Thread.Sleep(_delay));
            Destroy(effect);
        }
        
    }
}
