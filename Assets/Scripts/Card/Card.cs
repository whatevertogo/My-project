using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;

public class Card : MonoBehaviour,IHover
{
    public TextMeshProUGUI CardText;
    public Image image;

    public void OnHoverEnter()
    {

    }

    public void OnHoverExit()
    {
        
    }

    public void UpdateCardText(string newText)
    {
        CardText.text = newText;
    }
}
