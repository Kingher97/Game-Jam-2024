using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Slider healthBarSlider;
    public Transform healthBarPosition;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        if (healthBarSlider == null)
        {
            Debug.LogError("Health bar slider not assigned.");
        }

        if (healthBarPosition == null)
        {
            Debug.LogError("Health bar position not assigned.");
        }
    }

    void Update()
    {
        if (healthBarSlider != null && healthBarPosition != null)
        {
            healthBarSlider.transform.position = healthBarPosition.position;
            healthBarSlider.transform.LookAt(mainCamera.transform);
            healthBarSlider.transform.Rotate(0, 180, 0); // Correct the facing direction
        }
    }

    public void UpdateHealthBar(float healthPercentage)
    {
        if (healthBarSlider != null)
        {
            Debug.Log("Updating health bar to: " + healthPercentage);
            healthBarSlider.value = healthPercentage;
        }
        else
        {
            Debug.LogError("Health bar slider is null.");
        }
    }
}
