/*************************************************
Author Names : 		    Jacob Bateman, Clare Grady, Cade Naylor
Date Created : 		    08/12/2026
Date Last Modified : 	08/12/202
Brief Description : 	Central script with player info for design
                        Controls movement system management
Jacob Note:             Taken from PHISH and edited for AS.

External Resources :    	
***************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using System.Collections;

public class AlchemyPlayerController : PlayerController
{
    #region VARS

    [SerializeField] private Camera playerCamera;
    [SerializeField] private float cameraSensitivity;
    [SerializeField] private Transform cameraRotationParent;

    private Vector3 lastCamPosition;
    private float lineLength;

    #endregion

    #region FUNCTIONS

    /// <summary>
    /// Start function
    /// Populates MovementType Movement dictionary
    /// Sets default movement script to active
    /// </summary>
    private void Start()
    {
        Cursor.visible = false;
    }

    #endregion
}
