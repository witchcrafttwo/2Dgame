using UnityEngine;

namespace Starter2D.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public sealed class SimpleEnemyPatrol2D : MonoBehaviour
    {
        [SerializeField] private float speed = 2f;
        [SerializeField] private Transform wallCheck;
        [SerializeField] private Transform ledgeCheck;
        [SerializeField] private float checkDistance = 0.25f;
        [SerializeField] private LayerMask groundLayer = ~0;

        private Rigidbody2D body;
        private Collider2D bodyCollider;
        private int direction = 1;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            bodyCollider = GetComponent<Collider2D>();
            body.freezeRotation = true;
        }

        private void FixedUpdate()
        {
            body.linearVelocity = new Vector2(direction * speed, body.linearVelocity.y);

            if (ShouldTurnAround())
            {
                TurnAround();
            }
        }

        private bool ShouldTurnAround()
        {
            Vector2 wallOrigin = wallCheck != null ? wallCheck.position : transform.position;
            Vector2 ledgeOrigin = ledgeCheck != null ? ledgeCheck.position : transform.position + Vector3.down * 0.45f;

            bool hitWall = HasBlockingHit(wallOrigin, Vector2.right * direction, checkDistance);
            bool hasGroundAhead = HasBlockingHit(ledgeOrigin, Vector2.down, checkDistance * 2f);
            return hitWall || !hasGroundAhead;
        }

        private bool HasBlockingHit(Vector2 origin, Vector2 directionVector, float distance)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, directionVector, distance, groundLayer);
            for (int i = 0; i < hits.Length; i++)
            {
                Collider2D hitCollider = hits[i].collider;
                if (hitCollider != null && hitCollider != bodyCollider && !hitCollider.isTrigger)
                {
                    return true;
                }
            }

            return false;
        }

        private void TurnAround()
        {
            direction *= -1;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * direction;
            transform.localScale = scale;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Vector3 wallOrigin = wallCheck != null ? wallCheck.position : transform.position;
            Vector3 ledgeOrigin = ledgeCheck != null ? ledgeCheck.position : transform.position + Vector3.down * 0.45f;
            Gizmos.DrawLine(wallOrigin, wallOrigin + Vector3.right * direction * checkDistance);
            Gizmos.DrawLine(ledgeOrigin, ledgeOrigin + Vector3.down * checkDistance * 2f);
        }
    }
}
