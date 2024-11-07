using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool juiceOn = true;

    [SerializeField] PlayerHealth health;
    public PlayerMovement juicyMovment; 
    public SimplePlayerMovment simpleMovment; 
    
    // Start is called before the first frame update
    void Start()
    {
        GameInput.instance.OnToggleAction += GameManager_OnToggleAction; ;

        GameInput.instance.OnDebugAction += GameManager_OnDebugAction;

        simpleMovment.enabled = false;
    }

    private void GameManager_OnToggleAction(object sender, System.EventArgs e)
    {
        if (juiceOn)
        {
            juiceOn = false;
            Debug.Log("Juicy Movment On");
            simpleMovment.enabled = false;
            juicyMovment.enabled = true;
        }
        else
        {
            juiceOn = true;
            Debug.Log("Juicy Movment Off");
            simpleMovment.enabled = true;
            juicyMovment.enabled = false;
        }
    }

    private void GameManager_OnDebugAction(object sender, System.EventArgs e)
    {
        Debug.Log("10 Damage Taken current: " + health.GetCurrentHealth() );
        health.TakeDamage(10);
    }

    // Update is called once per frame
    void Update()
    {
  
    }
}
