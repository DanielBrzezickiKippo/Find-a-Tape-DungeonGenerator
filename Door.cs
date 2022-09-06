using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    public enum DoorType
    {
        left,right,top,bottom
    };
    public DoorType doorType;

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("wall_door"))
        {
            Destroy(other.gameObject);
            this.GetComponent<BoxCollider2D>().enabled = false;
            this.GetComponent<BoxCollider2D>().enabled = true;
            this.GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
