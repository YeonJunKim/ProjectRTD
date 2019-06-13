using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrePlacer : MonoBehaviour
{
    Renderer m_renderer;

    Color originalColor;
    Color occupiedColor;

    List<GameObject> touchingObjects;

    bool isSuitable;

    private void Awake()
    {
        m_renderer = GetComponent<Renderer>();
        originalColor = m_renderer.material.color;
        occupiedColor = new Color(1, 0.3f, 0.3f, 0.8f); 
        touchingObjects = new List<GameObject>();
        isSuitable = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        touchingObjects.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        touchingObjects.Remove(other.gameObject);
    }

    private void Update()
    {
        isSuitable = false;
        m_renderer.material.color = occupiedColor;
        foreach (var obj in touchingObjects)
        {
            if (obj.tag == "Tower")
            {
                isSuitable = false;
                m_renderer.material.color = occupiedColor;
                break;
            }
            if (obj.tag == "Ground")
            {
                isSuitable = true;
                m_renderer.material.color = originalColor;
            }
        }
           
    }

    public void Init()
    {
        transform.position = new Vector3(-100, -100, -100);
        isSuitable = false;
        touchingObjects.Clear();
        m_renderer.material.color = originalColor;
    }


    public bool IsSuitable()
    {
        return isSuitable;
    }
}
