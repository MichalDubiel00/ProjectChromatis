using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class CharacterRanged : MonoBehaviour
{
    [SerializeField] private GameObject colorDroplet;
    [SerializeField] private GameObject pivotPointThrow;
    [SerializeField] private float strength = 2.0f;
    private Vector3 mousePosition;
    private Vector2 characterPosition;
    private PlayerMovement movementScript;
    
    // Start is called before the first frame update
    void Start()
    {        
        movementScript = gameObject.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            characterPosition = mousePosition - gameObject.transform.position;
            characterPosition.Normalize();
            throwDroplet(characterPosition);

            if (movementScript.IsFacingRight && transform.position.x > mousePosition.x)
            {
                movementScript.CheckDirectionToFace(!movementScript.IsFacingRight);
            }
            if (!movementScript.IsFacingRight && transform.position.x < mousePosition.x)
            {
                movementScript.CheckDirectionToFace(!movementScript.IsFacingRight);
            }


        }   
    }

    private void throwDroplet(Vector2 relativeMousePosition)
    {
        var ball = Instantiate<GameObject>(colorDroplet, pivotPointThrow.transform.position, Quaternion.identity);
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        var throwVelocity = relativeMousePosition *strength;
        rb.AddForce(throwVelocity, ForceMode2D.Impulse);
    }
}
