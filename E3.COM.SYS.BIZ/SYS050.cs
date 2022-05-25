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

namespace E3.COM.SYS.BIZ
{
    /// <summary>
    ///버늩 권한 정보 조회
    /// </summary>
    public class SYS050
    {
        public DataPackage SelectItem(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                if(!package.KeyValues.ContainsKey("LoginedUserID")) package.KeyValues.Add("LoginedUserID", package.Token.GetUserIDFromToken());
                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("SYS050_SELECT_BUTTONS", package.KeyValues);
                returnPackage.JsonData = dataTable.ToJson<DataTable>();
                returnPackage.KeyValues.Add("Method", "BUTTONS");
                returnPackage.KeyValues.Add("Result", dataTable.Rows.Count.ToString());
                returnPackage.KeyValues.Add("ToTalCount", dataTable.Rows.Count.ToString());
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Method", "BUTTONS");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }
    }
}
