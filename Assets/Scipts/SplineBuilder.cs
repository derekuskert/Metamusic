using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineBuilder : MonoBehaviour
{
    private const float DefaultLength = 3.0f;
    private LineRenderer _lineRenderer = null;
    private RaycastHit _hit;

    private bool _flip;

    private GameObject _newSpline = null;

    private Spline OverlappingSplineScript;

    public GameObject splinePrefab;

    private int _splineSpawnCount = 0;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.transform.root.gameObject.GetComponent<Spline>()) return;

        OverlappingSplineScript = other.transform.root.gameObject.GetComponent<Spline>();
    }
    
    private void OnTriggerExit(Collider other)
    {
        OverlappingSplineScript = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            if (OverlappingSplineScript)
            {
                Destroy(OverlappingSplineScript.gameObject.transform.root.gameObject);
            }
        }
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.5)
        {
            if (_flip)
            {
                if (_newSpline == null)
                {
                    _lineRenderer.startColor = Color.red;
                    _newSpline = Instantiate(splinePrefab, _hit.point, Quaternion.identity);
                    _newSpline.name = "Spline" + _splineSpawnCount++.ToString();
                }
                else
                {
                    _lineRenderer.startColor = Color.green;
                    _newSpline = null;
                }
                _flip = false;
            }
        }
        else if(OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) < 0.1)
        {
            _flip = true;
        }
        if (_newSpline != null)
        {
            _lineRenderer.startColor = Color.red;
            GameObject.Find(_newSpline.name).transform.GetChild(2).position = _hit.point;
        }
        UpdateLength();
    }
    private void UpdateLength()
    {
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, CalculateEnd());
    }

    private RaycastHit CreateForwardRaycast()
    {
        var transform1 = transform;
        Ray ray = new Ray(transform1.position, transform1.forward);

        Physics.Raycast(ray, out _hit, DefaultLength);
        return _hit;
    }

    private Vector3 CalculateEnd()
    {
        RaycastHit hit = CreateForwardRaycast();
        Vector3 endPosition = DefaultEnd(DefaultLength);

        if (hit.collider)
        {
            endPosition = hit.point;
        }

        return endPosition;
    }

    private Vector3 DefaultEnd(float length)
    {
        var transform1 = transform;
        return transform1.position + (transform1.forward * length);
    }
}
