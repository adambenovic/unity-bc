using UnityEngine;
using UnityEngine.UIElements;
using Valve.VR.Extras;

public class VrUiInput : MonoBehaviour
{
    public SteamVR_LaserPointer leftLaser;
    public SteamVR_LaserPointer rightLaser;

    void Awake()
    {
        leftLaser.PointerIn += PointerInside;
        leftLaser.PointerOut += PointerOutside;
        leftLaser.PointerClick += PointerClick;
        
        rightLaser.PointerIn += PointerInside;
        rightLaser.PointerOut += PointerOutside;
        rightLaser.PointerClick += PointerClick;
    }

    public void PointerClick(object sender, PointerEventArgs e)
    {
        e.target.GetComponent<Clickable>().triggerAction.Invoke();
    }

    public void PointerInside(object sender, PointerEventArgs e)
    {
        Debug.Log("Cube was entered");
    }

    public void PointerOutside(object sender, PointerEventArgs e)
    {
        Debug.Log("Cube out");
    }
}