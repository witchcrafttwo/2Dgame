using Starter2D.Combat;
using UnityEngine;
using UnityEngine.Events;

namespace Starter2D.Enemies
{
    public sealed class EnemyHealth2D : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 2;
        [SerializeField] private float destroyDelay = 0.05f;
        [SerializeField] private UnityEvent defeated;

        private int currentHealth;
        private bool isDefeated;

        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;
        public bool IsDefeated => isDefeated;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(int damage, Vector2 hitDirection, float knockbackForce)
        {
            if (isDefeated || damage <= 0)
            {
                return;
            }

            currentHealth = Mathf.Max(0, currentHealth - damage);

            if (TryGetComponent(out Rigidbody2D body))
            {
                Vector2 knockback = new(Mathf.Sign(hitDirection.x == 0f ? transform.localScale.x : hitDirection.x) * knockbackForce, knockbackForce * 0.35f);
                body.linearVelocity = knockback;
            }

            if (TryGetComponent(out HitFlash2D hitFlash))
            {
                hitFlash.Flash();
            }

            if (currentHealth <= 0)
            {
                Defeat();
            }
        }

        private void Defeat()
        {
            isDefeated = true;
            defeated?.Invoke();
            Destroy(gameObject, destroyDelay);
        }
    }
}
