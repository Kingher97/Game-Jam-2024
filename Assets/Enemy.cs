using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public GameObject particleEffectGameObject;
    public Slider healthBarSlider;
    public float maxHealth = 100f;
    private float currentHealth;

    private ParticleSystem particleEffect;
    public Transform player; // Reference to the player GameObject

    public Animator angelAnim;

    void Start()
    {
        currentHealth = maxHealth;

        if (particleEffectGameObject != null)
        {
            particleEffect = particleEffectGameObject.GetComponent<ParticleSystem>();
        }
        else
        {
            Debug.LogError("Particle Effect GameObject not assigned.");
        }

        if (healthBarSlider == null)
        {
            Debug.LogError("Health bar slider not assigned.");
        }
        else
        {
            UpdateHealthBar(); // Initial update
        }

        Debug.Log("Enemy initialized with max health: " + maxHealth);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            PlayEffect();
        }

        if (player != null)
        {
            // Rotate to face the player only on the Y axis
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0; // Ignore the Y component
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("ball"))
        {
            PlayEffect();
            angelAnim.SetTrigger("damage");
            ReduceHealth(50f); // Reduce health by 50 units
        }
    }

    private void PlayEffect()
    {
        if (particleEffect != null)
        {
            particleEffect.Play();
        }
        else
        {
            Debug.LogError("Particle effect is null.");
        }
    }

    private void ReduceHealth(float amount)
    {
        if (currentHealth > 0)
        {
            Debug.Log("Reducing health by: " + amount);
            StartCoroutine(SmoothHealthDecrease(amount));
        }
    }

    private IEnumerator SmoothHealthDecrease(float amount)
    {
        float targetHealth = currentHealth - amount;
        float duration = 1f; // Duration for the health decrease effect
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            currentHealth = Mathf.Lerp(currentHealth, targetHealth, elapsed / duration);
            UpdateHealthBar();
            Debug.Log("Current health: " + currentHealth);
            yield return null;
        }

        currentHealth = targetHealth;
        Debug.Log("Final health: " + currentHealth);
        UpdateHealthBar(); // Final update
    }

    private void UpdateHealthBar()
    {
        if (healthBarSlider != null)
        {
            healthBarSlider.value = currentHealth / maxHealth;
            Debug.Log("Updating health bar to: " + healthBarSlider.value);

            if (currentHealth <= 0)
            {
                TriggerDeathAnimation();
            }
        }
        else
        {
            Debug.LogError("Health bar slider is null.");
        }
    }

    private void TriggerDeathAnimation()
    {
        if (angelAnim != null)
        {
            angelAnim.SetTrigger("death");
            Debug.Log("Death animation triggered.");
            Destroy(healthBarSlider.gameObject); // Destroy the health bar
            Destroy(gameObject, 2f); // Destroy the game object after 2 seconds
        }
        else
        {
            Debug.LogError("Animator is null.");
        }
    }
}
