using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spline : MonoBehaviour
{
    private Vector3[] splinePoint;
    private int splineCount;

    private GameObject NewSplinePoint;

    public bool debug_drawspline = true;
    public bool isInteractedWith = false;

    public new LineRenderer Line;

    private void Start()
    {
        splineCount = transform.childCount;
        splinePoint = new Vector3[splineCount];
    }


    private void Update()
    {
        UpdateSplinePointLocations();

        // set the color of the line
        Line.startColor = Color.blue;
        Line.endColor = Color.green;

        // set width of the renderer
        Line.startWidth = 0.05f;
        Line.endWidth = 0.05f;

        Line.SetPosition(0, splinePoint[0]);
        Line.SetPosition(1, splinePoint[2]);

        UpdateMeshes();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.name == "OVRControllerPrefab")
        {
            isInteractedWith = true;
        }
    }

    private void UpdateSplinePointLocations()
    {
        for (int i = 0; i < splineCount; i++)
        {
            splinePoint[i] = transform.GetChild(i).position;
        }

        if (!isInteractedWith)
        {
            transform.GetChild(1).position = Vector3.Lerp(transform.GetChild(0).position, transform.GetChild(2).position, 0.5f);
        }
        else
        {
            transform.GetChild(1).position = Vector3.zero; //GameObject.Find("RightHandAnchor").transform.position;
        }
    }

    //Used when creating the spline and when grabbing in the middle of the spline
    public void AttachNewPoint(Vector3 SpawnLocation)
    {
        
    }

    //Used when the player lets go of the spline
    public void ReturnControlPoint()
    {
        
    }

    private void UpdateMeshes()
    {
        GameObject Mesh1 = transform.GetChild(0).GetChild(0).gameObject;
        GameObject Mesh2 = transform.GetChild(2).GetChild(0).gameObject;

        Mesh1.transform.localScale = new Vector3(0.05f, Vector3.Distance(transform.GetChild(0).position, transform.GetChild(1).position), 0.05f);
        Mesh2.transform.localScale = new Vector3(0.05f, Vector3.Distance(transform.GetChild(2).position, transform.GetChild(1).position), 0.05f);

        Mesh1.transform.LookAt(transform.GetChild(1), Vector3.right);
        Mesh2.transform.LookAt(transform.GetChild(1), Vector3.right);

        Mesh1.transform.rotation *= Quaternion.FromToRotation(Vector3.up, Vector3.forward);
        Mesh2.transform.rotation *= Quaternion.FromToRotation(Vector3.up, Vector3.forward);
    }
    /*
     Spawn point(this is for the creation and when the player creates the spline or tries modifyig music)

     Spawn meshes along spline

     Update meshes along the spline
     
     Grabbed

     Released
     
     */

}