using Starter2D.CameraTools;
using Starter2D.Player;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Starter2D.EditorTools
{
    public static class PlayableCharacterSetup
    {
        private const string MenuPath = "2D Action Starter/Make Selected Character Playable";
        private const string SpawnMenuPath = "2D Action Starter/Spawn Selected Asset As Player";
        private const float TargetCharacterHeight = 1.6f;

        [MenuItem(MenuPath, true)]
        public static bool CanMakeSelectedPlayable()
        {
            return Selection.activeGameObject != null;
        }

        [MenuItem(MenuPath)]
        public static void MakeSelectedPlayable()
        {
            GameObject character = Selection.activeGameObject;
            if (character == null)
            {
                Debug.LogWarning("Select a character GameObject first.");
                return;
            }

            if (EditorUtility.IsPersistent(character))
            {
                character = SpawnAssetAsPlayer(character);
            }

            ConfigurePlayableCharacter(character);
        }

        [MenuItem(SpawnMenuPath, true)]
        public static bool CanSpawnSelectedAssetAsPlayer()
        {
            return Selection.activeObject is GameObject gameObject && EditorUtility.IsPersistent(gameObject);
        }

        [MenuItem(SpawnMenuPath)]
        public static void SpawnSelectedAssetAsPlayer()
        {
            GameObject selectedAsset = Selection.activeObject as GameObject;
            if (selectedAsset == null)
            {
                Debug.LogWarning("Select a character prefab or imported character asset in the Project window first.");
                return;
            }

            GameObject character = SpawnAssetAsPlayer(selectedAsset);
            ConfigurePlayableCharacter(character);
        }

        private static GameObject SpawnAssetAsPlayer(GameObject asset)
        {
            Vector3 spawnPosition = FindPreferredSpawnPosition();
            GameObject instance = PrefabUtility.InstantiatePrefab(asset) as GameObject;
            if (instance == null)
            {
                instance = Object.Instantiate(asset);
            }

            Undo.RegisterCreatedObjectUndo(instance, "Spawn playable character");
            instance.name = asset.name;
            instance.transform.position = spawnPosition;
            Selection.activeGameObject = instance;
            return instance;
        }

        private static void ConfigurePlayableCharacter(GameObject character)
        {
            Undo.RegisterFullObjectHierarchyUndo(character, "Make character playable");
            DisableOtherPlayerControllers(character);
            character.tag = "Player";
            NormalizeCharacterScale(character);

            Rigidbody2D body = character.GetComponent<Rigidbody2D>();
            if (body == null)
            {
                body = Undo.AddComponent<Rigidbody2D>(character);
            }

            body.gravityScale = 3.4f;
            body.freezeRotation = true;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            Collider2D collider = character.GetComponent<Collider2D>();
            if (collider == null)
            {
                CapsuleCollider2D capsule = Undo.AddComponent<CapsuleCollider2D>(character);
                EstimateCapsule(character, out Vector2 size, out Vector2 offset);
                capsule.size = size;
                capsule.offset = offset;
                collider = capsule;
            }

            Vector3 groundCheckPosition = EstimateGroundCheckPosition(collider);
            Transform groundCheck = EnsureChild(character.transform, "GroundCheck", groundCheckPosition);
            Transform attackPoint = EnsureChild(character.transform, "AttackPoint", groundCheckPosition + new Vector3(0.7f, 0.75f, 0f));

            PlayerController2D controller = character.GetComponent<PlayerController2D>();
            if (controller == null)
            {
                controller = Undo.AddComponent<PlayerController2D>(character);
            }

            SetObjectReference(controller, "groundCheck", groundCheck);

            PlayerAttack2D attack = character.GetComponent<PlayerAttack2D>();
            if (attack == null)
            {
                attack = Undo.AddComponent<PlayerAttack2D>(character);
            }

            SetObjectReference(attack, "attackPoint", attackPoint);

            if (character.GetComponent<PlayerHealth2D>() == null)
            {
                Undo.AddComponent<PlayerHealth2D>(character);
            }

            PlayerAnimationDriver2D animationDriver = character.GetComponent<PlayerAnimationDriver2D>();
            if (animationDriver == null && character.GetComponentInChildren<Animator>() != null)
            {
                animationDriver = Undo.AddComponent<PlayerAnimationDriver2D>(character);
            }

            if (animationDriver != null)
            {
                ConfigureAnimationDriver(animationDriver, character.GetComponentInChildren<Animator>());
            }

            CameraFollow2D cameraFollow = Object.FindObjectOfType<CameraFollow2D>();
            if (cameraFollow != null)
            {
                Undo.RecordObject(cameraFollow, "Set camera target");
                cameraFollow.SetTarget(character.transform);
                EditorUtility.SetDirty(cameraFollow);
            }

            if (PrefabUtility.IsPartOfPrefabInstance(character))
            {
                PrefabUtility.RecordPrefabInstancePropertyModifications(character);
            }
            EditorUtility.SetDirty(character);
            Debug.Log($"'{character.name}' is now playable. Use A/D or arrow keys to move, Space/W/Up to jump.");
        }

        private static void NormalizeCharacterScale(GameObject character)
        {
            if (!TryGetRendererBounds(character, out Bounds bounds) || bounds.size.y <= 0.01f)
            {
                return;
            }

            float scaleFactor = Mathf.Clamp(TargetCharacterHeight / bounds.size.y, 0.02f, 20f);
            if (Mathf.Abs(scaleFactor - 1f) < 0.01f)
            {
                return;
            }

            Undo.RecordObject(character.transform, "Normalize character scale");
            character.transform.localScale *= scaleFactor;
        }

        private static Vector3 FindPreferredSpawnPosition()
        {
            PlayerController2D existingPlayer = Object.FindObjectOfType<PlayerController2D>();
            if (existingPlayer != null)
            {
                return existingPlayer.transform.position;
            }

            GameObject spawn = GameObject.Find("PlayerSpawn");
            if (spawn != null)
            {
                return spawn.transform.position;
            }

            return Vector3.zero;
        }

        private static void DisableOtherPlayerControllers(GameObject activeCharacter)
        {
            PlayerController2D[] controllers = Object.FindObjectsOfType<PlayerController2D>();
            for (int i = 0; i < controllers.Length; i++)
            {
                PlayerController2D controller = controllers[i];
                if (controller.gameObject == activeCharacter)
                {
                    continue;
                }

                Undo.RecordObject(controller, "Disable old player controller");
                controller.enabled = false;

                if (controller.CompareTag("Player"))
                {
                    Undo.RecordObject(controller.gameObject, "Untag old player");
                    controller.gameObject.tag = "Untagged";
                }
            }
        }

        private static void ConfigureAnimationDriver(PlayerAnimationDriver2D animationDriver, Animator animator)
        {
            if (animationDriver == null || animator == null)
            {
                return;
            }

            SerializedObject serializedObject = new(animationDriver);
            serializedObject.FindProperty("animator").objectReferenceValue = animator;

            AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;
            if (controller != null)
            {
                string idleState = FindStateName(controller, "idle", "wait", "stand");
                if (string.IsNullOrEmpty(idleState))
                {
                    idleState = FindDefaultStateName(controller);
                }

                SetString(serializedObject, "idleState", idleState);
                SetString(serializedObject, "runState", FindStateName(controller, "run", "walk", "move"));
                SetString(serializedObject, "jumpState", FindStateName(controller, "jump", "air", "fall"));
            }

            serializedObject.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(animationDriver);
        }

        private static string FindStateName(AnimatorController controller, params string[] keywords)
        {
            for (int layerIndex = 0; layerIndex < controller.layers.Length; layerIndex++)
            {
                ChildAnimatorState[] states = controller.layers[layerIndex].stateMachine.states;
                for (int stateIndex = 0; stateIndex < states.Length; stateIndex++)
                {
                    string stateName = states[stateIndex].state.name;
                    string lowerName = stateName.ToLowerInvariant();
                    for (int keywordIndex = 0; keywordIndex < keywords.Length; keywordIndex++)
                    {
                        if (lowerName.Contains(keywords[keywordIndex]))
                        {
                            return stateName;
                        }
                    }
                }
            }

            return string.Empty;
        }

        private static string FindDefaultStateName(AnimatorController controller)
        {
            if (controller.layers.Length == 0)
            {
                return string.Empty;
            }

            AnimatorState defaultState = controller.layers[0].stateMachine.defaultState;
            return defaultState != null ? defaultState.name : string.Empty;
        }

        private static void SetString(SerializedObject serializedObject, string propertyName, string value)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property != null)
            {
                property.stringValue = value;
            }
        }

        private static Transform EnsureChild(Transform parent, string childName, Vector3 localPosition)
        {
            Transform existing = parent.Find(childName);
            if (existing != null)
            {
                return existing;
            }

            GameObject child = new(childName);
            Undo.RegisterCreatedObjectUndo(child, $"Create {childName}");
            child.transform.SetParent(parent);
            child.transform.localPosition = localPosition;
            child.transform.localRotation = Quaternion.identity;
            child.transform.localScale = Vector3.one;
            return child.transform;
        }

        private static void EstimateCapsule(GameObject character, out Vector2 size, out Vector2 offset)
        {
            if (!TryGetRendererBounds(character, out Bounds bounds))
            {
                size = new Vector2(0.8f, 1.6f);
                offset = new Vector2(0f, 0.8f);
                return;
            }

            Vector3 localSize = character.transform.InverseTransformVector(bounds.size);
            Vector3 localCenter = character.transform.InverseTransformPoint(bounds.center);
            float width = Mathf.Clamp(Mathf.Abs(localSize.x) * 0.55f, 0.35f, 1.2f);
            float height = Mathf.Clamp(Mathf.Abs(localSize.y) * 0.9f, 0.8f, 2.4f);
            size = new Vector2(width, height);
            offset = new Vector2(localCenter.x, localCenter.y);
        }

        private static bool TryGetRendererBounds(GameObject character, out Bounds bounds)
        {
            Renderer[] renderers = character.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
            {
                bounds = default;
                return false;
            }

            bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            return true;
        }

        private static Vector3 EstimateGroundCheckPosition(Collider2D collider)
        {
            if (collider is CapsuleCollider2D capsule)
            {
                return new Vector3(capsule.offset.x, capsule.offset.y - capsule.size.y * 0.5f - 0.04f, 0f);
            }

            if (collider is BoxCollider2D box)
            {
                return new Vector3(box.offset.x, box.offset.y - box.size.y * 0.5f - 0.04f, 0f);
            }

            Bounds bounds = collider.bounds;
            Transform transform = collider.transform;
            Vector3 bottom = new(bounds.center.x, bounds.min.y - 0.04f, bounds.center.z);
            return transform.InverseTransformPoint(bottom);
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
