using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FacadeAd
{
    public static Action<EAdSource, Action, Action> PlayRewardAd;
    public static Action<EAdSource, Action, Action> PlayInterAd;
    public static Action<EAdSource, Action, Action> PlayBannerAd;
    public static Action<EAdSource> StopBannerAd;
}
