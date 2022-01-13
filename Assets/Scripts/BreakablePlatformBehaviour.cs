using UnityEngine;

public class BreakablePlatformBehaviour : MonoBehaviour {

    [SerializeField] private int maxHits = 1;

    private int currentHits;

    private void Awake() {

        currentHits = 0;

    }

    // Use this for initialization
    void Start () { }
	
	// Update is called once per frame
	void Update () {
		
        if(currentHits == maxHits) {
            Destroy(gameObject);
        }

	}

    private void OnCollisionEnter2D(Collision2D collision) {
        
        if(collision.gameObject.CompareTag(Global.gameObjectTag_Player) || collision.gameObject.CompareTag(Global.gameObjectTag_PlayerSmoked)) {
            currentHits++;
        }

    }

}
