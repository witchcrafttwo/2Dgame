using System.IO;
using Starter2D.CameraTools;
using Starter2D.Combat;
using Starter2D.Core;
using Starter2D.Enemies;
using Starter2D.Level;
using Starter2D.Player;
using Starter2D.Story;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

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
            Transform attackPoint = CreateEmpty("AttackPoint", player.transform.position + Vector3.right * 0.75f, player.transform).transform;
            PlayerController2D playerController = player.AddComponent<PlayerController2D>();
            SetObjectReference(playerController, "groundCheck", groundCheck);
            PlayerAttack2D playerAttack = player.AddComponent<PlayerAttack2D>();
            SetObjectReference(playerAttack, "attackPoint", attackPoint);
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
            enemy.AddComponent<EnemyHealth2D>();
            enemy.AddComponent<HitFlash2D>();
            SimpleEnemyPatrol2D enemyPatrol = enemy.AddComponent<SimpleEnemyPatrol2D>();
            Transform wallCheck = CreateEmpty("WallCheck", enemy.transform.position + Vector3.right * 0.55f, enemy.transform).transform;
            Transform ledgeCheck = CreateEmpty("LedgeCheck", enemy.transform.position + new Vector3(0.55f, -0.55f, 0f), enemy.transform).transform;
            SetObjectReference(enemyPatrol, "wallCheck", wallCheck);
            SetObjectReference(enemyPatrol, "ledgeCheck", ledgeCheck);

            GameObject heavyEnemy = CreateSpriteObject("Guard Enemy", new Vector3(5.2f, 3.35f, 0f), new Vector3(1f, 1f, 1f), new Color(0.6f, 0.15f, 0.85f), sprite, root.transform);
            heavyEnemy.AddComponent<BoxCollider2D>();
            Rigidbody2D heavyEnemyBody = heavyEnemy.AddComponent<Rigidbody2D>();
            heavyEnemyBody.gravityScale = 3f;
            heavyEnemyBody.freezeRotation = true;
            heavyEnemy.AddComponent<DamageOnTouch2D>();
            EnemyHealth2D heavyEnemyHealth = heavyEnemy.AddComponent<EnemyHealth2D>();
            SetInt(heavyEnemyHealth, "maxHealth", 3);
            heavyEnemy.AddComponent<HitFlash2D>();
            SimpleEnemyPatrol2D heavyEnemyPatrol = heavyEnemy.AddComponent<SimpleEnemyPatrol2D>();
            Transform heavyWallCheck = CreateEmpty("WallCheck", heavyEnemy.transform.position + Vector3.right * 0.55f, heavyEnemy.transform).transform;
            Transform heavyLedgeCheck = CreateEmpty("LedgeCheck", heavyEnemy.transform.position + new Vector3(0.55f, -0.55f, 0f), heavyEnemy.transform).transform;
            SetObjectReference(heavyEnemyPatrol, "wallCheck", heavyWallCheck);
            SetObjectReference(heavyEnemyPatrol, "ledgeCheck", heavyLedgeCheck);

            GameObject checkpoint = CreateTrigger("Checkpoint", new Vector3(4.5f, -0.25f, 0f), new Vector3(0.45f, 1.5f, 1f), new Color(0.25f, 0.9f, 0.45f), sprite, root.transform);
            checkpoint.AddComponent<Checkpoint2D>();

            GameObject goal = CreateTrigger("Goal", new Vector3(8f, 3.35f, 0f), new Vector3(0.55f, 1.7f, 1f), new Color(1f, 0.85f, 0.2f), sprite, root.transform);
            goal.AddComponent<LevelGoal2D>();

            GameObject storyManagerObject = CreateStoryUi(root.transform);

            GameObject introStory = CreateTrigger("Story Trigger - Intro", new Vector3(-4.1f, 0.15f, 0f), new Vector3(0.35f, 1.8f, 1f), new Color(0.2f, 0.7f, 1f, 0.35f), sprite, root.transform);
            StoryTrigger2D introTrigger = introStory.AddComponent<StoryTrigger2D>();
            SetStoryLines(
                introTrigger,
                new StoryLine2D { speaker = "\u4E3B\u4EBA\u516C", text = "\u3053\u306E\u5148\u306E\u7826\u306B\u3001\u6751\u304B\u3089\u596A\u308F\u308C\u305F\u5149\u306E\u6B20\u7247\u304C\u3042\u308B\u3002" },
                new StoryLine2D { speaker = "\u76F8\u68D2", text = "\u6575\u3092\u5012\u3057\u306A\u304C\u3089\u9032\u3082\u3046\u3002\u653B\u6483\u306F J / F / \u5DE6\u30AF\u30EA\u30C3\u30AF \u3060\u3002" });

            GameObject goalStory = CreateTrigger("Story Trigger - Goal", new Vector3(7.1f, 3.35f, 0f), new Vector3(0.35f, 1.7f, 1f), new Color(0.2f, 0.7f, 1f, 0.35f), sprite, root.transform);
            StoryTrigger2D goalTrigger = goalStory.AddComponent<StoryTrigger2D>();
            SetStoryLines(
                goalTrigger,
                new StoryLine2D { speaker = "\u76F8\u68D2", text = "\u3042\u308C\u304C\u5149\u306E\u6B20\u7247\u3060\u3002\u6700\u5F8C\u306E\u898B\u5F35\u308A\u3092\u7A81\u7834\u3057\u3088\u3046\u3002" },
                new StoryLine2D { speaker = "\u4E3B\u4EBA\u516C", text = "\u3053\u3053\u304B\u3089\u304C\u59CB\u307E\u308A\u3060\u3002\u5FC5\u305A\u53D6\u308A\u623B\u3059\u3002" });

            GameObject killZone = CreateEmpty("Kill Zone", new Vector3(0f, -5f, 0f), root.transform);
            BoxCollider2D killZoneCollider = killZone.AddComponent<BoxCollider2D>();
            killZoneCollider.size = new Vector2(30f, 1f);
            killZoneCollider.isTrigger = true;
            killZone.AddComponent<KillZone2D>();

            Selection.activeGameObject = player;
            EditorUtility.SetDirty(gameManagerObject);
            EditorUtility.SetDirty(storyManagerObject);
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

        private static GameObject CreateStoryUi(Transform parent)
        {
            GameObject canvasObject = new("Story Canvas");
            Undo.RegisterCreatedObjectUndo(canvasObject, "Create story canvas");
            canvasObject.transform.SetParent(parent);

            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            canvasObject.AddComponent<GraphicRaycaster>();

            GameObject panelObject = new("Dialogue Panel");
            Undo.RegisterCreatedObjectUndo(panelObject, "Create dialogue panel");
            panelObject.transform.SetParent(canvasObject.transform, false);
            Image panelImage = panelObject.AddComponent<Image>();
            panelImage.color = new Color(0.05f, 0.06f, 0.08f, 0.88f);
            CanvasGroup panelGroup = panelObject.AddComponent<CanvasGroup>();
            RectTransform panelRect = panelObject.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.08f, 0.04f);
            panelRect.anchorMax = new Vector2(0.92f, 0.28f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            Text speakerText = CreateText("Speaker", panelObject.transform, 30, FontStyle.Bold, new Color(1f, 0.86f, 0.32f));
            RectTransform speakerRect = speakerText.GetComponent<RectTransform>();
            speakerRect.anchorMin = new Vector2(0.04f, 0.66f);
            speakerRect.anchorMax = new Vector2(0.96f, 0.92f);
            speakerRect.offsetMin = Vector2.zero;
            speakerRect.offsetMax = Vector2.zero;

            Text messageText = CreateText("Message", panelObject.transform, 28, FontStyle.Normal, Color.white);
            RectTransform messageRect = messageText.GetComponent<RectTransform>();
            messageRect.anchorMin = new Vector2(0.04f, 0.12f);
            messageRect.anchorMax = new Vector2(0.96f, 0.66f);
            messageRect.offsetMin = Vector2.zero;
            messageRect.offsetMax = Vector2.zero;

            StoryManager2D storyManager = canvasObject.AddComponent<StoryManager2D>();
            SetObjectReference(storyManager, "dialoguePanel", panelGroup);
            SetObjectReference(storyManager, "speakerText", speakerText);
            SetObjectReference(storyManager, "messageText", messageText);

            return canvasObject;
        }

        private static Text CreateText(string name, Transform parent, int fontSize, FontStyle fontStyle, Color color)
        {
            GameObject textObject = new(name);
            Undo.RegisterCreatedObjectUndo(textObject, $"Create {name}");
            textObject.transform.SetParent(parent, false);

            Text text = textObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = fontSize;
            text.fontStyle = fontStyle;
            text.color = color;
            text.alignment = TextAnchor.MiddleLeft;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            return text;
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

        private static void SetInt(Object target, string propertyName, int value)
        {
            SerializedObject serializedObject = new(target);
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property == null)
            {
                Debug.LogWarning($"Property '{propertyName}' was not found on {target.name}.");
                return;
            }

            property.intValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetStoryLines(StoryTrigger2D target, params StoryLine2D[] lines)
        {
            SerializedObject serializedObject = new(target);
            SerializedProperty linesProperty = serializedObject.FindProperty("lines");
            linesProperty.arraySize = lines.Length;

            for (int i = 0; i < lines.Length; i++)
            {
                SerializedProperty lineProperty = linesProperty.GetArrayElementAtIndex(i);
                lineProperty.FindPropertyRelative("speaker").stringValue = lines[i].speaker;
                lineProperty.FindPropertyRelative("text").stringValue = lines[i].text;
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
