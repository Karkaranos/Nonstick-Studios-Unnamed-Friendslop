using UnityEngine;
using Unity.Netcode;

public class MenuManager : NetworkBehaviour
{
    [SerializeField] private NetcodePasswordManager npm;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(npm!= null)
        {
            npm.joinButton.
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
