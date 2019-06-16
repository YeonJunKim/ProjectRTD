using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthbar : MonoBehaviour
{
    public GameObject healthBar;

    Vector3 originalScale;

    // Start is called before the first frame update
    void Start()
    {
        healthBar.SetActive(false);
        originalScale = healthBar.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.identity;  // dont rotate following parent
    }

    public void UpdateHealthbar(float originalHP, float curHP)
    {
        if(originalHP == curHP)
        {
            healthBar.SetActive(false);
            return;
        }

        healthBar.SetActive(true);

        float ratio = curHP / originalHP;
        float scaleX = originalScale.x * ratio;
        if (scaleX < 0.1f)
            scaleX = 0.1f;
        healthBar.transform.localScale = new Vector3(scaleX, originalScale.y, originalScale.z);
    }
}
