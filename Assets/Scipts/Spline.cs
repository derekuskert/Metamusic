using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spline : MonoBehaviour
{
    [SerializeField]
    private SynthPlayer Synth;

    private OVRPassthroughLayer passthroughLayer;

    private Vector3[] splinePoint;
    private int splineCount;

    private GameObject NewSplinePoint;

    public bool debug_drawspline = true;
    public bool isInteractedWith = false;

    public new LineRenderer Line;

    public Transform objectToFollow;

    private void Start()
    {
        splineCount = transform.childCount;
        splinePoint = new Vector3[splineCount];

        // set the color of the line
        Line.startColor = Color.blue;
        Line.endColor = Color.blue;

        // set width of the renderer
        Line.startWidth = 0.05f;
        Line.endWidth = 0.05f;

        passthroughLayer = FindObjectOfType<OVRPassthroughLayer>();
    }


    private void Update()
    {
        UpdateSplinePointLocations();

        Line.SetPosition(0, splinePoint[0]);
        Line.SetPosition(1, splinePoint[2]);

        UpdateMeshes();

        if (isInteractedWith)
        {
            UpdateSynthPitch();
            passthroughLayer.edgeRenderingEnabled = true;
            Synth.gain = Synth.volume;
        }
        else
        {
            passthroughLayer.edgeRenderingEnabled = false;
            Synth.gain = 0;
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
            transform.GetChild(1).position = objectToFollow.position; //GameObject.Find("RightHandAnchor").transform.position;
        }
    }

    private void UpdateSynthPitch()
    {
        //Creates a value of -1 to 1 by calculating the 'control point' between the 2 ending points.
        //This is then plugged into an inverse lerp which will produce a value between 0 and 1 for the lerp to use for determining the
        //frequency of the synth
        Synth.frequency = Mathf.Lerp(10, 11000,Mathf.InverseLerp(-1, 1,(
                Vector3.Distance(transform.GetChild(2).position, transform.GetChild(1).position) - 
                Vector3.Distance(transform.GetChild(0).position, transform.GetChild(1).position)
            ) / Vector3.Distance(transform.GetChild(2).position, transform.GetChild(0).position)));
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