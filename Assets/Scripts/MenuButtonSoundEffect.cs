using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//[RequireComponent(typeof(Button))]
public class MenuButtonSoundEffect : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, ISelectHandler
{
    
    public Button btn { get { return GetComponent<Button>(); } }

    // Start is called before the first frame update
    void Start()
    {
        btn.onClick.AddListener( () => onClickSound() );
        
    }

    // Update is called once per frame
    void onClickSound()
    {
        FindObjectOfType<AudioManager>().playSound("Menu_On_Click");
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        FindObjectOfType<AudioManager>().playSound("Menu_Choosing");
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        FindObjectOfType<AudioManager>().playSound("Menu_On_Click");
    }

    public void OnSelect(BaseEventData eventData)
    {
        FindObjectOfType<AudioManager>().playSound("Menu_Choosing");
    }

}
