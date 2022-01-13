using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogController : MonoBehaviour
{

    [SerializeField] private Text dialogText;
    [SerializeField] private float timerForWaitDialogBoxAnimatorDisplayFinish;
    [SerializeField] private float timerForShowWhenASentenceFinish;
    [SerializeField] private float timerForWaitToCloseText;

    [SerializeField] private int[] letterAmount;
    [SerializeField] private int[] firstSentencesAmount;

    //For start counting how many letter
    [SerializeField] private bool noCountLetter;
    [SerializeField] private int numberSentence;

    [TextArea(3, 10)]
    [SerializeField] private string[] dialogSentence;

    private Animator dialogBoxAnimator;

    private bool[] timeToShow;

    // Use this for initialization
    private void Start() {

        dialogBoxAnimator = transform.GetComponent<Animator>();
        //startDialog(numberSentence);

    }

    // Update is called once per frame
    private void Update() {

    }

    public void startDialog(int whichSentenceIndex) {

        dialogBoxAnimator.SetBool("ShowDialogBox", true);
        StartCoroutine(TypeSentence(whichSentenceIndex));

    }

    IEnumerator TypeSentence(int sentenceIndex) {

        dialogText.text = "";
        int textCount = 0;

        yield return StartCoroutine(startTimer());

        if (!noCountLetter) {

            foreach (char characterLetter in dialogSentence[sentenceIndex].ToCharArray()) {

                textCount++;
                dialogText.text += characterLetter;
                Debug.Log(textCount);

                yield return null;
            }

        }

        if (noCountLetter) {

            foreach (char characterLetter in dialogSentence[sentenceIndex].ToCharArray()) {

                textCount++;
                dialogText.text += characterLetter;

                if (textCount == firstSentencesAmount[sentenceIndex] && firstSentencesAmount[sentenceIndex] != 0) {

                    yield return StartCoroutine(waitTimer());
                    dialogText.text = "";

                }

                yield return null;
            }

            yield return StartCoroutine(closeText());

        }
        
        
    }

    IEnumerator startTimer()
    {

        for (float i = 0; i < timerForWaitDialogBoxAnimatorDisplayFinish; i += Time.deltaTime)
        {

            yield return 0;

        }

    }

    IEnumerator waitTimer() {

        for(float i=0; i < timerForShowWhenASentenceFinish; i+=Time.deltaTime) {

            yield return 0;

        }
            
    }

    IEnumerator closeText()
    {

        for (float i = 0; i < timerForWaitToCloseText; i += Time.deltaTime)
        {

            yield return 0;

        }

        dialogBoxAnimator.SetBool("ShowDialogBox", false);

    }

}