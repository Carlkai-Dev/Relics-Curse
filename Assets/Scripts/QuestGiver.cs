// QuestGiver.cs
// Handles NPC interaction, quest progression, dialogue UI, and player movement lock during dialogue.
// 🟢 NOTES (for future Carlos):
// - TypeText coroutine animates dialogue and plays blip at the START of a sentence (to ensure sound plays even when skipped).
// - Pressing E skips typing or progresses to next line.
// - Movement is frozen during dialogue to keep player's attention.
// - Always have fun on everything you do.

using UnityEngine;
using TMPro;
using System.Collections;

public class QuestGiver : MonoBehaviour
{
    [Header("NPC UI")]
    public GameObject dialoguePanel;
    public TMP_Text PressEText;
    public TMP_Text questDetailText;
    public GameObject continuePromptPanel;
    public Transform player;
    public float interactionRange = 3f;
    public KeyCode interactKey = KeyCode.E;

    [Header("Player Controller")]
    public ThirdPersonController playerController; //Locks movement during dialogue

    [Header("Audio")]
    public AudioClip dialogueEndClip;
    public AudioClip dialogueBlip; //Plays at start of every sentence
    public AudioSource audioSource;

    [Header("Animation")]
    public Animator npcAnimator; //Controls talking animation

    [Header("Quest Status UI")]
    public TMP_Text questStatusText;

    [Header("Quest Complete Popup")]
    public GameObject questCompletePanel;

    [Header("Typing Settings")]
    [Range(0.01f, 0.1f)]
    public float typingSpeed = 0.035f;

    public enum QuestStage { NotStarted, TalkedToNPC, ReachedWizard, DefeatWizard, FindOrb, ReturnToNPC, Complete }
    public QuestStage currentStage = QuestStage.NotStarted;

    private bool isPlayerNear = false;
    private bool questCompleted = false;
    private bool isShowingQuestMessage = false;
    private int dialogueIndex = 0;
    private bool isTyping = false;
    private Coroutine currentTypingCoroutine;
    private string currentFullText = "";

    // 📜 Dialogue lines when first talking to NPC
    private string[] dialogueSequence = new string[] {
        "Hello! Please retrieve the sacred item to save our tribe!",
        "The wizard who once protected us has gone mad...",
        "He’s trying to use our sacred relic for evil!",
        "Be careful on the road — there are guards watching the path. Don’t let them see you!"
    };

    private string returnToNPCDialogue = "You found it! Thank you, hero.";
    private string postQuestDialogue = "You have saved my village. I will go and find the others to rebuild!";

    void Start()
    {
        dialoguePanel?.SetActive(false);
        continuePromptPanel?.SetActive(false);
        SetQuestStage(QuestStage.NotStarted);
    }

    void Update()
    {
        if (!player || !dialoguePanel || !PressEText || !questDetailText) return;

        float distance = Vector3.Distance(transform.position, player.position);
        isPlayerNear = distance <= interactionRange;

        if (isPlayerNear && !questCompleted)
        {
            if (!dialoguePanel.activeSelf)
            {
                dialoguePanel.SetActive(true);
                PressEText.text = "Press E to talk";
                questDetailText.text = "";
                isShowingQuestMessage = false;
                dialogueIndex = 0;
            }

            if (currentStage == QuestStage.Complete && dialogueIndex == 0)
            {
                npcAnimator?.SetBool("isTalking", true);
                currentTypingCoroutine = StartCoroutine(TypeText(postQuestDialogue));
                dialogueIndex = 999;
                return;
            }

            if (Input.GetKeyDown(interactKey))
            {
                PressEText.text = "";

                if (!isShowingQuestMessage)
                {
                    isShowingQuestMessage = true;
                    LockPlayerMovement(true);
                }

                if (isTyping)
                {
                    if (currentTypingCoroutine != null)
                        StopCoroutine(currentTypingCoroutine);

                    questDetailText.text = currentFullText;
                    isTyping = false;
                    npcAnimator?.SetBool("isTalking", false);
                    return;
                }

                if (currentStage == QuestStage.ReturnToNPC && dialogueIndex == 0)
                {
                    npcAnimator?.SetBool("isTalking", true);
                    currentTypingCoroutine = StartCoroutine(TypeText(returnToNPCDialogue));
                    dialogueIndex = 999;
                    Invoke(nameof(MarkQuestComplete), 2f);
                }
                else if (currentStage != QuestStage.ReturnToNPC && dialogueIndex < dialogueSequence.Length)
                {
                    npcAnimator?.SetBool("isTalking", true);
                    currentTypingCoroutine = StartCoroutine(TypeText(dialogueSequence[dialogueIndex]));
                    dialogueIndex++;

                    if (currentStage == QuestStage.NotStarted && dialogueIndex == 1)
                        SetQuestStage(QuestStage.ReachedWizard);
                }
                else
                {
                    if (currentStage == QuestStage.ReachedWizard && dialogueIndex >= dialogueSequence.Length)
                    {
                        if (dialogueEndClip != null)
                        {
                            if (audioSource != null)
                                audioSource.PlayOneShot(dialogueEndClip);
                            else
                                AudioSource.PlayClipAtPoint(dialogueEndClip, transform.position);
                        }
                    }

                    dialoguePanel?.SetActive(false);
                    continuePromptPanel?.SetActive(false);
                    isShowingQuestMessage = false;
                    LockPlayerMovement(false);
                }

                continuePromptPanel?.SetActive(true);
            }
        }
        else if (!isPlayerNear && dialoguePanel.activeSelf)
        {
            dialoguePanel.SetActive(false);
            isShowingQuestMessage = false;
            continuePromptPanel?.SetActive(false);
            LockPlayerMovement(false);
        }
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        currentFullText = text;
        questDetailText.text = "";

        // 🎵 Play blip at start of sentence (always plays even if skipped)
        if (dialogueBlip != null)
        {
            if (audioSource != null)
                audioSource.PlayOneShot(dialogueBlip);
            else
                AudioSource.PlayClipAtPoint(dialogueBlip, transform.position);
        }

        foreach (char c in text)
        {
            questDetailText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        npcAnimator?.SetBool("isTalking", false);
    }

    public void SetQuestStage(QuestStage newStage)
    {
        currentStage = newStage;
        if (questStatusText == null) return;

        switch (newStage)
        {
            case QuestStage.NotStarted:
                questStatusText.text = "Speak with the villager."; break;
            case QuestStage.ReachedWizard:
                questStatusText.text = "Reach the wizard, but stay hidden! His guards will spot you..."; break;
            case QuestStage.DefeatWizard:
                questStatusText.text = "Survive! Each cast drains his HP!"; break;
            case QuestStage.FindOrb:
                questStatusText.text = "With the wizard dead, the path to the relic is open..."; break;
            case QuestStage.ReturnToNPC:
                questStatusText.text = "Go back to the quest giver."; break;
            case QuestStage.Complete:
                questStatusText.text = "Mission complete! Thank you for playing."; break;
            default:
                questStatusText.text = ""; break;
        }
    }

    public void MarkOrbCollected()
    {
        if (currentStage == QuestStage.FindOrb)
        {
            Debug.Log("🟢 Orb collected — quest stage set to ReturnToNPC");
            SetQuestStage(QuestStage.ReturnToNPC);
        }
    }

    public void MarkQuestComplete()
    {
        Debug.Log("✅ MarkQuestComplete() called");
        questCompleted = true;
        SetQuestStage(QuestStage.Complete);

        dialoguePanel?.SetActive(false);
        continuePromptPanel?.SetActive(false);

        if (questCompletePanel != null)
        {
            questCompletePanel.SetActive(true);
            AudioSource audio = questCompletePanel.GetComponent<AudioSource>();
            if (audio != null) audio.Play();
        }

        LockPlayerMovement(false);
    }

    private void LockPlayerMovement(bool freeze)
    {
        if (playerController != null)
            playerController.canMove = !freeze; // 🚫 Prevents movement during dialogue
    }
}
