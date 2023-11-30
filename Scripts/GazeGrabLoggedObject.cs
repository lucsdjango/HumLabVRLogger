using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class GazeGrabLoggedObject : MonoBehaviour
{

    public bool logGrabbed = true, logGazed = true, logPos = false;

    private XRGrabInteractable grabbable;

    // Start is called before the first frame update
    void Start()
    {
        grabbable = GetComponent<XRGrabInteractable>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsGrabbed()
    {
        return grabbable.isSelected;
    }

}
