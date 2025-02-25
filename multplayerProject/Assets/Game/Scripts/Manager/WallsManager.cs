using System.Collections;
using Unity.Netcode;
using UnityEngine;

[System.Serializable]
public class Spawns
{
    public Transform point;
    public Wall[] availableWalls;

    public Spawns(Transform _point, Wall[] _availableWalls)
    {
        point = _point;
        availableWalls = _availableWalls;
    }
}

[System.Serializable]
public class Wall
{
    public GameObject obj;
    public int levelStart;
    public float wallSpeed;
    public float XmodifyerIndex;
}

[RequireComponent(typeof(NetworkObject))]
public class WallsManager : NetworkBehaviour
{

    [SerializeField] NetworkVariable<int> level = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


    public float maxXdif;
    public Spawns[] spwns;
    public Transform center;

    public float timewait;
    public bool wait;

    public float moveSpeedAditionalIndex;
    public float timeWaitReduceIndex;
    public float timeToUpdateDificultLevel;

    private float speedAdded = 0;
    private float timeWaitReduced = 0;
    private float lastUpdateTime = 0f;
    public static WallsManager Instance;
    private void Awake()
    {
        level.OnValueChanged += UpdatePlayerSttsUIValues;
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    public int GetLevel()
    {
        return level.Value;
    }
    void UpdatePlayerSttsUIValues(int a, int b)
    {
        PlayerSttsUIManager.instance.UpdateUIValues(null);
    }
    private void Update()
    {
        // A lógica de spawn e dificuldade deve ser gerenciada apenas pelo servidor
        if (!IsServer)
            return;

        // Verifica o tempo decorrido desde a última atualização
        if (Time.time - lastUpdateTime >= timeToUpdateDificultLevel)
        {
            // Incrementa os índices de dificuldade
            speedAdded += moveSpeedAditionalIndex;
            timeWaitReduced += timeWaitReduceIndex;

            // Incrementa o nível do jogador
            level.Value++;

            // Atualiza o tempo da última verificação
            lastUpdateTime = Time.time;
        }

        // Lógica de spawn
        if (!wait)
        {
            StartCoroutine(Lauch());
        }
    }

    private IEnumerator Lauch()
    {
        wait = true;
        CreateWallServerRpc(); // Chama a RPC para criar a parede
        yield return new WaitForSeconds(timewait - timeWaitReduced);
        wait = false;
    }

    [ServerRpc]
    private void CreateWallServerRpc()
    {
        Spawns selectedSpwn = spwns[Random.Range(0, spwns.Length)];

        Wall[] availableWallsForLevel = System.Array.FindAll(
            selectedSpwn.availableWalls,
            wall => wall.levelStart <= level.Value

        );

        if (availableWallsForLevel.Length == 0)
        {
            return;
        }

        Wall selectedWallData = availableWallsForLevel[Random.Range(0, availableWallsForLevel.Length)];
        GameObject selectedWall = selectedWallData.obj;

        Vector3 spawnPosition = selectedSpwn.point.position;

        int randomCorner = Random.Range(0, 4);
        switch (randomCorner)
        {
            case 0: // Canto superior direito
                spawnPosition.x += selectedWallData.XmodifyerIndex;
                spawnPosition.z += selectedWallData.XmodifyerIndex;
                break;
            case 1: // Canto superior esquerdo
                spawnPosition.x -= selectedWallData.XmodifyerIndex;
                spawnPosition.z += selectedWallData.XmodifyerIndex;
                break;
            case 2: // Canto inferior direito
                spawnPosition.x += selectedWallData.XmodifyerIndex;
                spawnPosition.z -= selectedWallData.XmodifyerIndex;
                break;
            case 3: // Canto inferior esquerdo
                spawnPosition.x -= selectedWallData.XmodifyerIndex;
                spawnPosition.z -= selectedWallData.XmodifyerIndex;
                break;
        }

        // Instanciar o objeto mantendo toda a hierarquia do prefab
        GameObject createdWall = Instantiate(selectedWall, spawnPosition, selectedWall.transform.rotation);
        NetworkObject networkObject = createdWall.GetComponent<NetworkObject>();
        networkObject.Spawn(true); // Garante que o objeto seja sincronizado com os clientes
        
        //Destroy(createdWall);

        // Configura o objeto no servidor
        SetupWallClientRpc(networkObject.NetworkObjectId, selectedWallData.wallSpeed + speedAdded);
    }

    [ClientRpc]
    private void SetupWallClientRpc(ulong networkObjectId, float wallSpeed)
    {
        NetworkObject networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];

        if (networkObject != null)
        {
            GameObject createdWall = networkObject.gameObject;
           // createdWall.transform.SetParent(center);

            if (createdWall.TryGetComponent<WallMovimentController>(out WallMovimentController wallControler))
            {
                wallControler.MoveWallServerRpc(center.position, wallSpeed);
            }
        }
    }
}
