using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour
{
    float duration = 0.5f;

    public void ButtonUX_onMouseEnter(GameObject g)
    {
        GameObject highLight = g.transform.GetChild(0).gameObject;
        GameObject img = g.transform.GetChild(1).gameObject;
        GameObject txt = g.transform.GetChild(2).gameObject;

        g.transform.DOScale(1.2f, duration);

        highLight.GetComponent<Image>().DOFade(.5f, duration).OnComplete(() =>
        {
            highLight.GetComponent<Image>().DOFade(.2f, duration).From(.5f).SetLoops(-1, LoopType.Yoyo);
        });
        img.GetComponent<Image>().DOFade(.9f, duration);
        txt.GetComponent<TextMeshProUGUI>().DOFade(1f, duration);
        txt.transform.DOScale(1.2f, duration);
    }

    public void ButtonUX_onMouseLeave(GameObject g)
    {
        GameObject highLight = g.transform.GetChild(0).gameObject;
        GameObject img = g.transform.GetChild(1).gameObject;
        GameObject txt = g.transform.GetChild(2).gameObject;

        g.transform.DOScale(1f, duration);

        highLight.GetComponent<Image>().DOKill();
        highLight.GetComponent<Image>().DOFade(0f, duration);
        img.GetComponent<Image>().DOFade(.2f, duration);
        txt.GetComponent<TextMeshProUGUI>().DOFade(.7f, duration);
        txt.transform.DOScale(1f, duration);
    }

}
