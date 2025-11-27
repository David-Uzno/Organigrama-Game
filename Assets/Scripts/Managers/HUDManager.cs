using UnityEngine;
using UnityEngine.UI;
public class HUDManager : MonoBehaviour
{
    [SerializeField] private Image[] heartImages;
    [SerializeField] private Sprite[] heartStatuses;

    public Image[] GetHeartImages() => heartImages;
    public Sprite[] GetHeartStatuses() => heartStatuses;
}

