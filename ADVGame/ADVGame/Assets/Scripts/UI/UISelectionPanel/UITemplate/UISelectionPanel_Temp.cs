using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XrCode
{
    public partial class UISelectionPanel : BaseUI
    {	protected RectTransform mRoot;
        protected override void LoadPanel()
        {
            base.LoadPanel();
            		   mRoot = mTransform.Find("Plane/root").GetComponent<RectTransform>();
        }
    
        protected override void BindButtonEvent() 
        {
            
        }
    
        protected override void UnBindButtonEvent() 
        {
            
        }
    
    }
}