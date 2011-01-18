﻿using UnityEngine;
using System.Collections;


public class InPoint:MonoBehaviour
{
    public int pointId;

    public delegate void ProcessFunc(InPoint sender, float value);

    public delegate void ProcessFuncVoidArg();

    public delegate void ProcessFuncFloatArg(float value);

    event ProcessFunc inDataFunc;

    public void addProcessFunc(ProcessFunc pFunc)
    {
        inDataFunc += pFunc;
    }

    public void addProcessFuncVoidArg(ProcessFuncVoidArg pFunc)
    {
        inDataFunc += (InPoint sender, float value) => pFunc();
    }

    public void addProcessFuncFloatArg(ProcessFuncFloatArg pFunc)
    {
        inDataFunc += (InPoint sender, float value) => pFunc(value);
    }

    [SerializeField]
    float _powerValue;

    void Awake()
    {
        _powerValue = 0f;
    }

    public float powerValue
    {
        get { return _powerValue; }
    }

    public void send(float pValue)
    {
        _powerValue = pValue;
        inDataFunc(this, pValue);
    }
}