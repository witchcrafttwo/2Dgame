using System.IO;
using Starter2D.CameraTools;
using Starter2D.Core;
using Starter2D.Enemies;
using Starter2D.Level;
using Starter2D.Player;
using UnityEditor;
using UnityEngine;

namespace Starter2D.EditorTools
{
    public static class StarterSceneBuilder
    {
        private const string MenuPath = "2D Action Starter/Build Starter Scene";
        private const string SpriteAssetPath = "Assets/Art/Starter/Square.png";

        [MenuItem(MenuPath)]
        public static void BuildStarterScene()
        {
            Sprite sprite = EnsureStarterSprite();
            GameObject root = new("Starter Level");
            Undo.RegisterCreatedObjectUndo(root, "Create starter level");

            Transform spawn = CreateEmpty("PlayerSpawn", new Vector3(-5f, 0.5f, 0f), root.transform).transform;

            GameObject gameManagerObject = CreateEmpty("GameManager", Vector3.zero, root.transform);
            GameManager2D gameManager = gameManagerObject.AddComponent<GameManager2D>();
            SetObjectReference(gameManager, "playerSpawnPoint", spawn);

            GameObject player = CreateSpriteObject("Player", spawn.position, new Vector3(0.8f, 1.5f, 1f), new Color(0.15f, 0.55f, 1f), sprite, root.transform);
            player.tag = "Player";
            Rigidbody2D playerBody = player.AddComponent<Rigidbody2D>();
            playerBody.gravityScale = 3.4f;
            playerBody.interpolation = RigidbodyInterpolation2D.Interpolate;
            playerBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            CapsuleCollider2D playerCollider = player.AddComponent<CapsuleCollider2D>();
            playerCollider.size = new Vector2(0.85f, 1.55f);
            Transform groundCheck = CreateEmpty("GroundCheck", player.transform.position + Vector3.down * 0.85f, player.transform).transform;
            PlayerController2D playerController = player.AddComponent<PlayerController2D>();
            SetObjectReference(playerController, "groundCheck", groundCheck);
            player.AddComponent<PlayerHealth2D>();
            gameManager.RegisterPlayer(player);

            Camera sceneCamera = Camera.main;
            if (sceneCamera == null)
            {
                sceneCamera = new GameObject("Main Camera").AddComponent<Camera>();
                Undo.RegisterCreatedObjectUndo(sceneCamera.gameObject, "Create camera");
            }
            sceneCamera.tag = "MainCamera";
            sceneCamera.orthographic = true;
            sceneCamera.orthographicSize = 5f;
            sceneCamera.transform.position = player.transform.position + new Vector3(0f, 1.5f, -10f);
            CameraFollow2D cameraFollow = sceneCamera.GetComponent<CameraFollow2D>() ?? sceneCamera.gameObject.AddComponent<CameraFollow2D>();
            cameraFollow.SetTarget(player.transform);

            CreatePlatform("Ground", new Vector3(0f, -1f, 0f), new Vector3(16f, 1f, 1f), new Color(0.25f, 0.25f, 0.28f), sprite, root.transform);
            CreatePlatform("Step 1", new Vector3(1.5f, 1.1f, 0f), new Vector3(3.4f, 0.35f, 1f), new Color(0.25f, 0.25f, 0.28f), sprite, root.transform);
            CreatePlatform("Step 2", new Vector3(5.2f, 2.6f, 0f), new Vector3(3f, 0.35f, 1f), new Color(0.25f, 0.25f, 0.28f), sprite, root.transform);

            GameObject enemy = CreateSpriteObject("Patrol Enemy", new Vector3(2f, -0.25f, 0f), new Vector3(0.9f, 0.9f, 1f), new Color(0.95f, 0.25f, 0.25f), sprite, root.transform);
            enemy.AddComponent<BoxCollider2D>();
            Rigidbody2D enemyBody = enemy.AddComponent<Rigidbody2D>();
            enemyBody.gravityScale = 3f;
            enemyBody.freezeRotation = true;
            enemy.AddComponent<DamageOnTouch2D>();
            SimpleEnemyPatrol2D enemyPatrol = enemy.AddComponent<SimpleEnemyPatrol2D>();
            Transform wallCheck = CreateEmpty("WallCheck", enemy.transform.position + Vector3.right * 0.55f, enemy.transform).transform;
            Transform ledgeCheck = CreateEmpty("LedgeCheck", enemy.transform.position + new Vector3(0.55f, -0.55f, 0f), enemy.transform).transform;
            SetObjectReference(enemyPatrol, "wallCheck", wallCheck);
            SetObjectReference(enemyPatrol, "ledgeCheck", ledgeCheck);

            GameObject checkpoint = CreateTrigger("Checkpoint", new Vector3(4.5f, -0.25f, 0f), new Vector3(0.45f, 1.5f, 1f), new Color(0.25f, 0.9f, 0.45f), sprite, root.transform);
            checkpoint.AddComponent<Checkpoint2D>();

            GameObject goal = CreateTrigger("Goal", new Vector3(8f, 3.35f, 0f), new Vector3(0.55f, 1.7f, 1f), new Color(1f, 0.85f, 0.2f), sprite, root.transform);
            goal.AddComponent<LevelGoal2D>();

            GameObject killZone = CreateEmpty("Kill Zone", new Vector3(0f, -5f, 0f), root.transform);
            BoxCollider2D killZoneCollider = killZone.AddComponent<BoxCollider2D>();
            killZoneCollider.size = new Vector2(30f, 1f);
            killZoneCollider.isTrigger = true;
            killZone.AddComponent<KillZone2D>();

            Selection.activeGameObject = player;
            EditorUtility.SetDirty(gameManagerObject);
            Debug.Log("Starter scene built. Save the scene when you are happy with it.");
        }

        private static GameObject CreatePlatform(string name, Vector3 position, Vector3 scale, Color color, Sprite sprite, Transform parent)
        {
            GameObject platform = CreateSpriteObject(name, position, scale, color, sprite, parent);
            platform.AddComponent<BoxCollider2D>();
            return platform;
        }

        private static GameObject CreateTrigger(string name, Vector3 position, Vector3 scale, Color color, Sprite sprite, Transform parent)
        {
            GameObject trigger = CreateSpriteObject(name, position, scale, color, sprite, parent);
            BoxCollider2D collider = trigger.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            return trigger;
        }

        private static GameObject CreateSpriteObject(string name, Vector3 position, Vector3 scale, Color color, Sprite sprite, Transform parent)
        {
            GameObject gameObject = new(name);
            Undo.RegisterCreatedObjectUndo(gameObject, $"Create {name}");
            gameObject.transform.SetParent(parent);
            gameObject.transform.position = position;
            gameObject.transform.localScale = scale;

            SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;
            spriteRenderer.color = color;
            return gameObject;
        }

        private static GameObject CreateEmpty(string name, Vector3 position, Transform parent)
        {
            GameObject gameObject = new(name);
            Undo.RegisterCreatedObjectUndo(gameObject, $"Create {name}");
            gameObject.transform.SetParent(parent);
            gameObject.transform.position = position;
            return gameObject;
        }

        private static Sprite EnsureStarterSprite()
        {
            Sprite existing = AssetDatabase.LoadAssetAtPath<Sprite>(SpriteAssetPath);
            if (existing != null)
            {
                return existing;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(SpriteAssetPath));
            Texture2D texture = new(16, 16, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[16 * 16];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.white;
            }

            texture.SetPixels(pixels);
            texture.Apply();
            File.WriteAllBytes(SpriteAssetPath, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);

            AssetDatabase.ImportAsset(SpriteAssetPath);
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(SpriteAssetPath);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 16f;
            importer.filterMode = FilterMode.Point;
            importer.SaveAndReimport();

            return AssetDatabase.LoadAssetAtPath<Sprite>(SpriteAssetPath);
        }

        private static void SetObjectReference(Object target, string propertyName, Object value)
        {
            SerializedObject serializedObject = new(target);
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property == null)
            {
                Debug.LogWarning($"Property '{propertyName}' was not found on {target.name}.");
                return;
            }

            property.objectReferenceValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
