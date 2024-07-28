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
    private string selectedSkill = "";
    private bool isCastingSpell = false;

    public Animator playerAnim;
    public Animator healthBarAnim;
    public Animator shadowBarAnim;

    private Camera mainCamera;
    public float horizontalOffset = 2f;

    public GameObject shadowBody;
    public GameObject shadowBallCast;
    public GameObject shadowBall;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        mainCamera = Camera.main;
        if (crosshairImage != null)
        {
            crosshairImage.enabled = true;
        }
    }

    void Update()
    {
        if (!isCastingSpell)
        {
            Move();
            HandleJump();
        }
        HandleSpellsSelection();
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
        }
        else
        {
            playerAnim.SetBool("run", false);
        }
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerAnim.SetTrigger("jump");
            StartCoroutine(DelayedJump());
        }
    }

    IEnumerator DelayedJump()
    {
        yield return new WaitForSeconds(0.3f); // 1 second delay
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void HandleSpellsSelection()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedSkill = "1";
            //Debug.Log("1 selected");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedSkill = "2";
            Debug.Log("2 selected");
        }
        
    }

    void HandleSpellCasting()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            if (!string.IsNullOrEmpty(selectedSkill))
            {
                float requiredShadow = 0f;

                if (selectedSkill == "1")
                {
                    requiredShadow = shadowSlider.maxValue * 0.2f;
                }
                else if (selectedSkill == "2")
                {
                    requiredShadow = shadowSlider.maxValue * 0.3f;
                }

                if (shadowSlider.value >= requiredShadow)
                {
                    if (selectedSkill == "1")
                    {
                        //playerAnim.SetTrigger("spell");
                        isCastingSpell = true;
                        StartCoroutine(EndSpellAnimation());
                        TriggerParticleSystem(shadowBody);
                        StartCoroutine(ReduceShadowSlider(shadowSlider.maxValue * 0.2f));
                    }
                    else if (selectedSkill == "2")
                    {
                        playerAnim.SetTrigger("spell");
                        shadowBarAnim.SetTrigger("shake");
                        TriggerParticleSystem(shadowBallCast);
                        CastShadowBall();
                        StartCoroutine(ReduceShadowSlider(shadowSlider.maxValue * 0.3f));
                    }
                }
                else
                {
                    Debug.Log("Not enough shadow energy.");
                }
            }
        }
    }

    void TriggerParticleSystem(GameObject particleSystemObject)
    {
        if (particleSystemObject != null)
        {
            ParticleSystem ps = particleSystemObject.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play(); // Avvia il ParticleSystem
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
    }

    IEnumerator EndSpellAnimation()
    {
        yield return new WaitForSeconds(2.3f); // spell animation duration
        isCastingSpell = false;
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
        shadowSlider.value = Mathf.Clamp(shadowSlider.value + 0.3f, 0, shadowSlider.maxValue);
    }

    // Uncomment these sections to implement damage and death animations
    /*
    void TakeDamage()
    {
        playerAnim.SetTrigger("damage");
    }
    */
}
