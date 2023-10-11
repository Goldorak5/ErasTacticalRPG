using UnityEngine;
using UnityEngine.EventSystems;

public class HoveringCharacter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{

    public PaoloCharacter character;

    private void Start()
    {
        AddPhysics2DRaycaster();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {  
        // Highlight the character
        GetComponent<Renderer>().material.color = Color.red;
    }

    public void OnPointerExit(PointerEventData eventData)
    {     
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
