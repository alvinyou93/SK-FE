using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E3.Common;

using E3.Common.IBatis;
using E3.Common.Supports;
using E3.Common.Wcf;

/// <summary>
/// 프로그램관리
/// </summary>
namespace E3.COM.SYS.BIZ
{
    class SYS200
    {
        public DataPackage DeleteItem(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("SYS200_DELETE", package.JsonData.ToDictionary());
                returnPackage.KeyValues.Add("Method", "DELETE");
                returnPackage.KeyValues.Add("Result", dataTable.Rows.Count.ToString());
                returnPackage.JsonData = dataTable.ToJson<DataTable>();
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Method", "DELETE");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }

        public DataPackage InsertItem(DataPackage package)
        {
            throw new NotImplementedException();
        }

        public DataPackage SelectItems(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("SYS200_SELECT", package.KeyValues);
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

        public DataPackage UpdateItem(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                Int32 ret = 0;
                Dictionary<string, string> param = package.JsonData.ToDictionary();
                param.Add("Now", package.KeyValues.GetValue("Now"));
                param.Add("LoginedUserID", package.Token.GetUserIDFromToken());

                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("SYS200_PROGRAM_COUNT", param);

                if (dataTable != null && dataTable.Rows[0]["CNT"].ToInt() > 0)
                {
                    ret = DataMapper.DefaultMapper.QueryForObject<Int32>("SYS200_UPDATE", param);
                }
                else
                {
                    ret = DataMapper.DefaultMapper.QueryForObject<Int32>("SYS200_INSERT", param);
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

        public DataPackage GetPgmList(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("SYS200_PGM_SELECT", package.KeyValues);;

                returnPackage.KeyValues.Add("Method", "GetPgmList");
                returnPackage.KeyValues.Add("Result", dataTable.Rows.Count.ToString());
                returnPackage.JsonData = dataTable.ToJson<DataTable>();
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Method", "GetPgmList");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }
    }
}
