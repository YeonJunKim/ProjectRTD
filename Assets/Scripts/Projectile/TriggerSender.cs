using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSender : MonoBehaviour
{
    public Projectile_Base master;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
            master.OnTriggerEnter_FromCollider(other.GetComponent<BaseGameEntity>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
            master.OnTriggerExit_FromCollider(other.GetComponent<BaseGameEntity>());
    }
}
