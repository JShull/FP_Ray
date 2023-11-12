using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Spawn : MonoBehaviour
{

    [Space]
    [Header("Coin to Spawn")]
    public GameObject object1;  
    [Space]
    [Header("Rupoor to Spawn")]
    public GameObject object2; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.C)){
           
            Instantiate(object1, new Vector3(1.422f, 3f, 0), Quaternion.identity); //transform.position spawn at object this is attached to

        }

        if (Input.GetKeyDown(KeyCode.R)){
           
            Instantiate(object2, new Vector3(-3.01f, 3f, 0), Quaternion.identity); //transform.position spawn at object this is attached to

        }
        
    }
}
