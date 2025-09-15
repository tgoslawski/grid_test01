using UnityEngine;
using System.Collections;

public class TileAnimator : MonoBehaviour
{
    [SerializeField] private float duration = 0.2f;

    void OnEnable()
    {
        StartCoroutine(AnimateScale());
    }

    IEnumerator AnimateScale()
    {
        float t = 0f;
        Vector3 start = Vector3.zero;
        Vector3 end = Vector3.one;
        while (t < duration)
        {
            transform.localScale = Vector3.Lerp(start, end, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        transform.localScale = end;
    }
}