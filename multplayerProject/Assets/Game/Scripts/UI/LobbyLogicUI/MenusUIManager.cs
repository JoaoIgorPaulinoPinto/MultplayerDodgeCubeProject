using System.Collections;
using Newtonsoft.Json.Bson;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class MenusUIManager : MonoBehaviour
{

    public LobbyAPI LobbyAPI;

    public GameObject UI_LobbiesList;
    public GameObject UI_mainMenu;
    public GameObject UI_PlayerJoinConfig;
    public GameObject UI_CreateLobby;
    public GameObject UI_Lobby;
    public GameObject UI_joinByCodeForm;
    public GameObject UI_joinByClickForm;
    
    public void PlayGame()
    {
        UI_LobbiesList.SetActive(true);
        LobbyAPI.ListLobbies();
        UI_mainMenu.SetActive(false);
    }

    public void BackToMainMenu()
    {
        UI_mainMenu.SetActive(true);
        UI_LobbiesList.SetActive(false);
    }

    public void CreateLobby()
    {
        UI_CreateLobby.SetActive(true);
    }
    public async void CreateLobbyFormSubmiteLobbyData(string playerName, string lobbyName, int maxPlayers)
    {
        await LobbyAPI.CreateLobby(playerName, lobbyName, maxPlayers);
        UI_Lobby.SetActive(true);

        //adicionar os dados a UI da lobby

        UI_LobbiesList.SetActive(false);
    }

    public void JoinLobbyByCode()
    {
        UI_joinByCodeForm.SetActive(true);
    }
    public void JoinByCodeSubmite(string code, string name)
    {
        UI_Lobby.SetActive(true);
        UI_LobbiesList.SetActive(false);
        LobbyAPI.JoinLobbyByCode(code, name);
    }

    public void JoinLobbyByClick(Lobby lobbyData)
    {
        UI_joinByClickForm.SetActive(true);
        UI_joinByClickForm.GetComponent<JoinByClickForm>().SetLobbyData(lobbyData);
    }
    public void JoinLobbyByClickSubmite(Lobby lobbyData, string playername)
    {
        LobbyAPI.JoinLobbyByID(lobbyData.Id,playername);
        UI_Lobby.SetActive(true);
        UI_LobbiesList.SetActive(true);
    }
    public void StartGame()
    {
        LobbyAPI.StartGame();
        UI_Lobby.SetActive(false);
        UI_LobbiesList.SetActive(false);
    }
    public void KickPlayerFromLobby(Player player)
    {
        LobbyAPI.KickPlayer(player);
    }
    public void LeaveLobby()
    {
        UI_Lobby.SetActive(false);
        UI_LobbiesList.SetActive(true);
        UI_PlayerJoinConfig.SetActive(false);
        UI_CreateLobby.SetActive(false);

        LobbyAPI.LeaveLobby();
    }
}
