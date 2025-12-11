using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public GameObject front;
    public GameObject back;
    public bool isFlipped = false;
    public string cardID;
    public bool IsMatched { get; private set; } = false;

    private Image cardImage;
    private bool isBeingDestroyed = false;

    private void Awake()
    {
        cardImage = GetComponent<Image>();
        FlipBack();
    }

    public void SetFrontImage(Sprite sprite)
    {
        if (front == null) return;

        Image img = front.GetComponent<Image>();
        if (img != null)
            img.sprite = sprite;

        cardID = sprite.name;
    }

    public void Flip()
    {
        if (isFlipped || IsMatched || !GameManager.Instance.CanFlipCard()) return;

        isFlipped = true;
        if (front != null) front.SetActive(true);
        if (back != null) back.SetActive(false);

        GameManager.Instance.CardFlipped(this);
    }

    public void FlipBack()
    {
        if (IsMatched || isBeingDestroyed) return;

        isFlipped = false;
        if (front != null) front.SetActive(false);
        if (back != null) back.SetActive(true);
    }

    public void MarkAsMatched()
    {
        if (isBeingDestroyed || this == null) return;

        IsMatched = true;

        // Change the color but keep the sprite visible
        if (cardImage != null)
        {
            cardImage.color = new Color(0.5f, 1f, 0.5f, 0.7f);
        }

        // Ensure the front is visible
        if (front != null) front.SetActive(true);
        if (back != null) back.SetActive(false);
    }

    private void OnDestroy()
    {
        isBeingDestroyed = true;
    }

    // Keep the OnMouseDown if you're using it for non-UI cards
    private void OnMouseDown()
    {
        Flip();
    }
}