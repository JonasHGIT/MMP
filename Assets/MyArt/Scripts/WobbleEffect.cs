using UnityEngine;
using System.Collections;

public class WobbleEffect : MonoBehaviour
{
    public float wobbleDuration = 3.0f;
    public float wobbleAmount = 3f;
    private bool isWobbling = false;

    public void StartWobble()
    {
        if (!isWobbling)
        {
            StartCoroutine(Wobble());
        }
    }

    private IEnumerator Wobble()
    {
        isWobbling = true;
        float elapsedTime = 0;

        while (elapsedTime < wobbleDuration)
        {
            float wobbleOffsetX = Mathf.Sin(elapsedTime * Mathf.PI * 4) * wobbleAmount;
            float wobbleOffsetY = Mathf.Cos(elapsedTime * Mathf.PI * 4) * wobbleAmount;
            float wobbleOffsetZ = wobbleOffsetX * wobbleOffsetY;
            
            transform.Translate(new Vector3(wobbleOffsetX, wobbleOffsetY, wobbleOffsetZ), Space.Self); // Lokale Verschiebung

            elapsedTime += Time.deltaTime;
            yield return null;

            // Rückkehr zur Originalposition in kleinen Schritten
            transform.Translate(new Vector3(-wobbleOffsetX, -wobbleOffsetY, -wobbleOffsetZ), Space.Self);
        }

        isWobbling = false;
    }
}
