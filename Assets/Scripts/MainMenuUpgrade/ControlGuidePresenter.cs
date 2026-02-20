using UnityEngine;
using UnityEngine.UI;

public class ControlGuidePresenter : MonoBehaviour
{
    [SerializeField] private GameObject guidePanel;
    [SerializeField] private string guidePanelName = "ControlGuidePanel";
    [SerializeField] private Image overlayImage;
    [SerializeField] private Color overlayColor = new Color(0f, 0f, 0f, 0.6f);

    private void Awake()
    {
        if (guidePanel == null && !string.IsNullOrEmpty(guidePanelName))
        {
            GameObject found = GameObject.Find(guidePanelName);
            if (found != null)
            {
                guidePanel = found;
            }
        }

        if (overlayImage == null)
        {
            overlayImage = GetComponent<Image>();
        }

        ApplyOverlayStyle();
    }

    private void OnEnable()
    {
        ControlGuideRequestor.GuideRequested += ShowGuide;
        ControlGuideRequestor.GuideCloseRequested += HideGuide;
    }

    private void OnDisable()
    {
        ControlGuideRequestor.GuideRequested -= ShowGuide;
        ControlGuideRequestor.GuideCloseRequested -= HideGuide;
    }

    public void ShowGuide()
    {
        if (guidePanel != null)
        {
            guidePanel.SetActive(true);
        }
    }

    public void HideGuide()
    {
        if (guidePanel != null)
        {
            guidePanel.SetActive(false);
        }
    }

    private void OnValidate()
    {
        if (overlayImage == null)
        {
            overlayImage = GetComponent<Image>();
        }

        ApplyOverlayStyle();
    }

    private void ApplyOverlayStyle()
    {
        if (overlayImage != null)
        {
            overlayImage.color = overlayColor;
        }
    }
}
