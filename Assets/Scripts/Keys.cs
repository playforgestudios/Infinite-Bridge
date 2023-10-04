using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keys : MonoBehaviour
{
    internal static int totalKeys;

    private void Awake()
    {
        var isFirstTime = true;
        if (isFirstTime)
        {
            totalKeys = 10;
        }
    }
}
