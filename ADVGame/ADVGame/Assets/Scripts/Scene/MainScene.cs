namespace XrCode
{
    //主场景
    public class MainScene : BaseScene
    {
        /// 主场景进入
        protected override void OnLoad()
        {
            UIManager.Instance.OpenAsync<UIGameTest>(EUIType.EUIUIGameTest, (BaseUI) =>
            {
                /*UIManager.Instance.OpenAsync<UIEffect>(EUIType.EUIEffect);
                 UIManager.Instance.OpenAsync<UIGuide>(EUIType.EUIGuide);
                 */
                // UIManager.Instance.CloseUI(EUIType.EUIGamePlay);
                UIManager.Instance.CloseUI(EUIType.EUILoading);
            });
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
        }

        protected override void OnDispose()
        {
            base.OnDispose();
        }
    }
}