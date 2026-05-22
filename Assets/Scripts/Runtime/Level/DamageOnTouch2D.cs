using Starter2D.Player;
using UnityEngine;

namespace Starter2D.Level
{
    public sealed class DamageOnTouch2D : MonoBehaviour
    {
        [SerializeField] private int damage = 1;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Damage(collision.collider);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Damage(other);
        }

        private void Damage(Collider2D other)
        {
            if (other.TryGetComponent(out PlayerHealth2D health))
            {
                health.TakeDamage(damage);
            }
        }
    }
}
