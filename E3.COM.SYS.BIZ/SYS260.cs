using E3.Common;
using E3.Common.IBatis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using E3.Common.Wcf;
using E3.Common.Supports;

namespace E3.COM.SYS.BIZ
{
    /// <summary>
    /// 메뉴별 버튼 권한 관리
    /// </summary>
    public class SYS260
    {
        public DataPackage selectAuth(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("SYS260_SELECT_AUTH", package.KeyValues);

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
        
        public DataPackage selectAuthMenus(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("SYS260_SELECT", package.KeyValues);

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

        public DataPackage selectAuthMenusButton(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("SYS260_SELECT_BUTTON", package.KeyValues);

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


        public DataPackage InsertAuthMenusButton(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                Int32 ret = 0;
                List<Dictionary<string, string>> listDict = package.JsonData.ToDictionaryList();
                if (listDict.Count > 0)
                {
                    DataMapper.DefaultMapper.QueryForObject<Int32>("SYS260_DELETE", listDict[0]);
                    foreach (Dictionary<string, string> dict in listDict)
                    {
                        ret++;
                        dict.Add("Now", package.KeyValues.GetValue("Now"));
                        dict.Add("LoginedUserID", package.Token.GetUserIDFromToken());
                        DataMapper.DefaultMapper.QueryForObject<Int32>("SYS260_INSERT", dict);
                    }
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

        public DataPackage InsertAuthAllMenusButton(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                Int32 ret = 0;
                int paramCount = 0;
                string TokenUserId = package.Token.GetUserIDFromToken();
                List<Dictionary<string, string>> listDict = package.JsonData.ToDictionaryList();

                if (listDict.Count > 0)
                {
                    DataMapper.DefaultMapper.QueryForObject<Int32>("SYS260_DELETE4AUTH", listDict[0]);
                    System.Collections.ArrayList list = new System.Collections.ArrayList();

                    foreach (Dictionary<string, string> dict in listDict)
                    {
                        dict.Add("LoginedUserID", TokenUserId);
                        //dict.Add("Now", package.KeyValues.GetValue("Now"));
                        list.Add(dict);
                        //DataMapper.DefaultMapper.QueryForObject<Int32>("SYS260_INSERT", dict);
                        
                        ret++;
                        paramCount += 4;

                        // 들어오는 요청에 매개 변수가 너무 많습니다.
                        // 서버에서 지원하는 최대 매개 변수 개수는 2100개입니다.
                        // 매개 변수 개수를 줄이고 요청을 다시 보내십시오.
                        if (paramCount > 2000) {
                            Dictionary<string, object> param1 = new Dictionary<string, object>();
                            param1.Add("param", list);
                            DataMapper.DefaultMapper.Insert("SYS260_INSERT_LIST", param1);

                            list = new System.Collections.ArrayList();
                            paramCount = 0;
                        }
                    }
                    
                    Dictionary<string, object> param = new Dictionary<string, object>();
                    param.Add("param", list);
                    DataMapper.DefaultMapper.Insert("SYS260_INSERT_LIST", param);
                }
                returnPackage.KeyValues.Add("Method", "List Insert");
                returnPackage.KeyValues.Add("Result", ret.ToString());
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Method", "List Insert");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }
    }
}
