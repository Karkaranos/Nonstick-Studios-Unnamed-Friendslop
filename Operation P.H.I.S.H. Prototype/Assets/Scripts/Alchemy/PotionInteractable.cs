using UnityEngine;

public class PotionInteractable : MonoBehaviour, IAlchemyInteractable
{
    public string PotionID = "Default";
    public void EnterHover()
    {
        return;
    }

    public void EnterInteract(AlchemyPlayerController pc, bool standardInteraction = true)
    {
        transform.parent = pc.PickupPoint;
        transform.localPosition = Vector3.zero;
        Debug.Log($"GRABBED {this.PotionID}.");
    }

    public void ExitHover()
    {
        return;
    }

    public void ExitInteract()
    {
        transform.parent = null;
        Debug.Log($"DROPPED {this.PotionID}.");
    }
}
