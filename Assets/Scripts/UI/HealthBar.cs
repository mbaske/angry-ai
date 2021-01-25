using UnityEngine.UI;
using UnityEngine;
using System.Collections;

namespace MBaske
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField]
        private Gradient m_Gradient;
        [SerializeField]
        private float m_MaxDistance = 20;
        [SerializeField]
        private float m_Offset = 40;
        [SerializeField]
        private float m_DimSpeed = 4;
        [SerializeField]
        private Image m_BGSprite;
        [SerializeField]
        private Image m_BarSprite;

        private Camera cam;
        private float m_Opacity;
        private Vector2 m_SizeRect;
        private Vector2 m_OffsetRect;

        private void Awake()
        {
            m_SizeRect = m_BGSprite.rectTransform.sizeDelta;
            m_OffsetRect = new Vector2(-m_SizeRect.x * 0.5f, m_Offset);
            cam = Camera.main;
            UpdatOpacity();
        }

        public void OnBulletHitSuffered(FighterAgent agent)
        {
            var pos = cam.WorldToScreenPoint(agent.WorldPosition);
            if (pos.z > 0 && pos.z <= m_MaxDistance)
            {
                m_Opacity = 1;
                UpdatOpacity();
                SetValue(agent.Health);
                StopAllCoroutines();
                StartCoroutine(Dim(agent));
            }
        }

        protected IEnumerator Dim(FighterAgent agent)
        {
            SetPosition(cam.WorldToScreenPoint(agent.WorldPosition));

            float dt = Time.deltaTime;
            yield return new WaitForSeconds(dt);

            m_Opacity = Mathf.Max(0, m_Opacity - dt * m_DimSpeed);
            UpdatOpacity();

            if (m_Opacity > 0)
            {
                StartCoroutine(Dim(agent));
            }
        }

        private void SetValue(float norm)
        {
            m_BarSprite.rectTransform.sizeDelta = Vector2.Scale(m_SizeRect, new Vector2(norm, 1));
            m_BarSprite.color = m_Gradient.Evaluate(norm);
        }

        private void SetPosition(Vector2 pos)
        {
            pos += m_OffsetRect;
            m_BGSprite.rectTransform.anchoredPosition = pos;
            m_BarSprite.rectTransform.anchoredPosition = pos;
        }

        private void UpdatOpacity()
        {
            var color = m_BGSprite.color;
            color.a = m_Opacity;
            m_BGSprite.color = color;

            color = m_BarSprite.color;
            color.a = m_Opacity;
            m_BarSprite.color = color;
        }
    }
}