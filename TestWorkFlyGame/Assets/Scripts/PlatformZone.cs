using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Gift"))
        {
            collision.transform.parent = transform.parent;    
        }
    }
}
