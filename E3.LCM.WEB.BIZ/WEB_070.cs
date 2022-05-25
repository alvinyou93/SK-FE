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
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing;

// 증빙서류제출
namespace E3.LCM.WEB.BIZ
{
    public class WEB_070
    {
        public DataPackage getDeviceList(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WEB070_SELECT_DEVICE", package.KeyValues);
                returnPackage.KeyValues.Add("Method", "SELECT_DEVICE");
                returnPackage.KeyValues.Add("Result", dataTable.Rows.Count.ToString());
                returnPackage.JsonData = dataTable.ToJson<DataTable>();
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Method", "SELECT_DEVICE"); 
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }
        public DataPackage selectMainList(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WEB070_SELECT", package.KeyValues);
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

        public DataPackage selectCntInfo(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WEB070_SELECT_CNT_INFO", package.KeyValues);
                returnPackage.KeyValues.Add("Method", "SELECT");
                returnPackage.KeyValues.Add("Result", dataTable.Rows[0]["CNT"].ToString());
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

        // popup

    }

}

