using UnityEngine;
using System.Collections;

class ObjectPicker:MonoBehaviour
{
    public delegate void ObjectCall(GameObject pObject);
    public delegate void VoidCall();

    static void nullObjectCall(GameObject pObject)
    { 
    }

    public LayerMask pickLayerMask;
    public float pickDistance = 1000.0f;

    public KeyCode button0 = KeyCode.Mouse0;
    public KeyCode button1 = KeyCode.Mouse1;

    public bool checkButton0 = true;
    public bool checkButton1 = true;

    public bool pickWhenDown;
    public bool pickWhenUp;

    Ray getMainCameraRay()
    {
        var lMousePos = Input.mousePosition;
        var lRay = Camera.main.ScreenPointToRay(
            new Vector3(lMousePos.x, lMousePos.y, Camera.main.nearClipPlane));
        return lRay;

    }

    static ObjectCall toObjectCall(VoidCall pVoidCall)
    {
        return (GameObject pObject) => pVoidCall();
    }


    public void addButton0DownObjectReceiver(ObjectCall pPickEvent)
    {
        button0DownEvent += pPickEvent;
    }

    public void addButton1DownObjectReceiver(ObjectCall pPickEvent)
    {
        button1DownEvent += pPickEvent;
    }

    public void addButton0UpObjectReceiver(ObjectCall pPickEvent)
    {
        button0UpEvent += pPickEvent;
    }

    public void addButton1UpObjectReceiver(ObjectCall pPickEvent)
    {
        button1UpEvent += pPickEvent;
    }

    public void addButton0UpObjectReceiver(VoidCall pPickEvent)
    {
        button0UpEvent += toObjectCall(pPickEvent);
    }

    public void addButton1UpObjectReceiver(VoidCall pPickEvent)
    {
        button1UpEvent += toObjectCall(pPickEvent);
    }

    GameObject check()
    {
        RaycastHit lRaycastHit;
        if (Physics.Raycast(getMainCameraRay(), out lRaycastHit, pickDistance, pickLayerMask))
            return lRaycastHit.transform.gameObject;
        return null;

    }

    ObjectCall button0DownEvent;
    ObjectCall button0UpEvent;

    ObjectCall button1DownEvent;
    ObjectCall button1UpEvent;

    public delegate bool AblePickJudgeFunc();

    static bool nullAblePickJudgeFunc()
    {
        return true;
    }

    public AblePickJudgeFunc ableDownPickJudgeFunc = nullAblePickJudgeFunc;

    void Start()
    {
        if (button0DownEvent==null)
            button0DownEvent += nullObjectCall;

        if (button1DownEvent == null)
            button1DownEvent += nullObjectCall;


        if (button0UpEvent == null)
            button0UpEvent += nullObjectCall;

        if (button1UpEvent == null)
            button1UpEvent += nullObjectCall;
    }

    void Update()
    {
        if (ableDownPickJudgeFunc())
        {
            if (checkButton0 && Input.GetKeyDown(button0))
                button0DownEvent(pickWhenDown ? check() : null);

            if (checkButton1 && Input.GetKeyDown(button1))
                button1DownEvent(pickWhenDown ? check() : null);
        }
        if (checkButton0 && Input.GetKeyUp(button0))
            button0UpEvent(pickWhenUp ? check() : null);

        if (checkButton1 && Input.GetKeyUp(button1))
            button1UpEvent(pickWhenUp ? check() : null);
    }
}