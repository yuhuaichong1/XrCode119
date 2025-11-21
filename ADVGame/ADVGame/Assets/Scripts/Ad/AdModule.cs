using System;

namespace XrCode
{
    public class AdModule : BaseModule
    {
        protected override void OnLoad() 
        {
            FacadeAd.PlayRewardAd += PlayRewardAd;
            FacadeAd.PlayInterAd += PlayInterAd;
            FacadeAd.PlayBannerAd += PlayBannerAd;
            FacadeAd.StopBannerAd += StopBannerAd;
        }

        public void PlayRewardAd(EAdSource eAdSource, Action successAction, Action failAction = null)
        {
            if(GameDefines.ifSkipAD)
                successAction?.Invoke();
            else
            {

            }
        }

        public void PlayInterAd(EAdSource eAdSource, Action successAction, Action failAction = null)
        {
            if (GameDefines.ifSkipAD)
                successAction?.Invoke();
            else
            {

            }
        }

        public void PlayBannerAd(EAdSource eAdSource, Action successAction, Action failAction = null)
        {
            if (GameDefines.ifSkipAD)
                successAction?.Invoke();
            else
            {

            }
        }

        public void StopBannerAd(EAdSource eAdSource)
        {

        }

        protected override void OnDispose()
        {
            FacadeAd.PlayRewardAd -= PlayRewardAd;
            FacadeAd.PlayInterAd -= PlayInterAd;
            FacadeAd.PlayBannerAd -= PlayBannerAd;
            FacadeAd.StopBannerAd -= StopBannerAd;
        }
    }
}
