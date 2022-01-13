using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDetailManager : MonoBehaviour {

    private Animator itemFrameAnimator;
    private List<Animator> staminaAnimator;
    private Slider heavyAttackMeterBar;
    private Animator heavyAttackMeterAnimator;

    // Use this for initialization
    void Awake() {

        itemFrameAnimator = transform.Find("UI_Background/ItemFrame").GetComponent<Animator>();

        staminaAnimator = new List<Animator>();

        for (int i = 1; i <= 3; i++) {

            staminaAnimator.Add(transform.Find("UI_Background/StaminaBar/Stamina_0" + i).GetComponent<Animator>());

        }

        heavyAttackMeterBar = transform.Find("UI_Background/HeavyAttackMeterBar").GetComponent<Slider>();
        heavyAttackMeterAnimator = transform.Find("UI_Background/HeavyAttackMeterBar").GetComponent<Animator>();

    }

    //****  Item Manager  ****//

    public void switchCurrentItemNumber(int itemNumberIndex) {

        //Debug.Log(itemNumberIndex);

        itemFrameAnimator.SetInteger("ItemUIType", itemNumberIndex);

    }



    //****  Heavy Attack Meter Bar Manager  ****//

    public void fillMeterBar(float currentMeter) {

        //Debug.Log(currentMeter);

        heavyAttackMeterBar.value = currentMeter;

        //Switch color animator
        heavyAttackMeterAnimator.SetFloat("HeavyAttackMeter", currentMeter);

    }



    //****  Stamina State Manager  ****//

    public void switchStaminaAnimation(float stamina) {

        //Debug.Log(stamina);

        for (int i = 0; i < staminaAnimator.Count; i++) {

            staminaAnimator[i].SetFloat("Stamina", stamina);

        }

    }


}
