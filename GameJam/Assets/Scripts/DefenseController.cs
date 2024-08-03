using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseController : MonoBehaviour
{

    public GameObject currentTarget;
    bool targetLocked;
    public GameObject defenderPosition;

    public GameObject trailPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (targetLocked && currentTarget != null)
        {
            
        }
    }

    void OnTriggerEnter(Collider c) 
    {
        if (c.gameObject.tag == "Enemy" && currentTarget == null) {
            if (!targetLocked) {
                targetLocked = true;
                currentTarget = c.gameObject;
            }
        }
    }

    void OnTriggerExit(Collider c) 
    {
        if (c.gameObject.tag == "Enemy" == c.gameObject)
        {
            targetLocked = false;
            currentTarget = null;
        }
    }
}
