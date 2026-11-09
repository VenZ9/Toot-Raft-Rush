using System.Collections;
using UnityEngine;

namespace TootRaftRush.Obstacles
{
    public class ObstacleMove : MonoBehaviour
    {
        [Header("Sliding Gate Settings")]
        [SerializeField] private Vector3 moveOffset = new Vector3(3.2f, 0, 0);
        [SerializeField] private float travelDuration = 2.0f;
        [SerializeField] private float pauseAtEndpoints = 0.5f;

        [Header("Components")]
        [SerializeField] private Transform slidingDoor;

        private Vector3 startLocalPos;
        private Vector3 endLocalPos;

        private void Start()
        {
            if (slidingDoor == null)
            {
                slidingDoor = transform;
            }

            startLocalPos = slidingDoor.localPosition;
            endLocalPos = startLocalPos + moveOffset;

            StartCoroutine(MovementLoop());
        }

        private IEnumerator MovementLoop()
        {
            while (true)
            {
                // Move from start to end
                yield return MoveBetweenPoints(startLocalPos, endLocalPos);
                yield return new WaitForSeconds(pauseAtEndpoints);

                // Move from end to start
                yield return MoveBetweenPoints(endLocalPos, startLocalPos);
                yield return new WaitForSeconds(pauseAtEndpoints);
            }
        }

        private IEnumerator MoveBetweenPoints(Vector3 from, Vector3 to)
        {
            float elapsed = 0f;
            while (elapsed < travelDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / travelDuration);
                // Smooth step easing
                t = t * t * (3f - 2f * t);
                slidingDoor.localPosition = Vector3.Lerp(from, to, t);
                yield return null;
            }
            slidingDoor.localPosition = to;
        }
    }
}
