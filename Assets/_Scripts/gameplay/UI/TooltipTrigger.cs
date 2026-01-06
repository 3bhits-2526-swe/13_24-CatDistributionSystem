using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea(3, 6)]
    public string tooltipText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipUI.Instance?.Show(tooltipText);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Instance?.Hide();
    }
}