using UnityEngine;
using UnityEngine.UI;

public class BossHPBar : MonoBehaviour
{
    public Slider hpSlider;

    public void SetHealth(float normalizedHealth)
    {
        if (hpSlider != null)
            hpSlider.value = normalizedHealth;
    }
}
