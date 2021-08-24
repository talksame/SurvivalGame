using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ComputerButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField] private ComputerKit theComputer;
    [SerializeField] private int buttonNum;

    public void OnPointerEnter(PointerEventData eventData)
    {
        theComputer.ShowToolTip(buttonNum);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        theComputer.HideToolTip();
    }
}
