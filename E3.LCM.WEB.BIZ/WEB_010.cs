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

/// <summary>
/// 파트너사관리
/// </summary>
/// <param name="package"></param>
/// <returns></returns>
namespace E3.LCM.WEB.BIZ
{
    public class WEB_010
    {
        /// <summary>
        /// 파트너사 조회
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage GetCmpList(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WEB010_SELECT", package.KeyValues);
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

        /// <summary>
        /// 파트너사 저장
        /// 사업자번호로 중복확인
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage InserCmpList(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();            
            try
            {
                Dictionary<string, string> param = package.JsonData.ToDictionary();
                param.Add("Now", package.KeyValues.GetValue("Now"));
                param.Add("LoginedUserID", package.Token.GetUserIDFromToken());                

                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WEB010_CMPID_COUNT", param);

                if (dataTable.Rows[0]["CNT"].ToInt() > 0)
                {
                    Int32 ret = DataMapper.DefaultMapper.QueryForObject<Int32>("WEB010_UPDATE", param);
                    returnPackage.KeyValues.Add("Method", "UPDATE");
                    returnPackage.KeyValues.Add("Result", ret.ToString());
                }
                else
                {
                    DataTable dataTableChk = DataMapper.DefaultMapper.QueryForDataTable("WEB010_CMP_NO_COUNT", param);

                    if (dataTableChk.Rows[0]["CNT"].ToInt() == 0)
                    {
                        Int32 ret = DataMapper.DefaultMapper.QueryForObject<Int32>("WEB010_INSERT", param);
                        returnPackage.KeyValues.Add("Method", "INSERT");
                        returnPackage.KeyValues.Add("Result", ret.ToString());
                    }
                    else {
                        returnPackage.ErrorMessage = "같은 사업자번호의 파트너사가 이미 존재합니다. 확인 후 다시 저장해주십시오.";
                        returnPackage.KeyValues.Add("Method", "INSERT");
                        returnPackage.KeyValues.Add("Result", "같은 사업자번호의 파트너사가 이미 존재합니다. 확인 후 다시 저장해주십시오.2");
                    }
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

        /// <summary>
        /// 파트너사 삭제
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage DeleteCmpList(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                Dictionary<string, string> param = package.JsonData.ToDictionary();
                param.Add("Now", package.KeyValues.GetValue("Now"));

                DataTable dataTableUse = DataMapper.DefaultMapper.QueryForDataTable("WEB010_SELECT_USE_CNT", param);
                if (dataTableUse.Rows[0]["CNT"].ToInt() > 0)
                {   
                    returnPackage.KeyValues.Add("Method", "ERROR");
                    returnPackage.KeyValues.Add("Result", (-1).ToString());
                    // returnPackage.ErrorMessage = "출입대상자가 등록된 파트너사입니다.";
                }
                else
                {
                    DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WEB010_DELETE", param);
                    returnPackage.KeyValues.Add("Method", "DELETE");
                    returnPackage.KeyValues.Add("Result", dataTable.Rows.Count.ToString());
                    returnPackage.JsonData = dataTable.ToJson<DataTable>();
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

        /// <summary>
        /// 파트너사 엑셀 업로드
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage InserCmpListExcel(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();            
            List<Dictionary<string, string>> dataErrList = new List<Dictionary<string, string>>();
            DataTable dataErrTable = new DataTable();            
            dataErrTable.Columns.Add("CMP_NM", typeof(string));
            dataErrTable.Columns.Add("CMP_NO", typeof(string));
            dataErrTable.Columns.Add("MNGR_NM", typeof(string));
            dataErrTable.Columns.Add("MNGR_TEL_NO", typeof(string));
            dataErrTable.Columns.Add("MNGR_EMAIL", typeof(string));
            dataErrTable.Columns.Add("ERR_TYPE", typeof(string));

            try
            {
                Int32 ret = 0;
                List<Dictionary<string, string>> listDict = package.JsonData.ToDictionaryList();
                
                
                if (listDict.Count > 0)
                {
                    foreach (Dictionary<string, string> dict in listDict)
                    {
                        string err_type = "";
                        // 유효성 체크 - 사업자번호
                        if (dict.GetValue("사업자번호").IsNullOrEmpty() || !IsValidBizNo(dict.GetValue("사업자번호")))
                        {
                            err_type = "사업자번호오류";
                        }
                        if (dict.GetValue("담당자연락처").IsNullOrEmpty() ||  !IsValidHpNo(dict.GetValue("담당자연락처")))
                        {
                            if (err_type != "") err_type += ",";
                            err_type += "연락처오류";
                        }
                        if (dict.GetValue("담당자EMAIL").IsNullOrEmpty() ||  !IsValidEmail(dict.GetValue("담당자EMAIL")))
                        {
                            if (err_type != "") err_type += ",";
                            err_type += "메일형식오류";
                        }

                        dict.Add("CMP_NO", dict.GetValue("사업자번호"));
                        dict.Add("CMP_NO2", dict.GetValue("사업자번호"));
                        DataTable dataTableChk = DataMapper.DefaultMapper.QueryForDataTable("WEB010_CMP_NO_COUNT", dict);
                        if (dataTableChk.Rows[0]["CNT"].ToInt() > 0) {                            
                            if (err_type != "") err_type += ",";
                            err_type += "사업자번호중복";
                        }

                        if (err_type.Equals(""))
                        {
                            dict.Add("Now", package.KeyValues.GetValue("Now"));
                            dict.Add("LoginedUserID", package.Token.GetUserIDFromToken());

                            dict.Add("CMP_NM", dict.GetValue("회사명"));
                            // dict.Add("CMP_NO", dict.GetValue("사업자번호"));
                            dict.Add("MNGR_NM", dict.GetValue("담당자명"));
                            dict.Add("MNGR_TEL_NO", dict.GetValue("담당자연락처"));
                            dict.Add("MNGR_EMAIL", dict.GetValue("담당자EMAIL"));
                             
                            DataMapper.DefaultMapper.QueryForObject<Int32>("WEB010_INSERT", dict);
                        }

                        if (err_type.IsNotNullOrEmpty())
                        {
                            DataRow errListDict = dataErrTable.NewRow();

                            errListDict.SetField("CMP_NM", dict.GetValue("회사명"));
                            errListDict.SetField("CMP_NO", dict.GetValue("사업자번호"));
                            errListDict.SetField("MNGR_NM", dict.GetValue("담당자명"));
                            errListDict.SetField("MNGR_TEL_NO", dict.GetValue("담당자연락처"));
                            errListDict.SetField("MNGR_EMAIL", dict.GetValue("담당자EMAIL"));
                            errListDict.SetField("ERR_TYPE", err_type);

                            dataErrTable.Rows.Add(errListDict);                            
                        }
                    }
                }

                if (dataErrTable.Rows.Count > 0)
                {
                    returnPackage.KeyValues.Add("Method", "ERROR");
                    returnPackage.JsonData = dataErrTable.ToJson<DataTable>();
                }
                else
                {
                    returnPackage.KeyValues.Add("Method", "INSERT");
                }
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

        /// <summary>
        /// 사업자등록번호 유효성 검사
        /// 자릿수만 체크하기로함.
        /// </summary>
        /// <param name="biz_no"></param>
        /// <returns></returns>
        public static bool IsValidBizNo(string biz_no)
        {
            biz_no = biz_no.Replace(" ", ""); //공백 제거
            biz_no = biz_no.Replace("-", ""); // 문자 '-' 제거

            if (biz_no.Length != 10) //사업자 등록번호가 10자리인가?
            {
                return false;
            }
            return true;
            /*int sum = 0;
            int checknum = 0;
            int[] arrNumList = new int[10];
            int[] arrCheckNum = { 1, 3, 7, 1, 3, 7, 1, 3, 5 };

            for (int i = 0; i < 10; i++)
            {
                arrNumList[i] = Convert.ToInt32(biz_no[i].ToString());
            }

            for (int i = 0; i < 9; i++)
            {
                sum += arrNumList[i] * arrCheckNum[i];
            }

            sum += ((arrNumList[8] * 5) / 10);
            checknum = (10 - sum % 10) % 10;

            return (checknum == arrNumList[9]);*/
        }

        /// <summary>
        /// 전화번호 유효성 검사
        /// </summary>
        /// <param name="hp_no"></param>
        /// <returns></returns>        
        public static bool IsValidHpNo(string hp_no)
        {
            Regex regex = new Regex(@"^(01[016789]{1}|02|0[3-9]{1}[0-9]{1})-?[0-9]{3,4}-?[0-9]{4}$");

            if (regex.IsMatch(hp_no) )
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// EMAIL 유효성 검사 
        /// </summary>
        /// <param name="eMail"></param>
        /// <returns></returns>
        public static bool IsValidEmail(string eMail)
        {
            Regex regex = new Regex(@"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?");
            
            if (regex.IsMatch(eMail))
            {
                return true;
            }
            return false;
        }
    }
}
