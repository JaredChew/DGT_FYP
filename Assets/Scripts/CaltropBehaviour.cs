using System.Collections;
using UnityEngine;

public class CaltropBehaviour : MonoBehaviour {

    [SerializeField] private float xThrowDistance = 2;
    [SerializeField] private float yThrowDistance = 1;
    private bool shootingRight;

    // Use this for initialization
    void Start()
    {

        StartCoroutine(throwCaltrop());

    }

    IEnumerator throwCaltrop()
    {
        if (shootingRight)
        {
            transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(xThrowDistance, yThrowDistance));
        }
        else
        {
            transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(-xThrowDistance, yThrowDistance));
        }

        yield return 0;

    }

    public void placeCaltrop(float x, float y, bool playerLookingRight) {

        shootingRight = playerLookingRight;
        transform.position = new Vector2(x, y);

    }

}
