// Assets/Scripts/Level1/LevelOneManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelOneManager : MonoBehaviour
{
    private enum Speaker
    {
        Character1 = 0,
        Character2 = 1
    }

    [System.Serializable]
    private struct DialogueLine
    {
        public Speaker speaker;
        [TextArea(2, 5)] public string text;
    }

    [Header("Dialogue")]
    [SerializeField] private GameObject dialogueCanvas;
    [SerializeField] private Text dialogueText;

    [Tooltip("Configure each line with speaker + text. If empty, old 'Dialogue Lines (Legacy)' will be used.")]
    [SerializeField] private DialogueLine[] dialogue;

    [Header("Dialogue Lines (Legacy)")]
    [Tooltip("Legacy support: If 'dialogue' is empty, these lines will be used and default to Character1.")]
    [SerializeField, TextArea(2, 5)] private string[] dialogueLines;

    [Header("Dialogue Portraits")]
    [Tooltip("Shown when speaker = Character1")]
    [SerializeField] private GameObject character1Image;
    [Tooltip("Shown when speaker = Character2")]
    [SerializeField] private GameObject character2Image;

    [Header("Player Control")]
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Sea Urchin")]
    [SerializeField] private string seaUrchinTag = "SeaUrchin";
    [SerializeField] private float seaUrchinCheckInterval = 0.25f;

    [Header("Collection")]
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private Collider2D collectionTrigger;

    [Header("Complete UI")]
    [SerializeField] private GameObject gameCompleteCanvas;

    [Header("Next Level")]
    [SerializeField] private string nextSceneName;

    private int _dialogueIndex;
    private float _checkTimer;
    private bool _dialogueFinished;
    private bool _collectionUnlocked;
    private bool _levelCompleted;

    private void Start()
    {
        if (gameCompleteCanvas != null) gameCompleteCanvas.SetActive(false);
        if (collectionTrigger != null) collectionTrigger.enabled = false;

        SetPlayerControl(false);

        // Start dialogue if provided
        if (HasDialogueConfigured() && dialogueCanvas != null && dialogueText != null)
        {
            dialogueCanvas.SetActive(true);
            _dialogueIndex = 0;
            ApplyDialogueLine(_dialogueIndex);
        }
        else
        {
            FinishDialogue();
        }
    }

    private void Update()
    {
        if (!_dialogueFinished)
        {
            if (Input.GetMouseButtonDown(0))
                OnDialogueClick();
            return;
        }

        if (!_collectionUnlocked)
        {
            _checkTimer += Time.deltaTime;
            if (_checkTimer >= seaUrchinCheckInterval)
            {
                _checkTimer = 0f;
                if (AreAllSeaUrchinsDestroyed())
                    UnlockCollection();
            }
        }

        if (_collectionUnlocked && !_levelCompleted && playerCollider != null && collectionTrigger != null)
        {
            if (playerCollider.bounds.Intersects(collectionTrigger.bounds))
                CompleteLevel();
        }
    }

    public void OnDialogueClick()
    {
        if (_dialogueFinished) return;

        if (!HasDialogueConfigured())
        {
            FinishDialogue();
            return;
        }

        _dialogueIndex++;
        if (_dialogueIndex >= GetDialogueCount())
        {
            FinishDialogue();
            return;
        }

        ApplyDialogueLine(_dialogueIndex);
    }

    private void ApplyDialogueLine(int index)
    {
        // Sanity
        if (dialogueText == null) return;

        Speaker speaker;
        string text;

        if (dialogue != null && dialogue.Length > 0)
        {
            index = Mathf.Clamp(index, 0, dialogue.Length - 1);
            speaker = dialogue[index].speaker;
            text = dialogue[index].text;
        }
        else
        {
            // Legacy fallback
            index = Mathf.Clamp(index, 0, (dialogueLines?.Length ?? 1) - 1);
            speaker = Speaker.Character1;
            text = (dialogueLines != null && dialogueLines.Length > 0) ? dialogueLines[index] : string.Empty;
        }

        dialogueText.text = text;
        SetSpeakerVisuals(speaker);
    }

    private void SetSpeakerVisuals(Speaker speaker)
    {
        if (character1Image != null)
            character1Image.SetActive(speaker == Speaker.Character1);

        if (character2Image != null)
            character2Image.SetActive(speaker == Speaker.Character2);
    }

    private int GetDialogueCount()
    {
        if (dialogue != null && dialogue.Length > 0)
            return dialogue.Length;

        return dialogueLines != null ? dialogueLines.Length : 0;
    }

    private bool HasDialogueConfigured()
    {
        return GetDialogueCount() > 0;
    }

    private void FinishDialogue()
    {
        _dialogueFinished = true;
        if (dialogueCanvas != null) dialogueCanvas.SetActive(false);

        // Hide portraits when dialogue ends (optional, but avoids leftover UI)
        if (character1Image != null) character1Image.SetActive(false);
        if (character2Image != null) character2Image.SetActive(false);

        SetPlayerControl(true);
    }

    private bool AreAllSeaUrchinsDestroyed()
    {
        return GameObject.FindGameObjectsWithTag(seaUrchinTag).Length == 0;
    }

    private void UnlockCollection()
    {
        _collectionUnlocked = true;
        if (collectionTrigger != null) collectionTrigger.enabled = true;
    }

    private void CompleteLevel()
    {
        _levelCompleted = true;
        if (gameCompleteCanvas != null) gameCompleteCanvas.SetActive(true);
    }

    public void LoadNextLevel()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
            return;
        }

        int current = SceneManager.GetActiveScene().buildIndex;
        int next = current + 1;
        if (next < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(next);
    }

    private void SetPlayerControl(bool isEnabled)
    {
        if (playerMovement != null)
            playerMovement.enabled = isEnabled;
    }
}
