using UnityEngine;

public class GamePanel : UIPanel
{
    private void Awake()
    {
        Hide();

        UIEvents.ShowGame += Show;
        UIEvents.HideGame += Hide;
    }

    private void OnDestroy()
    {
        UIEvents.ShowGame -= Show;
        UIEvents.HideGame -= Hide;
    }
}
