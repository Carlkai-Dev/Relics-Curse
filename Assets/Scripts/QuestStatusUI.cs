// This script handles the on-screen quest objective UI.
// It updates based on quest stage progression.
// Carlkai Note: Keep this clean and modular — this script can be reused across different quests.

using UnityEngine;
using TMPro;

public class QuestStatusUI : MonoBehaviour
{
    public TMP_Text questText;

    public enum QuestStage { NotStarted, FindOrb, ReturnToNPC, Complete }
    public QuestStage currentStage = QuestStage.NotStarted;

    void Start()
    {
        UpdateQuestText();
    }

    public void SetStage(QuestStage newStage)
    {
        currentStage = newStage;
        UpdateQuestText();
    }

    void UpdateQuestText()
    {
        switch (currentStage)
        {
            case QuestStage.FindOrb:
                questText.text = "Go find the magic orb to save the tribe.";
                break;
            case QuestStage.ReturnToNPC:
                questText.text = "Return to the quest giver.";
                break;
            case QuestStage.Complete:
                questText.text = "Quest Complete! Thank you for playing.";
                break;
            default:
                questText.text = "Pending Quest...";
                break;
        }
    }
}
