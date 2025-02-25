using Unity.Services.Authentication;
using UnityEngine;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;

public class RelayTest : MonoBehaviour
{

    public string lobbyKey;
    //private async void Start()
    //{
    //    await Unity.Services.Core.UnityServices.InitializeAsync();
    //    AuthenticationService.Instance.SignedIn += () =>
    //    {
    //        Debug.Log("Signed in + " + AuthenticationService.Instance.PlayerId);
    //    };
    //    await AuthenticationService.Instance.SignInAnonymouslyAsync();
    //}
    private void Update()
    {
    }
    public async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);  // Aqui é o Relay de Unity.Services.Relay
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            lobbyKey  = joinCode;

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(

                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData,
                null,true
                
            );
            NetworkManager.Singleton.StartHost();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("RelayServiceException: " + e.Message);
        }
    }
    public async void JoinRelay()
    {
        try
        {
            if(lobbyKey != null || lobbyKey != "")
            {
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(lobbyKey);

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                    joinAllocation.RelayServer.IpV4,
                    (ushort)joinAllocation.RelayServer.Port,
                    joinAllocation.AllocationIdBytes,
                    joinAllocation.Key,
                    joinAllocation.ConnectionData,
                    joinAllocation.HostConnectionData,
                    true
                    );

                NetworkManager.Singleton.StartClient();
            }
        }
        catch (RelayServiceException e)
        {
            Debug.Log("RelayServiceException: " + e.Message);
        }
    }
}
        