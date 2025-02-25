using TMPro;
using UnityEngine;

public class CreateLobbyForm : MonoBehaviour
{
    [SerializeField] MenusUIManager menusUIManager;
    [SerializeField] TMP_InputField iptf_playerName;
    [SerializeField] TMP_InputField iptf_maxplayers;
    [SerializeField] TMP_InputField iptf_lobbyName;
    public void Submite()
    {
        menusUIManager.CreateLobbyFormSubmiteLobbyData(iptf_playerName.text, iptf_lobbyName.text, int.Parse(iptf_maxplayers.text));
    }
}