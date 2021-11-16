using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineBuilder : MonoBehaviour
{
    private float defaultLength = 3.0f;
    private LineRenderer LineRenderer = null;
    RaycastHit hit;

    bool flip;

    private GameObject newSpline = null;

    public GameObject splinePrefab;

    private int SplineSpawnCount = 0;

    private void Awake()
    {
        LineRenderer = GetComponent<LineRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.5 || Input.GetKeyDown(KeyCode.Space))
        {
            if (flip)
            {
                if (newSpline == null)
                {
                    LineRenderer.startColor = Color.red;
                    newSpline = Instantiate(splinePrefab, hit.point, Quaternion.identity);
                    newSpline.name = "Spline" + SplineSpawnCount++.ToString();
                }
                else
                {
                    LineRenderer.startColor = Color.green;
                    newSpline = null;
                }
                flip = false;
            }
        }
        else if(OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) < 0.1)
        {
            flip = true;
        }
        if (newSpline != null)
        {
            LineRenderer.startColor = Color.red;
            GameObject.Find(newSpline.name).transform.GetChild(2).position = hit.point;
        }
        UpdateLength();
    }
    private void UpdateLength()
    {
        LineRenderer.SetPosition(0, transform.position);
        LineRenderer.SetPosition(1, CalculateEnd());
    }

    private RaycastHit CreateForwardRaycast()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        Physics.Raycast(ray, out hit, defaultLength);
        return hit;
    }

    private Vector3 CalculateEnd()
    {
        RaycastHit hit = CreateForwardRaycast();
        Vector3 endPosition = DefaultEnd(defaultLength);

        if (hit.collider)
        {
            endPosition = hit.point;
        }

        return endPosition;
    }

    private Vector3 DefaultEnd(float length)
    {
        return transform.position + (transform.forward * length);
    }
}
