using System.Collections;
using UnityEngine;

namespace TootRaftRush.Utility
{
    public class CharacterVisualController : MonoBehaviour
    {
        [Header("Squash & Stretch Transforms")]
        [SerializeField] private Transform visualRoot;
        [SerializeField] private Transform characterModel;

        [Header("Animation Parameters")]
        [SerializeField] private float idleBounceSpeed = 3.2f;
        [SerializeField] private float idleBounceAmount = 0.04f;
        [SerializeField] private float walkWaddleSpeed = 10f;
        [SerializeField] private float walkWaddleAngle = 9f;

        private Vector3 initialScale;
        private Vector3 initialLocalPos;
        private Quaternion initialLocalRot;

        private bool isWalking = false;
        private bool isWipedOut = false;
        private Coroutine activeSquashRoutine;

        private void Awake()
        {
            if (visualRoot == null)
            {
                visualRoot = transform;
            }
            if (characterModel == null && visualRoot.childCount > 0)
            {
                characterModel = visualRoot.GetChild(0);
            }

            initialScale = visualRoot.localScale;
            initialLocalPos = visualRoot.localPosition;
            initialLocalRot = visualRoot.localRotation;
        }

        private void Update()
        {
            if (isWipedOut) return;

            float t = Time.time;

            if (isWalking)
            {
                // Walking waddle
                float waddle = Mathf.Sin(t * walkWaddleSpeed) * walkWaddleAngle;
                visualRoot.localRotation = initialLocalRot * Quaternion.Euler(0, 0, waddle);
                float stepBounce = Mathf.Abs(Mathf.Sin(t * walkWaddleSpeed)) * 0.08f;
                visualRoot.localPosition = initialLocalPos + Vector3.up * stepBounce;
            }
            else
            {
                // Idle breathing bounce
                float bounce = Mathf.Sin(t * idleBounceSpeed) * idleBounceAmount;
                visualRoot.localPosition = initialLocalPos + Vector3.up * bounce;
                visualRoot.localRotation = initialLocalRot;
            }
        }

        public void SetWalking(bool walking)
        {
            isWalking = walking;
            if (!walking && !isWipedOut)
            {
                visualRoot.localRotation = initialLocalRot;
            }
        }

        public void TriggerJumpSquash()
        {
            if (activeSquashRoutine != null) StopCoroutine(activeSquashRoutine);
            activeSquashRoutine = StartCoroutine(SquashStretchRoutine(new Vector3(0.8f, 1.3f, 0.8f), 0.15f));
        }

        public void TriggerLandSquash()
        {
            if (activeSquashRoutine != null) StopCoroutine(activeSquashRoutine);
            activeSquashRoutine = StartCoroutine(SquashStretchRoutine(new Vector3(1.35f, 0.7f, 1.35f), 0.2f));
        }

        public void TriggerFartBurst(float chargeRatio)
        {
            if (activeSquashRoutine != null) StopCoroutine(activeSquashRoutine);
            float stretch = Mathf.Lerp(1.25f, 1.6f, chargeRatio);
            float squash = Mathf.Lerp(0.8f, 0.6f, chargeRatio);
            activeSquashRoutine = StartCoroutine(FartBurstRoutine(squash, stretch));
        }

        private IEnumerator FartBurstRoutine(float squash, float stretch)
        {
            // Initial squash recoil
            visualRoot.localScale = new Vector3(initialScale.x / squash, initialScale.y * squash, initialScale.z / squash);
            yield return new WaitForSeconds(0.08f);

            // Explosive forward stretch
            visualRoot.localScale = new Vector3(initialScale.x / stretch, initialScale.y * stretch, initialScale.z * stretch);
            yield return new WaitForSeconds(0.18f);

            // Return to normal with smooth decay
            float elapsed = 0f;
            Vector3 current = visualRoot.localScale;
            while (elapsed < 0.2f)
            {
                elapsed += Time.deltaTime;
                visualRoot.localScale = Vector3.Lerp(current, initialScale, elapsed / 0.2f);
                yield return null;
            }
            visualRoot.localScale = initialScale;
        }

        private IEnumerator SquashStretchRoutine(Vector3 targetScaleMult, float duration)
        {
            Vector3 target = new Vector3(initialScale.x * targetScaleMult.x, initialScale.y * targetScaleMult.y, initialScale.z * targetScaleMult.z);
            float half = duration * 0.5f;

            float elapsed = 0f;
            while (elapsed < half)
            {
                elapsed += Time.deltaTime;
                visualRoot.localScale = Vector3.Lerp(initialScale, target, elapsed / half);
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < half)
            {
                elapsed += Time.deltaTime;
                visualRoot.localScale = Vector3.Lerp(target, initialScale, elapsed / half);
                yield return null;
            }
            visualRoot.localScale = initialScale;
        }

        public void SetWipedOut(bool wipedOut)
        {
            isWipedOut = wipedOut;
            if (wipedOut)
            {
                // Comedic upside down tumble
                visualRoot.localRotation = Quaternion.Euler(180f, Random.Range(-45f, 45f), 0);
                visualRoot.localPosition = initialLocalPos + Vector3.up * 0.35f;
            }
            else
            {
                ResetVisuals();
            }
        }

        public void ResetVisuals()
        {
            isWipedOut = false;
            isWalking = false;
            visualRoot.localScale = initialScale;
            visualRoot.localPosition = initialLocalPos;
            visualRoot.localRotation = initialLocalRot;
        }
    }
}
