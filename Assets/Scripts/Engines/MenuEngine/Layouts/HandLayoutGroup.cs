using UnityEngine;
using UnityEngine.UI;

public class HandLayoutGroup : LayoutGroup
{
    public float spacing = 30f;
    public float curve = 20f; // rotation curve

    public override void CalculateLayoutInputHorizontal() { Arrange(); }
    public override void CalculateLayoutInputVertical() { Arrange(); }
    public override void SetLayoutHorizontal() { Arrange(); }
    public override void SetLayoutVertical() { Arrange(); }

    private void Arrange()
    {
        if (transform.childCount == 0) return;

        float center = (transform.childCount - 1) * spacing * 0.5f;

        for (int i = 0; i < transform.childCount; i++)
        {
            RectTransform child = (RectTransform)transform.GetChild(i);

            float x = (i * spacing) - center;
            float angle = (i - (transform.childCount - 1) / 2f) * curve;

            SetChildAlongAxis(child, 0, x);
            SetChildAlongAxis(child, 1, 0);

            child.localRotation = Quaternion.Euler(0, 0, -angle);
        }
    }
}