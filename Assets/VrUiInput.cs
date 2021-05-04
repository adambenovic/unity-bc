using UnityEngine;
using Valve.VR;
using Valve.VR.Extras;

public class VrUiInput : MonoBehaviour
{
    public SteamVR_LaserPointer leftLaser;
    public SteamVR_LaserPointer rightLaser;
    public SteamVR_Action_Single ctrlButton;
    private bool isCtrlHeld = false;

    void Awake()
    {
        leftLaser.PointerClick += PointerClick;
        rightLaser.PointerClick += PointerClick;
        
        leftLaser.PointerIn += PointerIn;
        rightLaser.PointerIn += PointerIn;
        
        leftLaser.PointerOut += PointerOut;
        rightLaser.PointerOut += PointerOut;
    }

    private void Update()
    {
        float triggerValue = ctrlButton.GetAxis(SteamVR_Input_Sources.Any);
        if (triggerValue > 0.0f)
        {
            isCtrlHeld = true;
        }
        else
        {
            isCtrlHeld = false;
        }
    }

    public void PointerClick(object sender, PointerEventArgs e)
    {
        Debug.Log("ctrl: " + isCtrlHeld);
        if (isCtrlHeld)
        {
            e.target.GetComponent<Clickable>().triggerCtrlAction.Invoke(e.target.gameObject, false);
        }
        else
        {
            e.target.GetComponent<Clickable>().triggerAction.Invoke(e.target.gameObject);
        }
    }

    public void PointerIn(object sender, PointerEventArgs e)
    {
        var highliter = e.target.GetComponent<BackgroundHighlighter>();
        if (highliter && !highliter.IsHighlightedOutline())
        {
            highliter.HighlightOutline(highliter.myGreen);
        }
    }

    public void PointerOut(object sender, PointerEventArgs e)
    {
        var highliter = e.target.GetComponent<BackgroundHighlighter>();
        if (highliter && highliter.IsHighlightedOutline() && highliter.highlightColor == highliter.myGreen)
        {
            highliter.UnhighlightOutline();
        }
    }
}
