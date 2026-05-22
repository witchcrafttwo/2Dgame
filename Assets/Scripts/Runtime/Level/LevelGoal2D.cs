using UnityEngine;
using UnityEngine.Events;

namespace Starter2D.Level
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class LevelGoal2D : MonoBehaviour
    {
        [SerializeField] private UnityEvent reached;

        private bool completed;

        private void Reset()
        {
            GetComponent<Collider2D>().isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (completed || !other.CompareTag("Player"))
            {
                return;
            }

            completed = true;
            reached?.Invoke();
            Debug.Log("Level clear!");
        }
    }
}
