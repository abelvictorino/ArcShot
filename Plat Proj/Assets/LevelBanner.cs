using System.Collections;
using UnityEngine;
using TMPro;

public class LevelBanner : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private string text = "LEVEL 1";
    [SerializeField] private float hold = 0.8f;
    [SerializeField] private float fade = 0.3f;

    IEnumerator Start()
    {
        if (!label) yield break;
        label.text = text;
        Color c = label.color; c.a = 0; label.color = c;

        // fade in
        float t = 0f;
        while (t < fade) { t += Time.deltaTime; c.a = Mathf.Lerp(0, 1, t/fade); label.color = c; yield return null; }
        yield return new WaitForSeconds(hold);

        // fade out
        t = 0f;
        while (t < fade) { t += Time.deltaTime; c.a = Mathf.Lerp(1, 0, t/fade); label.color = c; yield return null; }
    }
}
