using MoreMountains.TopDownEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachinGunS_Bullet : MonoBehaviour
{
    // �Ѿ��� ������Ʈ Ǯ������ ��Ȱ��ȭ�Ǹ� circle collider �ٽ� ���󺹱�
    [SerializeField] float originColliderRadius;
    [SerializeField] GameObject impactEffect;

    [SerializeField] GameObject enchantedEffect;
    [SerializeField] GameObject originEffect;

    public static int shootCnt=0;

    private void OnEnable()
    {
        shootCnt++;
        Debug.Log(shootCnt);

        if (shootCnt % 5 == 0)
        {
            shootCnt = 0;
            impactEffect.transform.GetChild(0).localScale = new Vector3(1f, 1f, 1f);
            impactEffect.transform.GetChild(1).localScale = new Vector3(1f, 1f, 1f);

            this.GetComponent<DamageOnTouch>().HitAnythingEvent.RemoveAllListeners();
        }
        else
        {
            impactEffect.transform.GetChild(0).localScale = new Vector3(.1f, .1f,.1f);
            impactEffect.transform.GetChild(1).localScale = new Vector3(.1f, .1f, .1f);

            this.GetComponent<DamageOnTouch>().HitAnythingEvent.AddListener(grenadeEvent);
        }
    }

    private void grenadeEvent(GameObject arg0)
    {
        GetComponent<CircleCollider2D>().radius = 5f;
        GetComponent<CircleCollider2D>().enabled = true;
        GetComponent<CircleCollider2D>().radius = 0f;
        GetComponent<CircleCollider2D>().enabled = false;
    }

    private void OnDisable()
    {
        GetComponent<CircleCollider2D>().radius = originColliderRadius;
    }
}
