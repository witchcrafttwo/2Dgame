using Starter2D.Core;
using UnityEngine;

namespace Starter2D.Level
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class KillZone2D : MonoBehaviour
    {
        private void Reset()
        {
            GetComponent<Collider2D>().isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                GameManager2D.Instance?.RespawnPlayer();
            }
        }
    }
}
