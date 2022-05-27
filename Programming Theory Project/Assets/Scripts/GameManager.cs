using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{    
    public static GameManager Instance;
    public GameObject UIMenu;
    public static GameObject Menu;

    public static TextMeshProUGUI SelectionName;
    public static TextMeshProUGUI SelectionDescription;
    public static TextMeshProUGUI SelectionProperties;
    public static GameObject SelectionInteractable;
    public static Image SelectionThumbnail;
    public static bool isCurrentPlayer = false;

    private static DetailsUI m_CurrentDetails;


    public static TextMeshProUGUI GoldText;
    private static int m_currentGold=0;
    private static int m_targetGold;   
   
    
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

    private void Awake()
    {
        if(GameManager.Instance!=null)
        {
            Destroy(this);
        }
        else
        {
        Instance = this;
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

        GoldText.text =m_currentGold.ToString("N0");
        ClearDetails();
        AddGold(100);
        
    }

    
     public IEnumerator SyncUpValue(int c_val,int amountVal)
    {
        yield return new WaitForSecondsRealtime(0.02f);
        if(c_val < m_currentGold)
        {
            c_val += amountVal / 10;
            if(m_currentGold < c_val)
            {
                c_val = m_currentGold;
            }
            GoldText.text =  c_val.ToString("N0");
            StartCoroutine(SyncUpValue(c_val,amountVal));
        }        
    }
  
    public IEnumerator SyncDownValue(int c_val,int amountVal)  
    {          
        yield return new WaitForSecondsRealtime(0.02f);
        if (c_val > m_currentGold)
        {
            c_val -= amountVal/10;            
            if (m_currentGold > c_val)
            {
                c_val = m_currentGold;                
            }
            GoldText.text = c_val.ToString("N0");
            StartCoroutine(SyncDownValue(c_val,amountVal));
        }
    }

    public void AddGold(int amount)
    {
        int currentVal = m_currentGold;
        m_currentGold += amount;
        StartCoroutine(SyncUpValue(currentVal,amount));
    }
   
   
    public void SubtractGold(int amount)
    {       
        int currentVal = m_currentGold; // save the initla reference for display
        m_currentGold -= amount;        // the actual operation  
        StartCoroutine(SyncDownValue(currentVal,amount));        // showing the update
    }
   
}
