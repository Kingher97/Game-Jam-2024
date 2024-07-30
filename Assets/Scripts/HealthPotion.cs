using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    public GameObject model; // Reference to the model child
    public GameObject effect; // Reference to the effect child

    public AudioClip potionSound;

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

        potionSound = Resources.Load<AudioClip>("potion");
    }

    private void potionAudio()
    {
        GameObject tempAudio = new GameObject("TempAudio");
        AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
        audioSource.clip = potionSound;
        audioSource.Play();

        Destroy(tempAudio, audioSource.clip.length);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Assumes the player has the tag "Player"
        {
            Character character = other.GetComponent<Character>();
            if (character != null)
            {
                character.IncreaseHealthSlider(35f);
                ConsumePotion();
                potionAudio();
            }
        }
    }

    private void ConsumePotion()
    {
        if (model != null)
        {
            Destroy(model);
            Debug.Log("Health potion model destroyed."); // Debug message
        }

        if (particleEffect != null)
        {
            particleEffect.Play();
            Debug.Log("Health potion effect played."); // Debug message
            Destroy(gameObject, particleEffect.main.duration); // Destroy the entire potion object after the effect finishes
        }
        else
        {
            Debug.LogError("Particle effect is null.");
        }
    }
}
