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
    [SerializeField] private Material standardMat;

    [SerializeField] private Material hoverMat;

    [SerializeField] private Material interactMat;

    private MeshRenderer mr;
    public void EnterHover()
    {
        mr.material = hoverMat;
    }

    public void EnterInteract()
    {
        mr.material = interactMat;
    }

    public void ExitHover()
    {
        mr.material = standardMat;
    }

    public void ExitInteract()
    {
        mr.material = standardMat;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
    }
}
