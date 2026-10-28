using UnityEngine;

public class LossPanel : UIPanel
{
    private void Awake()
    {
        Hide();

        UIEvents.ShowLoss += Show;
        UIEvents.HideLoss += Hide;
    }

    private void OnDestroy()
    {
        UIEvents.ShowLoss -= Show;
        UIEvents.HideLoss -= Hide;
    }
}
