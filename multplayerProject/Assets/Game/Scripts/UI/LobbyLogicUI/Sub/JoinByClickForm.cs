using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class JoinByClickForm : MonoBehaviour
{
    [SerializeField] TMP_InputField playername;
    [SerializeField] MenusUIManager menusUIManager;
    Lobby lobby;
    public void submite()
    {
        menusUIManager.JoinLobbyByClickSubmite(lobby, playername.text);
    }
    public void SetLobbyData(Lobby lobby)
    {
        this.lobby = lobby; 
    }
}