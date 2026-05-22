using Starter2D.Core;
using UnityEngine;

namespace Starter2D.Level
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class Checkpoint2D : MonoBehaviour
    {
        [SerializeField] private Transform respawnPoint;
        [SerializeField] private Color activatedColor = new(0.35f, 0.95f, 0.55f);

        private bool activated;

        private void Reset()
        {
            GetComponent<Collider2D>().isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (activated || !other.CompareTag("Player"))
            {
                return;
            }

            activated = true;
            Vector3 point = respawnPoint != null ? respawnPoint.position : transform.position;
            GameManager2D.Instance?.SetCheckpoint(point);

            if (TryGetComponent(out SpriteRenderer spriteRenderer))
            {
                spriteRenderer.color = activatedColor;
            }
        }
    }
}
