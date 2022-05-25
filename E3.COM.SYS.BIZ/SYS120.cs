using E3.Common;

using E3.Common.IBatis;
using E3.Common.Supports;
using E3.Common.Wcf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 사용자 관리 
/// </summary>
namespace E3.COM.SYS.BIZ
{
    public class SYS120
    {
        private readonly string INIT_PASSWORD = E3.Common.FrameworkConfigManager.GetAppSetting("INIT_PASSWORD", @"1234!@#$Ab");
        /// <summary>
        /// 사용자 조회
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage GetUsers(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("SYS120_SELECT", package.KeyValues);
                returnPackage.KeyValues.Add("Method", "SELECT");
                returnPackage.KeyValues.Add("Result", dataTable.Rows.Count.ToString());
                returnPackage.JsonData = dataTable.ToJson<DataTable>();
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Method", "SELECT");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }

        /// <summary>
        /// 사용자 조회 (Dialog)
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage GetUsers4Dialog(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("SYS120_DIALOG_SELECT", package.KeyValues);
                returnPackage.KeyValues.Add("Method", "DIALOG_SELECT");
                returnPackage.KeyValues.Add("Result", dataTable.Rows.Count.ToString());
                returnPackage.JsonData = dataTable.ToJson<DataTable>();
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Method", "DIALOG_SELECT");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }

        /// <summary>
        /// 아이디 중복 확인
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage duplicationCheck(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("SYS120_USER_COUNT", package.KeyValues);
                returnPackage.KeyValues.Add("Method", "SELECT");
                returnPackage.KeyValues.Add("Result", dataTable.Rows.Count.ToString());
                returnPackage.JsonData = dataTable.ToJson<DataTable>();
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Method", "SELECT");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }

        /// <summary>
        /// 사용자 등록
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage insertUser(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                Int32 ret = 0;
                Dictionary<string, string> param = package.JsonData.ToDictionary();
                param.Add("Now", package.KeyValues.GetValue("Now"));
                param.Add("LoginedUserID", package.Token.GetUserIDFromToken());
                param.Add("expiryDate", DateTime.Now.AddMonths(6).ToString("yyyy-MM-dd HH:mm:ss"));

                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("SYS120_USER_COUNT", param);
                
                if (dataTable != null && dataTable.Rows[0]["CNT"].ToInt() > 0)
                {
                    ret = DataMapper.DefaultMapper.QueryForObject<Int32>("SYS120_UPDATE", param);
                }
                else
                {
                    param.Add("userPassword", CryptographyUtility.Encrypt(Convert.ToBase64String(Encoding.UTF8.GetBytes(INIT_PASSWORD))));

                    ret = DataMapper.DefaultMapper.QueryForObject<Int32>("SYS120_INSERT", param);
                }
                returnPackage.KeyValues.Add("Method", "INSERT");
                returnPackage.KeyValues.Add("Result", ret.ToString());
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Method", "INSERT");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }

        /// <summary>
        /// 사용자 수정
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage updateUser(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                package.KeyValues.Add("LoginedUserID", package.Token.GetUserIDFromToken());
                Int32 ret = DataMapper.DefaultMapper.QueryForObject<Int32>("SYS120_UPDATE", package.KeyValues);

                returnPackage.KeyValues.Add("Method", "updateUser");
                returnPackage.KeyValues.Add("Result", ret.ToString());
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Method", "updateUser");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }

        /// <summary>
        /// 사용자 삭제
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage deleteUser(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                Int32 ret = DataMapper.DefaultMapper.QueryForObject<Int32>("SYS120_DELETE", package.KeyValues);

                returnPackage.KeyValues.Add("Method", "deleteUser");
                returnPackage.KeyValues.Add("Result", ret.ToString());
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Method", "deleteUser");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }

        public DataPackage initPassword(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("userId", package.JsonData.ToDictionary()["userId"]);
                param.Add("password", CryptographyUtility.Encrypt(Convert.ToBase64String(Encoding.UTF8.GetBytes(INIT_PASSWORD))));

                Int32 ret = DataMapper.DefaultMapper.QueryForObject<Int32>("SYS120_INITPASSWORD", param);

                returnPackage.KeyValues.Add("Method", "initPassword");
                returnPackage.KeyValues.Add("Result", ret.ToString());
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Method", "initPassword");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }

    }
}
