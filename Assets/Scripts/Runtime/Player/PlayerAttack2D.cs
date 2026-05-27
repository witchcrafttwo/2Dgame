using Starter2D.Core;
using Starter2D.Enemies;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Starter2D.Player
{
    public sealed class PlayerAttack2D : MonoBehaviour
    {
        [SerializeField] private Transform attackPoint;
        [SerializeField] private float attackRadius = 0.55f;
        [SerializeField] private int damage = 1;
        [SerializeField] private float cooldown = 0.22f;
        [SerializeField] private float knockbackForce = 5f;
        [SerializeField] private LayerMask targetLayer = ~0;

        private float cooldownCounter;

        private void Update()
        {
            if (GameManager2D.Instance != null && GameManager2D.Instance.IsPaused)
            {
                return;
            }

            if (cooldownCounter > 0f)
            {
                cooldownCounter -= Time.deltaTime;
            }

            if (WasAttackPressed())
            {
                TryAttack();
            }
        }

        private bool WasAttackPressed()
        {
            bool pressed = false;

#if ENABLE_INPUT_SYSTEM
            Keyboard keyboard = Keyboard.current;
            Mouse mouse = Mouse.current;
            Gamepad gamepad = Gamepad.current;

            pressed |= keyboard != null && (keyboard.jKey.wasPressedThisFrame || keyboard.fKey.wasPressedThisFrame);
            pressed |= mouse != null && mouse.leftButton.wasPressedThisFrame;
            pressed |= gamepad != null && gamepad.buttonWest.wasPressedThisFrame;
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            pressed |= Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0);
#endif

            return pressed;
        }

        private void TryAttack()
        {
            if (cooldownCounter > 0f)
            {
                return;
            }

            cooldownCounter = cooldown;
            Vector2 center = attackPoint != null
                ? attackPoint.position
                : transform.position + Vector3.right * Mathf.Sign(transform.localScale.x) * 0.7f;
            Collider2D[] hits = Physics2D.OverlapCircleAll(center, attackRadius, targetLayer);

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].TryGetComponent(out EnemyHealth2D enemyHealth))
                {
                    Vector2 direction = enemyHealth.transform.position - transform.position;
                    enemyHealth.TakeDamage(damage, direction, knockbackForce);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Vector3 center = attackPoint != null ? attackPoint.position : transform.position;
            Gizmos.DrawWireSphere(center, attackRadius);
        }
    }
}
