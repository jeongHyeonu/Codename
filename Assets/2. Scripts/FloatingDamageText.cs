using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;

public class FloatingDamageText : MonoBehaviour, MMEventListener<MMDamageTakenEvent>
{
    [SerializeField]
    private DamageNumber damageNumber;
    private Health health;

    private void Start()
    {
        health = this.GetComponent<Health>();

    }
    public void FloatingText()
    {
        damageNumber.Spawn(new Vector3(transform.position.x, transform.position.y + 0.5f, 0), health.LastDamage);

    }
    public void OnMMEvent(MMDamageTakenEvent damageTakenEvent)
    {
        if(damageTakenEvent.AffectedHealth == health)
        {
            Debug.Log("Damage : " + damageTakenEvent.DamageCaused);
            damageNumber.Spawn(new Vector3(transform.position.x, transform.position.y + 1f, 0), damageTakenEvent.DamageCaused);
        }

    }

    private void OnEnable()
    {
        this.MMEventStartListening<MMDamageTakenEvent>();
    }

    private void OnDisable()
    {
        this.MMEventStopListening<MMDamageTakenEvent>();
    }

}