using System.Collections.Generic;
using UnityEngine;

namespace SteamTesting
{
    public class PanelSwapper : MonoBehaviour
    {
        public List<Panel> panels = new List<Panel>();

        public void SwapPanel(string panelName)
        {
            foreach(Panel p in panels)
            {
                if(p.PanelName == panelName)
                {
                    p.gameObject.SetActive(true);
                }
                else
                {
                    p.gameObject.SetActive(false);
                }
            }
        }
    }
}