using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProximityDetector : MonoBehaviour
{

    public GameObject currentTarget;
    public bool targetLocked;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTarget == null) {
            targetLocked = false;
            Player.inst.currentEnemy = null;
        }
    }

    void OnTriggerStay(Collider c)
    {
        if (c.gameObject.tag == "Enemy" && currentTarget == null)
        {
            if (!targetLocked)
            {
                targetLocked = true;
                currentTarget = c.gameObject;
                Player.inst.currentEnemy = c.gameObject;
            }
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.gameObject.tag == "Enemy" == c.gameObject)
        {
            targetLocked = false;
            currentTarget = null;
            Player.inst.currentEnemy = null;
        }
    }
}
