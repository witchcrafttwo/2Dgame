using Starter2D.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Starter2D.Player
{
    public sealed class PlayerHealth2D : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 3;
        [SerializeField] private float invincibleTime = 0.7f;
        [SerializeField] private UnityEvent<int, int> healthChanged;

        private int currentHealth;
        private float invincibleCounter;

        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;

        private void Awake()
        {
            RestoreFullHealth();
        }

        private void Update()
        {
            if (invincibleCounter > 0f)
            {
                invincibleCounter -= Time.deltaTime;
            }
        }

        public void TakeDamage(int damage)
        {
            if (damage <= 0 || invincibleCounter > 0f)
            {
                return;
            }

            currentHealth = Mathf.Max(0, currentHealth - damage);
            invincibleCounter = invincibleTime;
            healthChanged?.Invoke(currentHealth, maxHealth);

            if (currentHealth <= 0)
            {
                GameManager2D.Instance?.RespawnPlayer();
            }
        }

        public void RestoreFullHealth()
        {
            currentHealth = maxHealth;
            invincibleCounter = 0f;
            healthChanged?.Invoke(currentHealth, maxHealth);
        }
    }
}
