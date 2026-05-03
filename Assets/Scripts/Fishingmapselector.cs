using UnityEngine;
using UnityEngine.SceneManagement;

public class FishingMapSelector : MonoBehaviour, IInteractable
{
    [Header("Scene Names")]
    public string fishingSceneName = "FishingMinigame";

    public bool CanInteract() => true;

    public void Interact()
    {
        GoFishing();
    }

    public void GoFishing()
    {
        SceneManager.LoadScene(fishingSceneName);
    }
}