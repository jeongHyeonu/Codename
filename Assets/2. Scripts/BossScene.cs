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
    [SerializeField] public CinemachineVirtualCamera vcam1; // 플레이어 카메라
    [SerializeField] public CinemachineVirtualCamera vcam2; // 보스 카메라
    [SerializeField] public GameObject minimap; // 미니맵
    [SerializeField] public GameObject minimapRawImage; // 미니맵
    [SerializeField] public GameObject bossVideo; // 플레이어 vs 보스 영상 캔버스
    [SerializeField] public GameObject bossRoomCenter; // 보스 방 중앙
    [SerializeField] public GameObject bossPos; // 보스 위치

    [SerializeField] public GameObject leftRoom; // 왼쪽방 object
    [SerializeField] public GameObject rightRoom; // 오른쪽방 object
    [SerializeField] public CinemachineVirtualCamera vcamLeftRoom; // 왼쪽방 가상카메라
    [SerializeField] public CinemachineVirtualCamera vcamRightRoom; // 오른쪽방 가상카메라

    public static BossScene instance;
    private void Awake() { instance = this; }


    // 카메라 연출, DoorEvent에서 보스방 진입시 호출됨
    public void CameraMoveToBoss()
    {
        // 먼저 플레이어 일시정지, 키 입력 막는다
        LevelManager.Instance.Players[0].GetComponent<CharacterMovement>().enabled = false;
        InputManager.Instance.InputDetectionActive = false;

        // UI OFF
        GUIManager.Instance.gameObject.SetActive(false);
        minimap.SetActive(false);
        minimapRawImage.SetActive(false);
        // 0.5초 뒤, 가상 카메라 우선순위 설정 및 가상카메라 on
        StartCoroutine(VcamSetPriority(.5f));

        // 카메라 활성화
        vcam1.Follow = LevelManager.Instance.Players[0].transform; // 카메라 바라보는 객체 플레이어로
        vcam1.LookAt = LevelManager.Instance.Players[0].transform; // 카메라 바라보는 객체 플레이어로

        gameObject.GetComponentInChildren<Character>()._animator.SetBool("Enter", true);

        // 3f초 뒤 보스 연출
        StartCoroutine(BossProduce(3f));
    }


    // _time 시간 뒤에 보스 연출, 가상카메라 우선순위 설정
    public IEnumerator VcamSetPriority(float _time)
    {
        yield return new WaitForSeconds(_time);

        // 가상 카메라 우선순위 설정 및 활성화
        vcam1.Priority = 0;
        vcam2.Priority = 10;
        vcam1.gameObject.SetActive(true);
        vcam2.gameObject.SetActive(true);
    }

    // _time 시간 뒤에 등장 보스 연출
    public IEnumerator BossProduce(float _time)
    {
        yield return new WaitForSeconds(_time);
        gameObject.GetComponentInChildren<Character>()._animator.SetBool("Enter", false);
        // 플레이어vs보스 영상
        bossVideo.SetActive(true);

        //  bossVideo.transform.GetChild(0).GetComponent<RawImage>().DOFade(.7f,.5f).SetDelay(1.0f); // 영상 페이드인
        //  bossVideo.transform.GetChild(0).DOScaleY(1f, .5f).From(0f).SetDelay(1.0f); // 영상 페이드인

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

        // 보스 연출 끝나고 6f초 뒤에 플레이어로 화면 이동
        StartCoroutine(CameraMoveToPlayer(8f));
    }


    // _time 시간 뒤에 플레이어로 카메라 이동
    private IEnumerator CameraMoveToPlayer(float _time)
    {
        yield return new WaitForSeconds(_time);
        // 플레이어 vs 보스 UI off
        bossVideo.SetActive(false);

        // UI On
        GUIManager.Instance.gameObject.SetActive(true);

        // 카메라 우선순위 변경, ( → 플레이어 카메라로 이동함)
        vcam1.Priority = 10;
        vcam2.Priority = 0;
        vcamLeftRoom.Priority = 0;
        vcamRightRoom.Priority = 0;

        // 일시정지 해제, 키 입력 다시 받는다
        LevelManager.Instance.Players[0].GetComponent<CharacterMovement>().enabled = true;
        InputManager.Instance.InputDetectionActive = true;

        // 연출 끝나고 가상카메라 끄기 전에, 보스 체력바 등장
        StoneGolemHealthBar.instance.bossUI_enable();

        // 2f초뒤 가상카메라 OFF, 
        // (카메라 간 전환 속도는 메인 카메라의 Custom Blends 로 설정)
        StartCoroutine(VcamOff(2f));
        LevelManager.Instance.Players[0].gameObject.GetComponent<CharacterMovement>().MovementSpeedMultiplier = 1;
        LevelManager.Instance.Players[0].gameObject.GetComponent<Character>().ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
    }


    // _time 시간 뒤에 가상 카메라 비활성화
    private IEnumerator VcamOff(float _time)
    {
        yield return new WaitForSeconds(_time);
        vcam1.gameObject.SetActive(false);
        vcam2.gameObject.SetActive(false);
        vcamLeftRoom.gameObject.SetActive(false);
        vcamRightRoom.gameObject.SetActive(false);
    }

    // 보스 사망시, 방 등장
    public void BossRoomAppear()
    {
        // 보스 위치 체크
        float leftOrRight = bossPos.transform.position.x - bossRoomCenter.transform.position.x;

        // 먼저 플레이어 일시정지, 키 입력 막는다
        LevelManager.Instance.Players[0].gameObject.GetComponent<CharacterMovement>().MovementSpeedMultiplier = 0;
        InputManager.Instance.InputDetectionActive = false;

        // 가상 카메라 우선순위 설정 및 활성화
        vcam1.Priority = 0;
        vcam2.Priority = 10;
        vcam1.gameObject.SetActive(true);
        vcam2.gameObject.SetActive(true);

        if (leftOrRight > 0) // 3초 뒤 왼쪽 방 오픈
        {
            StartCoroutine(LeftDoorOpen(4f));
        }
        else // 3초 뒤 오른쪽 방 오픈
        {
            StartCoroutine(RightDoorOpen(4f));
        }

    }

    IEnumerator RightDoorOpen(float _time)
    {
        yield return new WaitForSeconds(_time);

        // 가상 카메라 우선순위 설정 및 활성화
        vcam1.Priority = 0;
        vcam2.Priority = 0;
        vcamRightRoom.Priority = 10;
        rightRoom.SetActive(true);
        vcamRightRoom.gameObject.SetActive(true);

        // 4초 뒤 플레이어로 카메라 이동
        StartCoroutine(CameraMoveToPlayer(4f));
    }

    IEnumerator LeftDoorOpen(float _time)
    {
        yield return new WaitForSeconds(_time);

        // 가상 카메라 우선순위 설정 및 활성화
        vcam1.gameObject.SetActive(false);
        vcam2.Priority = 0;
        vcamLeftRoom.Priority = 10;
        leftRoom.SetActive(true);
        vcamLeftRoom.gameObject.SetActive(true);

        // 4초 뒤 플레이어로 카메라 이동
        StartCoroutine(CameraMoveToPlayer(4f));
    }

}
