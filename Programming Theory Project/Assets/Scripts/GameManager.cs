using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public delegate void Link();
    Link link;
    public static GameManager Instance;
    public GameObject UIMenu;
    public static GameObject Menu;

    public static TextMeshProUGUI SelectionName;
    public static TextMeshProUGUI SelectionDescription;
    public static TextMeshProUGUI SelectionProperties;
    public GameObject SelectionInteractable;
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
        GameManager.Instance.SelectionInteractable = Instantiate(m_CurrentDetails.Interactable, Menu.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).transform, false);
        

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
        if(GameManager.Instance.SelectionInteractable != null)
        {
            foreach (Transform child in GameManager.Instance.SelectionInteractable.transform.parent.transform)
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

        GoldText.text = m_currentGold.ToString("N0");
        ClearDetails();
        m_targetGold = 500;
        StartCoroutine(SyncUpValue(m_targetGold));
    }

    
     public IEnumerator SyncUpValue(int val)
    {
        yield return new WaitForSecondsRealtime(0.02f);
        if(val> m_currentGold)
        {
            m_currentGold += val / 50;
            if(m_currentGold>val)
            {
                m_currentGold = val;
            }
            GoldText.text = m_currentGold.ToString("N0");
            StartCoroutine(SyncUpValue(val));
        }

        
    }
  
    public IEnumerator SyncDownValue(int n_val,int amountVal)
    {           
        
        yield return new WaitForSecondsRealtime(0.02f);
        if (n_val < m_currentGold)
        {
            m_currentGold -= amountVal / 50;            
            if (m_currentGold < n_val)
            {
                m_currentGold = n_val;
            }
            GoldText.text = m_currentGold.ToString("N0");
            StartCoroutine(SyncDownValue(n_val,amountVal));
        }


    }

    public void AddGold(int amount)
    {
        m_targetGold = m_currentGold + amount; // same helper to be added as for Subtract
        StartCoroutine(SyncUpValue(m_targetGold));
    }

    public void SubtractGold(int amount)
    {
        int newVal = m_currentGold - amount; // should add a helper counter here to avoid bad calculation if player is spending gold fast
        Debug.LogFormat($"new value:{newVal}  ->  currentGold:{m_currentGold} || amount:{amount}");
        StartCoroutine(SyncDownValue(newVal,amount));        
    }
    
}
