using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class GazeGrabLoggedObject : MonoBehaviour
{

    public bool logGrabbed = true, logGazed = true, logPos = false;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabbable;

    // Start is called before the first frame update
    void Start()
    {
        grabbable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
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
