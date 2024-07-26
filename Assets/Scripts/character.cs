using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float spellRange = 100f;
    public Image crosshairImage;

    private Rigidbody rb;
    private string selectedSkill = "";

    public Animator playerAnim;
    private LineRenderer lineRenderer;
    private Camera mainCamera;
    public float horizontalOffset = 2f;

    void Start(){
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

    void Update(){
        Move();
        Jump();
        HandleSpellsSelection();
        HandleSpellCasting();
    }

    void Move(){
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = transform.forward * moveVertical + transform.right * moveHorizontal;
        movement.y = 0;

        rb.MovePosition(transform.position + movement * moveSpeed * Time.deltaTime);

        if (moveHorizontal != 0 || moveVertical != 0){
            playerAnim.SetBool("run", true);
        }
        else{
            playerAnim.SetBool("run", false);
        }
    }

    void Jump(){
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void HandleSpellsSelection(){
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedSkill = "1";
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedSkill = "2";
        }
    }

    void HandleSpellCasting(){
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)){
            if (!string.IsNullOrEmpty(selectedSkill)){
                if (selectedSkill == "2")
                {
                    CastShadowBall();
                }
            }
        }
    }

    void CastShadowBall()
    {

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, spellRange))
        {
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("mob"))
                {
                    Destroy(hit.collider.gameObject);
                }
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
}
