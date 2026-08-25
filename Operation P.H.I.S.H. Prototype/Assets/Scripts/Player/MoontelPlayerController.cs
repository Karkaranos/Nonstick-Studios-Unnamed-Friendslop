/*************************************************
Author Names : 		    Jacob Bateman, Clare Grady, Cade Naylor, Jay Embry
Date Created : 		    08/25/2026
Brief Description : 	Central script with player info for design
                        Controls movement system management

Jay Note:               hey guys

External Resources :    	
***************************************************/

using UnityEngine;
using UnityEngine.UI;

public class MoontelPlayerController : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float cameraSensitivity;
    [SerializeField] private Transform cameraRotationParent;

    public Camera PlayerCam { get { return playerCamera; } }
    public float CameraSensitivity { get { return cameraSensitivity; } }
    public Transform CameraRotationParent { get { return cameraRotationParent; } }

    [SerializeField] private Image playerCrosshair;
    public Image CrosshairImage { get { return playerCrosshair; } }
    [SerializeField] private Sprite standard;
    public Sprite StandardSprite { get { return standard; } }
    [SerializeField] private Sprite interactable;
    public Sprite InteractableSprite { get { return interactable; } }

    public Transform PickupPoint;

    private void Start()
    {
        StaticUtilities.HideCursor();
    }
}
