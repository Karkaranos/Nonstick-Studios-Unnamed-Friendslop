/*************************************************
Author Names : 		    Toby
Date Created : 		    8/1/2026

Brief Description : 	Temp ui display for the tether
***************************************************/

using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class TetherLengthDisplay : MonoBehaviour
{
    [SerializeField, Required] private TMP_Text text;

    /// <summary>
    /// Refresh Display
    /// </summary>
    void Update()
    {
        int tetherDisplayLength = Mathf.RoundToInt(TetherManager.Instance.GetTotalTetherLength());
        text.text = $"Tether Length: {tetherDisplayLength} / {TetherManager.Instance.TotalMaxTetherLength}";
    }
}
