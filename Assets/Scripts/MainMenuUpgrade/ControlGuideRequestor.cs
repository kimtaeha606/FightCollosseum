using System;
using UnityEngine;

public class ControlGuideRequestor : MonoBehaviour
{
    public static event Action GuideRequested;
    public static event Action GuideCloseRequested;

    public void RequestGuide()
    {
        GuideRequested?.Invoke();
    }

    public void RequestCloseGuide()
    {
        GuideCloseRequested?.Invoke();
    }
}
