using FishNet.Component.Spawning;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Object;
using UnityEngine;

public class TesterScript : NetworkBehaviour
{
    [SerializeField] private NetworkObject player;

    public override void OnStartServer()
    {
        base.OnStartServer();
        SceneManager.OnClientLoadedStartScenes += SpawnPlayer;
    }

    private void SpawnPlayer(NetworkConnection client, bool asServer)
    {

    }

    public void OnDestroy()
    {
        SceneManager.OnClientLoadedStartScenes -= SpawnPlayer;
    }
}
