using E3.Common;

using E3.Common.IBatis;
using E3.Common.Supports;
using E3.Common.Wcf;
using IBatisNet.DataMapper;
using IBatisNet.DataMapper.Configuration.Statements;
using IBatisNet.DataMapper.MappedStatements;
using IBatisNet.DataMapper.Scope;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E3.SYS.HOME
{
    public class SGN100 : IBaseUser, IBase
    {
        public DataPackage GetLogin(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                DataTable retTable = new DataTable();
                retTable.Columns.Add("USER_ID", typeof(string));
                retTable.Columns.Add("USER_NAME", typeof(string));
                retTable.Columns.Add("USER_NAME_EN", typeof(string));
                retTable.Columns.Add("PLT_ID", typeof(string));
                retTable.Columns.Add("UID", typeof(string));    // MAIL ID
                retTable.Columns.Add("RESULT", typeof(int));
                retTable.Columns.Add("UUID", typeof(string));
                retTable.Columns.Add("Token", typeof(string));

                string username = package.KeyValues["username"].ToString().Trim();
                string pass = package.KeyValues["password"].ToString().Trim();

                retTable.Rows.Add("", "", "", "", username, -1, "", "");

                if (username.IsNullOrEmpty())
                {
                    retTable.Rows[0]["RESULT"] = 501;
                }
                else if (pass.IsNullOrEmpty())
                {
                    retTable.Rows[0]["RESULT"] = 502;
                }
                else
                {
                    DataTable dataTable = SGNBase.DefaultMapper().QueryForDataTable("SGN100_LOGIN", package.KeyValues);
                    if (dataTable.Rows.Count == 1)
                    {
                        if (dataTable.Rows[0]["USER_PWD"].ToString().Trim().Equals(pass))
                        {
                            retTable.Rows[0]["RESULT"] = 200;
                            retTable.Rows[0]["UUID"] = Guid.NewGuid();
                            retTable.Rows[0]["USER_ID"] = dataTable.Rows[0]["USER_ID"];
                            retTable.Rows[0]["USER_NAME"] = dataTable.Rows[0]["USER_NAME"];
                            retTable.Rows[0]["USER_NAME_EN"] = dataTable.Rows[0]["USER_NAME_EN"];
                            retTable.Rows[0]["PLT_ID"] = dataTable.Rows[0]["PLT_ID"];
                            retTable.Rows[0]["Token"] = string.Format("{0}:{1}:{2}", dataTable.Rows[0]["PLT_ID"].ToString(), username, pass);
                        }
                        else
                        {
                            retTable.Rows[0]["RESULT"] = 404;
                        }
                    }
                    else if (dataTable.Rows.Count == 0)
                    {
                        retTable.Rows[0]["RESULT"] = 402;
                    }
                    else if (dataTable.Rows.Count > 1)
                    {
                        retTable.Rows[0]["RESULT"] = 403;
                    }

                }

                returnPackage.JsonData = retTable.ToJson<DataTable>();
                returnPackage.KeyValues.Add("Method", "GetLogin");
                returnPackage.KeyValues.Add("Result", retTable.Rows.Count.ToString());
                returnPackage.KeyValues.Add("ToTalCount", retTable.Rows.Count.ToString());
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Method", "GetLogin");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }

        public DataPackage GetMenus(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                if (package.KeyValues.ContainsKey("USER_ID")) package.KeyValues["USER_ID"] = package.Token.GetUserIDFromToken();
                else package.KeyValues.Add("USER_ID", package.Token.GetUserIDFromToken());
                DataTable dataTable = SGNBase.DefaultMapper().QueryForDataTable("SGN100_SELECT", package.KeyValues);

                //(ms.PreparedCommand as IBatisNet.DataMapper.Commands.IPreparedCommand)
                dataTable.Columns.Add("NAME_PATH", typeof(string));
                /*var root = dataTable.AsEnumerable().Where(x => x.Field<string>("PRNT_MNU_ID").Length == 0).FirstOrDefault();
                var qry = from table in dataTable.AsEnumerable() select table;*/
                returnPackage.JsonData = testLoad(dataTable).ToJson<DataTable>();
                returnPackage.KeyValues.Add("Method", "GetMenus");
                returnPackage.KeyValues.Add("Result", dataTable.Rows.Count.ToString());
                returnPackage.KeyValues.Add("ToTalCount", dataTable.Rows.Count.ToString());
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Method", "GetMenus");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }

        public DataPackage ChangePassword(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                DataTable retTable = new DataTable();
                retTable.Columns.Add("USER_ID", typeof(string));
                retTable.Columns.Add("PLT_ID", typeof(string));
                retTable.Columns.Add("RESULT", typeof(int));


                string PLT_ID = package.KeyValues["PLT_ID"].ToString().Trim();
                string USER_ID = package.Token.GetUserIDFromToken().Trim();
                retTable.Rows.Add(USER_ID, PLT_ID, -1);


                //E3.Common.Supports.TokenProvider
                if (package.KeyValues.ContainsKey("USER_ID")) package.KeyValues["USER_ID"] = package.Token.GetUserIDFromToken();
                else package.KeyValues.Add("USER_ID", package.Token.GetUserIDFromToken());
                DataTable dataTable = SGNBase.DefaultMapper().QueryForDataTable("SGN100_SELECTUSER", package.KeyValues);
                if (dataTable.Rows.Count == 1)
                {
                    string DBValue = Encoding.UTF8.GetString(Convert.FromBase64String(CryptographyUtility.Decrypt(dataTable.Rows[0]["USER_PASSWORD"].ToString())));
                    string ParamValue = Encoding.UTF8.GetString(Convert.FromBase64String(package.KeyValues["OLD_PASS"].ToString()));
                    if (DBValue.Equals(ParamValue))
                    {
                        /// 비밀번호 변경 이력에서 확인
                        if (check_old_password(package))
                        {
                            Int32 ret = SGNBase.DefaultMapper().QueryForObject<Int32>("SGN100_PASS_HISTORY_INSERT", package.KeyValues);
                            package.KeyValues["NEW_PASS"] = CryptographyUtility.Encrypt(package.KeyValues["NEW_PASS"].ToString());
                            dataTable = SGNBase.DefaultMapper().QueryForDataTable("SGN100_CHANGEPASS", package.KeyValues);
                            retTable.Rows[0]["RESULT"] = 200;
                        }
                        else 
                        {
                            returnPackage.ErrorMessage = "이전 비밀번호와 동일한 비밀번호입니다.";
                            retTable.Rows[0]["RESULT"] = -303;
                        }
                    }
                    else
                    {
                        returnPackage.ErrorMessage = "현재 비밀번호와 일치하지 않습니다.";
                        retTable.Rows[0]["RESULT"] = -302;
                    }
                }
                else
                {
                    returnPackage.ErrorMessage = "자료가 존재하지 않습니다.";
                    retTable.Rows[0]["RESULT"] = -301;
                }

                returnPackage.JsonData = retTable.ToJson<DataTable>();
                returnPackage.KeyValues.Add("Method", "CHANGEPASS");
                returnPackage.KeyValues.Add("Result", dataTable.Rows.Count.ToString());
                returnPackage.KeyValues.Add("ToTalCount", dataTable.Rows.Count.ToString());
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Method", "CHANGEPASS");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }
        /// <summary>
        /// 이전 비밀번호 이력과 비교하여 동일한 비밀번호가 있으면 False
        /// </summary>
        /// <param name="package">비밀번호 변경의 데이타</param>
        /// <returns>true/false</returns>
        private bool check_old_password(DataPackage package)
        {
            bool ret = true;
            try
            {
                DataTable oldPasswordTable = SGNBase.DefaultMapper().QueryForDataTable("SGN100_PASS_HISTORY_SELECT", package.KeyValues);
                string ParamValue = Encoding.UTF8.GetString(Convert.FromBase64String(package.KeyValues["NEW_PASS"].ToString()));
                foreach (DataRow row in oldPasswordTable.Rows)
                {
                    string DBValue = Encoding.UTF8.GetString(Convert.FromBase64String(CryptographyUtility.Decrypt(row[0].ToString())));
                    if(DBValue.Equals(ParamValue)) ret = false;
                }
            }
            catch (Exception e)
            {
                e.Data.Clear();
            }
            return ret;
        }

        public DataPackage GetFavoritMenu(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                if (package.KeyValues.ContainsKey("USER_ID")) package.KeyValues["USER_ID"] = package.Token.GetUserIDFromToken();
                else package.KeyValues.Add("USER_ID", package.Token.GetUserIDFromToken());
                DataTable dataTable = SGNBase.DefaultMapper().QueryForDataTable("SGN100_GETFAVORIT", package.KeyValues);
                returnPackage.JsonData = dataTable.ToJson<DataTable>();
                returnPackage.KeyValues.Add("Method", "GETFAVORIT");
                returnPackage.KeyValues.Add("Result", dataTable.Rows.Count.ToString());
                returnPackage.KeyValues.Add("ToTalCount", dataTable.Rows.Count.ToString());
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Method", "GETFAVORIT");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }

        public DataPackage SetFavoritMenu(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                if (package.KeyValues.ContainsKey("USER_ID")) package.KeyValues["USER_ID"] = package.Token.GetUserIDFromToken();
                else package.KeyValues.Add("USER_ID", package.Token.GetUserIDFromToken());
                DataTable dataTable = SGNBase.DefaultMapper().QueryForDataTable("SGN100_FAVORITSELECT", package.KeyValues);
                var chk = dataTable.AsEnumerable().Where(x => x.Field<string>("MENU_ID").ToString().Trim().Equals(package.KeyValues["MENU_ID"].ToString().Trim()));

                if (chk.Count() > 0)
                {
                    int ret = SGNBase.DefaultMapper().QueryForObject<Int32>("SGN100_FAVORITDELETE", package.KeyValues);
                }
                else 
                {
                    int ret = SGNBase.DefaultMapper().QueryForObject<Int32>("SGN100_FAVORITINSERT", package.KeyValues);
                }
                dataTable = SGNBase.DefaultMapper().QueryForDataTable("SGN100_FAVORITSELECT", package.KeyValues);
                returnPackage.JsonData = dataTable.ToJson<DataTable>();
                returnPackage.KeyValues.Add("Method", "SETFAVORIT");
                returnPackage.KeyValues.Add("Result", dataTable.Rows.Count.ToString());
                returnPackage.KeyValues.Add("ToTalCount", dataTable.Rows.Count.ToString());
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Method", "SETFAVORIT");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }

        public DataPackage SelectItem(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                if (package.KeyValues.ContainsKey("USER_ID")) package.KeyValues["USER_ID"] = package.Token.GetUserIDFromToken();
                else package.KeyValues.Add("USER_ID", package.Token.GetUserIDFromToken());
                DataTable dataTable = SGNBase.DefaultMapper().QueryForDataTable("SGN100_SELECTUSERINFO", package.KeyValues);
                returnPackage.JsonData = dataTable.ToJson<DataTable>();
                returnPackage.KeyValues.Add("Method", "USERINFO");
                returnPackage.KeyValues.Add("Result", dataTable.Rows.Count.ToString());
                returnPackage.KeyValues.Add("ToTalCount", dataTable.Rows.Count.ToString());
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Method", "USERINFO");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }

        public DataPackage SelectItems(DataPackage package)
        {
            throw new NotImplementedException();
        }

        public DataPackage InsertItem(DataPackage package)
        {
            throw new NotImplementedException();
        }

        public DataPackage InsertItems(DataPackage package)
        {
            throw new NotImplementedException();
        }

        public DataPackage UpdateItem(DataPackage package)
        {
            throw new NotImplementedException();
        }

        public DataPackage UpdateItems(DataPackage package)
        {
            throw new NotImplementedException();
        }

        public DataPackage DeleteItem(DataPackage package)
        {
            throw new NotImplementedException();
        }

        public DataPackage DeleteItems(DataPackage package)
        {
            throw new NotImplementedException();
        }

        public DataPackage PagingItems(DataPackage package)
        {
            throw new NotImplementedException();
        }

        private DataTable testLoad(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                //row["LVL"] = checkLVL(dataTable, row) + 1;
                if (Int16.Parse(row["LEVEL"].ToString()) == 1)
                    row["NAME_PATH"] = row["MENU_NAME"];
                else
                    row["NAME_PATH"] = string.Format("{0}>{1}", checkNMPTH(dataTable, row), row["MENU_NAME"]);
            }
            return dataTable;
        }

        private int checkLVL(DataTable dataTable, DataRow dataRow)
        {
            int ret = 0;
            foreach (DataRow row in dataTable.Rows)
            {
                if (dataRow.Field<string>("PARENT_ID") == row.Field<string>("MENU_ID"))
                {
                    ret++;
                    ret += checkLVL(dataTable, row);
                    break;
                }
            }
            return ret;
        }

        private string checkNMPTH(DataTable dataTable, DataRow dataRow)
        {
            string ret = "";
            foreach (DataRow row in dataTable.Rows)
            {
                if (dataRow.Field<string>("PARENT_ID") == row.Field<string>("MENU_ID"))
                {
                    ret = string.IsNullOrEmpty((string)row.Field<string>("NAME_PATH")) ? row.Field<string>("MENU_NAME") : row.Field<string>("MENU_NAME");
                    break;
                }
            }
            return ret;
        }
    }

}
