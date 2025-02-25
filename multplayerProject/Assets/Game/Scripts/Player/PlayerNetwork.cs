using System;
using System.Collections;
using Unity.Netcode;
using Unity.Services.Lobbies;
using UnityEngine;



public struct ClientData : INetworkSerializable
{
    public string PlayerName;
    public string PlayerPoints;
    public string PlayerLifes;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref PlayerPoints);
        serializer.SerializeValue(ref PlayerLifes);
    }
}


public class PlayerData : NetworkBehaviour
{
    // Usando NetworkVariable para sincronizar dados entre servidor e clientes
    public NetworkVariable<string> PlayerName = new NetworkVariable<string>();

    // Construtor (ainda que o construtor não seja essencial para a sincronização)
    public PlayerData(string playerName)
    {
        PlayerName.Value = playerName;
    }

    // Inicializa ou configura algo quando o objeto é instanciado
    public override void OnNetworkSpawn()
    {
        if (IsOwner)  // Verifica se o objeto é o dono (o jogador que controla essa instância)
        {
            // Aqui você pode inicializar a string de alguma forma, por exemplo:
            PlayerName.Value = "Nome do Jogador";  // Define um valor inicial
        }
    }
}

public class PlayerNetwork : NetworkBehaviour
{
    public CameraEffectsManager cameraEffectsManager;

    private Rigidbody rb;
    public float speed;

    [SerializeField] private NetworkVariable<int> lifes = new NetworkVariable<int>(5, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] private NetworkVariable<int> points = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] private Color otherPlayersColor;

    ClientData ClientData;
    private void Update()
    {
        if (IsOwner) HandleMovement();
        if (Input.GetKeyDown(KeyCode.V)){
            print(ClientData.PlayerName);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            lifes.Value = 5;
           
        }
        if (!IsOwner)
        {
            gameObject.name = "--- Jogador 2 ---";
            var meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {

                meshRenderer.material.color = otherPlayersColor;
            }
            return;
        }
        else
        {
            PlayerSttsUIManager.instance.UpdateUIValues(this);
            rb = GetComponent<Rigidbody>();
            gameObject.name = "--- Jogador 1 ---";
        }
        base.OnNetworkSpawn();

        points.OnValueChanged += AddPointDebug;
        lifes.OnValueChanged += ReduceLifeDebug;

        points.OnValueChanged += UpdatePlayerSttsUIValues;
        lifes.OnValueChanged += UpdatePlayerSttsUIValues;

        NetworkManager.gameObject.GetComponent<UImanager>().DisableButtons();

        PlayersFeedUIManager.Instance.Show();
        StartCoroutine(WaitToTry()); 

    }

    IEnumerator WaitToTry()
    {
        yield return new WaitForSeconds(0.5f);
        PlayersFeedUIManager.Instance.UpdateValues();
        foreach (var item in GameObject.FindGameObjectsWithTag("MainCamera"))
        {
            item.GetComponent<CameraController>().UpdatePlayersList();
        }
    }
    void UpdatePlayerSttsUIValues(int a, int b)
    {
        PlayerSttsUIManager.instance.UpdateUIValues(this);
    }
    public void AddPointToPoints()
    {
        if (IsServer)
        {
            points.Value++;
        }

        PlayPointEffectForClientServerRpc();
    }

    public void ReduceLifes()
    {
        if (IsServer)
        {
            lifes.Value--;
        }

        PlayHitEffectForClientServerRpc();
    }

    private void AddPointDebug(int oldValue, int newValue)
    {
        PlayersFeedUIManager.Instance.UpdateValues();
    }

    private void ReduceLifeDebug(int oldValue, int newValue)
    {
        PlayersFeedUIManager.Instance.UpdateValues();
    }

    public int GetPoints()
    {
        return points.Value;
    }

    public int GetLifes()
    {
        return lifes.Value;
    }

    public void Die()
    {
        if (lifes.Value <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void HandleMovement()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveDir = new Vector3(horizontal, 0f, vertical);

        moveDir = moveDir.normalized;

        if (moveDir != Vector3.zero && rb != null)
        {
            rb.linearVelocity = moveDir * speed;
        }
        else if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayPointEffectForClientServerRpc()
    {
        PlayPointEffectForClientsClientRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayHitEffectForClientServerRpc()
    {
        PlayHitEffectForClientsClientRpc();
    }

    [ClientRpc]
    public void PlayPointEffectForClientsClientRpc()
    {
        var playerEffects = GetComponent<PlayerEffectsManager>();
        if (playerEffects != null)
        {
            playerEffects.PlayPointEffect();
        }
    }

    [ClientRpc]
    public void PlayHitEffectForClientsClientRpc()
    {
        var playerEffects = GetComponent<PlayerEffectsManager>();
        if (playerEffects != null)
        {
            playerEffects.PlayHitEffect();
        }
    }
}
