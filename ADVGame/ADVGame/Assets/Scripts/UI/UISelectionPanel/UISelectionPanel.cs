
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XrCode
{

    public partial class UISelectionPanel : BaseUI
    {
        List<int> availableChoices;
        protected override void OnSetParam(params object[] args)
        {
            base.OnSetParam(args);
            if (availableChoices == null) return;
            availableChoices = (List<int>)args[0];
        }
        protected override void OnAwake() { }
        protected override void OnEnable() { }

        protected override void OnDisable() { }
        protected override void OnDispose() { }

    }
}