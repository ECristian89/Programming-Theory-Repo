using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTile : MonoBehaviour
{
    private Renderer rend;
    private Color32 m_color;
    [SerializeField]
    private bool canBuild;
    public GameObject Menu;
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        m_color = rend.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if(canBuild)
        {
            Menu.SetActive(true);
        }
    }

    private void OnMouseEnter()
    {
        rend.material.color = new Color32(50, 200, 172,255);
    }

    private void OnMouseExit()
    {
        rend.material.color = m_color;
    }

    private void Build()
    {

    }
    private void Demolish()
    {

    }

    private void OpenActionMenu()
    {

    }
}
