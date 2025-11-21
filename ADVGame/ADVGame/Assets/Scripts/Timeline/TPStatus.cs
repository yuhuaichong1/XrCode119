using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//时间点状态
public enum TPStatus
{
    Locked = 0,//未到达
    UnLocked = 1,//到达
    Playing = 2,//处于该片段中
}
