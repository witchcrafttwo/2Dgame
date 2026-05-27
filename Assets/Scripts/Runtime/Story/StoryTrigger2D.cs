using UnityEngine;

namespace Starter2D.Story
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class StoryTrigger2D : MonoBehaviour
    {
        [SerializeField] private StoryLine2D[] lines;
        [SerializeField] private bool playOnlyOnce = true;

        private bool hasPlayed;

        private void Reset()
        {
            GetComponent<Collider2D>().isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (hasPlayed && playOnlyOnce)
            {
                return;
            }

            if (!other.CompareTag("Player"))
            {
                return;
            }

            hasPlayed = true;
            StoryManager2D.Instance?.Play(lines);
        }
    }
}
