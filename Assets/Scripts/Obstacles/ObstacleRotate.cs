using UnityEngine;

namespace TootRaftRush.Obstacles
{
    public class ObstacleRotate : MonoBehaviour
    {
        [Header("Rotation Settings")]
        [SerializeField] private float rotationSpeed = 120f; // degrees per sec
        [SerializeField] private Vector3 rotationAxis = Vector3.right;
        [SerializeField] private bool clockwise = true;

        [Header("Components")]
        [SerializeField] private Transform rotatingPart;

        private void Start()
        {
            if (rotatingPart == null)
            {
                rotatingPart = transform;
            }
        }

        private void Update()
        {
            float dir = clockwise ? 1f : -1f;
            rotatingPart.Rotate(rotationAxis * (rotationSpeed * dir * Time.deltaTime), Space.Self);
        }
    }
}
