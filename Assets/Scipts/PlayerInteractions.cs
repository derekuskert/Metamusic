using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    Spline OverlappingSplineScript;
    Spline SelectedSplineScript;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) > 0.15f)
        {
            Grab();
        }
        else
        {
            Release();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        OverlappingSplineScript = other.transform.root.gameObject.GetComponent<Spline>();
    }
    private void OnTriggerExit(Collider other)
    {
        if(OverlappingSplineScript == other.transform.root.gameObject.GetComponent<Spline>())
        {
            OverlappingSplineScript = null;
        }
    }

    //Tells the spline that it's being interacted with and gives it the position to attach to.
    void Grab()
    {
        if (!OverlappingSplineScript) return;

        SelectedSplineScript = OverlappingSplineScript;

        SelectedSplineScript.isInteractedWith = true;
        SelectedSplineScript.objectToFollow = transform;
    }

    //Derefernces the spline and tells the spline that it's no longer being interacted with.
    void Release()
    {
        if (!SelectedSplineScript) return;

        SelectedSplineScript.isInteractedWith = false;
        SelectedSplineScript = null;
    }
}
