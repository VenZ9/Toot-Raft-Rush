using UnityEngine;
using UnityEngine.EventSystems;

namespace TootRaftRush.UI
{
    public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public static VirtualJoystick Instance { get; private set; }

        [Header("Components")]
        [SerializeField] private RectTransform backgroundRect;
        [SerializeField] private RectTransform handleRect;

        [Header("Settings")]
        [SerializeField] private float handleRange = 64f;
        [SerializeField] private float deadZone = 0.05f;

        private Vector2 inputDirection = Vector2.zero;
        public Vector2 InputDirection => inputDirection;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            if (backgroundRect == null)
            {
                backgroundRect = GetComponent<RectTransform>();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 position = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, backgroundRect.position);
            Vector2 radius = backgroundRect.sizeDelta / 2f;
            Vector2 rawInput = (eventData.position - position) / (radius * backgroundRect.lossyScale);

            inputDirection = rawInput.magnitude > 1f ? rawInput.normalized : rawInput;

            if (inputDirection.magnitude < deadZone)
            {
                inputDirection = Vector2.zero;
            }

            if (handleRect != null)
            {
                handleRect.anchoredPosition = inputDirection * handleRange;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            inputDirection = Vector2.zero;
            if (handleRect != null)
            {
                handleRect.anchoredPosition = Vector2.zero;
            }
        }
    }
}
