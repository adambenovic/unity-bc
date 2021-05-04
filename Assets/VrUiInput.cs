using UnityEngine;
using Valve.VR.Extras;

public class VrUiInput : MonoBehaviour
{
    public SteamVR_LaserPointer leftLaser;
    public SteamVR_LaserPointer rightLaser;
    
    void Awake()
    {
        leftLaser.PointerClick += PointerClick;
        rightLaser.PointerClick += PointerClick;
    }
    
    public void PointerClick(object sender, PointerEventArgs e)
    {
        e.target.GetComponent<Clickable>().triggerAction.Invoke(e.target.gameObject);
    }
}