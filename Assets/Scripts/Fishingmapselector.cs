using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Attach to an NPC or a portal/door object in the Hub.
/// When the player interacts, it loads the appropriate fishing scene.
///
/// SETUP:
///   1. Name your fishing scenes "FishingMap1" and "FishingMap2" in Build Settings.
///   2. Attach this to your fishing portal NPC/object and wire up the IInteractable interface,
///      OR call GoFishing() directly from an NPC's quest dialogue trigger.
/// </summary>
public class FishingMapSelector : MonoBehaviour, IInteractable
{
    [Header("Scene Names (must match Build Settings exactly)")]
    public string map1SceneName = "Fishing";
    public string map2SceneName = "FishingMap2";

    public bool CanInteract() => true;

    public void Interact()
    {
        GoFishing();
    }

    public void GoFishing()
    {
        bool map2Unlocked = QuestManager.Instance != null && QuestManager.Instance.fishingMap2Unlocked;

        if (map2Unlocked)
        {
            // Could show a UI picker here — for now just load map 2
            // (swap for a simple choice UI if you have time)
            SceneManager.LoadScene(map2SceneName);
        }
        else
        {
            SceneManager.LoadScene(map1SceneName);
        }
    }
}