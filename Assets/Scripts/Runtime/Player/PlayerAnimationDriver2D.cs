using System.Collections.Generic;
using UnityEngine;

namespace Starter2D.Player
{
    [RequireComponent(typeof(PlayerController2D))]
    public sealed class PlayerAnimationDriver2D : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private string speedParameter = "Speed";
        [SerializeField] private string horizontalSpeedParameter = "HorizontalSpeed";
        [SerializeField] private string verticalSpeedParameter = "VerticalSpeed";
        [SerializeField] private string movingParameter = "IsMoving";
        [SerializeField] private string groundedParameter = "IsGrounded";
        [SerializeField] private string idleState;
        [SerializeField] private string runState;
        [SerializeField] private string jumpState;
        [SerializeField] private float crossFadeTime = 0.08f;

        private readonly HashSet<string> parameterNames = new();
        private PlayerController2D controller;
        private string currentState;

        private void Awake()
        {
            controller = GetComponent<PlayerController2D>();
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }

            CacheParameters();
        }

        private void Update()
        {
            if (animator == null || controller == null)
            {
                return;
            }

            Vector2 velocity = controller.Velocity;
            SetFloat(speedParameter, Mathf.Abs(velocity.x));
            SetFloat(horizontalSpeedParameter, velocity.x);
            SetFloat(verticalSpeedParameter, velocity.y);
            SetBool(movingParameter, Mathf.Abs(controller.MoveInput) > 0.01f);
            SetBool(groundedParameter, controller.IsGrounded);
            PlayBestState(velocity);
        }

        private void CacheParameters()
        {
            parameterNames.Clear();
            if (animator == null)
            {
                return;
            }

            AnimatorControllerParameter[] parameters = animator.parameters;
            for (int i = 0; i < parameters.Length; i++)
            {
                parameterNames.Add(parameters[i].name);
            }
        }

        private void SetFloat(string parameterName, float value)
        {
            if (!string.IsNullOrEmpty(parameterName) && parameterNames.Contains(parameterName))
            {
                animator.SetFloat(parameterName, value);
            }
        }

        private void SetBool(string parameterName, bool value)
        {
            if (!string.IsNullOrEmpty(parameterName) && parameterNames.Contains(parameterName))
            {
                animator.SetBool(parameterName, value);
            }
        }

        private void PlayBestState(Vector2 velocity)
        {
            string nextState = string.Empty;
            if (!controller.IsGrounded && !string.IsNullOrEmpty(jumpState))
            {
                nextState = jumpState;
            }
            else if (Mathf.Abs(velocity.x) > 0.05f && !string.IsNullOrEmpty(runState))
            {
                nextState = runState;
            }
            else if (!string.IsNullOrEmpty(idleState))
            {
                nextState = idleState;
            }

            if (string.IsNullOrEmpty(nextState) || nextState == currentState)
            {
                return;
            }

            int stateHash = Animator.StringToHash(nextState);
            if (!animator.HasState(0, stateHash))
            {
                return;
            }

            animator.CrossFade(stateHash, crossFadeTime);
            currentState = nextState;
        }
    }
}
