using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffect : MonoBehaviour {

    public Rigidbody particle;
    private Transform character;

    private void Awake()
    {
        //character = GetComponent<Transform>();
    }

    public void spawn()
    {
        Rigidbody clone;
        float x = 0.1880002f;
        float y = 0.06799984f;
        
        clone = Instantiate(particle, new Vector3( transform.position.x - x, transform.position.y - y, transform.position.z) , Quaternion.identity);

    }

}
