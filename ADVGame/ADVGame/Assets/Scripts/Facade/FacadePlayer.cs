using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XrCode
{
    // 登录模块外部接口定义
    public static class FacadePlayer
    {
        public static Func<string> GetPlayerName;       //获取当前玩家姓名

        public static Func<string> GetPlayerID;         //获取当前玩家ID
    }

}