/*************************************************
Author Names : 		    Cade Naylor
Date Created : 		    07/29/2026
Date Last Modified : 	07/29/2026
Brief Description : 	A test object for interactions
External Resources :    	
***************************************************/
using Unity.VisualScripting;
using UnityEngine;

public class TestItem : MonoBehaviour, IInteractable
{
    #region VARS
    private Material standardMat;

    [SerializeField] private Material hoverMat;

    [SerializeField] private Material interactMat;

    private MeshRenderer mr;

    #endregion

    #region Functions
    /// <summary>
    /// Start is called on the first frame update
    /// Grabs a reference to the mesh renderer and sets the base material
    /// </summary>
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        standardMat = mr.material;
    }

    /// <summary>
    /// Implemented function stub from IInteractable
    /// Changes the object's material when hovered over
    /// </summary>
    public void EnterHover()
    {
        mr.material = hoverMat;
    }

    /// <summary>
    /// Implemented function stub from IInteractable
    /// Changes the object's material when interacted with
    /// </summary>
    public void EnterInteract()
    {
        mr.material = interactMat;
    }

    /// <summary>
    /// Implemented function stub from IInteractable
    /// Resets the object's material when hover ends
    /// </summary>
    public void ExitHover()
    {
        mr.material = standardMat;
    }

    /// <summary>
    /// Implemented function stub from IInteractable
    /// Resets the object's material when interaction ends
    /// </summary>
    public void ExitInteract()
    {
        mr.material = standardMat;
    }
    #endregion


}
