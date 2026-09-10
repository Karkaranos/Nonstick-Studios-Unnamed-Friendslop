using UnityEngine;

public class CleanFixMinigameInteractable : MoontelPickupInteractable
{
    private enum MinigameObjectType { Broom, Toolbox };
    [Header("Minigame")]
    [SerializeField] private MinigameObjectType objectType;
}
