using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class DesktopTouch : MonoBehaviour
{
#if UNITY_EDITOR

    private static Touch[] touches;
    
    private void Awake()
    {
        var method1 = typeof(Input).GetMethod("GetTouch");  
        var hkMethod1 = typeof(DesktopTouch).GetMethod("GetTouch");
        RedirectionHelper.RedirectCalls(method1, hkMethod1);
        
        var method2 = typeof(Input).GetMethod("get_touchCount");
        var hkMethod2 = typeof(DesktopTouch).GetMethod("get_touchCount");
        RedirectionHelper.RedirectCalls(method2, hkMethod2);
        
    }

    public static int touchCount
    {
        get { return touches == null ? 0 : touches.Length; }
    }

    public static Touch GetTouch(int index)
    {
        return touches[index];
    }

    private void Update()
    { 
        if (Input.GetMouseButtonDown(0))
        {
            touches = new Touch[1];
            
            touches[0] = new Touch()
            {
                deltaPosition = Vector2.zero,
                position = Input.mousePosition,
                phase = TouchPhase.Began,
                fingerId = 0
            };
            return;
        }
        
        if (Input.GetMouseButton(0))
        {            
            touches[0].deltaPosition = (Vector2) Input.mousePosition - touches[0].position;
            touches[0].position = Input.mousePosition;
            touches[0].phase = TouchPhase.Moved;
            touches[0].fingerId = 0;
            return;        
        }

        if (Input.GetMouseButtonUp(0))
        {
            touches[0].deltaPosition = (Vector2) Input.mousePosition - touches[0].position;
            touches[0].position = Input.mousePosition;
            touches[0].phase = TouchPhase.Ended;
            touches[0].fingerId = 0;
            return;
        }

        touches = null;
    }
    #endif
}
