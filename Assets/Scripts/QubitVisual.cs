using UnityEngine;
using UnityEngine.UI;

public class QubitVisual : MonoBehaviour
{
    public int X { get; private set; }
    public int Y { get; private set; }

    private Image image;
    public bool HasError { get; private set; } = false;

    void Awake()
    {
        image = GetComponent<Image>();
        if (!image)
            image = gameObject.AddComponent<Image>();
    }

    public void Init(int x, int y)
    {
        X = x;
        Y = y;
        SetError(false);
    }

    public void SetError(bool active)
    {
        HasError = active;
        image.color = active ? Color.red : Color.white;
    }

    public void FlashCorrected()
    {
        StartCoroutine(FlashGreen());
    }

    private System.Collections.IEnumerator FlashGreen()
    {
        image.color = Color.green;
        yield return new WaitForSeconds(0.5f);
        image.color = Color.white;
    }

    public void SetColor(Color color)
    {
        image.color = color;
    }

    private bool isSyndrome = false;

    public void MarkSyndrome(bool active)
    {
        isSyndrome = active;
        image.color = active ? new Color(1f, 0.5f, 0f) : Color.white; // orange for syndrome
    }

    public bool IsSyndrome() => isSyndrome;

}