using UnityEngine;

// NEEDS TO BE ATTACHED TO ALL OBJECTS THAT PERSIST IN THE HUB
public class HubState : MonoBehaviour
{
    private void OnEnable()
    {
        // Restore state when returning to hub
        if (HubManager.Instance != null)
        {
            RestoreState();
        }
    }

    public void SaveState()
    {
        HubManager.Instance.SaveHubState(
            transform.position,
            0 // replace with your actual score variable
        );
    }

    private void RestoreState()
    {
        transform.position = HubManager.Instance.playerPosition;
        // Restore other state here
    }
}