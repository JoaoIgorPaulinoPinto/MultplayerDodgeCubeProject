using TMPro;

using UnityEngine;

public class PlayerSttsUIManager : MonoBehaviour
{
    public static PlayerSttsUIManager instance;
   
    [SerializeField] TextMeshProUGUI lbl_lvl;
    [SerializeField] TextMeshProUGUI lbl_pnts;
    [SerializeField] TextMeshProUGUI lbl_lfs;

    public void UpdateUIValues(PlayerNetwork? player)
    {
        if(!player) {lbl_lvl.text = WallsManager.Instance.GetLevel().ToString(); return; }

        lbl_lfs.text = player.GetLifes().ToString();
        lbl_pnts.text = player.GetPoints().ToString();
        lbl_lvl.text = WallsManager.Instance.GetLevel().ToString();
    }
    private void Awake()
    {
        if (instance)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
}
