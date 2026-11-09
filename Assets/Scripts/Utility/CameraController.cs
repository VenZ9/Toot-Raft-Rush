using UnityEngine;

namespace TootRaftRush
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance { get; private set; }

        [Header("Target & Follow")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0, 4.2f, -6.5f);
        [SerializeField] private float positionSmoothTime = 0.12f;
        [SerializeField] private float rotationSmoothTime = 0.15f;

        [Header("Camera & FOV")]
        [SerializeField] private Camera cam;
        [SerializeField] private float defaultFov = 60f;
        [SerializeField] private float maxFov = 74f;
        [SerializeField] private float fovDecaySpeed = 4.0f;

        private Vector3 currentVelocity;
        private float currentRotVelocity;
        private float targetFov;

        // Screen Shake
        private float shakeIntensity = 0f;
        private float shakeDuration = 0f;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            if (cam == null)
            {
                cam = GetComponent<Camera>() ?? Camera.main;
            }

            targetFov = defaultFov;
            if (cam != null)
            {
                cam.fieldOfView = defaultFov;
            }
        }

        private void Start()
        {
            if (target == null)
            {
                var raft = FindObjectOfType<Gameplay.RaftController>();
                if (raft != null)
                {
                    target = raft.transform;
                }
            }
        }

        private void LateUpdate()
        {
            if (target == null) return;

            // Follow target position rotated by target heading
            Vector3 desiredPosition = target.position + target.TransformDirection(offset);

            // Screen shake displacement
            Vector3 shakeOffset = Vector3.zero;
            if (shakeDuration > 0f)
            {
                shakeDuration -= Time.deltaTime;
                shakeOffset = Random.insideUnitSphere * shakeIntensity;
            }

            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition + shakeOffset, ref currentVelocity, positionSmoothTime);

            // Look at slightly above raft
            Vector3 lookTarget = target.position + Vector3.up * 1.0f;
            Quaternion targetRotation = Quaternion.LookRotation(lookTarget - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 8.0f);

            // Dynamic FOV kick decay
            if (cam != null)
            {
                targetFov = Mathf.MoveTowards(targetFov, defaultFov, Time.deltaTime * fovDecaySpeed);
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, Time.deltaTime * 6.0f);
            }
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        public void TriggerShake(float intensity, float duration)
        {
            shakeIntensity = intensity;
            shakeDuration = duration;
        }

        public void TriggerFovKick(float amount)
        {
            targetFov = Mathf.Min(maxFov, targetFov + amount);
        }
    }
}
