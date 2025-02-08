using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndMatch : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector3 startPosition;
    public GameObject targetImage;
    private bool isOverlapping = false;
    public static System.Action OnCoinPlacedCorrectly;
    private bool isPlacedCorrectly = false;

    void Awake()
    {
        startPosition = transform.position;
    }

    void Start()
    {
        startPosition = transform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = transform.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 newPosition = eventData.position;
        newPosition.z = 0f;
        transform.position = newPosition;
        CheckOverlap();
    }

    public void CheckOverlap()
    {
        Collider2D targetCollider = targetImage.GetComponent<Collider2D>();
        Collider2D selfCollider = GetComponent<Collider2D>();

        Debug.Log("Coin Position: " + selfCollider.bounds.center);
        Debug.Log("Target Position: " + targetCollider.bounds.center);

        if (selfCollider.IsTouching(targetCollider))
        {
            isOverlapping = true;
            Debug.Log("Overlap detected!");
        }
        else
        {
            isOverlapping = false;
            Debug.Log("No overlap.");
        }
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject == targetImage)
        {
            Debug.Log("Coin is over target (Stay)");
            isOverlapping = true;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isOverlapping)
        {
            Debug.Log("Overlap successful, snap to target!");
            transform.position = targetImage.transform.position;

            if (!isPlacedCorrectly)
            {
                isPlacedCorrectly = true;
                OnCoinPlacedCorrectly?.Invoke();
            }
        }
        else
        {
            Debug.Log("No overlap, resetting position.");
            transform.position = startPosition;
        }
        isOverlapping = false;
    }

    public bool IsPlacedCorrectly()
    {
        return isPlacedCorrectly;
    }

    public void ResetPosition()
    {
        transform.position = startPosition;
        isPlacedCorrectly = false;
    }
}