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
/// 설비종류별JOB등록
/// </summary>
namespace E3.LCM.WRK.BIZ
{
    public class WRK200
    {
        /// <summary>
        /// 사용자 조회
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage selectMainList(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WRK200_SELECT", package.KeyValues);
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
        /// 저장
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage saveMainList(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                Int32 ret = 0;
                Dictionary<string, string> param = package.JsonData.ToDictionary();
                param.Add("LOGINID", package.Token.GetUserIDFromToken());

                //                param.Add("expiryDate", DateTime.Now.AddMonths(6).ToString("yyyy-MM-dd HH:mm:ss"));
                DataTable checkDt = DataMapper.DefaultMapper.QueryForDataTable("WRK200_CHECK_EMPTY", param);

                string checkData = checkDt.Rows[0]["CHK_DATA"].ToString();

                int cnt = checkData.IndexOf("FALSE");

                if (cnt > -1)
                {
                    string checkStr1 = checkData.Substring(cnt, checkData.Length - cnt);

                    string checkStr2 = checkStr1.Replace("FALSE", "");

                    if (checkStr2.IsNotNullOrEmpty())
                    {
                        returnPackage.ErrorMessage = "중간에 비워 있는 단계가 존재합니다.";
                        returnPackage.KeyValues.Add("Method", "INSERT");
                        returnPackage.KeyValues.Add("Result", (-1).ToString());

                        return returnPackage;
                    }
                }

                //WK_SIGN_SHEET에서 사용되고 있음 수정 불가함
                checkDt = DataMapper.DefaultMapper.QueryForDataTable("WRK200_CHECK_USE", param);

                if (checkDt != null && checkDt.Rows[0]["CNT"].ToInt() > 0)
                {
                    returnPackage.ErrorMessage = "SIGN SHEET에 이미 사용하고 있는 설비종류는 수정할 수 없습니다.";
                    returnPackage.KeyValues.Add("Method", "INSERT");
                    returnPackage.KeyValues.Add("Result", (-1).ToString());

                    return returnPackage;
                }

                string rowState = param.GetValue("rowState");

                //Insert
                if (rowState.Equals("I"))
                {
                    DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WRK200_CHECK_CNT", param);

                    if (dataTable != null && dataTable.Rows[0]["CNT"].ToInt() > 0)
                    {
                        returnPackage.ErrorMessage = "중복된 설비종류가 존재합니다.";
                        returnPackage.KeyValues.Add("Method", "INSERT");
                        returnPackage.KeyValues.Add("Result", (-1).ToString());

                        return returnPackage;
                    }
                    else
                    {

                        ret = DataMapper.DefaultMapper.QueryForObject<Int32>("WRK200_INSERT_MAIN_LIST", param);
                    }

                }
                //Update
                else
                {
                    ret = DataMapper.DefaultMapper.QueryForObject<Int32>("WRK200_UPDATE_MAIN_LIST", param);

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
        /// 사용자 삭제
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage deleteMainList(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                Int32 ret = DataMapper.DefaultMapper.QueryForObject<Int32>("WRK200_DELETE_MAIN_LIST", package.KeyValues);

                returnPackage.KeyValues.Add("Method", "DELETE");
                returnPackage.KeyValues.Add("Result", ret.ToString());
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Method", "DELETE");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }

    }
}
