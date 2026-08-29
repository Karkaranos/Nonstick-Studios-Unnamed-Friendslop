/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    08/16/2026
Brief Description : 	Instantiates a canvas when interacted with, if it hasn't opened a canvas
                        Destroys the canvas on interact end
External Resources :    	
***************************************************/
using UnityEngine;

public class OpenCanvasInteractable : MonoBehaviour, IAlchemyInteractable
{
    [SerializeField, Tooltip("The canvas that spawns when this object is interacted with")] private GameObject canvasToSpawn;
    private GameObject openedCanvas = null;


    private void Awake()
    {
        PublicEvents.ForceCloseCanvas += CheckIfCloseCalled;
    }

    private void OnDestroy()
    {
        PublicEvents.ForceCloseCanvas -= CheckIfCloseCalled;
    }

    public void EnterHover()
    {
        return;
    }

    public void EnterInteract(AlchemyPlayerController apc, bool standardInteraction = true)
    {
        if (openedCanvas == null)
        {
            openedCanvas = Instantiate(canvasToSpawn, apc.transform);
            StaticUtilities.ShowCursor();
            FindFirstObjectByType<AlchemyMovement>().SetPauseState(true);
        }
        else
        {
            Debug.Log("Canvas is already open");
        }
    }

    public void ExitHover()
    {
        return;
    }

    /// <summary>
    /// Since the public event broadcasts to every instance, check if the canvas is the one this instance spawned
    /// If it is, close it and set the user's interaction to null
    /// </summary>
    /// <param name="g"></param>
    private void CheckIfCloseCalled(GameObject g)
    {
        if (g == openedCanvas)
        {
            ExitInteract();
            PublicEvents.ResetInteractable?.Invoke();
        }
    }

    public void ExitInteract()
    {
        if (openedCanvas != null)
        {
            Destroy(openedCanvas);
            openedCanvas = null;

            FindFirstObjectByType<AlchemyMovement>().SetPauseState(false);
        }
        else
        {
            Debug.Log("No canvas to destroy");
        }
    }

    public void DropItem()
    {
        throw new System.NotImplementedException();
    }

    public void ThrowItem(Vector3 throwVec)
    {
        throw new System.NotImplementedException();
    }
}
