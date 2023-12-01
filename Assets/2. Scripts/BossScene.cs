using Cinemachine;
using DG.Tweening;
using JetBrains.Annotations;
using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class BossScene : MonoBehaviour
{
    [SerializeField] public CinemachineVirtualCamera vcam1; // �÷��̾� ī�޶�
    [SerializeField] public CinemachineVirtualCamera vcam2; // ���� ī�޶�
    [SerializeField] public GameObject minimap; // �̴ϸ�
    [SerializeField] public GameObject bossVideo; // �÷��̾� vs ���� ���� ĵ����
    [SerializeField] public GameObject bossRoomCenter; // ���� �� �߾�
    [SerializeField] public GameObject bossPos; // ���� ��ġ

    [SerializeField] public GameObject leftRoom; // ���ʹ� object
    [SerializeField] public GameObject rightRoom; // �����ʹ� object
    [SerializeField] public CinemachineVirtualCamera vcamLeftRoom; // ���ʹ� ����ī�޶�
    [SerializeField] public CinemachineVirtualCamera vcamRightRoom; // �����ʹ� ����ī�޶�

    public static BossScene instance;
    private void Awake() { instance = this; }


    // ī�޶� ����, DoorEvent���� ������ ���Խ� ȣ���
    public void CameraMoveToBoss()
    {
        // ���� �÷��̾� �Ͻ�����, Ű �Է� ���´�
        LevelManager.Instance.Players[0].GetComponent<CharacterMovement>().enabled = false;
        InputManager.Instance.InputDetectionActive = false;

        // UI OFF
        GUIManager.Instance.gameObject.SetActive(false);
        minimap.SetActive(false);
        // 0.5�� ��, ���� ī�޶� �켱���� ���� �� ����ī�޶� on
        StartCoroutine(VcamSetPriority(.5f));

        // ī�޶� Ȱ��ȭ
        vcam1.Follow = LevelManager.Instance.Players[0].transform; // ī�޶� �ٶ󺸴� ��ü �÷��̾��
        vcam1.LookAt = LevelManager.Instance.Players[0].transform; // ī�޶� �ٶ󺸴� ��ü �÷��̾��

        gameObject.GetComponentInChildren<Character>()._animator.SetBool("Enter", true);

        // 3f�� �� ���� ����
        StartCoroutine(BossProduce(3f));
    }


    // _time �ð� �ڿ� ���� ����, ����ī�޶� �켱���� ����
    public IEnumerator VcamSetPriority(float _time)
    {
        yield return new WaitForSeconds(_time);

        // ���� ī�޶� �켱���� ���� �� Ȱ��ȭ
        vcam1.Priority = 0;
        vcam2.Priority = 10;
        vcam1.gameObject.SetActive(true);
        vcam2.gameObject.SetActive(true);
    }

    // _time �ð� �ڿ� ���� ���� ����
    public IEnumerator BossProduce(float _time)
    {
        yield return new WaitForSeconds(_time);
        gameObject.GetComponentInChildren<Character>()._animator.SetBool("Enter", false);
        // �÷��̾�vs���� ����
        bossVideo.SetActive(true);

        //  bossVideo.transform.GetChild(0).GetComponent<RawImage>().DOFade(.7f,.5f).SetDelay(1.0f); // ���� ���̵���
        //  bossVideo.transform.GetChild(0).DOScaleY(1f, .5f).From(0f).SetDelay(1.0f); // ���� ���̵���

        //bossVideo.transform.GetChild(0).GetComponent<RawImage>().color = Color.black;

        bossVideo.transform.GetChild(0).GetComponent<VideoPlayer>().Prepare();
        bossVideo.transform.GetChild(0).GetComponent<VideoPlayer>().time = 0f;
        bossVideo.transform.GetChild(0).GetComponent<VideoPlayer>().Pause();
        bossVideo.transform.GetChild(0).DOScaleY(1f, .1f).From(0f).SetDelay(.5f).OnComplete(() =>
        {
            bossVideo.transform.GetChild(0).DOScaleY(1f, 0f).SetDelay(.5f).OnComplete(() =>
            {
                bossVideo.transform.GetChild(0).GetComponent<VideoPlayer>().Play();
            });
            bossVideo.transform.GetChild(0).DOScaleY(0f, .1f).From(1f).SetDelay(5.5f);
        });

        // ���� ���� ������ 6f�� �ڿ� �÷��̾�� ȭ�� �̵�
        StartCoroutine(CameraMoveToPlayer(8f));
    }


    // _time �ð� �ڿ� �÷��̾�� ī�޶� �̵�
    private IEnumerator CameraMoveToPlayer(float _time)
    {
        yield return new WaitForSeconds(_time);
        // �÷��̾� vs ���� UI off
        bossVideo.SetActive(false);

        // UI On
        GUIManager.Instance.gameObject.SetActive(true);

        // ī�޶� �켱���� ����, ( �� �÷��̾� ī�޶�� �̵���)
        vcam1.Priority = 10;
        vcam2.Priority = 0;
        vcamLeftRoom.Priority = 0;
        vcamRightRoom.Priority = 0;

        // �Ͻ����� ����, Ű �Է� �ٽ� �޴´�
        LevelManager.Instance.Players[0].GetComponent<CharacterMovement>().enabled = true;
        InputManager.Instance.InputDetectionActive = true;

        // ���� ������ ����ī�޶� ���� ����, ���� ü�¹� ����
        StoneGolemHealthBar.instance.bossUI_enable();

        // 2f�ʵ� ����ī�޶� OFF, 
        // (ī�޶� �� ��ȯ �ӵ��� ���� ī�޶��� Custom Blends �� ����)
        StartCoroutine(VcamOff(2f));
        LevelManager.Instance.Players[0].gameObject.GetComponent<CharacterMovement>().MovementSpeedMultiplier = 1;
        LevelManager.Instance.Players[0].gameObject.GetComponent<Character>().ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
    }


    // _time �ð� �ڿ� ���� ī�޶� ��Ȱ��ȭ
    private IEnumerator VcamOff(float _time)
    {
        yield return new WaitForSeconds(_time);
        vcam1.gameObject.SetActive(false);
        vcam2.gameObject.SetActive(false);
        vcamLeftRoom.gameObject.SetActive(false);
        vcamRightRoom.gameObject.SetActive(false);
    }

    // ���� �����, �� ����
    public void BossRoomAppear()
    {
        // ���� ��ġ üũ
        float leftOrRight = bossPos.transform.position.x - bossRoomCenter.transform.position.x;

        // ���� �÷��̾� �Ͻ�����, Ű �Է� ���´�
        LevelManager.Instance.Players[0].gameObject.GetComponent<CharacterMovement>().MovementSpeedMultiplier = 0;
        InputManager.Instance.InputDetectionActive = false;

        // ���� ī�޶� �켱���� ���� �� Ȱ��ȭ
        vcam1.Priority = 0;
        vcam2.Priority = 10;
        vcam1.gameObject.SetActive(true);
        vcam2.gameObject.SetActive(true);

        if (leftOrRight > 0) // 3�� �� ���� �� ����
        {
            StartCoroutine(LeftDoorOpen(4f));
        }
        else // 3�� �� ������ �� ����
        {
            StartCoroutine(RightDoorOpen(4f));
        }

    }

    IEnumerator RightDoorOpen(float _time)
    {
        yield return new WaitForSeconds(2f);
        //gameObject.GetComponentInChildren<Character>()._animator.SetTrigger("DieStart");
        
        yield return new WaitForSeconds(_time - 2f);

        // ���� ī�޶� �켱���� ���� �� Ȱ��ȭ
        vcam1.Priority = 0;
        vcam2.Priority = 0;
        vcamRightRoom.Priority = 10;
        rightRoom.SetActive(true);
        vcamRightRoom.gameObject.SetActive(true);

        // 4�� �� �÷��̾�� ī�޶� �̵�
        StartCoroutine(CameraMoveToPlayer(4f));
    }

    IEnumerator LeftDoorOpen(float _time)
    {
        yield return new WaitForSeconds(2f);
        //gameObject.GetComponentInChildren<Character>()._animator.SetTrigger("DieStart");

        yield return new WaitForSeconds(_time - 2f);

        // ���� ī�޶� �켱���� ���� �� Ȱ��ȭ
        vcam1.gameObject.SetActive(false);
        vcam2.Priority = 0;
        vcamLeftRoom.Priority = 10;
        leftRoom.SetActive(true);
        vcamLeftRoom.gameObject.SetActive(true);

        // 4�� �� �÷��̾�� ī�޶� �̵�
        StartCoroutine(CameraMoveToPlayer(4f));
    }

}
