using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    private Queue<Gold> _golds;

    private void Start()
    {
        _golds = new Queue<Gold>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Gold gold))
        {
            _golds.Enqueue(gold);
        }
    }

    public Gold TryGetNextGold()
    {
        if (_golds.Count > 0)
        {
            return _golds.Dequeue();
        }

        return null;
    }
}
