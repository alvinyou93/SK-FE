using E3.Common;
using E3.Common.Wcf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E3.LCM.MBL.BIZ
{
    public class MAPI200
    {
        public DataPackage SelectSignSheet(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            returnPackage.KeyValues.Add("Method", "SELECT_SIGN_SHEET");
            try
            {
                package.KeyValues.Add("USER_ID", package.Token.GetUserIDFromToken());
                DataTable dataTable = MAPIBase.DefaultMapper().QueryForDataTable("MAPI200_SELECT_SIGN_SHEET", package.KeyValues);
                returnPackage.JsonData = dataTable.ToJson<DataTable>();
                returnPackage.KeyValues.Add("Result", dataTable.Rows.Count.ToString());
                returnPackage.KeyValues.Add("ToTalCount", dataTable.Rows.Count.ToString());
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Result", (-1).ToString());
                returnPackage.KeyValues.Add("ToTalCount", (0).ToString());
            }
            return returnPackage;
        }

        public DataPackage InsertSignSheet(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            returnPackage.KeyValues.Add("Method", "INSERT_SIGN_SHEET");
            try
            {
                package.KeyValues.Add("LOGINID", package.Token.GetUserIDFromToken());
                Int32 ret = MAPIBase.DefaultMapper().QueryForObject<Int32>("MAPI200_INSERT_SIGN_SHEET", package.KeyValues);
                returnPackage.KeyValues.Add("CODE", "200");
                returnPackage.KeyValues.Add("Result", "OK");
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("CODE", "500");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }

        public DataPackage CancelSignSheet(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            returnPackage.KeyValues.Add("Method", "CANCEL");
            try
            {
                Int32 ret = 0;
                Dictionary<string, string> param = package.JsonData.ToDictionary();
                param.Add("LOGINID", package.Token.GetUserIDFromToken());

                //취소할 단계의 권한을 체크한다.
                DataTable dataTable = MAPIBase.DefaultMapper().QueryForDataTable("WRK300_CHECK_FACILITY_INFO", param);

                if (dataTable.Rows.Count > 0)
                {
                    if (dataTable.Rows[0]["AUTH_YN"].ToString() == "Y")
                    {
                        param.Add("STEP_CD", dataTable.Rows[0]["STEP_CD"].ToString());

                        ret = MAPIBase.DefaultMapper().QueryForObject<Int32>("WRK300_DELETE_SIGN_SHEET_STEP", param);
                    }
                    else
                    {
                        returnPackage.ErrorMessage = "현재 단계를 취소할 권한이 없습니다. 다시 확인하여 주십시요.";
                        returnPackage.KeyValues.Add("Result", (-1).ToString());

                        return returnPackage;
                    }
                }

                returnPackage.KeyValues.Add("Result", ret.ToString());

            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }
    }
}
