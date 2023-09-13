using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachinGunS_Bullet : MonoBehaviour
{
    // �Ѿ��� ������Ʈ Ǯ������ ��Ȱ��ȭ�Ǹ� circle collider �ٽ� ���󺹱�
    [SerializeField] float originColliderRadius;

    private void OnDisable()
    {
        GetComponent<CircleCollider2D>().radius = originColliderRadius;
    }
}
