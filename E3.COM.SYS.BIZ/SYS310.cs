using E3.Common;

using E3.Common.IBatis;
using E3.Common.Wcf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E3.COM.SYS.BIZ
{
    public class SYS310
    {
        public DataPackage SelectItems(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("SYS310_SELECT");
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

        public DataPackage InsertItem(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                if (!package.KeyValues.ContainsKey("LoginedUserID")) package.KeyValues.Add("LoginedUserID", package.Token.GetUserIDFromToken());
                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("SYS310_COUNT", package.KeyValues);

                if (dataTable.Rows[0]["COUNT"].ToInt() == 0)
                {
                    Int32 ret = DataMapper.DefaultMapper.QueryForObject<Int32>("SYS310_INSERT", package.KeyValues);
                    returnPackage.KeyValues.Add("CODE", "200");
                    returnPackage.KeyValues.Add("Method", "INSERT");
                    returnPackage.KeyValues.Add("Result", "OK");
                }
                else
                {
                    Int32 ret = DataMapper.DefaultMapper.QueryForObject<Int32>("SYS310_UPDATE", package.KeyValues);
                    returnPackage.KeyValues.Add("CODE", "200");
                    returnPackage.KeyValues.Add("Method", "UPDATE");
                    returnPackage.KeyValues.Add("Result", "OK");
                }
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("CODE", "500");
                returnPackage.KeyValues.Add("Method", "INSERT");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }

        public DataPackage UpdateItem(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                package.KeyValues.Add("LoginedUserID", package.Token.GetUserIDFromToken());
                Int32 ret = DataMapper.DefaultMapper.QueryForObject<Int32>("SYS310_UPDATE", package.KeyValues);
                returnPackage.KeyValues.Add("CODE", "200");
                returnPackage.KeyValues.Add("Method", "UPDATE");
                returnPackage.KeyValues.Add("Result", "OK");
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("CODE", "500");
                returnPackage.KeyValues.Add("Method", "UPDATE");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }

        public DataPackage DeleteItem(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                Int32 ret = DataMapper.DefaultMapper.QueryForObject<Int32>("SYS310_DELETE", package.KeyValues);
                returnPackage.KeyValues.Add("CODE", "200");
                returnPackage.KeyValues.Add("Method", "DELETE");
                returnPackage.KeyValues.Add("Result", ret.ToString());
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("CODE", "500");
                returnPackage.KeyValues.Add("Method", "DELETE");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }
    }
}
