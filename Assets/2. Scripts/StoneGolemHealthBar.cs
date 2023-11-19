using DG.Tweening;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoneGolemHealthBar : MonoBehaviour
{
    // ���� UI
    [SerializeField] public GameObject bossUI_prefab;
    [SerializeField] public GameObject bossUI_object;

    [SerializeField] public Sprite flickerImage_1;
    [SerializeField] public Sprite flickerImage_2;

    public static StoneGolemHealthBar instance = null;
    private Health _health;

    private void Awake()
    {
        instance = this;
    }

    public void bossUI_enable()
    {
        // ���� ü�� ui Ȱ��ȭ
        _health = this.transform.parent.GetComponent<Health>();
        bossUI_object = Instantiate(bossUI_prefab, GUIManager.Current.MainCanvas.transform);
        bossUI_object.SetActive(true);
        this.gameObject.SetActive(false);

        // ux.. �ϵ��ڵ� �˼��մϴ�!
        bossUI_object.transform.GetChild(0).DOScale(2f, 1f).SetDelay(1f).OnComplete(() => { bossUI_object.transform.GetChild(0).DOScale(1.5f, 1f).SetDelay(.5f); });
        bossUI_object.transform.GetChild(0).GetChild(0).GetComponent<Image>().DOFade(1f, 1f).SetDelay(1f).From(0f); // �����̴� frame ���̵���
        bossUI_object.transform.GetChild(0).GetChild(1).DOScaleX(1f,1f).SetDelay(1f).From(0f); // �����̴� fill �������
        bossUI_object.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().DOFade(1f, 1f).SetDelay(1f).From(0f); // ���� �̸� ���̵���
        bossUI_object.transform.GetChild(0).GetChild(3).GetComponent<Image>().DOFade(1f, 1f).SetDelay(1f).From(0f); // �����̴� border ���̵���
    }

    private void OnEnable()
    {
        if (!bossUI_object) return;
        bossUI_object.transform.GetChild(0).GetComponent<Slider>().value = _health.CurrentHealth / _health.MaximumHealth;

        // ux.. �ϵ��ڵ� �˼��մϴ�!
        // ������ �ǰ� �� ��� �����̴� ������
        bossUI_object.transform.GetChild(0).GetChild(1).GetChild(0).DOScale(1f, 0f).OnComplete(() =>
        {
            bossUI_object.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>().sprite = flickerImage_1;
            bossUI_object.transform.GetChild(0).GetChild(1).GetChild(0).DOScale(1f, .1f).OnComplete(() =>
            {
                bossUI_object.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>().sprite = flickerImage_2;
            });
        });

        if (_health.CurrentHealth <= 0f) Destroy(bossUI_object);
        this.gameObject.SetActive(false);
    }

}