using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashEffectScript : MonoBehaviour
{
    public float fadeDuration = 0.3f;
    private SpriteRenderer sr;
    private Color originalColor;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        StartCoroutine(FadeOut());
    }

    System.Collections.IEnumerator FadeOut()
    {
        float time = 0;
        while (time < fadeDuration)
        {
            float alpha = Mathf.Lerp(originalColor.a, 0, time / fadeDuration);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            time += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
