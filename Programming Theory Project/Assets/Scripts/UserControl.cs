using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script handles part of the control code, so detecting when the user clicks on a unit or building and selecting them
/// If a unit is selected it will give order to go to the clicked point when right clicking.
/// </summary>
public class UserControl : MonoBehaviour
{
    public Camera GameCamera;
    public float PanSpeed = 15.0f;
    public Transform LeftCameraBound;
    public Transform RightCameraBound;
    public GameObject Marker;
    public GameObject DestinationMarker;

    private Unit m_Selected = null;
    private DetailsUI detailsHit;


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
             detailsHit = hit.collider.GetComponentInParent<DetailsUI>();   
                if(detailsHit !=null)
                {                   
                    GameManager.Instance.SetUI(ref detailsHit);
                    GameManager.ClearDetails();
                    GameManager.SendDetails(detailsHit);    
                    
                    if(detailsHit.transform.GetComponentInParent<PlayerBuilding>())   // GET REFERMCE TO CURRENT DETAILS IN GAME MANAGER
                    {
                        if(GameManager.UpgradeButton != null)
                        {
                            GameManager.UpgradeButton.transform.GetComponentInParent<Button>().onClick.AddListener(detailsHit.transform.GetComponentInParent<PlayerBuilding>().UpgradeBuilding);
                        }


                        // we have to make sure we asign prefabs in the inspector
                            if (GameManager.SelectionInteractable[0] != null)
                            {

                                GameManager.SelectionInteractable[0].transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate
                                {
                                    detailsHit.transform.GetComponentInParent<PlayerBuilding>().CreateUnit(0);
                                });
                            }

                        if (GameManager.SelectionInteractable[1] != null)
                        {

                            GameManager.SelectionInteractable[1].transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate
                            {
                                detailsHit.transform.GetComponentInParent<PlayerBuilding>().CreateUnit(1);
                            });
                        }


                    }
                }
                else if(detailsHit==null)
                {                    
                    GameManager.ClearDetails();
                }
                // if the clicked object is player unit keep track of it in Game Manager
                // needed for Marker handling
            var unit = hit.collider.GetComponentInParent<PlayerUnit>();
                if(unit != null)
                {
                    GameManager.isCurrentPlayer = true;
                }
                else
                {
                    GameManager.isCurrentPlayer = false;
                }

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
                // show click feedback for the user
                DestinationMarker.transform.position = hit.point + new Vector3(0,0.67f,0);
                DestinationMarker.SetActive(true);
                DestinationMarker.transform.GetComponent<DisableTimer>().RefreshCounter();   // reset the timer
            }            
        }
    }
   
    void Update()
    {
        // move camera horizontally from keyboard
        // within set limits
        if (GameCamera.transform.position.z >= LeftCameraBound.position.z && GameCamera.transform.position.z <= RightCameraBound.position.z)
        {
            float move = Input.GetAxis("Horizontal");
            GameCamera.transform.position += new Vector3(0, 0, move) * PanSpeed * Time.deltaTime;
        }
        else if( GameCamera.transform.position.z < LeftCameraBound.position.z)
        {
            GameCamera.transform.position = LeftCameraBound.position; // must set the bounds objects to same depth distance as Game Camera
        }
        else if(GameCamera.transform.position.z > RightCameraBound.position.z)
        {
            GameCamera.transform.position = RightCameraBound.position;
        }

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
            // Clear UI only if the player unit is flagged
            if(GameManager.isCurrentPlayer)
            {
                GameManager.ClearDetails();
            }
        }
        else if (m_Selected != null && Marker.transform.parent != m_Selected.transform)
        {
            Marker.SetActive(true);
            Marker.transform.SetParent(m_Selected.transform, false);
            Marker.transform.localPosition = Vector3.zero;            
        }
    }
}
