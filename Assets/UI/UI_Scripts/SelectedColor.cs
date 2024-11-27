using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SelectedColor : MonoBehaviour
{
    float red = 120.0f;
    float yellow = 0.0f;
    float blue = 240.0f;

    Image colorTriangle;
    Coroutine rotateCoroutine;

    private void Start()
    {
        colorTriangle = GetComponent<Image>();
    }

    public void UpdateSelected(ColorPicker.ColorEnum selectedColor)
    {
        float targetRotation = 0f;

        switch (selectedColor)
        {
            case ColorPicker.ColorEnum.Red:
                targetRotation = red;
                break;
            case ColorPicker.ColorEnum.Blue:
                targetRotation = blue;
                break;
            case ColorPicker.ColorEnum.Yellow:
                targetRotation = yellow;
                break;
        }

        if (rotateCoroutine != null)
        {
            StopCoroutine(rotateCoroutine);
        }

        rotateCoroutine = StartCoroutine(SmoothRotate(targetRotation));
    }

    private IEnumerator SmoothRotate(float targetRotation)
    {
        float duration = 0.3f; 
        float elapsedTime = 0f;

        Quaternion initialRotation = colorTriangle.transform.rotation;
        Quaternion finalRotation = Quaternion.Euler(0, 0, targetRotation);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            colorTriangle.transform.rotation = Quaternion.Lerp(initialRotation, finalRotation, elapsedTime / duration);
            yield return null;
        }

        colorTriangle.transform.rotation = finalRotation; 
    }
}
