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

// 출입대상자 교육문서 관리
namespace E3.LCM.WEB.BIZ
{
    public class WEB_030
    {
        public DataPackage GetEduList(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                System.Data.DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WEB030_SELECT", package.KeyValues);
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

        //InsertEduDoc
        public DataPackage InsertEduDoc(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                Dictionary<string, string> param = package.JsonData.ToDictionary();
                param.Add("Now", package.KeyValues.GetValue("Now"));
                param.Add("LoginedUserID", package.Token.GetUserIDFromToken());

                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WEB030_EDU_DOC_COUNT", param);

                if (dataTable.Rows[0]["CNT"].ToInt() > 0)
                {
                    Int32 ret = DataMapper.DefaultMapper.QueryForObject<Int32>("WEB030_UPDATE", param);
                    returnPackage.KeyValues.Add("Method", "UPDATE");
                    returnPackage.KeyValues.Add("Result", ret.ToString());
                }
                else
                {
                    Int32 ret = DataMapper.DefaultMapper.QueryForObject<Int32>("WEB030_INSERT", param);
                    returnPackage.KeyValues.Add("Method", "INSERT");
                    returnPackage.KeyValues.Add("Result", ret.ToString());
                }
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Method", "INSERT or UPDATE");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }

        public DataPackage DeleteEduDoc(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                Dictionary<string, string> param = package.JsonData.ToDictionary();
                param.Add("Now", package.KeyValues.GetValue("Now"));

                DataTable dataTableQuiz = DataMapper.DefaultMapper.QueryForDataTable("WEB030_QUIZ_COUNT", param);

                if (dataTableQuiz.Rows[0]["CNT"].ToInt() == 0)
                {
                    DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WEB030_DELETE", param);
                    returnPackage.KeyValues.Add("Method", "DELETE");
                    returnPackage.KeyValues.Add("Result", dataTable.Rows.Count.ToString());
                    returnPackage.JsonData = dataTable.ToJson<DataTable>();
                }
                else {
                    returnPackage.ErrorMessage = "해당 교육자료를 사용하는 퀴즈가 존재하여 삭제할 수 없습니다.";
                    returnPackage.KeyValues.Add("Method", "DELETE");
                    returnPackage.KeyValues.Add("Result", (-1).ToString());
                }
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
