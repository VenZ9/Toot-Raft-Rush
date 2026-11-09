using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TootRaftRush.UI
{
    public class FartButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Visuals")]
        [SerializeField] private Image chargeRadialFill;
        [SerializeField] private Transform buttonTransform;
        [SerializeField] private float pressScale = 0.92f;

        private bool isHeld = false;
        private float holdTimer = 0f;
        private const float MaxCharge = 1.4f;

        public bool IsHeld => isHeld;

        private void Start()
        {
            if (buttonTransform == null)
            {
                buttonTransform = transform;
            }
        }

        private void Update()
        {
            if (isHeld)
            {
                holdTimer += Time.deltaTime;
                float ratio = Mathf.Clamp01(holdTimer / MaxCharge);
                if (chargeRadialFill != null)
                {
                    chargeRadialFill.fillAmount = ratio;
                }

                // Button wobble animation while charging
                float wobble = Mathf.Sin(Time.time * 24f) * 0.04f * ratio;
                buttonTransform.localScale = Vector3.one * (pressScale + wobble);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isHeld = true;
            holdTimer = 0f;
            if (chargeRadialFill != null)
            {
                chargeRadialFill.fillAmount = 0f;
            }
            if (buttonTransform != null)
            {
                buttonTransform.localScale = Vector3.one * pressScale;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isHeld = false;
            holdTimer = 0f;
            if (chargeRadialFill != null)
            {
                chargeRadialFill.fillAmount = 0f;
            }
            if (buttonTransform != null)
            {
                buttonTransform.localScale = Vector3.one;
            }

            if (UIManager.Instance != null)
            {
                UIManager.Instance.SignalFartButtonReleased();
            }
        }
    }
}
