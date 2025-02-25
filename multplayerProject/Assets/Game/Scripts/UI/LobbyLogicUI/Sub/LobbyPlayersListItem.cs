using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayersListItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI lbl_name;
    [SerializeField] GameObject isHostMark;
    [SerializeField] Image color;
    public Button kickBtn;

    MenusUIManager menusUIManager;
    public void SetLobbyPlayersListItemData(Player playerData, bool isHost, MenusUIManager uiManager)
    {
        if (playerData.Data != null && playerData.Data.ContainsKey("PlayerName"))
        {
            lbl_name.text = playerData.Data["PlayerName"].Value;
        }
        else
        {
            lbl_name.text = "Unknown Player"; // Retorna um nome padrão se não encontrar o nome
        };

        isHostMark.SetActive(isHost);
        //kickBtn.gameObject.SetActive(!isHost);
        menusUIManager = uiManager;
        kickBtn.onClick.AddListener(() => kick(playerData));
    }
    void kick(Player player)
    {
        menusUIManager.KickPlayerFromLobby(player);
    }
}