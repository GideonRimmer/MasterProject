using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class ProgressBar : MonoBehaviour
{
    public float minProgress;
    public float maxProgress;
    public float currentProgress;
    public Image maskImage;
    public Image fillImage;
    public Color barColor;

    void Update()
    {
        GetCurrentFill();
    }

    void GetCurrentFill()
    {
        // TODO: Set min / max limits to progress bar.

        // Offset: Starting the bar at a point higher than the minimum.
        // (E.g: Useful for XP bars where the player already has XP from previous levels and doesn't start at 0).
        float currentOffset = currentProgress - minProgress;
        float maximumOffset = maxProgress - minProgress;

        float fillAmount = currentOffset / maximumOffset;
        maskImage.fillAmount = fillAmount;

        fillImage.color = barColor;
    }
}
