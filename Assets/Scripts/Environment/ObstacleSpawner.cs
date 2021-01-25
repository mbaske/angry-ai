using UnityEngine;
using System.Collections;

namespace MBaske
{
    public class ObstacleSpawner : MonoBehaviour
    {
        [SerializeField]
        private float m_Scale = 0.8f;
        [SerializeField]
        private float m_Mass = 1000;
        [SerializeField]
        private float m_SpawnRadius = 20;
        [SerializeField]
        private float m_ClearRadius = 5;

        [SerializeField]
        private GameObject[] m_Prefabs;
        private GameObject m_Container;
        private SpawnPositionFinder m_Finder;

        public void Spawn(float delay = 0)
        {
            if (m_Container != null)
            {
                Destroy(m_Container);
            }

            StartCoroutine(SpawnDelayed(delay));
        }

        private IEnumerator SpawnDelayed(float delay = 0)
        {
            yield return new WaitForSeconds(delay);

            if (m_Finder == null)
            {
                m_Finder = new SpawnPositionFinder(m_SpawnRadius, m_ClearRadius);
            }

            m_Container = new GameObject("Obstacles");
            m_Container.transform.parent = transform;

            int i = -1;
            while (m_Finder.HasFreePosition(out Vector3 point))
            {
                var obj = Instantiate(
                    m_Prefabs[++i % m_Prefabs.Length], 
                    point + Vector3.up, 
                    Quaternion.AngleAxis(Random.value * 360, Vector3.up),
                    m_Container.transform);

                obj.transform.localScale = Vector3.one * m_Scale;
                obj.GetComponent<Rigidbody>().mass = m_Mass;
            }
        }
    }
}