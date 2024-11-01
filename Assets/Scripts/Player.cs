using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{ 
    [SerializeField] PlayerHealth health;
    // Start is called before the first frame update
    void Start()
    {
        GameInput.instance.OnDebugAction += GameManager_OnDebugAction;
    }

    private void GameManager_OnDebugAction(object sender, System.EventArgs e)
    {
        Debug.Log("10 Damage Taken current: " + health.GetCurrentHealth() );
        health.TakeDamage(10);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug
        
    }
}
