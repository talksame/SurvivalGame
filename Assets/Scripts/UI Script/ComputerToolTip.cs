using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComputerToolTip : MonoBehaviour {


    [SerializeField] private GameObject go_BaseUi;

    [SerializeField] private Text kitName;
    [SerializeField] private Text kitDes;
    [SerializeField] private Text kitNeedItem;

    public void ShowToolTip(string _kitName, string _kitDes, string[] _needItem, int[] _needItemNumber)
    {
        go_BaseUi.SetActive(true);

        kitName.text = _kitName;
        kitDes.text = _kitDes;

        for (int i = 0; i < _needItem.Length; i++)
        {
            kitNeedItem.text += _needItem[i];
            kitNeedItem.text += " x " + _needItemNumber[i].ToString() + "\n";
        }
    }

    public void HideToolTip()
    {
        go_BaseUi.SetActive(false);
        kitName.text = "";
        kitDes.text = "";
        kitNeedItem.text = "";
    }
}
