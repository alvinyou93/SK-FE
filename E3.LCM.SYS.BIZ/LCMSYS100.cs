using E3.Common;
using E3.Common.IBatis;
using E3.Common.Loggers;
using E3.Common.Supports;
using E3.Common.Wcf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E3.LCM.SYS.BIZ
{
    public class LCMSYS100
    {
        // 계정정보 조회
        private const string SYSTEM_USER_SELECT = "LCMSYS100_LOGIN";
        private const string SYSTEM_USER_SELECT_LOGIN_FAIL = "LCMSYS100_LOGIN_FAIL";
        private const string SYSTEM_USER_SELECT_LOGIN_SUCCESS = "LCMSYS100_LOGIN_SUCCESS";

        public DataPackage Login(DataPackage dataPackage)
        {
            DataPackage result = new DataPackage();  

            DataPackage pParam = dataPackage.JsonData.JsonTo<DataPackage>();

            string Authorization = Shell.GetWebHeaderData("Authorization");
            string[] AuthValue = string.IsNullOrEmpty(Authorization) ? new string[] {"", "", "" } : Encoding.UTF8.GetString(System.Convert.FromBase64String(Authorization)).Split(":");

            if (pParam.Division.Compare("user"))
            {
                // username :: AuthValue[0]
                // phonenumber :: AuthValue[1]
                // password :: AuthValue[2]
                //result = SystemSupport.ExecuteLogin(pParam);
                result = ExecuteLogin4PublicAccount(pParam);
            }
            else
            {
                // username :: AuthValue[0]
                // E3FRAMEWORK :: AuthValue[1]
                // password :: AuthValue[2]
                result = SystemSupport.ExecuteLogin(pParam);
            }
            return result;
        }



        /// <summary>
        /// Login 처리 후 Token 생성
        /// </summary>
        /// <param name="dataPackage">로그인 처리 DataPackage</param>
        /// <returns>Token을 담은 DataPackage</returns>
        public static DataPackage ExecuteLogin4PublicAccount(DataPackage dataPackage)
        {
            DataPackage returnPackage = new DataPackage();

            try
            {
                string username = dataPackage.KeyValues.GetValue("username");
                string phonenumber = dataPackage.KeyValues.GetValue("phonenumber");
                string password = dataPackage.KeyValues.GetValue("password");

                Dictionary<string, string> result = PublicAccountValidation(username, phonenumber, password);

                Dictionary<string, string> tokenParam = new Dictionary<string, string>();
                tokenParam.Add(TokenKeys.USER_ID, string.Format("{0}-{1}", result.GetValue("EMP_TEL_NO"), result.GetValue("EMP_ID")));
                tokenParam.Add(TokenKeys.USER_PWD, result.GetValue("CRTIC_NO"));
                tokenParam.Add(TokenKeys.PLANT_ID, result.GetValue(TokenKeys.PLANT_ID));

                returnPackage.KeyValues = result;
                returnPackage.Token = TokenProvider.CreateToken(tokenParam);

                //TokenProvider.ValidationTokenAdd(string.Format("{0}-{1}", result.GetValue("EMP_TEL_NO"), result.GetValue("EMP_ID")), returnPackage.Token);

                LOGIN_SUCCESS(username, phonenumber); // 로그인 성공
            }
            catch (Exception ex)
            {
                FileLogger.Error(ex);
                returnPackage.ErrorMessage = ex.Message.ToString();
            }

            return returnPackage;
        }

        /// <summary>
        /// 로그인 Validation 처리
        /// </summary>
        /// <param name="username"></param>
        /// <param name="phonenumber"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private static Dictionary<string, string> PublicAccountValidation(string username, string phonenumber, string password)
        {
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs.Add("EMP_NM", username);
            pairs.Add("EMP_TEL_NO", phonenumber);
            pairs.Add("CRTIC_NO", password);

            DataTable dataTable = LCMSYSBase.DefaultMapper().QueryForDataTable(SYSTEM_USER_SELECT, pairs);

            if (dataTable.Rows.Count > 1)
            {
                // 여러 건인 경우 사용자를 특정할 방법을 연구
                foreach (DataRow row in dataTable.Rows) {
                    Dictionary<string, string> dicData = row.ToDictionary();
                    if ((dicData.GetValue("CRTIC_NO")).Equals(GetBase64toString(password)))
                    {
                        return PublicAccountValidationDetail(dicData, username, phonenumber, password);
                    }
                }
                throw new Exception("비밀번호가 틀립니다.");
            }
            else
            {
                Dictionary<string, string> dicData = LCMSYSBase.DefaultMapper().QueryForDictionarry(SYSTEM_USER_SELECT, pairs);
                return PublicAccountValidationDetail(dicData, username, phonenumber, password);
            }
        }

        private static Dictionary<string, string> PublicAccountValidationDetail(Dictionary<string, string> dicData, string username, string phonenumber, string password)
        {
            if (dicData.GetValue("EMP_NM").IsNullOrEmpty())
            {
                //throw new Exception("등록된 계정이 없습니다.");
                throw new Exception("로그인 정보가 일치하지 않습니다.");
            }
            else if (dicData.GetValue("EMP_TEL_NO").IsNullOrEmpty())
            {
                //throw new Exception("등록된 계정이 없습니다.");
                throw new Exception("로그인 정보가 일치하지 않습니다.");
            }
            else if (dicData.GetValue("FAIL_CNT").ToInt() >= 5)
            {
                throw new Exception("계정이 잠겼습니다. 관리자에게 문의하세요.");
            }
            //else if (!dicData.GetValue("USER_PWD").Equals(password))
            else if (!(dicData.GetValue("CRTIC_NO")).Equals(GetBase64toString(password)))
            {
                LOGIN_FAIL(username, phonenumber); // 로그인 실패
                throw new Exception("로그인 정보가 일치하지 않습니다.");
            }
            //dicData.Remove("CRTIC_NO"); // 인증코드를 제외한 나머지 리턴
            return dicData;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isDecrupt">value의 복화와 여부</param>
        /// <returns></returns>
        private static string GetBase64toString(string value, bool isDecrupt = false)
        {
            try
            {
                if (isDecrupt)
                {
                    value = CryptographyUtility.Decrypt(value);
                }

                return Encoding.UTF8.GetString(Convert.FromBase64String(value));
            }
            catch (Exception ex)
            {
                FileLogger.Error(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// 로그인 실패한 경우 실패 카운트 증가
        /// </summary>
        /// <param name="username">사용자성명</param>
        /// <param name="phonenumber">휴대전화번호</param>
        private static void LOGIN_FAIL(string username, string phonenumber)
        {
            try
            {
                Dictionary<string, string> pairs = new Dictionary<string, string>();
                pairs.Add("EMP_NM", username);
                pairs.Add("EMP_TEL_NO", phonenumber);

                LCMSYSBase.DefaultMapper().QueryForObject<Int32>(SYSTEM_USER_SELECT_LOGIN_FAIL, pairs);
            }
            catch (Exception e)
            {
                FileLogger.Error(e);
                e.Data.Clear();
            }
        }

        /// <summary>
        /// 로그인 성공한 경우 실패한 기록을 초기화 한다.
        /// </summary>
        /// <param name="username">사용자성명</param>
        /// <param name="phonenumber">휴대전화번호</param>
        private static void LOGIN_SUCCESS(string username, string phonenumber)
        {
            try
            {
                Dictionary<string, string> pairs = new Dictionary<string, string>();
                pairs.Add("EMP_NM", username);
                pairs.Add("EMP_TEL_NO", phonenumber);

                LCMSYSBase.DefaultMapper().QueryForObject<Int32>(SYSTEM_USER_SELECT_LOGIN_SUCCESS, pairs);
            }
            catch (Exception e)
            {
                FileLogger.Error(e);
                e.Data.Clear();
            }
        }
    }
}
