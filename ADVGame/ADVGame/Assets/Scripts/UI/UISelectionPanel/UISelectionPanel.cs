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
            if (args.Length > 0 && args[0] is List<int>)
            {
                availableChoices = (List<int>)args[0];
            }
        }

        protected override void OnAwake() { }

        protected override void OnEnable()
        {
            UpdateSelectionButtons();
        }

        private void UpdateSelectionButtons()
        {
            if (availableChoices == null || mRoot == null) return;

            var availableChoicesCount = availableChoices.Count;
            var childCount = mRoot.transform.childCount;

            for (int i = 0; i < childCount; i++)
            {
                var child = mRoot.transform.GetChild(i);
                if (child != null)
                {
                    bool shouldShow = i < availableChoicesCount;
                    child.gameObject.SetActive(shouldShow);
                    if (shouldShow)
                    {
                        var button = child.GetComponent<Button>();
                        var text = child.GetComponentInChildren<Text>();
                        if (text != null)
                        {
                            text.text = $"选择 :{availableChoices[i]}";
                        }
                        if (button != null)
                        {
                            int choiceIndex = i; // 创建闭包变量
                            button.onClick.RemoveAllListeners();
                            button.onClick.AddListener(() => OnChoiceSelected(choiceIndex));
                        }
                    }
                }
            }
        }

        private void OnChoiceSelected(int choiceIndex)
        {
            if (availableChoices != null && choiceIndex >= 0 && choiceIndex < availableChoices.Count)
            {
                int selectedSn = availableChoices[choiceIndex];
                VideoPlayerEvent.SelectVideoPlay?.Invoke(selectedSn);
                Debug.Log($"Selected choice: {selectedSn}");
                UIManager.Instance.CloseUI(EUIType.EUIUISelectionPanel);
            }
        }

        protected override void OnDisable() { }

        protected override void OnDispose() { }
    }
}