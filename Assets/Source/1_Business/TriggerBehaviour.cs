﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBehaviour : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.tag == "Bullet") Destroy(collision.gameObject);
        if (collision.transform.tag == "Player" || collision.transform.tag == "Bot") GameController.Instance?.GameReset("");
    }
}
