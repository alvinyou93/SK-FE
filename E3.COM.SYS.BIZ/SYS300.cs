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

namespace E3.COM.SYS.BIZ
{
    /// <summary>
    /// Button 정보관리
    /// </summary>
    public class SYS300
    {
        public DataPackage DeleteItem(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("SYS300_DELETE", package.JsonData.ToDictionary());
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
            return UpdateItem(package);
        }

        public DataPackage SelectItems(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("SYS300_SELECT", package.KeyValues);
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
            string dataHandleMode = "INSERT";
            try
            {
                Int32 ret = 0;
                Dictionary<string, string> param = package.JsonData.ToDictionary();
                param.Add("Now", package.KeyValues.GetValue("Now"));
                param.Add("LoginedUserID", package.Token.GetUserIDFromToken());

                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("SYS300_COUNT", param);

                if (dataTable != null && dataTable.Rows[0]["CNT"].ToInt() > 0)
                {
                    dataHandleMode = "UPDATE";
                    ret = DataMapper.DefaultMapper.QueryForObject<Int32>("SYS300_UPDATE", param);
                }
                else
                {
                    ret = DataMapper.DefaultMapper.QueryForObject<Int32>("SYS300_INSERT", param);
                }

                returnPackage.KeyValues.Add("Method", dataHandleMode);
                returnPackage.KeyValues.Add("Result", ret.ToString());
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Method", dataHandleMode);
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }
    }
}
