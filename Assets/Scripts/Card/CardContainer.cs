using System;
using UnityEngine;

public class CardContainer : MonoBehaviour
{
    [SerializeField] private Card card;


    private void Start()
    {
        card.OnCardPointing += CardPointingHandler;
        card.OnCardNotPointing += CardNotPointingHandler;
    }

    private void CardNotPointingHandler(object sender, EventArgs e)
    {
        transform.SetAsFirstSibling(); //  放最下面s
    }

    private void CardPointingHandler(object sender, EventArgs e)
    {
        transform.SetAsLastSibling(); //  放最上面
    }
}
