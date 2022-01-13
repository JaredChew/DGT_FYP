using UnityEngine;

public class AttackManager : MonoBehaviour {
    
    private bool weaponHit;
    private bool enemyParrying;


    void Awake () {
        weaponHit = false;
        enemyParrying = false;
    }

    private void OnCollisionEnter2D(Collision2D collision) {

        if(collision.collider.gameObject.CompareTag(Global.gameObjectTag_Weapon)) {
            weaponHit = true;
        }
        else if(collision.collider.gameObject.CompareTag(Global.gameObjectTag_Parry)) {
            enemyParrying = true;
        }

    }

    // *** Set Tag *** //

    public void setWeaponTagParrying() {
        gameObject.tag = Global.gameObjectTag_Parry;
    }

    public void setWeaponTagDefault() {
        gameObject.tag = Global.gameObjectTag_Weapon;
    }

    public void setWeaponTagHeavyAttack() {
        gameObject.tag = Global.gameObjectTag_HeavyAttack;
    }

    // ** Setter Methods *** //

    public void setWeaponHit(bool weaponHit) {
        this.weaponHit = weaponHit;
    }

    public void setEnemyParrying(bool enemyParrying) {
        this.enemyParrying = enemyParrying;
    }

    // *** Getter Methods *** //

    public bool getWeaponHit() {
        return weaponHit;
    }

    public bool getEnemyParrying() {
        return enemyParrying;
    }

}
