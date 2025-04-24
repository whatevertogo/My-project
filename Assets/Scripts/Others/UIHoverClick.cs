using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UIHoverClick : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public abstract void OnPointerEnter(PointerEventData eventData);

    public abstract void OnPointerExit(PointerEventData eventData);

    public abstract void OnPointerClick(PointerEventData eventData);
}