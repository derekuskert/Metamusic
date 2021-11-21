using System;
using System.Collections;
using System.Collections.Generic;
using Facebook.WitAi;
using Oculus.Voice;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInteractions : MonoBehaviour
{
    Spline OverlappingSplineScript;
    Spline SelectedSplineScript;

    [SerializeField] public AppVoiceExperience wit;

    [SerializeField]
    public GameObject VoiceManager;

    private bool flip = false;

    // Start is called before the first frame update
    void Start()
    {
        //if (!wit) wit = FindObjectOfType<Wit>();
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

        if (OVRInput.GetDown(OVRInput.Button.Four) || Input.GetKeyDown(KeyCode.Space))
        {
            wit.Activate();
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (!other.transform.root.gameObject.GetComponent<Spline>()) return;
        if (SelectedSplineScript) return;
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
        
        if (!OverlappingSplineScript) return;
        
        SelectedSplineScript = OverlappingSplineScript;
        
        SelectedSplineScript.isInteractedWith = true; 
        SelectedSplineScript.objectToFollow = transform;

        flip = true;

        SelectedSplineScript.InteractSynthNote();
    }

    public void SetASDR(string[] values)
    {
        foreach (string str in values)
        {
            Debug.Log(str + " ");
        }
        SelectedSplineScript.Synth.SetADSR(values[1],values[0]);
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
    
    public void SetOctave(string[] values)
    {
        foreach (string str in values)
        {
            Debug.Log(str + " ");
        }

        switch (values[1])
        {
            case "Set":
                SelectedSplineScript.Synth.octaveLevel = int.Parse(values[0]);
                break;
            default:
                SelectedSplineScript.Synth.octaveLevel = int.Parse(values[0]);
                break;
        }

    }
}
