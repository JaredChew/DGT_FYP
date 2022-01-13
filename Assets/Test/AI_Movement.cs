using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Movement : MonoBehaviour {

    public GameObject[] pathForMove;
    public float batSpeed;
    public float timerForTurning;
    public float staySpeed;

    private GameObject savePathForMove;
    private bool triggerFlip;
    private bool lookingRight;
    private bool attacking;

    // Use this for initialization
    void Start () {

        triggerFlip = false;
        lookingRight = false;
        attacking = false;
        staySpeed = 0f;

        StartCoroutine("MoveToPath");

    }
	
    IEnumerator MoveToPath() {

        do {

            foreach (GameObject pointToTowards in pathForMove) {

                triggerFlip = false;
                savePathForMove = pointToTowards;

                yield return StartCoroutine( MoveToPoint(pointToTowards.transform.position) );

            }

        } while (true);

    }

    IEnumerator MoveToPoint( Vector3 target) {

        do{

            this.transform.position = Vector3.MoveTowards( this.transform.position, new Vector3(target.x, this.transform.position.y, this.transform.position.z), batSpeed * Time.deltaTime );
            
            if(this.transform.position.x >= target.x && !triggerFlip)
            {
                triggerFlip = true;
                lookingRight = true;
                Flip();
            }
            else if (this.transform.position.x <= target.x && !triggerFlip)
            {
                triggerFlip = true;
                lookingRight = false;
                Flip();
            }

            yield return 0;

        } while ((this.transform.position.x != target.x) || attacking ) ;

        yield return StartCoroutine(WaitFewSecond(timerForTurning));
        
    }

    IEnumerator WaitFewSecond( float delayTime ) {

        for (float timer = 0; timer < delayTime; timer += Time.deltaTime)
            yield return 0;

    }

    void Flip() {

        if( lookingRight && transform.localScale.x <= 0 )
        {
            transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if ( !lookingRight && transform.localScale.x >= 0 )
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

    }

}
