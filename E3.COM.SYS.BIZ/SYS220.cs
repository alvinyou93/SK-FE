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
    /// 사용자별 권한 관리
    /// </summary>
    public class SYS220
    {
        public DataPackage DeleteItem(DataPackage package)
        {
            throw new NotImplementedException();
        }

        public DataPackage InsertItem(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                
                Int32 ret = 0;

                ret = DataMapper.DefaultMapper.QueryForObject<Int32>("SYS220_DELETE", package.JsonData.ToDictionaryList()[0]);

                foreach (Dictionary<string, string> d in package.JsonData.ToDictionaryList())
                {
                    d.Add("Now", package.KeyValues.GetValue("Now"));
                    d.Add("LoginedUserID", package.Token.GetUserIDFromToken());
                    ret = DataMapper.DefaultMapper.QueryForObject<Int32>("SYS220_INSERT", d);
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

        public DataPackage InsertAllItem(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                Int32 ret = 0;
                int paramCount = 0;
                string TokenUserID = package.Token.GetUserIDFromToken();
                ret = DataMapper.DefaultMapper.QueryForObject<Int32>("SYS220_DELETE", package.JsonData.ToDictionaryList()[0]);

                System.Collections.ArrayList list = new System.Collections.ArrayList();

                foreach (Dictionary<string, string> d in package.JsonData.ToDictionaryList())
                {
                    d.Add("Now", package.KeyValues.GetValue("Now"));
                    d.Add("LoginedUserID", package.Token.GetUserIDFromToken());
                    list.Add(d);
                    //ret = DataMapper.DefaultMapper.QueryForObject<Int32>("SYS220_INSERT", d);

                    ret++;
                    paramCount += 4;

                    // 들어오는 요청에 매개 변수가 너무 많습니다.
                    // 서버에서 지원하는 최대 매개 변수 개수는 2100개입니다.
                    // 매개 변수 개수를 줄이고 요청을 다시 보내십시오.
                    if (paramCount > 2000)
                    {
                        Dictionary<string, object> param1 = new Dictionary<string, object>();
                        param1.Add("param", list);
                        DataMapper.DefaultMapper.Insert("SYS220_INSERT_LIST", param1);

                        list = new System.Collections.ArrayList();
                        paramCount = 0;
                    }
                }

                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("param", list);
                DataMapper.DefaultMapper.Insert("SYS220_INSERT_LIST", param);

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

        public DataPackage authCopyItem(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {

                Int32 ret = 0;

                foreach (Dictionary<string, string> d in package.JsonData.ToDictionaryList())
                {
                    d.Add("Now", package.KeyValues.GetValue("Now"));
                    d.Add("LoginedUserID", package.Token.GetUserIDFromToken());

                    ret = DataMapper.DefaultMapper.QueryForObject<Int32>("SYS220_DELETE", d);
                    ret = DataMapper.DefaultMapper.QueryForObject<Int32>("SYS220_AUTHCOPY_INSERT", d);
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

        public DataPackage PagingItems(DataPackage package)
        {
            throw new NotImplementedException();
        }

        public DataPackage GetUsers(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("SYS220_USER_SELECT", package.KeyValues);
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

        public DataPackage SelectItem(DataPackage package)
        {
            throw new NotImplementedException();
        }

        public DataPackage SelectItems(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("SYS220_SELECT", package.KeyValues);
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
            throw new NotImplementedException();
        }
    }
}
