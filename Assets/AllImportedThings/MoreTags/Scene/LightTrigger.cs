﻿using MoreTags;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LightTrigger : MonoBehaviour
{
    private HashSet<GameObject> m_RedObject = new HashSet<GameObject>();
    private HashSet<GameObject> m_GreenObject = new HashSet<GameObject>();
    private HashSet<GameObject> m_BlueObject = new HashSet<GameObject>();
    private Color m_Color = Color.black;

    void Start()
    {
    }

    void Update()
    {
    }

    void UpdateLight()
    {
        m_Color.r = m_RedObject.Any() ? 1 : 0;
        m_Color.g = m_GreenObject.Any() ? 1 : 0;
        m_Color.b = m_BlueObject.Any() ? 1 : 0;
        var light = GetComponent<Light>();
        light.color = m_Color;
        light.intensity = m_Color == Color.black ? 0 : 1;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.FindTags("*.Red").Any()) m_RedObject.Add(other.gameObject);
        if (other.gameObject.FindTags("*.Green").Any()) m_GreenObject.Add(other.gameObject);
        if (other.gameObject.FindTags("*.Blue").Any()) m_BlueObject.Add(other.gameObject);
        UpdateLight();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.FindTags("*.Red").Any()) m_RedObject.Remove(other.gameObject);
        if (other.gameObject.FindTags("*.Green").Any()) m_GreenObject.Remove(other.gameObject);
        if (other.gameObject.FindTags("*.Blue").Any()) m_BlueObject.Remove(other.gameObject);
        UpdateLight();
    }
}
