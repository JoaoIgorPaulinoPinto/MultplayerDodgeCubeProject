
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinByCodeForm  : MonoBehaviour
{
    [SerializeField] TMP_InputField iptf_name;
    [SerializeField] TMP_InputField iptf_code;
    [SerializeField] MenusUIManager uiManager;
    public void Submite()
    {
        uiManager.JoinByCodeSubmite(iptf_code.text, iptf_name.text);
    }   
}