using UnityEngine;
using UnityEngine.UI;

public class MoneyIconLoader : MonoBehaviour
{
    [SerializeField] private RawImage targetImage;
    [SerializeField] private string resourcePath = "MoneyIcon";

    private void Awake()
    {
        if (targetImage == null)
        {
            targetImage = GetComponent<RawImage>();
        }

        if (targetImage == null)
        {
            return;
        }

        Texture2D iconTexture = Resources.Load<Texture2D>(resourcePath);
        if (iconTexture != null)
        {
            targetImage.texture = iconTexture;
        }
    }
}
