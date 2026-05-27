using Starter2D.Core;
using UnityEngine;
using UnityEngine.UI;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Starter2D.Story
{
    public sealed class StoryManager2D : MonoBehaviour
    {
        public static StoryManager2D Instance { get; private set; }

        [SerializeField] private CanvasGroup dialoguePanel;
        [SerializeField] private Text speakerText;
        [SerializeField] private Text messageText;
        [SerializeField] private bool pauseGameplayDuringStory;

        private StoryLine2D[] currentLines;
        private int currentIndex;
        private bool isPlaying;

        public bool IsPlaying => isPlaying;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            Hide();
        }

        private void Update()
        {
            if (!isPlaying)
            {
                return;
            }

            if (WasAdvancePressed())
            {
                Advance();
            }
        }

        public void Play(StoryLine2D[] lines)
        {
            if (lines == null || lines.Length == 0)
            {
                return;
            }

            currentLines = lines;
            currentIndex = 0;
            isPlaying = true;
            Show();
            DisplayCurrentLine();

            if (pauseGameplayDuringStory)
            {
                GameManager2D.Instance?.SetPaused(true);
            }
        }

        public void Advance()
        {
            currentIndex++;
            if (currentIndex >= currentLines.Length)
            {
                Stop();
                return;
            }

            DisplayCurrentLine();
        }

        public void Stop()
        {
            isPlaying = false;
            currentLines = null;
            currentIndex = 0;
            Hide();

            if (pauseGameplayDuringStory)
            {
                GameManager2D.Instance?.SetPaused(false);
            }
        }

        private void DisplayCurrentLine()
        {
            StoryLine2D line = currentLines[currentIndex];
            if (speakerText != null)
            {
                speakerText.text = line.speaker;
            }

            if (messageText != null)
            {
                messageText.text = line.text;
            }
        }

        private void Show()
        {
            if (dialoguePanel == null)
            {
                return;
            }

            dialoguePanel.alpha = 1f;
            dialoguePanel.interactable = true;
            dialoguePanel.blocksRaycasts = true;
        }

        private void Hide()
        {
            if (dialoguePanel == null)
            {
                return;
            }

            dialoguePanel.alpha = 0f;
            dialoguePanel.interactable = false;
            dialoguePanel.blocksRaycasts = false;
        }

        private bool WasAdvancePressed()
        {
            bool pressed = false;

#if ENABLE_INPUT_SYSTEM
            Keyboard keyboard = Keyboard.current;
            Mouse mouse = Mouse.current;
            Gamepad gamepad = Gamepad.current;

            pressed |= keyboard != null && (keyboard.enterKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame || keyboard.eKey.wasPressedThisFrame);
            pressed |= mouse != null && mouse.leftButton.wasPressedThisFrame;
            pressed |= gamepad != null && gamepad.buttonSouth.wasPressedThisFrame;
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            pressed |= Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0);
#endif

            return pressed;
        }
    }
}
