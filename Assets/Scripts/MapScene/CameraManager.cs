﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public GameObject player;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void CameraSetPositionToPlayer()
    {
        transform.position = new Vector3(
            player.transform.position.x,
            player.transform.position.y,
            -10
            );
        //マップ作成後、端の時の処理を追加
    }
}