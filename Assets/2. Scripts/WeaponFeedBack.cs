using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFeedBack : MonoBehaviour
{
    // �ǵ�� ����
    [SerializeField] GameObject ShotGunS_feedback;

    // ���� ��ü �� �ǵ��
    // ���� S ��� �� ��� -> �÷��̾� �̼� ����

    public enum WeaponType{
        None,
        PistolS,
        MachinGunS,
    }

    [SerializeField] WeaponType weaponType;

    private void OnEnable()
    {
        switch (weaponType)
        {
            case WeaponType.PistolS:
                Debug.Log("�÷��̾� �̼� ����");
                LevelManager.Instance.Players[0].GetComponent<CharacterMovement>().MovementSpeed *= 2f;
                break;
            case WeaponType.MachinGunS:
                //this.GetComponent<MMSimpleObjectPooler>().GameObjectToPool.GetComponent<DamageOnTouch>().HitAnythingFeedback = ShotGunS_feedback.GetComponent<MMF_Player>();
                //Debug.Log(this.GetComponent<MMSimpleObjectPooler>().GameObjectToPool.GetComponent<DamageOnTouch>().HitAnythingFeedback);
                //ShotGunS_feedback.transform.parent = this.GetComponent<MMSimpleObjectPooler>().GameObjectToPool.transform;
                break;
        }   
    }

    private void OnDisable()
    {
        switch (weaponType)
        {
            case WeaponType.PistolS:
                Debug.Log("�÷��̾� �̼� ���󺹱�");
                LevelManager.Instance.Players[0].GetComponent<CharacterMovement>().MovementSpeed *= .5f;
                break;
            case WeaponType.MachinGunS:
                break;
        }
    }
}
