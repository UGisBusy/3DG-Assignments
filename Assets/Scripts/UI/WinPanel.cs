using UnityEngine;

public class WinPanel : UIPanel
{
    private void Awake()
    {
        Hide();

        UIEvents.ShowWin += Show;
        UIEvents.HideWin += Hide;
    }

    private void OnDestroy()
    {
        UIEvents.ShowWin -= Show;
        UIEvents.HideWin -= Hide;
    }
}
