using System.Collections.Generic;
using UnityEngine;

public abstract class Pool<T> : MonoBehaviour where T : Poolable
{
    [SerializeField]
    protected bool m_EnableSpawn = true;

    [SerializeField]
    protected T[] m_Prefabs;
    protected Stack<Poolable>[] m_Stacks;

    private void Awake()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        int n = m_Prefabs.Length;
        m_Stacks = new Stack<Poolable>[n];

        for (int i = 0; i < n; i++)
        {
            m_Stacks[i] = new Stack<Poolable>();
        }
    }

    protected Poolable Spawn(Vector3 pos, int index = 0)
    {
        Poolable obj;

        if (m_Stacks[index].Count > 0)
        {
            obj = m_Stacks[index].Pop();
            obj.gameObject.SetActive(true);
        }
        else
        {
            obj = Instantiate(m_Prefabs[index], transform);
            obj.DiscardEvent += OnDiscard;
            obj.Index = index;
        }

        obj.transform.position = pos;
        obj.OnSpawn();

        return obj;
    }

    protected void OnDiscard(Poolable obj)
    {
        obj.gameObject.SetActive(false);
        m_Stacks[obj.Index].Push(obj);
    }
}
