using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TipController : MonoBehaviour
{
    public float timeToDestroy = 1f;
    public void StartMove()
    {
        MoveTask();
    }

    public async UniTask MoveTask()
    {
        Image img = GetComponent<Image>();
        TextMeshProUGUI text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        Color startColorImg = img.color;
        img.DOColor(new Color(img.color.r, img.color.g, img.color.b, 1f), 0.3f);
        text.DOColor(new Color(text.color.r, text.color.g, text.color.b, 1f), 0.3f);
        await transform.DOMoveY(transform.position.y + 30f, timeToDestroy);
        img.DOColor(new Color(img.color.r, img.color.g, img.color.b, 0f), 0.3f);
        await text.DOColor(new Color(text.color.r, text.color.g, text.color.b, 0f), 0.3f);
        Destroy(gameObject);

    }
}
