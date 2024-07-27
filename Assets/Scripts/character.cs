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

    private Rigidbody rb;
    private string selectedSkill = "";
    private bool isCastingSpell = false;

    public Animator playerAnim;
    private LineRenderer lineRenderer;
    private Camera mainCamera;
    public float horizontalOffset = 2f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.positionCount = 0;

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
            Debug.Log("1 selected");
            playerAnim.SetTrigger("spell");
            isCastingSpell = true;
            StartCoroutine(EndSpellAnimation());
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
                if (selectedSkill == "2")
                {
                    CastShadowBall();
                }
                else
                {
                    Debug.Log(selectedSkill + " launch with " + (Input.GetMouseButtonDown(0) ? "left" : "right") + " click");
                }
            }
        }
    }

    void CastShadowBall()
    {
        Vector3 crosshairWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(crosshairImage.rectTransform.position.x, crosshairImage.rectTransform.position.y, mainCamera.nearClipPlane));

        Vector3 direction = (mainCamera.transform.forward).normalized;

        Ray ray = new Ray(crosshairWorldPosition, direction);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, spellRange))
        {
            if (hit.collider != null && hit.collider.CompareTag("mob"))
            {
                Destroy(hit.collider.gameObject);
            }
            ShowRay(ray.origin, hit.point);
        }
        else
        {
            ShowRay(ray.origin, ray.origin + ray.direction * spellRange);
        }
    }



    void ShowRay(Vector3 start, Vector3 end)
    {
        StartCoroutine(ShowRayCoroutine(start, end));
    }

    IEnumerator ShowRayCoroutine(Vector3 start, Vector3 end)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        yield return new WaitForSeconds(3f);

        lineRenderer.positionCount = 0;
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

    // Uncomment these sections to implement damage and death animations
    /*
    void TakeDamage()
    {
        playerAnim.SetTrigger("damage");
    }
    */
}
