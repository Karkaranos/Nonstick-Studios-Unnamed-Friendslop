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
        Debug.Log("SPAWN PLAYER");

        if (!asServer)
            return;

        if (!client.Scenes.Contains(gameObject.scene))
            SceneManager.AddConnectionToScene(client, gameObject.scene);

        NetworkObject obj = NetworkManager.GetPooledInstantiated(player, asServer: true);
        Spawn(obj, client, gameObject.scene);
    }

    public void OnDestroy()
    {
        SceneManager.OnClientLoadedStartScenes -= SpawnPlayer;
    }
}
