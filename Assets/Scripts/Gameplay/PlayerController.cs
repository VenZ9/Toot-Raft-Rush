using System;
using UnityEngine;

namespace TootRaftRush.Gameplay
{
    public enum AnimalType
    {
        Hippo,
        Rhino,
        Frog,
        Capybara
    }

    [RequireComponent(typeof(FartSystem))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Character Identity")]
        [SerializeField] private AnimalType animalType = AnimalType.Hippo;
        [SerializeField] private bool isHumanPlayer = false;
        [SerializeField] private int playerIndex = 0;
        public AnimalType CharacterAnimalType => animalType;
        public bool IsHumanPlayer => isHumanPlayer;
        public int PlayerIndex => playerIndex;

        [Header("Movement On Raft")]
        [SerializeField] private float moveSpeed = 4.5f;
        [SerializeField] private float jumpForce = 5.2f;
        [SerializeField] private float gravity = 18f;
        [SerializeField] private RaftController raft;

        [Header("Visuals & Animation")]
        [SerializeField] private Transform modelRoot;
        [SerializeField] private Animator animator;
        [SerializeField] private Utility.CharacterVisualController visualController;

        [Header("Fart System")]
        [SerializeField] private FartSystem fartSystem;
        public FartSystem CharacterFartSystem => fartSystem;

        private Vector3 localDeckPosition;
        private float verticalVelocity = 0f;
        private bool isGrounded = true;
        private Vector2 movementInput = Vector2.zero;
        private bool isWipedOut = false;

        public Vector3 WorldFartPosition => transform.position + transform.forward * -0.3f + Vector3.up * 0.2f;
        public Vector3 FartImpulseDirection => transform.forward; // Fart pushes character forward, pushing raft forward

        private void Awake()
        {
            if (fartSystem == null)
            {
                fartSystem = GetComponent<FartSystem>();
            }
            if (visualController == null)
            {
                visualController = GetComponentInChildren<Utility.CharacterVisualController>();
            }
        }

        private void Start()
        {
            if (raft == null)
            {
                raft = GetComponentInParent<RaftController>() ?? FindObjectOfType<RaftController>();
            }

            localDeckPosition = transform.localPosition;
        }

        private void Update()
        {
            if (isWipedOut) return;

            if (isHumanPlayer)
            {
                HandleHumanInput();
            }

            ApplyMovement();
        }

        private void HandleHumanInput()
        {
            // Read virtual joystick or fallback to keyboard axes
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            // Also check VirtualJoystick if active
            if (UI.VirtualJoystick.Instance != null && UI.VirtualJoystick.Instance.InputDirection.sqrMagnitude > 0.01f)
            {
                h = UI.VirtualJoystick.Instance.InputDirection.x;
                v = UI.VirtualJoystick.Instance.InputDirection.y;
            }

            movementInput = new Vector2(h, v);

            // Jump
            if (Input.GetButtonDown("Jump") || (UI.UIManager.Instance != null && UI.UIManager.Instance.ConsumeJumpPressed()))
            {
                Jump();
            }

            // Fart controls are handled via FartButton or Space/F key
            bool chargeButtonHeld = Input.GetKey(KeyCode.F) || (UI.UIManager.Instance != null && UI.UIManager.Instance.IsFartButtonHeld);
            bool chargeButtonUp = Input.GetKeyUp(KeyCode.F) || (UI.UIManager.Instance != null && UI.UIManager.Instance.ConsumeFartButtonReleased());

            if (chargeButtonHeld)
            {
                fartSystem.StartCharging();
            }
            if (chargeButtonUp)
            {
                fartSystem.ReleaseChargeAndFart();
            }
        }

        public void SetInput(Vector2 input)
        {
            movementInput = input;
        }

        public void Jump()
        {
            if (isGrounded)
            {
                verticalVelocity = jumpForce;
                isGrounded = false;
                if (visualController != null)
                {
                    visualController.TriggerJumpSquash();
                }
            }
        }

        private void ApplyMovement()
        {
            float dt = Time.deltaTime;

            // Horizontal deck motion relative to raft orientation
            if (movementInput.sqrMagnitude > 0.01f)
            {
                Vector3 moveDir = new Vector3(movementInput.x, 0, movementInput.y).normalized;
                localDeckPosition += moveDir * moveSpeed * dt;

                // Clamp to raft radius
                float maxRadius = raft != null ? raft.RaftRadius * 0.82f : 1.2f;
                Vector2 flatPos = new Vector2(localDeckPosition.x, localDeckPosition.z);
                if (flatPos.magnitude > maxRadius)
                {
                    flatPos = flatPos.normalized * maxRadius;
                    localDeckPosition.x = flatPos.x;
                    localDeckPosition.z = flatPos.y;
                }

                // Face move direction
                if (modelRoot != null)
                {
                    Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
                    modelRoot.localRotation = Quaternion.Slerp(modelRoot.localRotation, targetRot, dt * 12f);
                }

                if (visualController != null)
                {
                    visualController.SetWalking(true);
                }
            }
            else
            {
                if (visualController != null)
                {
                    visualController.SetWalking(false);
                }
            }

            // Vertical jump / gravity
            if (!isGrounded)
            {
                verticalVelocity -= gravity * dt;
                localDeckPosition.y += verticalVelocity * dt;

                if (localDeckPosition.y <= 0.2f)
                {
                    localDeckPosition.y = 0.2f;
                    verticalVelocity = 0f;
                    isGrounded = true;
                    if (visualController != null)
                    {
                        visualController.TriggerLandSquash();
                    }
                }
            }
            else
            {
                localDeckPosition.y = 0.2f;
            }

            transform.localPosition = localDeckPosition;
        }

        public void SetWipedOut(bool wipedOut)
        {
            isWipedOut = wipedOut;
            if (visualController != null)
            {
                visualController.SetWipedOut(wipedOut);
            }
        }

        public void ResetPosition(Vector3 localDeckPos)
        {
            localDeckPosition = localDeckPos;
            transform.localPosition = localDeckPos;
            verticalVelocity = 0f;
            isGrounded = true;
            isWipedOut = false;
            if (visualController != null)
            {
                visualController.ResetVisuals();
            }
        }
    }
}
