using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbiesListItem : MonoBehaviour
{
    [SerializeField] MenusUIManager menusUIManager;
    [SerializeField] TextMeshProUGUI lbl_lobbyName;
    [SerializeField] TextMeshProUGUI lbl_playersJoinedQuantity;

    public void SetLobbiesListItemData(Lobby lobbyData, MenusUIManager uimang)
    {
        lbl_lobbyName.text = lobbyData.Name;
        lbl_playersJoinedQuantity.text = $"{lobbyData.Players.Count}/{lobbyData.MaxPlayers}";
        menusUIManager = uimang;

        Button button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();

        if (menusUIManager != null)
        {
            button.onClick.AddListener(() => menusUIManager.JoinLobbyByClick(lobbyData));
        }
        else
        {
            Debug.LogError("menusUIManager não foi atribuído corretamente!");
        }
    }
}
