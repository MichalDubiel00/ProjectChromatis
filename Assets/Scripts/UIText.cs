using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIText : MonoBehaviour
{
    [SerializeField]
    private GameObject messenger;

    private TextMeshPro textMessage;

    private void Awake()
    {
        textMessage = messenger.GetComponent<TextMeshPro>();
    }

    private void Start()
    {
        textMessage.text = "AHHHHHHHHH";
    }
}
