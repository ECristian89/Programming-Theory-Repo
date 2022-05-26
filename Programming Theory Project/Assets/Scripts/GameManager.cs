using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject UIMenu;
    public static GameObject Menu;

    public static TextMeshProUGUI SelectionName;
    public static TextMeshProUGUI SelectionDescription;
    public static TextMeshProUGUI SelectionProperties;
    public static GameObject SelectionInteractable;
    public static Image SelectionThumbnail;

    private static DetailsUI m_CurrentDetails;


    public TextMeshProUGUI GoldText;
    private int m_currentGold=6459190;
    
    // Use this to get the data to populate the UI elements
    public static void GetDetails()
    {

    }       

    // Use this to send the gathered details to the UI
    public static void SendDetails(DetailsUI details)
    {
        m_CurrentDetails = details;        
        
        if(details.Interactable)
        SelectionInteractable = Instantiate(m_CurrentDetails.Interactable, Menu.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).transform, false);
        

        SelectionThumbnail.sprite = m_CurrentDetails.Thumbnail;
        SelectionName.text = m_CurrentDetails.EntityName;
        //SelectionProperties.text = properties;
        SelectionDescription.text = m_CurrentDetails.Properties +"\n" +m_CurrentDetails.Description;
    }

    public static void ClearDetails()
    {
        if(m_CurrentDetails!=null)
        {           
                SelectionName.text = "No selection";            
                SelectionDescription.text = "";          
                //SelectionProperties.text = "";         
                SelectionThumbnail.sprite = null;
        }
        
        // delete all child objects of Interactable 
        if(SelectionInteractable != null)
        {
            foreach (Transform child in SelectionInteractable.transform.parent.transform)
            {
            Destroy(child.gameObject);
            }
        }


    }
    // Start is called before the first frame update
    void Start()
    {
        Menu = Instantiate(UIMenu, transform.position, UIMenu.transform.rotation);
        SelectionName = Menu.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        SelectionDescription = Menu.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
        SelectionThumbnail = Menu.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
        GoldText = Menu.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();

        GoldText.text = m_currentGold.ToString("N0");
        ClearDetails();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
