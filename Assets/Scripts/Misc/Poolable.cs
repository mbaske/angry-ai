using System;
using System.Collections;
using UnityEngine;

public abstract class Poolable : MonoBehaviour
{
    public event Action<Poolable> DiscardEvent;

    public int Index { get; set; }

    [SerializeField]
    protected float m_Lifetime = 1;

    private IEnumerator m_Discard;

    public virtual void OnSpawn()
    {
        if (m_Lifetime > 0)
        {
            DiscardAfter(m_Lifetime);
        }
    }

    protected virtual void OnDiscard()
    {
        DiscardEvent.Invoke(this);
    }

    protected void DiscardAfter(float secs)
    {
        if (m_Discard != null)
        {
            StopCoroutine(m_Discard);
        }
        
        m_Discard = Discard(secs);
        StartCoroutine(m_Discard);
    }

    private IEnumerator Discard(float secs = 0)
    {
        yield return new WaitForSeconds(secs);
        OnDiscard();
    }
}
