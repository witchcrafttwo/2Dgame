using System.Collections;
using UnityEngine;

namespace Starter2D.Combat
{
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class HitFlash2D : MonoBehaviour
    {
        [SerializeField] private Color flashColor = Color.white;
        [SerializeField] private float flashTime = 0.08f;

        private SpriteRenderer spriteRenderer;
        private Color originalColor;
        private Coroutine flashRoutine;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            originalColor = spriteRenderer.color;
        }

        public void Flash()
        {
            if (flashRoutine != null)
            {
                StopCoroutine(flashRoutine);
            }

            flashRoutine = StartCoroutine(FlashRoutine());
        }

        private IEnumerator FlashRoutine()
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSecondsRealtime(flashTime);
            spriteRenderer.color = originalColor;
            flashRoutine = null;
        }
    }
}
