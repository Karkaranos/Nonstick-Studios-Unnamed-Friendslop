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
    public void EnterHover()
    {
        return;
    }

    public void EnterInteract(AlchemyPlayerController apc)
    {
        if(openedCanvas == null)
        {
            openedCanvas = Instantiate(canvasToSpawn, apc.transform);
            Cursor.visible = true;
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

    public void ExitInteract()
    {
        if(openedCanvas != null)
        {
            Destroy(openedCanvas);
            openedCanvas = null;
            Cursor.visible = false;

            FindFirstObjectByType<AlchemyMovement>().SetPauseState(false);
        }
        else
        {
            Debug.Log("No canvas to destroy");
        }
    }
}
