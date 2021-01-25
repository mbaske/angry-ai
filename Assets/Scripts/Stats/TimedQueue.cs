using System.Collections.Generic;
using UnityEngine;

namespace MBaske
{
    public struct TimedQueueItem<T>
    {
        public T Value;
        public float Time;
    }

    public class TimedQueue<T>
    {
        public string Name { get; private set; }
        public int Length => m_Queue.Count;
        public TimedQueueItem<T> First => m_Queue.Peek();
        public TimedQueueItem<T> Last => m_Latest;

        private readonly Queue<TimedQueueItem<T>> m_Queue;
        private TimedQueueItem<T> m_Latest;
        private int m_Capacity;

        public TimedQueue(int initCapacity, string name = "")
        {
            m_Queue = new Queue<TimedQueueItem<T>>(initCapacity);
            m_Capacity = initCapacity;
            Name = name;
        }

        public void Clear()
        {
            m_Queue.Clear();
        }

        public void Add(T value, int newCapacity)
        {
            m_Capacity = newCapacity;
            Add(value);
        }

        public void Add(T value)
        {
            m_Latest = new TimedQueueItem<T>() { Value = value, Time = Time.time };
            m_Queue.Enqueue(m_Latest);
            Prune();
        }

        public IEnumerable<TimedQueueItem<T>> Items()
        {
            foreach (TimedQueueItem<T> item in m_Queue)
            {
                yield return item;
            }
        }

        public IEnumerable<T> Values()
        {
            foreach (TimedQueueItem<T> item in m_Queue)
            {
                yield return item.Value;
            }
        }

        private void Prune()
        {
            while (m_Queue.Count > m_Capacity)
            {
                m_Queue.Dequeue();
            }
        }
    }
}