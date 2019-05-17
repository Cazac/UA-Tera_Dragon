using UnityEngine;
using System;
using System.Collections;

public class CursorSelection : MonoBehaviour
{

    [Header("The layer that floor tile is in")]
    public int tileLayer;

    private void Start() 
    {
        //Bit shift tileLayer
        tileLayer = 1 << tileLayer;
    }

    private void Update() => OnClickSelect();

    /// <summary>
    ///Detects mouse click and performs raycast to a tile, if detected then turn that tile to the color black
    /// </summary>
     ///<para>
    ///Uses raycast from mouse position to collider
    ///</para>
    private void OnClickSelect()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        Ray raycastMouse = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(raycastMouse, out hit, Mathf.Infinity, tileLayer))
        {
            //TODO: Color is hardcoded to black when a tile is clicked, need to change to dynamic
            hit.collider.gameObject.GetComponent<SpriteRenderer>().color = Color.black;
            LogRaycasthitObject(hit.collider.gameObject.transform.position.ToString(),
            hit.collider.gameObject.transform.parent.gameObject.name);
            //TODO: Delete logging
        }
    }

    /// <summary>
    ///Print position and type of object detected by raycast
    ///</summary>
     /// <param name="position">Transform position of gameobject</param>
     /// <param name="type">Parent name of gameobject</param>
    internal void LogRaycasthitObject(String position, String type)
    {
        String logString = String.Format("Hit node at position: {0}, is type of: {1}", position, type);
        Debug.Log(logString);
    }
}