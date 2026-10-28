using UnityEngine;

public class IntroPanel : UIPanel
{
    private void Awake()
    {
        Hide();

        UIEvents.ShowIntro += Show;
        UIEvents.HideIntro += Hide;
    }

    private void OnDestroy()
    {
        UIEvents.ShowIntro -= Show;
        UIEvents.HideIntro -= Hide;
    }
}
