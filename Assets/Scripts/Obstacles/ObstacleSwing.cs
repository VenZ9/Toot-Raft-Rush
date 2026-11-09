using UnityEngine;

namespace TootRaftRush.Obstacles
{
    public class ObstacleSwing : MonoBehaviour
    {
        [Header("Pendulum Motion Settings")]
        [SerializeField] private float maxAngle = 55f;
        [SerializeField] private float frequency = 1.1f;
        [SerializeField] private float phaseOffset = 0f;
        [SerializeField] private Vector3 rotationAxis = Vector3.forward;

        [Header("Components")]
        [SerializeField] private Transform swingingArm;

        private Quaternion baseRotation;
        private float timer = 0f;

        private void Start()
        {
            if (swingingArm == null)
            {
                swingingArm = transform;
            }
            baseRotation = swingingArm.localRotation;
            timer = phaseOffset;
        }

        private void Update()
        {
            timer += Time.deltaTime * frequency * 2 * Mathf.PI;
            float currentAngle = Mathf.Sin(timer) * maxAngle;
            swingingArm.localRotation = baseRotation * Quaternion.AngleAxis(currentAngle, rotationAxis);
        }
    }
}
