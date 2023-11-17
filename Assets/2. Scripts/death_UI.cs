using DG.Tweening;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class death_UI : MonoBehaviour
{
    [SerializeField] Image deathBar;
    [SerializeField] TextMeshProUGUI deathText1; // You Died
    [SerializeField] TextMeshProUGUI deathText2; // Go to Lobby Any Button

    private bool isDied;

    private void OnEnable()
    {
        // ��� �� ���̵��εǸ鼭 ����
        deathBar.DOFade(.9f, 3f).From(0f);

        // YOU DIED �ؽ�Ʈ ���̵��εǸ鼭 ���� �� scale ����
        deathText1.DOFade(.8f, 3f).From(0f).SetDelay(.5f);
        deathText1.transform.DOScale(1.2f, 2f).SetDelay(.5f);

        // Go to Lobby Any Button �ؽ�Ʈ ��������ȿ��
        deathText2.DOFade(.5f, 2f).From(0f).SetDelay(2f).OnComplete(() =>
        {
            deathText2.DOFade(.2f, 1f).From(.7f).SetLoops(-1, LoopType.Yoyo);
            isDied = true;
        });
    }

    private void Update()
    {
        // �ƹ� Ű �Է½� �κ� ȭ������ �̵�, �� �׾����� ����X
        if (!isDied) return;
        if (Input.anyKeyDown)
            MMSceneLoadingManager.LoadScene("1. Scenes/StartScreen", "StartScreen");
    }
}
