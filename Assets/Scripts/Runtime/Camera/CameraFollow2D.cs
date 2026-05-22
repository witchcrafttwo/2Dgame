using UnityEngine;

namespace Starter2D.CameraTools
{
    public sealed class CameraFollow2D : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new(0f, 1.5f, -10f);
        [SerializeField] private float smoothTime = 0.18f;
        [SerializeField] private bool followX = true;
        [SerializeField] private bool followY = true;

        private Vector3 velocity;

        public void SetTarget(Transform followTarget)
        {
            target = followTarget;
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            Vector3 desired = target.position + offset;
            Vector3 current = transform.position;

            if (!followX)
            {
                desired.x = current.x;
            }

            if (!followY)
            {
                desired.y = current.y;
            }

            transform.position = Vector3.SmoothDamp(current, desired, ref velocity, smoothTime);
        }
    }
}
