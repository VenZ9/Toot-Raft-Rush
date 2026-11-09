using System.Collections;
using UnityEngine;
using TootRaftRush.Gameplay;

namespace TootRaftRush.AI
{
    [RequireComponent(typeof(PlayerController))]
    public class AICompanion : MonoBehaviour
    {
        [Header("AI Personality & Settings")]
        [SerializeField] private PlayerController playerController;
        [SerializeField] private RaftController raft;
        [SerializeField] private float decisionInterval = 0.5f;
        [SerializeField] private float targetSpeed = 14f;
        [SerializeField] private float obstacleDetectionRange = 18f;

        [Header("Seat / Home Position")]
        [SerializeField] private Vector3 homeDeckPosition;
        [SerializeField] private float steerOffsetDistance = 0.85f;

        private FartSystem fartSystem;
        private float decisionTimer = 0f;
        private bool isExecutingManeuver = false;

        private void Awake()
        {
            if (playerController == null)
            {
                playerController = GetComponent<PlayerController>();
            }
            fartSystem = playerController.CharacterFartSystem;
        }

        private void Start()
        {
            if (raft == null)
            {
                raft = GetComponentInParent<RaftController>() ?? FindObjectOfType<RaftController>();
            }

            homeDeckPosition = transform.localPosition;
            decisionTimer = Random.Range(0f, decisionInterval);
        }

        private void Update()
        {
            if (Core.GameManager.Instance != null && Core.GameManager.Instance.CurrentState != Core.GameState.Playing)
            {
                playerController.SetInput(Vector2.zero);
                return;
            }

            decisionTimer -= Time.deltaTime;
            if (decisionTimer <= 0f && !isExecutingManeuver)
            {
                decisionTimer = decisionInterval + Random.Range(-0.15f, 0.25f);
                DecideNextAction();
            }
        }

        private void DecideNextAction()
        {
            if (raft == null || fartSystem == null) return;

            // 1. Scan ahead for obstacles requiring evasive steering
            Vector3 raftPos = raft.transform.position;
            Vector3 raftForward = raft.transform.forward;

            RaycastHit hit;
            bool obstacleAhead = Physics.SphereCast(raftPos + Vector3.up * 0.5f, 1.8f, raftForward, out hit, obstacleDetectionRange);

            if (obstacleAhead && (hit.collider.CompareTag("Obstacle") || hit.collider.CompareTag("Hazard")))
            {
                // Determine which side the obstacle is on
                Vector3 toObstacle = hit.point - raftPos;
                float side = Vector3.Dot(raft.transform.right, toObstacle);

                if (side > 0.3f)
                {
                    // Obstacle is on right side -> steer LEFT by moving to right side and farting rightwards
                    StartCoroutine(PerformSteeringManeuver(Vector2.right, -1.0f));
                    return;
                }
                else if (side < -0.3f)
                {
                    // Obstacle is on left side -> steer RIGHT by moving to left side and farting leftwards
                    StartCoroutine(PerformSteeringManeuver(Vector2.left, 1.0f));
                    return;
                }
            }

            // 2. Propel raft if speed is below target
            if (raft.CurrentSpeed < targetSpeed && fartSystem.CanFart)
            {
                // Random stagger among companions (so they don't fart all at once)
                if (Random.value < 0.65f)
                {
                    StartCoroutine(PerformPropulsionFart());
                    return;
                }
            }

            // 3. Return to home deck position
            Vector2 toHome = new Vector2(homeDeckPosition.x - transform.localPosition.x, homeDeckPosition.z - transform.localPosition.z);
            if (toHome.sqrMagnitude > 0.05f)
            {
                playerController.SetInput(toHome.normalized * 0.7f);
            }
            else
            {
                playerController.SetInput(Vector2.zero);
            }
        }

        private IEnumerator PerformPropulsionFart()
        {
            isExecutingManeuver = true;

            // Move to rear of raft (facing forward)
            Vector2 rearInput = new Vector2(0, -1f);
            playerController.SetInput(rearInput);
            yield return new WaitForSeconds(Random.Range(0.2f, 0.4f));

            // Face forward
            playerController.SetInput(new Vector2(0, 0.1f));
            yield return new WaitForSeconds(0.1f);

            // Charge fart
            fartSystem.StartCharging();
            float chargeTime = Random.Range(0.4f, 1.1f);
            yield return new WaitForSeconds(chargeTime);

            // Release!
            fartSystem.ReleaseChargeAndFart();
            yield return new WaitForSeconds(0.25f);

            isExecutingManeuver = false;
        }

        private IEnumerator PerformSteeringManeuver(Vector2 moveSide, float steerDir)
        {
            isExecutingManeuver = true;

            // Move towards side of raft
            playerController.SetInput(moveSide);
            yield return new WaitForSeconds(0.35f);

            if (fartSystem.CanFart)
            {
                fartSystem.StartCharging();
                yield return new WaitForSeconds(0.5f);
                fartSystem.ReleaseChargeAndFart();
            }

            yield return new WaitForSeconds(0.3f);
            isExecutingManeuver = false;
        }
    }
}
