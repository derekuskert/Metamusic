using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    Spline OverlappingSplineScript;
    Spline SelectedSplineScript;

    [SerializeField]
    public GameObject VoiceManager;

    private bool flip = false;

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
        if (flip) return;

        flip = true;
        if (!OverlappingSplineScript) return;

        SelectedSplineScript = OverlappingSplineScript;

        SelectedSplineScript.isInteractedWith = true;
        SelectedSplineScript.objectToFollow = transform;
        SelectedSplineScript.InteractSynthNote();
    }

    void SetSustainAmp(float f)
    {
        SelectedSplineScript.Synth._envelope.SustainAmplitude = f;
    }

    //Derefernces the spline and tells the spline that it's no longer being interacted with.
    void Release()
    {
        if (!flip) return;

        flip = false;
        if (!SelectedSplineScript) return;

        SelectedSplineScript.isInteractedWith = false;
        SelectedSplineScript.InteractSynthNote();
        SelectedSplineScript = null;
    }
}
