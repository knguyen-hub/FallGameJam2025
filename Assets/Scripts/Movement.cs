using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Movement : MonoBehaviour
{

    [SerializeField] private float speed = 20f;
    
    private float _currentVelocity;
    private Rigidbody2D _rigidbody;
    private Animator anim;
    private bool grounded;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        _rigidbody.linearVelocity = new Vector2(horizontalInput* speed, _rigidbody.linearVelocity.y);

        if (horizontalInput > 0.01f) {
            transform.localScale = Vector3.one * 16;
        } else if (horizontalInput < -0.01f) {
            transform.localScale = new Vector3(-1,1,1) * 16;
        }

        if (Input.GetKey(KeyCode.Space) && grounded) {
            Jump();
        }

        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", grounded);
    }

    private void Jump() {
        _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, speed);
        anim.SetTrigger("jump");
        grounded = false;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Ground") {
            grounded = true;
        }
    }
    
}