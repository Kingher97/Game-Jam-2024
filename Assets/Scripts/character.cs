using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float spellRange = 100f;

    public GameObject gameOverScreen;
    public Image crosshairImage;
    public Slider healthSlider; // Reference to the health slider
    public Slider shadowSlider;

    private Rigidbody rb;

    public Animator playerAnim;
    public Animator healthBarAnim;
    public Animator shadowBarAnim;

    private Camera mainCamera;
    public float horizontalOffset = 2f;

    public GameObject shadowBody;
    public GameObject shadowBallCast;
    public GameObject shadowBall;

    public GameObject particleEffectGameObject;
    private ParticleSystem particleEffect;

    public float maxHealth = 100f;
    private float currentHealth;

    private Coroutine lightDamageCoroutine; // Coroutine reference for continuous light damage

    public AudioClip spellSound;
    public AudioClip spellSound2;
    public AudioClip hitSound;
    public AudioSource runningAudioSource;
    public AudioSource jumpAudioSource;

    private bool isGrounded; // Track if player is on the ground
    private bool isCastingSpell; // Track if player is casting a spell
    private bool isCooldown; // Track if player is in cooldown state

    void Start()
    {
        currentHealth = maxHealth;

        rb = GetComponent<Rigidbody>();

        mainCamera = Camera.main;
        if (crosshairImage != null)
        {
            crosshairImage.enabled = true;
        }

        if (particleEffectGameObject != null)
        {
            particleEffect = particleEffectGameObject.GetComponent<ParticleSystem>();
        }
        else
        {
            Debug.LogError("Particle Effect GameObject not assigned.");
        }

        spellSound = Resources.Load<AudioClip>("spell2");
        spellSound2 = Resources.Load<AudioClip>("spell1");
        hitSound = Resources.Load<AudioClip>("hit");
    }

    void Update()
    {
        if (currentHealth > 0 && !isCastingSpell && !isCooldown)
        {
            Move();
            HandleJump();
        }

        HandleSpellCasting();
    }

    void Move()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = transform.forward * moveVertical + transform.right * moveHorizontal;
        movement.y = 0;

        rb.MovePosition(transform.position + movement * moveSpeed * Time.deltaTime);

        if (moveHorizontal != 0 || moveVertical != 0)
        {
            playerAnim.SetBool("run", true);
            if (!runningAudioSource.isPlaying)
            {
                runningAudioSource.Play();
            }
        }
        else
        {
            playerAnim.SetBool("run", false);
            if (runningAudioSource.isPlaying)
            {
                runningAudioSource.Stop();
            }
        }
    }

    void HandleJump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            playerAnim.SetTrigger("jump");
            StartCoroutine(DelayedJump());
        }
    }

    IEnumerator DelayedJump()
    {
        yield return new WaitForSeconds(0.3f); // 1 second delay
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        jumpAudioSource.Play();
        isGrounded = false; // Player is now in the air
    }

    void HandleSpellCasting()
    {
        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
        {
            float requiredShadow = 0f;
            requiredShadow = shadowSlider.maxValue * 0.1f;
          
            if (shadowSlider.value >= requiredShadow)
            {
                isCastingSpell = true; // Player starts casting a spell

                playerAnim.SetTrigger("spell");
                shadowBarAnim.SetTrigger("shake");
                StartCoroutine(ExecuteAfterDelay(1.0f));
                StartCoroutine(ReduceShadowSlider(shadowSlider.maxValue * 0.1f));
               
            }
            else
            {
                Debug.Log("Not enough shadow energy.");
            }
        }
    }

    private IEnumerator ExecuteAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        TriggerParticleSystem(shadowBallCast);
        CastShadowBall();
        isCastingSpell = false; // Player finishes casting a spell
    }

    void TriggerParticleSystem(GameObject particleSystemObject)
    {
        if (particleSystemObject != null)
        {
            ParticleSystem ps = particleSystemObject.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }
            else
            {
                Debug.LogWarning("Il GameObject non contiene un ParticleSystem.");
            }
        }
        else
        {
            Debug.LogWarning("Il riferimento al GameObject è nullo.");
        }
    }

    void CastShadowBall()
    {
        Vector3 crosshairWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(crosshairImage.rectTransform.position.x, crosshairImage.rectTransform.position.y, mainCamera.nearClipPlane));

        Vector3 direction = (mainCamera.transform.forward).normalized;
        Vector3 offset = mainCamera.transform.position + direction * 5.0f;

        Ray ray = new Ray(offset, direction);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, spellRange))
        {
            if (hit.collider != null && hit.collider.CompareTag("mob"))
            {
                Destroy(hit.collider.gameObject);
            }
            FireShadowBall(ray.origin, hit.point);
        }
        else
        {
            FireShadowBall(ray.origin, ray.origin + ray.direction * spellRange);
        }
    }

    void FireShadowBall(Vector3 start, Vector3 end)
    {
        GameObject projectile = Instantiate(shadowBall, start, Quaternion.identity);
        projectile.SetActive(true);
        projectile.GetComponent<Rigidbody>().velocity = (end - start).normalized * spellRange * 0.5f;
        projectile.AddComponent<ShadowBall>();
        CastSpell();
    }

    private void CastSpell()
    {
        GameObject tempAudio = new GameObject("TempAudio");
        AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
        audioSource.clip = spellSound;
        audioSource.Play();

        Destroy(tempAudio, audioSource.clip.length);
    }

    private void CastSpell2()
    {
        GameObject tempAudio = new GameObject("TempAudio");
        AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
        audioSource.clip = spellSound2;
        audioSource.Play();

        Destroy(tempAudio, audioSource.clip.length);
    }

    private void hitSpell()
    {
        GameObject tempAudio = new GameObject("TempAudio");
        AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
        audioSource.clip = hitSound;
        audioSource.Play();

        Destroy(tempAudio, audioSource.clip.length);
    }

    IEnumerator EndSpellAnimation()
    {
        yield return new WaitForSeconds(2.3f); // spell animation duration
        isCastingSpell = false; // Player finishes casting a spell
    }

    void Die()
    {
        playerAnim.SetTrigger("death");
        StartCoroutine(ShowGameOverScreen());
        StartCoroutine(ReduceHealthSlider());
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "fall")
        {
            Die();
        }

        if (other.gameObject.CompareTag("ballEnemy"))
        {
            PlayEffect();
            playerAnim.SetTrigger("damage");
            hitSpell();
            ReduceHealth(20f);
            StartCoroutine(Cooldown(1.0f)); // Start cooldown when hit by enemy ball
        }

        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; // Player is back on the ground
        }
    }

    private IEnumerator Cooldown(float duration)
    {
        isCooldown = true;
        yield return new WaitForSeconds(duration);
        isCooldown = false;
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
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / maxHealth;
            Debug.Log("Updating health bar to: " + healthSlider.value);

            if (currentHealth <= 0)
            {
                Die();
            }
        }
        else
        {
            Debug.LogError("Health bar slider is null.");
        }
    }

    IEnumerator ShowGameOverScreen()
    {
        yield return new WaitForSeconds(2.0f);
        gameOverScreen.SetActive(true);
        Debug.Log("Game OVER!");
    }

    IEnumerator ReduceHealthSlider()
    {
        while (healthSlider.value > 0)
        {
            healthSlider.value -= Time.deltaTime * 2; // Adjust the speed as needed
            yield return null;
        }
        healthSlider.value = 0;
    }

    IEnumerator ReduceShadowSlider(float amount)
    {
        float targetValue = shadowSlider.value - amount;
        if (targetValue < 0)
        {
            targetValue = 0;
        }

        while (shadowSlider.value > targetValue)
        {
            shadowSlider.value -= Time.deltaTime * 2;
            yield return null;
        }
        shadowSlider.value = targetValue;
    }

    public void IncreaseShadowSlider(float amount)
    {
        shadowSlider.value = Mathf.Clamp(shadowSlider.value + .35f, 0, shadowSlider.maxValue);
    }

    public void IncreaseHealthSlider(float amount)
    {
        healthSlider.value = Mathf.Clamp(healthSlider.value + .35f, 0, healthSlider.maxValue);
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

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "light")
        {
            Debug.Log("You are in Light");
            if (lightDamageCoroutine == null)
            {
                lightDamageCoroutine = StartCoroutine(TakeDamageOverTime(5f, 1f)); // Adjust damage and interval as needed
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.tag == "light")
        {
            Debug.Log("You left the Light");
            if (lightDamageCoroutine != null)
            {
                StopCoroutine(lightDamageCoroutine);
                lightDamageCoroutine = null;
            }
        }
    }

    private IEnumerator TakeDamageOverTime(float damageAmount, float interval)
    {
        while (true)
        {
            ReduceHealth(damageAmount);
            yield return new WaitForSeconds(interval);
        }
    }
}
