using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using System;

namespace MBaske
{
    public class WalkerControls : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        public event Action WalkUpdateEvent;
        public event Action LookUpdateEvent;

        private enum Mode
        {
            Active, Passive
        }
        [SerializeField]
        private Mode m_Mode;
        [SerializeField]
        private WalkerAgent m_Walker;

        [SerializeField]
        private Image m_BGSprite;
        [SerializeField]
        private Image m_WalkSprite;
        [SerializeField]
        private Image m_LookSprite;

        private float m_WalkMagnitude;
        private Vector2 m_WalkVector = Vector2.up;
        private Vector2 m_LookVector = Vector2.up;

        private bool m_UpdateWalk;

        public float NormSpeed
        {
            get { return m_WalkMagnitude * 2 - 1; }
            set
            {
                m_WalkMagnitude = (value + 1) * 0.5f;
                UpdateWalkSprite();
            }
        }

        public float NormWalkAngle
        {
            get { return Vector2.SignedAngle(m_WalkVector, Vector2.up) / 180f; }
            set
            {
                m_WalkVector.x = Mathf.Sin(value * 180 * Mathf.Deg2Rad);
                m_WalkVector.y = Mathf.Cos(value * 180 * Mathf.Deg2Rad);
                UpdateWalkSprite();
            }
        }

        public float NormLookAngle
        {
            get { return Vector2.SignedAngle(m_LookVector, Vector2.up) / 180f; }
            set
            {
                m_LookVector.x = Mathf.Sin(value * 180 * Mathf.Deg2Rad);
                m_LookVector.y = Mathf.Cos(value * 180 * Mathf.Deg2Rad);
                UpdateLookSprite();
            }
        }

        private void Awake()
        {
            if (m_Mode == Mode.Passive && m_Walker == null)
            {
                m_Walker = FindObjectOfType<WalkerAgent>();
            }
        }

        private void Update()
        {
            if (m_Mode == Mode.Passive)
            {
                NormSpeed = m_Walker.NormTargetSpeed;
                NormWalkAngle = m_Walker.NormTargetWalkAngle;
                NormLookAngle = m_Walker.NormTargetLookAngle;
                UpdateWalkSprite();
                UpdateLookSprite();
            }
        }

        public void OnPointerDown(PointerEventData e)
        {
            if (m_Mode == Mode.Active)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(
                    m_WalkSprite.rectTransform, e.position, e.pressEventCamera))
                {
                    m_UpdateWalk = true;
                    OnDrag(e);
                }
                else if (RectTransformUtility.RectangleContainsScreenPoint(
                    m_LookSprite.rectTransform, e.position, e.pressEventCamera))
                {
                    m_UpdateWalk = false;
                    OnDrag(e);
                }
            }
        }

        public void OnDrag(PointerEventData e)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                m_BGSprite.rectTransform, e.position, e.pressEventCamera, out Vector2 p))
            {
                var d = m_BGSprite.rectTransform.sizeDelta * 0.5f;
                p.x += d.x;
                p.y -= d.y;

                if (m_UpdateWalk)
                {
                    p /= 0.7f;
                    p.x /= d.x;
                    p.y /= d.y;

                    m_WalkMagnitude = Mathf.Clamp01(p.magnitude);
                    m_WalkVector = p.normalized;
                    WalkUpdateEvent?.Invoke();
                    UpdateWalkSprite();
                }
                else
                {
                    m_LookVector = p.normalized;
                    LookUpdateEvent?.Invoke();
                    UpdateLookSprite();
                }
            }
        }

        private void UpdateWalkSprite()
        {
            var d = m_BGSprite.rectTransform.sizeDelta * 0.35f;
            m_WalkSprite.rectTransform.anchoredPosition = Vector3.Scale(m_WalkVector * m_WalkMagnitude, d);
        }

        private void UpdateLookSprite()
        {
            var d = m_BGSprite.rectTransform.sizeDelta * 0.5f;
            m_LookSprite.rectTransform.anchoredPosition = Vector3.Scale(m_LookVector, d);
        }
    }
}