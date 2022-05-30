using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{    
    public GameObject UIMenu;
    public GameObject EndScreen;
    public GameObject[] NotificationPf;
    public GameObject TopNotification;
    public Sprite NoSelectionThumbnail;

    public static GameManager Instance = null; // singleton
    public static GameObject Menu;
    public static TextMeshProUGUI SelectionName;
    public static TextMeshProUGUI[] SelectionProperty=new TextMeshProUGUI[4];
    public static TextMeshProUGUI SelectionProduction;
    public static GameObject[] SelectionInteractable= new GameObject[6];    // clickable button/s that creat units
    public static GameObject UpgradeButton;                                 // clickable button that upgrades
    public static GameObject HpBar,NameBg,DetailBg;
    public static Image SelectionThumbnail;
    public static bool isCurrentPlayer = false;
    public static bool isGameStarted = false;
    public static bool canSpendGold = false;  // flag to keep gold balance on positive side

    private static DetailsUI m_CurrentDetails;
    private static TextMeshProUGUI GoldText;
    private static int m_currentGold=0;   
    
   
    // make this class a singleton and allow scene persistance
    private void Awake()
    {
        if(Instance!=null)
        {
            Destroy(this.gameObject);
        }
        else
        {
        Instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        isGameStarted = true;  // 
        Initializelevel();     // FOR FAST TESTING ONLY, remove for builds
    }
    // SCENE MANAGEMENT
    public void StartGame()  // load first scene/level
    {
        SceneManager.LoadScene(1);        
        SceneManager.sceneLoaded += OnSceneLoaded;   // use this to know when the scene has fully loaded by passing the delegate
    }

    static public void BackToMain()
    {
        isGameStarted = false;
        SceneManager.LoadScene(0);
    }

    // do the initial stuff in this method since we use it right after the scene has loaded      
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex != 0)      // we can do checks here to see which scene is loaded and customize behaviour
        {
            isGameStarted = true;       // a flag to check when game is started   
            Initializelevel();
        }
    }
    private void Initializelevel()
    {        

        if (isGameStarted)
        {
            Menu = Instantiate(UIMenu, Vector3.zero, UIMenu.transform.rotation);
            TopNotification = Menu.transform.GetChild(0).GetChild(2).gameObject;
            SelectionName = Menu.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
            NameBg = Menu.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(1).gameObject;
            DetailBg = Menu.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(1).gameObject;
            // thse values will change a lot so we better hold a reference
            SelectionProperty[0] = DetailBg.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();  // HitPoints
            SelectionProperty[1] = DetailBg.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();  // Speed
            SelectionProperty[2] = DetailBg.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();  // Attack
            SelectionProperty[3] = DetailBg.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();  // Attack Speed

            SelectionThumbnail = Menu.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
            GoldText = Menu.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
            HpBar = Menu.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(2).gameObject;

            GoldText.text = m_currentGold.ToString("N0");
            ClearDetails();    // clear the UI first
            if(m_currentGold<100)  // to avoid getting multiple calls on add gold initializer
            AddGold(1700);      // if we need to set an initial gold amount
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;   // remove the event
    }
    public void QuitGame()  // exit the application
    {
        // should implement the save system here if you need that
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    private IEnumerator CountEnemyUnits() //count enemy's units and buildings
    {
        yield return new WaitForEndOfFrame();
        var unitsCount = GameObject.FindObjectsOfType<EnemyUnit>().Length;
        var buildingsCount = GameObject.FindObjectsOfType<EnemyBuilding>().Length;

        //Debug.LogFormat($"Player units remaining: {unitsCount}");                   // could add a UI element to give feedback to the player
        //Debug.LogFormat($"Player buildings remaining: {buildingsCount}");

        if (unitsCount == 0 && buildingsCount == 0)
        {
            ShowNotification(NotificationPf[0], "You are VICTORIOUS!");  // replace with an actual GAME OVER notification and options
            Instantiate(EndScreen);
        }
    }

    private IEnumerator CountPlayerUnits() // count player's units and buildings
    {
        yield return new WaitForEndOfFrame();
        var unitsCount=GameObject.FindObjectsOfType<PlayerUnit>().Length;
        var buildingsCount = GameObject.FindObjectsOfType<PlayerBuilding>().Length;

        //Debug.LogFormat($"Player units remaining: {unitsCount}");                   // could add a UI element to give feedback to the player
       // Debug.LogFormat($"Player buildings remaining: {buildingsCount}");

        if(unitsCount==0 && buildingsCount==0)
        {
            Debug.Log("GAME OVER!");  // replace with an actual GAME OVER notification and options
            var End = Instantiate(EndScreen);
            End.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "DEFEAT!";
            yield return new WaitForSecondsRealtime(5);
            BackToMain();
        }
    }
    public void IsGameOver()  // check if the Game Over conditions are met
    {
        StartCoroutine(CountPlayerUnits());
    }

    public void IsVictorious()  // check if the Game Over conditions are met
    {
        StartCoroutine(CountEnemyUnits());
    }


    public static int GetCurrentGold()
    {
        return m_currentGold;
    }
    // UI CONTENT UPDATE

    public void ShowNotification(GameObject notificationPf)
    {
        Instantiate(notificationPf, TopNotification.transform);        
    }

    /// <summary>
    /// limit the message to 20 chararcters(space included)
    /// </summary>
    /// <param name="notificationPf">notification template</param>
    /// <param name="message">text to be shown</param>
    public void ShowNotification(GameObject notificationPf,string message)
    {
        var notify =Instantiate(notificationPf, TopNotification.transform);
        notify.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
    }

    public void ShowNotification(GameObject notificationPf, string message,Sprite thumbnail)
    {
        var notify = Instantiate(notificationPf, TopNotification.transform);
        notify.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
        notify.transform.GetChild(1).GetComponent<Image>().sprite = thumbnail;
    }

    // Use this to send the gathered details to the UI
    public static void SendDetails(DetailsUI details)
    {
        m_CurrentDetails = details;        
        
        // create unit creation buttons
            for (int i = 0; i < details.Interactable.Length; i++)
            {
                if(details.Interactable[i] != null)
                SelectionInteractable[i] = Instantiate(m_CurrentDetails.Interactable[i], Menu.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).transform, false);
            }
        // create upgrade button
        if (m_CurrentDetails.UpgradeBtn != null)
        {
            UpgradeButton = Instantiate(m_CurrentDetails.UpgradeBtn, Menu.transform.GetChild(1).GetChild(0).GetChild(1).GetChild(2).transform, false);
            UpgradeButton.transform.GetComponent<AsignValue>().Upgrade = m_CurrentDetails.UpgradeCost;
        } 
        

        SelectionThumbnail.sprite = m_CurrentDetails.Thumbnail;
        SelectionName.text = m_CurrentDetails.EntityName;

        SelectionProperty[0].text = m_CurrentDetails.CurrentHitPoints + "/" + m_CurrentDetails.MaxHitPoints;
        SelectionProperty[1].text = m_CurrentDetails.Speed.ToString("N1"); 
        SelectionProperty[2].text = m_CurrentDetails.AttackPower.ToString();
        SelectionProperty[3].text = m_CurrentDetails.AttackSpeed.ToString("N1");

        UpdateUIHp();
    }

    public static void UpdateUIHp()   // sync the shown Hitpoints
    {
        HpBar.SetActive(true);
        NameBg.SetActive(true);
        DetailBg.SetActive(true);
        float hpVal = (float)m_CurrentDetails.CurrentHitPoints / m_CurrentDetails.MaxHitPoints;         
        HpBar.transform.GetChild(0).transform.GetComponent<Image>().fillAmount = hpVal;
        SelectionProperty[0].text = m_CurrentDetails.CurrentHitPoints + "/" + m_CurrentDetails.MaxHitPoints;
    }

    public void SetUI(ref DetailsUI details)
    {
        m_CurrentDetails = details;
    }

    public DetailsUI GetCurrentUI()
    {
        return m_CurrentDetails;
    }
    
    // use this to clear the UI
    public static void ClearDetails()
    {
        if(m_CurrentDetails!=null)
        {
            SelectionName.text = "";            
            SelectionThumbnail.sprite = GameManager.Instance.NoSelectionThumbnail;  // default black sprite for no selection
            Destroy(UpgradeButton);          
            HpBar.SetActive(false);
            NameBg.SetActive(false);
            DetailBg.SetActive(false);
        }

        
        // delete all child objects of Interactable 

        for (int i = 0; i < SelectionInteractable.Length; i++)
            {
                if (SelectionInteractable[i] != null)
                {
                    foreach (Transform child in SelectionInteractable[i].transform.parent.transform)
                    {
                        Destroy(child.gameObject);
                    }
                }
            }
            
        
    }

    

// never access this directly 
// use AddGold or SubtractGold methods instead
#region Sync Value Algorithm
    private IEnumerator SyncUpValue(int c_val,int amountVal)  // keep this private
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

    private IEnumerator SyncDownValue(int c_val,int amountVal)  // keep this private
    {          
        yield return new WaitForSecondsRealtime(0.02f);
        if (c_val > m_currentGold)               // compare the current value to the actual gold value
        {
            c_val -= amountVal/10;               // because our amount is of type int we must make sure we never divide to a higher value than amount
            if (m_currentGold > c_val)           // when the current value exceeeds the actual gold value
            {
                c_val = m_currentGold;           // we make sure to show the right value on the screen too     
            }
            GoldText.text = c_val.ToString("N0");
            StartCoroutine(SyncDownValue(c_val,amountVal));   // repeat until the values are syncronized
        }
    }
#endregion

#region Gold Manipulation Methods
// use this from any other script that needs to add gold
    public void AddGold(int amount)  // keep this public
    {        
        int currentVal = m_currentGold;
        m_currentGold += amount;
        StartCoroutine(SyncUpValue(currentVal,amount));        
    }

    // use this from any other script that needs to subtract gold
    public void SubtractGold(int amount)  // keep this public 
    {
        if (m_currentGold - amount < 0)
        {
            canSpendGold = false;
        }
        else
        {
            canSpendGold = true;
        }

        if (canSpendGold)
        {
            int currentVal = m_currentGold; // save the inital reference for display
            m_currentGold -= amount;        // the actual operation  
            StartCoroutine(SyncDownValue(currentVal, amount));        // showing the update
        }
    }
#endregion
}
