using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    private Rigidbody rb;
    private string selectedSkill = "";

    public Animator playerAnim;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Move();
        Jump();
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

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void HandleSpellsSelection()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedSkill = "1";
            Debug.Log("1 selected");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedSkill = "2";
            Debug.Log("2 selected");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedSkill = "3";
            Debug.Log("3 selected");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectedSkill = "4";
            Debug.Log("4 selected");
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            selectedSkill = "R";
            Debug.Log("R selected");
        }
    }

    void HandleSpellCasting()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!string.IsNullOrEmpty(selectedSkill))
            {
                Debug.Log(selectedSkill + " launch with left click");
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (!string.IsNullOrEmpty(selectedSkill))
            {
                Debug.Log(selectedSkill + " launch with right click");
            }
        }
    }
}
