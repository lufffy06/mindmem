using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSpawner : MonoBehaviour
{
    public static CardSpawner Instance;

    public GameObject cardPrefab;
    public GameObject matchedCardPrefab; // New: Prefab for showing matched cards
    public Transform parentTransform;
    public List<Sprite> availableSprites;
    public Difficulty selectedDifficulty = Difficulty.Easy;

    private GridLayoutGroup gridLayout;
    private int rows;
    private int columns;
    private Dictionary<string, Sprite> spriteDictionary = new Dictionary<string, Sprite>();

    private void Awake()
    {
        Instance = this;
        gridLayout = parentTransform.GetComponent<GridLayoutGroup>();
        if (gridLayout == null)
        {
            gridLayout = parentTransform.gameObject.AddComponent<GridLayoutGroup>();
        }

        gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
        gridLayout.childAlignment = TextAnchor.MiddleCenter;
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;

        foreach (Sprite sprite in availableSprites)
        {
            spriteDictionary[sprite.name] = sprite;
        }
    }

    public void SpawnCardsWrapper(bool loadGame = false)
    {
        ClearExistingCards();
        SetLayoutByDifficulty();

        if (!loadGame)
        {
            int requiredPairs = (rows * columns) / 2;
            if (availableSprites.Count < requiredPairs)
            {
                Debug.LogError($"Not enough sprites available. Need {requiredPairs} pairs but only have {availableSprites.Count} sprites.");
                return;
            }
            SpawnCards();
        }
        else
        {
            SpawnCardsFromSavedGame();
        }
    }

    private void SetLayoutByDifficulty()
    {
        switch (selectedDifficulty)
        {
            case Difficulty.Easy:
                rows = 2; columns = 2;
                gridLayout.constraintCount = columns;
                gridLayout.cellSize = new Vector2(200, 250);
                gridLayout.spacing = new Vector2(30, 30);
                break;

            case Difficulty.Medium:
                rows = 2; columns = 3;
                gridLayout.constraintCount = columns;
                gridLayout.cellSize = new Vector2(180, 230);
                gridLayout.spacing = new Vector2(25, 25);
                break;

            case Difficulty.Hard:
                rows = 5; columns = 6;
                gridLayout.constraintCount = columns;
                gridLayout.cellSize = new Vector2(120, 170);
                gridLayout.spacing = new Vector2(15, 15);
                break;
        }

        RectTransform rt = parentTransform.GetComponent<RectTransform>();
        float width = columns * (gridLayout.cellSize.x + gridLayout.spacing.x);
        float height = rows * (gridLayout.cellSize.y + gridLayout.spacing.y);
        rt.sizeDelta = new Vector2(width, height);

        GameManager.Instance?.SetTotalPairs((rows * columns) / 2);
    }

    private void SpawnCards()
    {
        int totalCards = rows * columns;
        int pairCount = totalCards / 2;

        // Select random sprites
        List<Sprite> selectedSprites = new List<Sprite>();
        List<Sprite> tempSprites = new List<Sprite>(availableSprites);

        for (int i = 0; i < pairCount; i++)
        {
            if (tempSprites.Count == 0) break;

            int randomIndex = Random.Range(0, tempSprites.Count);
            selectedSprites.Add(tempSprites[randomIndex]);
            tempSprites.RemoveAt(randomIndex);
        }

        List<Sprite> shuffledSprites = new List<Sprite>();

        // Duplicate for pairs and shuffle
        foreach (Sprite sprite in selectedSprites)
        {
            shuffledSprites.Add(sprite);
            shuffledSprites.Add(sprite);
        }

        Shuffle(shuffledSprites);

        // Calculate and store positions
        List<Vector2> positions = CalculateCardPositions(shuffledSprites.Count);

        // Spawn cards with positions
        for (int i = 0; i < shuffledSprites.Count; i++)
        {
            SpawnCard(shuffledSprites[i], positions[i]);
        }

        gridLayout.enabled = false;
    }

    private void SpawnCardsFromSavedGame()
    {
        GameState savedState = GameManager.Instance.LoadGameState();
        if (savedState == null) return;

        selectedDifficulty = savedState.difficulty;
        SetLayoutByDifficulty();

        // Create a dictionary of matched card IDs for quick lookup
        HashSet<string> matchedIDs = new HashSet<string>(savedState.matchedCardIDs);

        // Only spawn remaining (unmatched) cards
        List<Vector2> positions = CalculateCardPositions(savedState.remainingCardIDs.Count + savedState.matchedCardIDs.Count);

        int positionIndex = 0;
        foreach (string cardID in savedState.remainingCardIDs)
        {
            if (spriteDictionary.TryGetValue(cardID, out Sprite sprite))
            {
                GameObject cardObj = Instantiate(cardPrefab, parentTransform);
                Card card = cardObj.GetComponent<Card>();
                card.SetFrontImage(sprite);
                card.cardID = cardID;

                // Position the card (skip positions where matched cards would be)
                RectTransform rt = cardObj.GetComponent<RectTransform>();
                rt.anchoredPosition = positions[positionIndex + savedState.matchedCardIDs.Count];

                card.FlipBack(); // Show remaining cards face-down
                positionIndex++;
            }
        }

        gridLayout.enabled = false;

        // Update game state
        GameManager.Instance?.SetTotalPairs(savedState.totalPairs);
        GameManager.Instance?.UpdatePairs(savedState.pairsMatched, savedState.totalPairs);
    }

    private void SpawnCard(Sprite sprite, Vector2 position)
    {
        GameObject card = Instantiate(cardPrefab, parentTransform);
        card.transform.localScale = Vector3.one;

        Card cardComponent = card.GetComponent<Card>();
        cardComponent.SetFrontImage(sprite);
        cardComponent.cardID = sprite.name;

        RectTransform rt = card.GetComponent<RectTransform>();
        rt.anchoredPosition = position;
    }

    private List<Vector2> CalculateCardPositions(int cardCount)
    {
        List<Vector2> positions = new List<Vector2>();
        float offsetX = (columns - 1) * (gridLayout.cellSize.x + gridLayout.spacing.x) * 0.5f;
        float offsetY = (rows - 1) * (gridLayout.cellSize.y + gridLayout.spacing.y) * 0.5f;

        for (int i = 0; i < cardCount; i++)
        {
            int row = i / columns;
            int col = i % columns;
            Vector2 position = new Vector2(
                col * (gridLayout.cellSize.x + gridLayout.spacing.x) - offsetX,
                -row * (gridLayout.cellSize.y + gridLayout.spacing.y) + offsetY
            );
            positions.Add(position);
        }

        return positions;
    }

    private void Shuffle(List<Sprite> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            Sprite temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private void ClearExistingCards()
    {
        foreach (Transform child in parentTransform)
        {
            Destroy(child.gameObject);
        }
    }

    public List<Card> GetAllCards()
    {
        List<Card> cards = new List<Card>();
        foreach (Transform child in parentTransform)
        {
            Card card = child.GetComponent<Card>();
            if (card != null)
            {
                cards.Add(card);
            }
        }
        return cards;
    }

    public List<Vector2> GetAllCardPositions()
    {
        List<Vector2> positions = new List<Vector2>();
        foreach (Transform child in parentTransform)
        {
            positions.Add(child.GetComponent<RectTransform>().anchoredPosition);
        }
        return positions;
    }
}