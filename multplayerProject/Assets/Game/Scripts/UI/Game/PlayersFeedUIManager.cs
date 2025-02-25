using Unity.Netcode;
using UnityEngine;

public class PlayersFeedUIManager : NetworkBehaviour
{
    public static PlayersFeedUIManager Instance;

    [SerializeField] private GameObject listObject;
    [SerializeField] private Transform listParent;
    [SerializeField] private GameObject prefabSlot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void Show()
    {
        listObject.SetActive(true);
    }

    public void Hide()
    {
        listObject.SetActive(false);
    }

    public void UpdateValues()
    {
        var clientDataList = new System.Collections.Generic.List<ClientData>();

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject.TryGetComponent(out PlayerNetwork playerNetwork))
            {
                var clientData = new ClientData
                {
                    PlayerName = DataSync.instance.playername,
                    PlayerPoints = playerNetwork.GetPoints().ToString(),
                    PlayerLifes = playerNetwork.GetLifes().ToString()
                };
                clientDataList.Add(clientData);
            }
            else
            {
                print("Cliente não possui PlayerNetwork.");
            }
        }
        UpdateValuesClientRpc(clientDataList.ToArray());
    
    }

    [ClientRpc]
    private void UpdateValuesClientRpc(ClientData[] clientDataList)
    {
        foreach (Transform child in listParent)
        {
            Destroy(child.gameObject);
        }
        foreach (var clientData in clientDataList)
        {
            GameObject newSlot = Instantiate(prefabSlot, listParent);
            newSlot.GetComponent<PlayersFeedSlot>().SetValues(clientData.PlayerName, clientData.PlayerPoints, clientData.PlayerLifes);
        }
    }
}
