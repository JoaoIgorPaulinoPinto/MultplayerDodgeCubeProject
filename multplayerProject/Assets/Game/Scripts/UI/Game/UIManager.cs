using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UImanager : MonoBehaviour 
{
    [SerializeField] Button btn_host;
    [SerializeField] Button btn_client;
    [SerializeField] Button btn_server;

    private void Start()
    {
        if (btn_client == null || btn_host == null || btn_server == null) return;
        btn_host.onClick.AddListener(() =>
        {
            print("Host Initialized");
            NetworkManager.Singleton.StartHost();
        });
        btn_client.onClick.AddListener(() =>
        {
            print("Client Initialized");
            NetworkManager.Singleton.StartClient();
        });
        btn_server.onClick.AddListener(() =>
        {
            print("Server Initialized");
            NetworkManager.Singleton.StartServer();
        });
    }
    public void DisableButtons()
    {
        btn_client.gameObject.SetActive(false);
        btn_host.gameObject.SetActive(false);
        btn_server.gameObject.SetActive(false);
    }
}
