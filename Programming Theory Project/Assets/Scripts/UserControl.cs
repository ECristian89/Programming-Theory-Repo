using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script handles part of the control code, so detecting when the user clicks on a unit or building and selecting them
/// If a unit is selected it will give order to go to the clicked point when right clicking.
/// </summary>
public class UserControl : MonoBehaviour
{
    public Camera GameCamera;
    public float PanSpeed = 10.0f;
    public GameObject Marker;

    private Unit m_Selected = null;

   
    void Start()
    {
        GameCamera = Camera.main;
        Marker.SetActive(false);
    }

    void HandleSelection()
    {
        var ray = GameCamera.ScreenPointToRay(Input.mousePosition);

        // limit the area where raycasts are cast
        if (Input.mousePosition.y > Screen.height / 5)
        {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {           
            // collider could be children of the unit, so we make sure to check in the parent            
            var unitHit= hit.collider.GetComponentInParent<Unit>();

            // check if the collider parent has any information for us to show in the UI
            var detailsHit = hit.collider.GetComponentInParent<DetailsUI>();   
                if(detailsHit !=null)
                {
                    GameManager.ClearDetails();
                    GameManager.SendDetails(detailsHit);
                }
                else if(detailsHit==null)
                {
                    GameManager.ClearDetails();
                }

            var unit = hit.collider.GetComponentInParent<PlayerUnit>();
                var obj = hit.collider.GetComponentInParent<GroundTile>();
                if (obj != null)
                {
                    obj.OpenActionMenu();
                }
                    m_Selected = unit;
        }
        }
    }
    // populate the UI with selected object details
    void HandleUIContent()
    {

    }
    // highlight the buildable area
    void HandleBuildArea()
    {
        var ray = GameCamera.ScreenPointToRay(Input.mousePosition);

        // limit the area where raycasts are cast
        if (Input.mousePosition.y > Screen.height / 5)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var obj = hit.collider.GetComponentInParent<GroundTile>();
                if (obj != null)
                {
                    obj.Highlight();
                }
            }
        }
    }
    void HandleAction()
    {

        var ray = GameCamera.ScreenPointToRay(Input.mousePosition);               
        
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var unit = hit.collider.GetComponentInParent<Unit>();

            if (unit != null)
            {
                m_Selected.GoTo(unit);
            }
            else
            {
                m_Selected.GoTo(hit.point);
            }            
        }
    }
   
    void Update()
    {        
        // move camera horizontally from keyboard
        float move = Input.GetAxis("Horizontal");
        GameCamera.transform.position += new Vector3(0, 0, move) * PanSpeed * Time.deltaTime;

            HandleBuildArea();

        if (Input.GetMouseButtonDown(0))
        {
            // ABSTRACTION
            HandleSelection();
        }
        else if (m_Selected != null && Input.GetMouseButtonDown(1))
        {
            //right click gives order to the unit
            HandleAction();        
        }
        MarkerHandling();
    }

    void MarkerHandling()
    {
        if(m_Selected == null && Marker.activeInHierarchy)
        {
            Marker.SetActive(false);
            Marker.transform.SetParent(null);
        }
        else if (m_Selected != null && Marker.transform.parent != m_Selected.transform)
        {
            Marker.SetActive(true);
            Marker.transform.SetParent(m_Selected.transform, false);
            Marker.transform.localPosition = Vector3.zero;
        }
    }
}
