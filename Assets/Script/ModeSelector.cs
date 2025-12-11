
using UnityEngine;

public class ModeSelector : MonoBehaviour
{
    public CardSpawner cardSpawner; // Assign your CardSpawner in the inspector

    public void SetEasyMode()
    {
        cardSpawner.selectedDifficulty = Difficulty.Easy;
        cardSpawner.SpawnCardsWrapper();
    }

    public void SetMediumMode()
    {
        cardSpawner.selectedDifficulty = Difficulty.Medium;
        cardSpawner.SpawnCardsWrapper();
    }

    public void SetHardMode()
    {
        cardSpawner.selectedDifficulty = Difficulty.Hard;
        cardSpawner.SpawnCardsWrapper();
    }
}
