using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

using UnityEngine;
using UnityEngine.UI;


public class LobbyAPI : MonoBehaviour
{
    // Variaveis gerais
    //[Header("Geral")]
    [HideInInspector] public string playerName = "Unknow Player";
    [SerializeField] Lobby hostLobby;
    [SerializeField] Lobby joinedLobby;
    [SerializeField] float heartBeatTimer;
    [SerializeField] float lobbyUpdateTimer;
    [SerializeField] float lobbiesListUpdateTimer;

    [Space]

    // Variaveis para controle da UI na lista de lobbies 
    [Header("Controle da UI na lista de lobbies")]
    [SerializeField] GameObject lobbiesListItemPreab;
    [SerializeField] Transform lobbiesLisItemsParent;

    [Space]

    // Variaveis para controle da UI na lista de lobbies 
    [Header("Controle da UI no Lobby")]
    [SerializeField] GameObject playersListItemPrefab;
    [SerializeField] GameObject startGameButton;
    [SerializeField] Transform playersLisItemsParent;
    [SerializeField] TextMeshProUGUI lobbyName;
    [SerializeField] TextMeshProUGUI lobbyCode;
    [SerializeField] TextMeshProUGUI playersInlobbyshower;

    [Space]
    [Header("UI MANAGER")]
    [SerializeField] MenusUIManager menusUiManger;

    [Space]
    public RelayTest relayManager;

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        try
        {
            // Aguardar a autenticação ser completada
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Player signed in: " + AuthenticationService.Instance.PlayerId);
            playerName = "Jogador" + UnityEngine.Random.Range(10, 99).ToString();
        }
        catch (Exception e)
        {
            Debug.LogError("Authentication failed: " + e.Message);
        }
    }

    private bool lobbiesListUpdated = false;
    private float nextLobbiesUpdateTime = 0f;
    private float lobbiesUpdateInterval = 10f; // 10 segundos

    void LateUpdate()
    {
        Timer();
        HandleLobbyPollForUpdates();

        if (Time.time >= nextLobbiesUpdateTime)
        {
            StartCoroutine(UpdateLobbiesListRotine());
            nextLobbiesUpdateTime = Time.time + lobbiesUpdateInterval;
        }
    }

    IEnumerator UpdateLobbiesListRotine()
    {
        yield return new WaitForSeconds(3);
        ListLobbies();
        lobbiesListUpdated = true;
    }

    async void Timer()
    {
        if (hostLobby != null)
        {
            heartBeatTimer -= Time.deltaTime;
            if (heartBeatTimer <= 0f)
            {
                float heartBeatTimerMax = 15;
                heartBeatTimer = heartBeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
                print("heartbeat sended");
            }
        }
    }
    async void HandleLobbyPollForUpdates()
    {
        if (joinedLobby != null)
        {
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer <= 0f)
            {
                float lobbyUpdateTimerMax = 1.1f;
                lobbyUpdateTimer = lobbyUpdateTimerMax;

             
                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                joinedLobby = lobby;


                UpdateLobbyUI(joinedLobby);

            }
        }
        else if (hostLobby != null)
        {
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer <= 0f)
            {
                float lobbyUpdateTimerMax = 1.1f;
                lobbyUpdateTimer = lobbyUpdateTimerMax;

               
                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(hostLobby.Id);
                hostLobby = lobby;

                
                UpdateLobbyUI(hostLobby);
            }
        }
    }

    // Cria lobby
    public async Task<bool> CreateLobby(string playername, string lobbyname, int maxPlayers)
    {
        try
        {
            playerName = playername;
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = GetPlayer()
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyname, maxPlayers, createLobbyOptions);
            lobbyName.text = lobby.Name;    
            lobbyCode.text = lobby.LobbyCode;
            hostLobby = lobby;
            UpdateLobbyUI(lobby);
            startGameButton.SetActive(lobby.HostId == GetPlayer().Id);
            return true;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
            return false;
        }
    }

    public async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Count = 24,
                Filters = new List<QueryFilter> {
                new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
            },
                Order = new List<QueryOrder>
            {
                new QueryOrder(false, QueryOrder.FieldOptions.Created)
            }
            };

            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();

            // Limpa os itens existentes
            foreach (Transform child in lobbiesLisItemsParent)
            {
                Destroy(child.gameObject);
            }

            // Cria novos itens da lista
            foreach (var result in queryResponse.Results)
            {
                GameObject newItemOnList = Instantiate(lobbiesListItemPreab, lobbiesLisItemsParent);
                newItemOnList.GetComponent<LobbiesListItem>().SetLobbiesListItemData(result, menusUiManger);
            }

            // Após a primeira atualização, marca como atualizado
            lobbiesListUpdated = true;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }

    public void ResetLobbiesUpdateFlag()
    {
        lobbiesListUpdated = false;
    }

    // Entra na lobby usando um codigo
    public async void JoinLobbyByCode(string lobbyCode, string playername)
    {
        try
        {
            playerName = playername;
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };
            Lobby joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            //Lobby joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);

            UpdateLobbyUI(joinedLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }

    // Entra na Lobby com ID 
    public async void JoinLobbyByID(string ID, string playername)
    {
        try
        {
            playerName = playername;
            JoinLobbyByIdOptions joinLobbyByCodeOptions = new JoinLobbyByIdOptions
            {
                Player = GetPlayer()
            };
            Lobby joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(ID, joinLobbyByCodeOptions);
            //Lobby joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);

            UpdateLobbyUI(joinedLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }

    // Entra na lobby qualquer
    public async void QuickJoinInLobby()
    {
        try
        {
            await LobbyService.Instance.QuickJoinLobbyAsync();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }

    // Atualiza a UI do lobby
    void UpdateLobbyUI(Lobby lobby)
    {
        if (lobby == null) return;

        lobbyName.text = lobby.Name;
        lobbyCode.text = lobby.LobbyCode;
        playersInlobbyshower.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";

        // Limpa a lista de jogadores no UI
        for (int i = playersLisItemsParent.childCount - 1; i >= 0; i--)
        {
            Destroy(playersLisItemsParent.GetChild(i).gameObject);
        }

        Player currentPlayer = GetPlayer(); // Evita chamar GetPlayer() múltiplas vezes
        bool isCurrentPlayerHost = currentPlayer != null && currentPlayer.Id == lobby.HostId;

        foreach (Player player in lobby.Players)
        {
            GameObject newOBJ = Instantiate(playersListItemPrefab, playersLisItemsParent);
            LobbyPlayersListItem item = newOBJ.GetComponent<LobbyPlayersListItem>();
            GameObject kickButton = newOBJ.GetComponentInChildren<LobbyPlayersListItem>().kickBtn.gameObject;

            bool isHost = (lobby.HostId == player.Id);
            string playerName = player.Data != null && player.Data.ContainsKey("PlayerName")
                                ? player.Data["PlayerName"].Value
                                : "Unknown Player";

            item.SetLobbyPlayersListItemData(player, isHost, menusUiManger);

            // Se o jogador atual for o host, ativa o botão, senão desativa
            if (kickButton != null)
            {
                kickButton.SetActive(isCurrentPlayerHost);
            }
        }
    }



    // Pega as informações do player
    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>{
                        {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)}
                    }
        };
    }

    // Pega as informacoes do jogador pelo seu ID
    private Player GetPlayerByPlayerID()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>{
                        {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)}
                    }
        };
    }
    
    // Modifica o nome do jogador 
    async void UpdatePlayerName(string newPlayerName)
    {
        playerName = newPlayerName;
        try
        {
            await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject> {

                 {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)}

                }       

            });
        }
        catch (LobbyServiceException e)
        {
            {
                Debug.LogException(e);
            }
        }
    }

    // Deixa o Lobby
    public async void LeaveLobby()
    {
        try
        {
            if (joinedLobby != null)
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            }
            else if(hostLobby != null)
            {
                DeleteLobby();
            }
                ListLobbies();
           
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }

    }

    // Retira algum jogador da lobby
    public async void KickPlayer(Player player)
    {
        try
        {
            StartCoroutine(KickPlayerRotine(player));
            await LobbyService.Instance.RemovePlayerAsync(hostLobby.Id, player.Id);
        
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }

    void KickedFromLobby(Player kickedPlayer)
    {
        if (GetPlayer().Id == kickedPlayer.Id)
        {
            menusUiManger.UI_Lobby.SetActive(false);
        }
    }
    IEnumerator KickPlayerRotine(Player player)
    {
        yield return new WaitForSeconds(0.5f);
        KickedFromLobby(player);
        if (joinedLobby != null) UpdateLobbyUI(joinedLobby); else if (hostLobby != null) UpdateLobbyUI(hostLobby);
    }
    // Torna outro jogador o Host
    async void MigrateLobbyHost()
    {
        try
        {
            hostLobby = await LobbyService.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                HostId = joinedLobby.Players[1].Id
            });
            joinedLobby = hostLobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }

    // Apaga a lobby
   async void DeleteLobby()
    {
        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(hostLobby.Id);

        }catch(LobbyServiceException e )
        {
            Debug.LogException(e);
        }
    }

    // Inicia o jogo
    public void StartGame()
    {
        Player player = GetPlayer();

        bool isHost = (hostLobby.HostId == player.Id);
        if (hostLobby != null)
        {
            relayManager.lobbyKey = hostLobby.LobbyCode;
            relayManager.CreateRelay();
            DataSync.instance.SetClientDataName(playerName);  // Chama a função para sincronizar o nome do jogador
        }
        if (joinedLobby != null)
        {
            relayManager.lobbyKey = joinedLobby.LobbyCode;
            relayManager.JoinRelay();
            DataSync.instance.SetClientDataName(playerName);  // Chama a função para sincronizar o nome do jogador

        }
    }
}
