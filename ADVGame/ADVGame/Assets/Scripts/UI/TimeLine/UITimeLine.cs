
using System;
using UnityEngine;
using UnityEngine.UI;

namespace XrCode
{

    public partial class UITimeLine : BaseUI
    {
        protected override void OnAwake() { }
        protected override void OnEnable() { }
        	    private void OnExitBtnClickHandle()        {            UIManager.Instance.CloseUI(EUIType.EUITimeLine);        }	    private void OnItemBtnClickHandle()
        {
        
        }

        protected override void OnDisable() { }
        protected override void OnDispose() { }
    }
}