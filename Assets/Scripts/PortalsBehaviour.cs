using UnityEngine;

public class PortalsBehaviour : MonoBehaviour {

    [SerializeField] private PortalsBehaviour connectingPortal;

    [SerializeField] private float pushOutFortalForceX = 2500f;
    [SerializeField] private float pushOutFortalForceY = 2500f;

    [SerializeField] private float offSetOutX = 3f;
    [SerializeField] private float offSetOutY = 3f;

    private void OnTriggerEnter2D(Collider2D collision) {

        if (collision.gameObject.CompareTag(Global.gameObjectTag_Player) || collision.gameObject.CompareTag(Global.gameObjectTag_PlayerSmoked)) {

            Player player = collision.gameObject.GetComponent<Player>();

            if (player.getIsAbleToPhaseWalk()) {
                player.setPlayerPosition(connectingPortal.transform.position.x + offSetOutX, connectingPortal.transform.position.y + offSetOutY);
                player.pushPlayer(pushOutFortalForceX, pushOutFortalForceY);
            }

        }

    }

}
