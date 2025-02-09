using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; 

public class Drop : MonoBehaviour, IDropHandler
{
    public string akzeptiertesObjekt; 
    private int correctItems = 0; 
    private int totalItems = 3; 

    private Image dropZoneImage; 
    private Color originalColor; 

    private void Awake()
    {
        dropZoneImage = GetComponent<Image>();
        if (dropZoneImage != null)
        {
            originalColor = dropZoneImage.color;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject fallenGelassen = eventData.pointerDrag; 
        if (fallenGelassen == null) return;

        DragNDrop draggable = fallenGelassen.GetComponent<DragNDrop>();

        if (fallenGelassen.name.Contains(akzeptiertesObjekt))
        {
            
            fallenGelassen.transform.SetParent(transform);
            fallenGelassen.transform.position = transform.position;
            correctItems++;

            
            if (correctItems == totalItems)
            {
                
            }

            if (dropZoneImage != null)
            {
                dropZoneImage.color = Color.green; 
                Invoke(nameof(ResetColor), 0.5f); 
            }
        }
        else
        {
            
            draggable.ResetPosition();
            StartCoroutine(draggable.ShakeObject());
        }
    }

    private void ResetColor()
    {
        
        if (dropZoneImage != null)
        {
            dropZoneImage.color = originalColor;
        }
    }
}
