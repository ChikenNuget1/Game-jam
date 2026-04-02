using UnityEngine;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null;
    public GameObject interactionIcon;
    public float iconHeightOffset = 0f;

    void Start()
    {
        interactionIcon.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E pressed. interactableInRange: " + (interactableInRange != null ? "assigned" : "null"));
            if (interactableInRange != null)
                interactableInRange.Interact();
        }

        if (interactableInRange != null && interactionIcon.activeSelf)
        {
            MonoBehaviour mb = interactableInRange as MonoBehaviour;
            if (mb != null)
                interactionIcon.transform.position = mb.transform.position + new Vector3(0, iconHeightOffset, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger entered by: " + collision.gameObject.name);
        if (collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            Debug.Log("Interactable found: " + collision.gameObject.name);
            interactableInRange = interactable;
            interactionIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange)
        {
            interactableInRange = null;
            interactionIcon.SetActive(false);
        }
    }
}