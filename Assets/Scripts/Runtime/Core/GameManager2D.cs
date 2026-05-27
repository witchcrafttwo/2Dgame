using System.Collections;
using Starter2D.Player;
using UnityEngine;

namespace Starter2D.Core
{
    public sealed class GameManager2D : MonoBehaviour
    {
        public static GameManager2D Instance { get; private set; }

        [SerializeField] private Transform playerSpawnPoint;
        [SerializeField] private float respawnDelay = 0.35f;

        private GameObject player;
        private Vector3 currentCheckpoint;
        private bool isRespawning;

        public bool IsPaused { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            Time.timeScale = 1f;
            IsPaused = false;
            currentCheckpoint = playerSpawnPoint != null ? playerSpawnPoint.position : Vector3.zero;
        }

        private void Start()
        {
            if (player == null)
            {
                PlayerController2D playerController = FindObjectOfType<PlayerController2D>();
                if (playerController != null)
                {
                    RegisterPlayer(playerController.gameObject);
                }
            }
        }

        public void RegisterPlayer(GameObject playerObject)
        {
            player = playerObject;

            if (playerSpawnPoint == null && player != null)
            {
                currentCheckpoint = player.transform.position;
            }
        }

        public void SetCheckpoint(Vector3 checkpointPosition)
        {
            currentCheckpoint = checkpointPosition;
        }

        public void RespawnPlayer()
        {
            if (player == null || isRespawning)
            {
                return;
            }

            StartCoroutine(RespawnRoutine());
        }

        public void SetPaused(bool paused)
        {
            IsPaused = paused;
            Time.timeScale = paused ? 0f : 1f;
        }

        private IEnumerator RespawnRoutine()
        {
            isRespawning = true;
            player.SetActive(false);

            if (respawnDelay > 0f)
            {
                yield return new WaitForSeconds(respawnDelay);
            }

            player.transform.position = currentCheckpoint;

            if (player.TryGetComponent(out Rigidbody2D body))
            {
                body.linearVelocity = Vector2.zero;
                body.angularVelocity = 0f;
            }

            if (player.TryGetComponent(out PlayerHealth2D health))
            {
                health.RestoreFullHealth();
            }

            player.SetActive(true);
            isRespawning = false;
        }
    }
}
