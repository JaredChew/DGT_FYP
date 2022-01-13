using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {

    public string moveDirection;
    public float extraPointToMoving;
    private Vector2 movingPostion;
    public Rigidbody2D rigidbody2D;

	// Use this for initialization
	void Start () {
        extraPointToMoving = Mathf.Abs(extraPointToMoving);
        calculate();

    }
	
	// Update is called once per frame
	void Update () {
        moving();
    }
    private void calculate()
    {
        switch (moveDirection)
        {
            case "Right":
                movingPostion = new Vector2(transform.position.x - extraPointToMoving, transform.position.y + (extraPointToMoving / 4) );
                break;
            case "Left":
                movingPostion = new Vector2(transform.position.x + extraPointToMoving, transform.position.y + (extraPointToMoving / 4));
                break;
            default:
                movingPostion = new Vector2(transform.position.x - extraPointToMoving, transform.position.y + (extraPointToMoving / 4));
                break;

        }
    }
    
    private void moving()
    {
        rigidbody2D.MovePosition(movingPostion);
    }
}
