using UnityEngine;

public class ShadowPotion : MonoBehaviour
{
    public GameObject model; // Reference to the model child
    public GameObject effect; // Reference to the effect child

    private ParticleSystem particleEffect;

    void Start()
    {
        if (model == null)
        {
            Debug.LogError("Model not assigned.");
        }

        if (effect != null)
        {
            particleEffect = effect.GetComponent<ParticleSystem>();
            if (particleEffect == null)
            {
                Debug.LogError("Effect does not have a ParticleSystem component.");
            }
        }
        else
        {
            Debug.LogError("Effect not assigned.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Assumes the player has the tag "Player"
        {
            Character character = other.GetComponent<Character>();
            if (character != null)
            {
                character.IncreaseShadowSlider(35f);
                ConsumePotion();
            }
        }
    }

    private void ConsumePotion()
    {
        if (model != null)
        {
            Destroy(model);
            Debug.Log("Shadow potion model destroyed."); // Debug message
        }

        if (particleEffect != null)
        {
            particleEffect.Play();
            Debug.Log("Shadow potion effect played."); // Debug message
            Destroy(gameObject, particleEffect.main.duration); // Destroy the entire potion object after the effect finishes
        }
        else
        {
            Debug.LogError("Particle effect is null.");
        }
    }
}
