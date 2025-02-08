using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndMatchDecoy : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector3 startPosition;

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 newPosition = eventData.position;
        newPosition.z = 0f;
        transform.position = newPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.position = startPosition;
    }
}