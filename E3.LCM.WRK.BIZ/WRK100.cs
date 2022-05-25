using E3.Common;

using E3.Common.IBatis;
using E3.Common.Supports;
using E3.Common.Wcf;
using E3.LCM.SYS.BIZ;
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
    public class WRK100
    {
        /// <summary>
        /// 사용자 조회
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage selectJobList(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WRK100_SELECT_JOB", package.KeyValues);
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

        public DataPackage selectAuthList(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WRK100_SELECT_AUTH", package.KeyValues);
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
        public DataPackage InsertAuthList(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {

                Int32 ret = 0;

                ret = DataMapper.DefaultMapper.QueryForObject<Int32>("WRK100_DELETE_AUTH", package.KeyValues);

                foreach (Dictionary<string, string> d in package.JsonData.ToDictionaryList())
                {
                    d.Add("LoginedUserID", package.Token.GetUserIDFromToken());
                    ret = DataMapper.DefaultMapper.QueryForObject<Int32>("WRK100_INSERT_AUTH", d);
                }

                returnPackage.KeyValues.Add("Method", "UPDATE");
                returnPackage.KeyValues.Add("Result", ret.ToString());
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Method", "UPDATE");
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
                Int32 ret = DataMapper.DefaultMapper.QueryForObject<Int32>("WRK100_DELETE_MAIN_LIST", package.KeyValues);

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
