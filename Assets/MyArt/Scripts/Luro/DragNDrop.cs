using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragNDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 startPosition;
    private Transform startParent;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        startPosition = rectTransform.position;
        startParent = transform.parent; 
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (transform.parent == null || transform.parent == startParent)
        {
            ResetPosition();
        }
    }

    public void ResetPosition()
    {
        transform.SetParent(startParent); 
        rectTransform.position = startPosition; 
    }

    
    public IEnumerator ShakeObject()
    {
        Vector3 originalPosition = transform.position;

        float duration = 0.3f;
        float magnitude = 10f;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float xOffset = Random.Range(-1f, 1f) * magnitude;
            transform.position = originalPosition + new Vector3(xOffset, 0, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
    }
}
