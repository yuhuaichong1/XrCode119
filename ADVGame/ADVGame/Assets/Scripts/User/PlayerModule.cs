using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
namespace XrCode
{
    // 用户模块
    public class PlayerModule : BaseModule
    {
        #region 玩家数据

        private string userName;//玩家姓名
        private string userID;//玩家ID

        #endregion

        private STimer recoverEnergyTimer;
        private DateTime lastRecoverTime;

        protected override void OnLoad()
        {
            base.OnLoad();
            FacadeAdd();
            LoadData();
        }

        #region Facade

        private void FacadeAdd()
        {
            FacadePlayer.GetPlayerName += GetUserName;

            FacadePlayer.GetPlayerID += GetUserID;
        }

        private void FacadeRemove() 
        {
            FacadePlayer.GetPlayerName -= GetUserName;

            FacadePlayer.GetPlayerID -= GetUserID;
        }

        #endregion

        #region Get/Set

        #region userName

        private string GetUserName()
        {
            return userName;
        }

        #endregion

        #region userID

        private string GetUserID()
        {
            return userID;
        }

        #endregion

        #endregion

        #region 其他

        /// <summary>
        /// 加载数据
        /// </summary>
        public void LoadData()
        {
            userName = SPlayerPrefs.HasKey(PlayerPrefDefines.userName) ? SPlayerPrefs.GetString(PlayerPrefDefines.userName) : GetRandomName();
            userID = SPlayerPrefs.HasKey(PlayerPrefDefines.userID) ? SPlayerPrefs.GetString(PlayerPrefDefines.userID) : GetRandomID();
        }

        /// <summary>
        /// 获取随机玩家姓名
        /// </summary>
        /// <returns>随机玩家姓名</returns>
        private string GetRandomName()
        {
            char[] nameChars = GameDefines.NameString.ToCharArray();
            int length = nameChars.Length;
            char c1 = nameChars[UnityEngine.Random.Range(0, length)];
            char c2 = nameChars[UnityEngine.Random.Range(0, length)];
            string target = $"{FacadeLanguage.GetText("10005")}_{c1}{c2}";

            SPlayerPrefs.SetString(PlayerPrefDefines.userName, target);
            SPlayerPrefs.Save();

            return target;
        }

        /// <summary>
        /// 获取随机玩家ID
        /// </summary>
        /// <returns>随机玩家ID</returns>
        private string GetRandomID()
        {
            string target = "";
            Guid GID = Guid.NewGuid();
            target = GID.ToString();

            SPlayerPrefs.SetString(PlayerPrefDefines.userID, target);
            SPlayerPrefs.Save();

            return target;
        }

        #endregion

        protected override void OnDispose()
        {
            base.OnDispose();

            FacadeRemove();
        }
    }
}