using BNG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Add this Component to any Canvas to make sure it can be interacted with in World Space
/// </summary>
[RequireComponent(typeof(GraphicRaycaster))]
[RequireComponent(typeof(Canvas))]
public class VRAdvancedCanvas : MonoBehaviour
{

    void Start()
    {
        VRUIAdvancedSystem.Instance.AddCanvas(GetComponent<Canvas>());
    }
}

