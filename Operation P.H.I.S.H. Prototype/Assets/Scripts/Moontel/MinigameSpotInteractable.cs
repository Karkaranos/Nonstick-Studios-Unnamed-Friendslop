/*************************************************
Author Names : 		    Sky Beal
Date Created : 		    09/09/2026
Brief Description : 	Controls functionality for the fix and clean minigame "messes"
External Resources :    	
***************************************************/
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MinigameSpotInteractable : MoontelPickupInteractable
{
    [HorizontalLine(color: EColor.Red, height: 4)]
    [Header("Design Variables")]
    [Tooltip("What tool is required to clean/fix this.")]
    public MinigameObjectType objectTypeNeeded;
    [SerializeField, Tooltip("How many seconds it takes to clean/fix this.")] private float secondsToCleanOrFix;

    [SerializeField, Tooltip("If this is a fix minigame, false is a mess minigame.")] private bool isFixMinigame;
    [ShowIf("isFixMinigame"), SerializeField, Tooltip("Each mesh renderer associated with the minigame model.")] private List<MeshRenderer> meshRenderers;
    [ShowIf("isFixMinigame"), SerializeField, Tooltip("Material/shader material that will be added to imply brokenness.")] private Material brokenMaterial;
    private bool isBroken;

    [Header("Required References")]
    [SerializeField, Tooltip("UI Slider that shows fixed/cleaned progress.")] private Slider fixedOrCleanedSlider;

    private float currentSecondsComplete;

    private Coroutine cleanOrFixCoroutine;
    private bool shouldRunCoroutine = false;



    [Button, ShowIf("isFixMinigame")]
    /// <summary>
    /// Breaks item for fix minigame, adds new shader to object
    /// </summary>
    public void BreakObject()
    {
        //if already broken or if it's a mess minigame
        if (isBroken || !isFixMinigame)
        {
            return;
        }

        isBroken = true;
        
        #region Setting Materials
        //go through each mesh renderer
        for (int i = 0; i < meshRenderers.Count; i++)
        {
            //get current materials
            Material[] materials = meshRenderers[i].materials;
            //set new array length
            Material[] newMaterials = new Material[materials.Length + 1];
            //set specifically first material
            newMaterials[0] = brokenMaterial;
            

            if (newMaterials.Length <= 1)
            {
                return;
            }

            //set new materials in array
            for (int j = 1; j <= materials.Length; j++)
            {
                newMaterials[j] = materials[j-1];
            }

            meshRenderers[i].materials = newMaterials;
        }
        #endregion

        fixedOrCleanedSlider.enabled = true;
    }

    /// <summary>
    /// Fixes item for fix minigame, removes broken shader
    /// </summary>
    private void FixObject()
    {
        isBroken = false;

        //Set materials back
        for (int i = 0; i < meshRenderers.Count; i++)
        {
            Material[] currentMaterials = meshRenderers[i].materials;
            currentMaterials = currentMaterials.Skip(1).ToArray();

            meshRenderers[i].materials = currentMaterials;
        }

        fixedOrCleanedSlider.enabled = false;
        fixedOrCleanedSlider.value = 0;
        currentSecondsComplete = 0;
    }

    [Button, HideIf("isFixMinigame")]
    /// <summary>
    /// Makes mess visible
    /// </summary>
    public void CreateMess()
    {
        if (!isFixMinigame)
        {
            gameObject.SetActive(true);
            fixedOrCleanedSlider.enabled = true;
        }
    }

    /// <summary>
    /// Cleans mess minigame, hides the mess
    /// </summary>
    private void CleanMess()
    {
        gameObject.SetActive(false);

        fixedOrCleanedSlider.enabled = false;
        fixedOrCleanedSlider.value = 0;
        currentSecondsComplete = 0;
    }

    /// <summary>
    /// Starts timer count up until complete
    /// </summary>
    public void StartCleanOrFixTimer()
    {
        if (cleanOrFixCoroutine == null && (isBroken || !isFixMinigame))
        {
            shouldRunCoroutine = true;
            cleanOrFixCoroutine = StartCoroutine(CleanOrFixTimer());
        }
    }

    /// <summary>
    /// Stops timer count up
    /// </summary>
    public void StopCleanOrFixTimer()
    {
        if (cleanOrFixCoroutine != null)
        {
            shouldRunCoroutine = false;
            cleanOrFixCoroutine = null;
            StopCoroutine(CleanOrFixTimer());
        }
    }

    /// <summary>
    /// Runs cleaning/fixing timer until completed, updates UI accordingly
    /// </summary>
    /// <returns></returns>
    private IEnumerator CleanOrFixTimer()
    {
        while (shouldRunCoroutine)
        {
            if (currentSecondsComplete > secondsToCleanOrFix)
            {
                //spot is cleaned or fixed
                if (isFixMinigame)
                {
                    FixObject();
                }
                else
                {
                    CleanMess();
                }
                cleanOrFixCoroutine = null;
                yield break; 
            }
            else
            {
                currentSecondsComplete += Time.deltaTime;
                fixedOrCleanedSlider.value = currentSecondsComplete / secondsToCleanOrFix;

                yield return null;
            }
        }

    }


    /// <summary>
    /// included to not change material
    /// </summary>
    public override void EnterHover()
    {
    }

    /// <summary>
    /// included to not change material
    /// </summary>
    public override void ExitHover()
    {
    }
}
