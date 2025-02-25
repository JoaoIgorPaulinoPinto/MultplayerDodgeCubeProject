using TMPro;
using UnityEngine;

public class PlayersFeedSlot : MonoBehaviour
{
    public TextMeshProUGUI lbl_name;
    public TextMeshProUGUI lbl_points;
    public TextMeshProUGUI lbl_lifes;

    public void SetValues(string name, string points, string lifes)
    {
        lbl_name.text = name;
        lbl_points.text = points;
        lbl_lifes.text = lifes;
    }
}
