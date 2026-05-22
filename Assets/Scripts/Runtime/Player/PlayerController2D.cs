using Starter2D.Core;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Starter2D.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public sealed class PlayerController2D : MonoBehaviour
    {
        [Header("Run")]
        [SerializeField] private float moveSpeed = 7f;
        [SerializeField] private float acceleration = 80f;
        [SerializeField] private float deceleration = 95f;

        [Header("Jump")]
        [SerializeField] private float jumpVelocity = 13f;
        [SerializeField] private float jumpCutMultiplier = 0.45f;
        [SerializeField] private float coyoteTime = 0.12f;
        [SerializeField] private float jumpBufferTime = 0.12f;

        [Header("Ground Check")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.18f;
        [SerializeField] private LayerMask groundLayer = ~0;

        private Rigidbody2D body;
        private Collider2D bodyCollider;
        private Vector2 input;
        private bool jumpPressed;
        private bool jumpHeld;
        private bool isGrounded;
        private float coyoteCounter;
        private float jumpBufferCounter;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            bodyCollider = GetComponent<Collider2D>();
            body.freezeRotation = true;
        }

        private void OnEnable()
        {
            GameManager2D.Instance?.RegisterPlayer(gameObject);
        }

        private void Update()
        {
            ReadInput();
            UpdateJumpTimers();
            CutJumpOnRelease();
            FaceMoveDirection();
        }

        private void FixedUpdate()
        {
            CheckGrounded();
            Move();
            TryJump();
        }

        private void ReadInput()
        {
            float horizontal = 0f;
            bool pressedJumpThisFrame = false;
            bool holdingJump = false;

#if ENABLE_INPUT_SYSTEM
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                {
                    horizontal -= 1f;
                }

                if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                {
                    horizontal += 1f;
                }

                pressedJumpThisFrame =
                    keyboard.spaceKey.wasPressedThisFrame ||
                    keyboard.wKey.wasPressedThisFrame ||
                    keyboard.upArrowKey.wasPressedThisFrame;

                holdingJump =
                    keyboard.spaceKey.isPressed ||
                    keyboard.wKey.isPressed ||
                    keyboard.upArrowKey.isPressed;
            }

            Gamepad gamepad = Gamepad.current;
            if (gamepad != null)
            {
                horizontal += gamepad.leftStick.ReadValue().x;
                pressedJumpThisFrame |= gamepad.buttonSouth.wasPressedThisFrame;
                holdingJump |= gamepad.buttonSouth.isPressed;
            }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            horizontal = Input.GetAxisRaw("Horizontal");
            pressedJumpThisFrame |= Input.GetButtonDown("Jump");
            holdingJump |= Input.GetButton("Jump");
#endif

            input = new Vector2(Mathf.Clamp(horizontal, -1f, 1f), 0f);
            jumpPressed = pressedJumpThisFrame;
            jumpHeld = holdingJump;
        }

        private void UpdateJumpTimers()
        {
            coyoteCounter = isGrounded ? coyoteTime : coyoteCounter - Time.deltaTime;
            jumpBufferCounter = jumpPressed ? jumpBufferTime : jumpBufferCounter - Time.deltaTime;
        }

        private void CheckGrounded()
        {
            Vector3 checkPosition = groundCheck != null
                ? groundCheck.position
                : transform.position + Vector3.down * 0.55f;

            isGrounded = false;
            Collider2D[] hits = Physics2D.OverlapCircleAll(checkPosition, groundCheckRadius, groundLayer);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i] != bodyCollider && !hits[i].isTrigger)
                {
                    isGrounded = true;
                    return;
                }
            }
        }

        private void Move()
        {
            float targetSpeed = input.x * moveSpeed;
            float rate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;
            float nextSpeed = Mathf.MoveTowards(body.linearVelocity.x, targetSpeed, rate * Time.fixedDeltaTime);
            body.linearVelocity = new Vector2(nextSpeed, body.linearVelocity.y);
        }

        private void TryJump()
        {
            if (jumpBufferCounter <= 0f || coyoteCounter <= 0f)
            {
                return;
            }

            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpVelocity);
            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
        }

        private void CutJumpOnRelease()
        {
            if (jumpHeld || body.linearVelocity.y <= 0f)
            {
                return;
            }

            body.linearVelocity = new Vector2(body.linearVelocity.x, body.linearVelocity.y * jumpCutMultiplier);
        }

        private void FaceMoveDirection()
        {
            if (Mathf.Abs(input.x) < 0.01f)
            {
                return;
            }

            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(input.x);
            transform.localScale = scale;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Vector3 checkPosition = groundCheck != null
                ? groundCheck.position
                : transform.position + Vector3.down * 0.55f;
            Gizmos.DrawWireSphere(checkPosition, groundCheckRadius);
        }
    }
}
