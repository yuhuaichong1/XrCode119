using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XrCode
{
    public partial class UITimeLine : BaseUI
    {	protected Image mBg;	protected Button mExitBtn;	protected Button mItemBtn;	protected RectTransform mContent;	protected ScrollRect mItemsScrollView;	protected Toggle mAllTog;	protected Toggle mGotTog;	protected Toggle mNotGotTog;	protected Text mCPTitle;	protected Text mCPChapter;	protected Slider mCPSlider;	protected Text mCPSliderText;
        protected override void LoadPanel()
        {
            base.LoadPanel();
            		mBg = mTransform.Find("Bg").GetComponent<Image>();		mExitBtn = mTransform.Find("ExitBtn").GetComponent<Button>();		mItemBtn = mTransform.Find("Items/ItemBtn").GetComponent<Button>();		mContent = mTransform.Find("TLScrollView/Viewport/Content").GetComponent<RectTransform>();		mItemsScrollView = mTransform.Find("Items/ItemsScrollView").GetComponent<ScrollRect>();		mAllTog = mTransform.Find("Items/ItemsToggles/AllTog").GetComponent<Toggle>();		mGotTog = mTransform.Find("Items/ItemsToggles/GotTog").GetComponent<Toggle>();		mNotGotTog = mTransform.Find("Items/ItemsToggles/NotGotTog").GetComponent<Toggle>();		mCPTitle = mTransform.Find("ChapterProgress/CPTitle").GetComponent<Text>();		mCPChapter = mTransform.Find("ChapterProgress/CPChapter").GetComponent<Text>();		mCPSlider = mTransform.Find("ChapterProgress/CPSlider").GetComponent<Slider>();		mCPSliderText = mTransform.Find("ChapterProgress/CPSlider/CPSliderText").GetComponent<Text>();
        }
    
        protected override void BindButtonEvent() 
        {
            		mExitBtn.onClick.AddListener( OnExitBtnClickHandle);		mItemBtn.onClick.AddListener( OnItemBtnClickHandle);
        }
    
        protected override void UnBindButtonEvent() 
        {
            		mExitBtn.onClick.RemoveAllListeners();		mItemBtn.onClick.RemoveAllListeners();
        }
    
    }
}