using UnityEngine;

public class GameManager : MonoBehaviour
{
    public DragAndMatch[] coinHandlers;
    public GameObject completionObject;

    private void OnEnable()
    {
        DragAndMatch.OnCoinPlacedCorrectly += CheckAllCoinsPlaced;
    }

    private void OnDisable()
    {
        DragAndMatch.OnCoinPlacedCorrectly -= CheckAllCoinsPlaced;
    }

    void CheckAllCoinsPlaced()
    {
        int correctCoinsPlaced = 0;

        foreach (DragAndMatch handler in coinHandlers)
        {
            if (handler.IsPlacedCorrectly())
            {
                correctCoinsPlaced++;
            }
        }

        if (correctCoinsPlaced == coinHandlers.Length)
        {
            ActivateCompletionObject();
        }
    }

    void ActivateCompletionObject()
    {
        if (completionObject != null)
        {
            completionObject.SetActive(true);
        }
    }

    public void ResetGame()
    {
        foreach (DragAndMatch handler in coinHandlers)
        {
            handler.ResetPosition();
        }
    }
}