using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSender : MonoBehaviour
{
    public Projectile_Base master;

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.parent.tag == "Enemy")
            master.OnTriggerEnter_FromCollider(other.transform.parent.GetComponent<BaseGameEntity>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent.tag == "Enemy")
            master.OnTriggerExit_FromCollider(other.transform.parent.GetComponent<BaseGameEntity>());
    }
}
