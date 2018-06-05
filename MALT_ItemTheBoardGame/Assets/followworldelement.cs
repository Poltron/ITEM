using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followworldelement : MonoBehaviour
{ 
    public Transform target;
    private Canvas cvs;

	void Start ()
    {
        cvs = FindObjectOfType<Canvas>();
    }
	
	void Update ()
    {
        if (target != null)
        {//Convert the world for screen point so that it can be used with ScreenPointToLocalPointInRectangle function
            Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position);
            Vector2 movePos;

            //Convert the screenpoint to ui rectangle local point
            RectTransformUtility.ScreenPointToLocalPointInRectangle(cvs.transform as RectTransform, screenPos, cvs.worldCamera, out movePos);
            //Convert the local point to world point
            Vector3 finalpos = cvs.transform.TransformPoint(movePos);
            GetComponent<RectTransform>().position = finalpos;
        }
	}

    public void clickityclick()
    {
        Debug.Log("oui");
    }
}
