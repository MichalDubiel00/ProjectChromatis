using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public Transform posA, posB;
    bool moveOn = false;

    Vector2 targetPos;
    // Start is called before the first frame update
    void Start()
    {
        targetPos = posB.position;
        GameInput.instance.OnDebugAction += Instance_OnDebugAction;

    }

    private void Instance_OnDebugAction(object sender, System.EventArgs e)
    {
        moveOn = !moveOn;
    }

    // Update is called once per frame
    void Update()
    {
        if (moveOn) 
        {
            MovePlatform();
        }

    }

    void MovePlatform()
    {
        if (Vector2.Distance(transform.position, posA.position) < .1f)
            targetPos = posB.position;
        if (Vector2.Distance(transform.position, posB.position) < .1f)
            targetPos = posA.position;

        transform.position = Vector2.MoveTowards(transform.position, targetPos, 1 * Time.deltaTime);
    }
}
