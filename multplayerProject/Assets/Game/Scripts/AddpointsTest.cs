using UnityEngine;
using Unity.Netcode;

public class AddpointTeste : NetworkBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.CompareTag("Player")) return;
        if (!NetworkManager.Singleton.IsServer) return;
        if (collision.transform.TryGetComponent(out PlayerNetwork player))
        {
            player.AddPointToPoints();
            GetComponent<NetworkObject>().Despawn();   
        }
    }
}