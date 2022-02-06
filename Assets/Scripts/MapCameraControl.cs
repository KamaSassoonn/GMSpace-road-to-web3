using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCameraControl : MonoBehaviour
{
    public static MapCameraControl ins;

    private Transform player;
    [Tooltip("Camera")]
    public Vector3 vector = new Vector3(0, 12, -20);
    
    private void Start()
    {
        ins = this;
    }

    public void SetPlayer(Transform player)
    {
        this.player = player;
    }
    private void LateUpdate()
    {
        if (player == null) return;
        ToFollow();
    }
    
    void ToFollow()
    {
        transform.position = player.position + vector;
    }
}
