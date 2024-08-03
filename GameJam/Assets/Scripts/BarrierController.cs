using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierController : MonoBehaviour
{
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(gameObject.transform.position, player.transform.position) < 0.3f) {
            ShowBlockedWayFeedback();
        }
    }

    public void ShowBlockedWayFeedback() {

    }

    public void RemoveLimits() {
        Destroy(gameObject);
    }
}
