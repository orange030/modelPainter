using UnityEngine;
using System.Collections;

class test : MonoBehaviour
{
    public delegate int testDelegate();
    public testDelegate testSignal;

    public int i = 0;

    public int testSlot()
    {
        print((++i).ToString());
        return i;
    }

    public int testSlot2()
    {
        print((i+=2).ToString());
        return i;
    }

    public int testSlot3()
    {
        print((i += 3).ToString());
        return i;
    }

    //void Start()
    //{
    //    print(testSignal == null);
    //    print(testSignal());
    //    print(testSignal == null);
    //}
    void OnTriggerStay(Collider other)
    {
        print("OnTriggerStay");
    }
}