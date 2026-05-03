using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    // Drag your TutorialPanel into this slot in the Inspector
    public GameObject tutorialPanel; 

    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene"); //
    }

    public void OpenTutorial()
    {
        tutorialPanel.SetActive(true); // Shows the panel
    }

    public void CloseTutorial()
    {
        tutorialPanel.SetActive(false); // Hides the panel
    }
}
