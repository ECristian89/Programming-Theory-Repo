using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTile : MonoBehaviour
{
    private Renderer rend;
    private Color32 m_color;    
    private bool canBuild=true;
    public GameObject Menu;
    public GameObject BuildingPf; 
    Vector3 offset = new Vector3(0, 5.8f,0);
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        m_color = rend.material.color;
    }       

    // reset the initial color
    private void OnMouseExit()
    {
        rend.material.color = m_color;
    }
   
    // issue the build order for this ground tile and flag it
    public void Build()
    {
        // since this requires spending gold, check if we have enough balance
            GameManager.Instance.SubtractGold(150);
        if (GameManager.canSpendGold)
        {
            var building=Instantiate(BuildingPf, transform.position + offset, BuildingPf.transform.rotation);
            canBuild = false;
            GameManager.Instance.ShowNotification(GameManager.Instance.NotificationPf[1],"built",building.GetComponent<DetailsUI>().Thumbnail);
        }
        else
            GameManager.Instance.ShowNotification(GameManager.Instance.NotificationPf[0],$"You need {150-GameManager.GetCurrentGold()} more gold");
    }
    private void Demolish()
    {

    }

    // if you can build on this groud tile and mouse is over it, change the color
    public void Highlight()
    {
        if (canBuild)
            rend.material.color = new Color32(50, 200, 172, 255);
       
    }

    // show the UI with building options
    public void OpenActionMenu()
    {
        if (!Menu.gameObject.activeInHierarchy && canBuild)
        {            
            Menu.transform.position = transform.position + new Vector3(0, 6.0f, 0);
            Menu.SetActive(true);
            Menu.transform.GetComponent<Builder>().AsignDel(Build);
        }
        else
        {
            Menu.SetActive(false);
        }
       
    }
}
