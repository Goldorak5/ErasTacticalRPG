using UnityEngine;
using UnityEngine.EventSystems;

public class HoveringCharacter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{

    public PaoloCharacter character;
    public bool isMouseOverCharacter = false;

    private void Start()
    {
        AddPhysics2DRaycaster();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Set the flag to true when the mouse is over the character
        character.IsMouseOver = true;
   
        // Highlight the character
        GetComponent<Renderer>().material.color = Color.red;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Set the flag to false when the mouse leaves the character
        character.IsMouseOver = false;
        // Remove the highlight
        GetComponent<Renderer>().material.color = Color.white;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Clicked: " + eventData.pointerCurrentRaycast.gameObject.name);
    }

    private void AddPhysics2DRaycaster()
    {
        Physics2DRaycaster physicsRaycaster = FindObjectOfType<Physics2DRaycaster>();
        if (physicsRaycaster == null)
        {
            physicsRaycaster = Camera.main.gameObject.AddComponent<Physics2DRaycaster>();
        }
        physicsRaycaster.eventMask = LayerMask.GetMask("Character");
    }

}
