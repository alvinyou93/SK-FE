using E3.Common;

using E3.Common.IBatis;
using E3.Common.Wcf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.IO;

// 출입자 관리 및 상세 팝업
namespace E3.LCM.WEB.BIZ
{
    public class WEB_020
    {   
        /// <summary>
        /// 출입자 조회
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage GetEmpList(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WEB020_SELECT", package.KeyValues);
                returnPackage.KeyValues.Add("Method", "SELECT");
                returnPackage.KeyValues.Add("Result", dataTable.Rows.Count.ToString());
                returnPackage.JsonData = dataTable.ToJson<DataTable>();
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Method", "SELECT");
                returnPackage.KeyValues.Add("Result", (-1).ToString()); // TEST
            }
            return returnPackage;
        }

        /// <summary>
        /// 회사 목록 조회
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage GetCmpList(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WEB020_SELECT_CMP_COMBO", package.KeyValues);
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
        /// 출입자 저장
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage InserEmpList(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                Dictionary<string, string> param = package.JsonData.ToDictionary();
                param.Add("Now", package.KeyValues.GetValue("Now"));
                param.Add("LoginedUserID", package.Token.GetUserIDFromToken());

                // 체크1. UPDATE_SEQ 존재여부
                
                String chkUploadSeq = package.KeyValues.GetValue("UPLOAD_SEQ");
                String empTelNo = param.GetValue("EMP_TEL_NO").Replace("-", "").ToString();
                param.SetValue("EMP_TEL_NO", empTelNo);
                

                // 체크2. 사업장, TA, 공사, 전화번호 중복여부
                DataTable dataTableExcel = DataMapper.DefaultMapper.QueryForDataTable("WEB020_EXCEL_COUNT", param); // 체크1. 사업장, TA, 공사, 전화번호 중복여부

                DataTable dataTableExcel2 = DataMapper.DefaultMapper.QueryForDataTable("WEB020_EXCEL_TEL_NM_COUNT", param); // 체크2. 동일 전화번호 다른 이름 존재여부 확인

                DataTable dataTableExcel3 = DataMapper.DefaultMapper.QueryForDataTable("WEB020_EXCEL_ACC_GB_COUNT", param); // 체크3. 다른 출입유형

                if (dataTableExcel2.Rows.Count > 0)
                {
                    returnPackage.ErrorMessage = "같은 전화번호를 사용하는 다른 사용자가 존재합니다.";
                    returnPackage.KeyValues.Add("Method_1", "INSERT or UPDATE");
                    returnPackage.KeyValues.Add("Result_1", (-1).ToString());
                }
                else if (dataTableExcel3.Rows.Count > 0)
                {
                    returnPackage.ErrorMessage = "다른 출입 유형이 이미 존재합니다.";
                    returnPackage.KeyValues.Add("Method_2", "INSERT or UPDATE");
                    returnPackage.KeyValues.Add("Result_2", (-1).ToString());
                }
                else
                {
                    if (!chkUploadSeq.IsNotNullOrEmpty() && dataTableExcel.Rows.Count > 0 && dataTableExcel.Rows[0]["CNT"].ToInt() > 0)
                    {
                        // 출입자 엑셀 테이블 등록
                        param.SetValue("UPLOAD_SEQ", dataTableExcel.Rows[0]["UPLOAD_SEQ"].ToString());
                        Int32 ret = DataMapper.DefaultMapper.QueryForObject<Int32>("WEB020_UPDATE", param);
                        returnPackage.KeyValues.Add("Method_3", "UPDATE");
                        returnPackage.KeyValues.Add("Result_3", ret.ToString());
                    }
                    else
                    {
                        // 출입자 엑셀 테이블 등록
                        Int32 ret2 = DataMapper.DefaultMapper.QueryForObject<Int32>("WEB020_INSERT", param);
                        returnPackage.KeyValues.Add("Method_4", "INSERT");
                        returnPackage.KeyValues.Add("Result_5", ret2.ToString());
                    }

                    // 출입자 테이블 등록, 증빙서류 테이블 등록
                    DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WEB020_EMP_COUNT2", param);
                    if (dataTable.Rows.Count > 0 && dataTable.Rows[0]["CNT"].ToInt() == 1)
                    {
                        param.Add("EMP_ID", dataTable.Rows[0]["EMP_ID"].ToString());                        
                        if(param.GetValue("NEW_YN") == "Y")
                            param.SetValue("ACCESS_POSSIBLE_YN", dataTable.Rows[0]["ACCESS_POSSIBLE_YN"].ToString());
                    }                   

                    Int32 ret3 = DataMapper.DefaultMapper.QueryForObject<Int32>("WEB020_INSERT_EMP", param);                    

                    DataTable dataTableSMS = DataMapper.DefaultMapper.QueryForDataTable("WEB020_SELECT_SMS_SND_YN", param); // 안면인식기 IF를 위한 EMP_ID 조회
                    
                    returnPackage.KeyValues.Add("Method_EMP", "INSERT");
                    returnPackage.KeyValues.Add("Result_EMP", ret3.ToString());
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
        /// 출입자 삭제
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage DeleteEmpList(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                Dictionary<string, string> param = package.JsonData.ToDictionary();
                param.Add("Now", package.KeyValues.GetValue("Now"));

                // 출입자 업로드 테이블 삭제
                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WEB020_DELETE", param);
                DataTable dataTableEmp = DataMapper.DefaultMapper.QueryForDataTable("WEB020_EMP_DELETE", param);

                returnPackage.KeyValues.Add("Method", "DELETE");
                returnPackage.KeyValues.Add("Result", dataTable.Rows.Count.ToString());
                returnPackage.JsonData = dataTable.ToJson<DataTable>();

                // 사용자 삭제
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
        /// 출입자 엑셀 업로드
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage InserEmpListExcel(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            List<Dictionary<string, string>> dataErrList = new List<Dictionary<string, string>>();
            DataTable dataErrTable = new DataTable();
            dataErrTable.Columns.Add("PLANT_NM", typeof(string));
            dataErrTable.Columns.Add("FACT_NM", typeof(string));
            dataErrTable.Columns.Add("TA_CODE_NM", typeof(string));
            dataErrTable.Columns.Add("REL_DEPT_NM", typeof(string));
            dataErrTable.Columns.Add("TA_SUB", typeof(string));
            dataErrTable.Columns.Add("CMP_NO", typeof(string));
            dataErrTable.Columns.Add("CMP_NM", typeof(string));
            dataErrTable.Columns.Add("EMP_NM", typeof(string));
            dataErrTable.Columns.Add("EMP_TEL_NO", typeof(string));
            dataErrTable.Columns.Add("ACCESS_GB_NM", typeof(string));
            dataErrTable.Columns.Add("BIRTH_DT", typeof(string));
            dataErrTable.Columns.Add("ADDR", typeof(string));
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
                        // 유효성 체크 - 사업장                            
                        if (dict.GetValue("사업장").IsNullOrEmpty())
                        {
                            if (err_type != "") err_type += ",";
                            err_type += "사업장 미입력";
                        }
                        // 유효성 체크 - 공장                            
                        else if (dict.GetValue("공장").IsNullOrEmpty())
                        {
                            if (err_type != "") err_type += ",";
                            err_type += "공장 미입력";
                        }
                        // 유효성 체크 - 정기보수 구분                            
                        else if (dict.GetValue("정기보수").IsNullOrEmpty())
                        {
                            if (err_type != "") err_type += ",";
                            err_type += "정기보수 구분 미입력";
                        }
                        // 유효성 체크 - 작업부서 구분                            
                        else if (dict.GetValue("작업부서").IsNullOrEmpty())
                        {
                            if (err_type != "") err_type += ",";
                            err_type += "작업부서 미입력";
                        }
                        // 유효성 체크 - 작업내용 구분                            
                        else if (dict.GetValue("작업내용").IsNullOrEmpty())
                        {
                            if (err_type != "") err_type += ",";
                            err_type += "작업내용 미입력";
                        }
                        // 유효성 체크 - 사업자번호                          
                        else if (dict.GetValue("사업자번호").IsNullOrEmpty())
                        {
                            if (err_type != "") err_type += ",";
                            err_type += "사업자번호 미입력";
                        }
                        // 유효성 체크 - 성명 구분                            
                        else if (dict.GetValue("성명").IsNullOrEmpty())
                        {
                            if (err_type != "") err_type += ",";
                            err_type += "성명 미입력";
                        }
                        
                        // 유효성 체크 - 연락처
                        else if (dict.GetValue("연락처(HP)").IsNullOrEmpty() )
                        {
                            if (err_type != "") err_type += ",";
                            err_type += "연락처 오류";
                        }
                        // 유효성 체크 - 연락처
                        else if (!IsValidHpNo(dict.GetValue("연락처(HP)")))
                        {
                            if (err_type != "") err_type += ",";
                            err_type += "연락처 미입력";
                        }
                        // 유효성 체크 - 출입구분                          
                        else if (dict.GetValue("출입구분").IsNullOrEmpty())
                        {
                            if (err_type != "") err_type += ",";
                            err_type += "출입구분 미입력";
                        }

                        if (err_type.Equals(""))
                        {
                            dict.Add("Now", package.KeyValues.GetValue("Now"));
                            dict.Add("LoginedUserID", package.Token.GetUserIDFromToken());

                            dict.Add("PLANT_ID", dict.GetValue("사업장"));
                            dict.Add("FACT_CD", dict.GetValue("공장"));
                            dict.Add("TA_CODE", dict.GetValue("정기보수"));
                            dict.Add("REL_DEPT_CD", dict.GetValue("작업부서"));
                            dict.Add("TA_SUB", dict.GetValue("작업내용"));
                            dict.Add("CMP_NM", dict.GetValue("파트너사명"));
                            dict.Add("CMP_NO", Regex.Replace(dict.GetValue("사업자번호").ToString(), @"\D", ""));
                            dict.Add("EMP_NM", dict.GetValue("성명"));
                            dict.Add("EMP_TEL_NO", Regex.Replace(dict.GetValue("연락처(HP)").ToString(), @"\D", ""));
                            dict.Add("ACCESS_GB", dict.GetValue("출입구분"));
                            dict.Add("BIRTH_DT", Regex.Replace(dict.GetValue("생년월일").ToString(), @"\D", ""));
                            dict.Add("ADDR", dict.GetValue("주소"));
                            dict.Add("CMP_NO2", dict.GetValue("사업자번호").ToString());

                            // 사업자 번호 존재 여부 확인
                            DataTable dataTableCMP = DataMapper.DefaultMapper.QueryForDataTable("WEB010_CMP_NO_COUNT", dict);

                            DataTable dataTableExcel2 = DataMapper.DefaultMapper.QueryForDataTable("WEB020_EXCEL_TEL_NM_COUNT", dict); // 체크2. 동일 전화번호 다른 이름 존재여부 확인
                            
                            DataTable dataTableExcel3 = DataMapper.DefaultMapper.QueryForDataTable("WEB020_EXCEL_ACC_GB_COUNT", dict); // 체크3. 다른 출입유형

                            DataTable dataTableExcel4 = DataMapper.DefaultMapper.QueryForDataTable("WEB020_SELECT_TA_CDOE", dict); // 체크4. TA 코드 확인

                            if (dataTableCMP.Rows[0]["CNT"].ToInt() == 0)
                            {
                                if (err_type != "") err_type += ",";
                                err_type += "미등록파트너사";
                            }
                            else if (dataTableExcel2.Rows.Count > 0) {
                                if (err_type != "") err_type += ",";
                                err_type += "같은 전화번호를 사용하는 다른 사용자";                                
                            }
                            else if (dataTableExcel3.Rows.Count > 0)
                            {
                                if (err_type != "") err_type += ",";
                                err_type += "다른 출입 유형이 이미 존재";
                            }
                            else if (dataTableExcel4.Rows.Count == 0)
                            {
                                if (err_type != "") err_type += ",";
                                err_type += "존재하지 않는 정기보수";
                            }
                            else
                            {
                                // EMP_EXCEL 중복 확인
                                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WEB020_EXCEL_COUNT", dict);

                                //if (dataTable.Rows.Count == 0 || (dict.GetValue("CMP_NO").IsNotNullOrEmpty() && dataTable.Rows[0]["CNT"].ToInt() == 0))
                                if (dataTable.Rows.Count == 0 || (dict.GetValue("CMP_NO").IsNotNullOrEmpty() && dataTable.Rows[0]["CNT"].ToInt() == 0))
                                        DataMapper.DefaultMapper.QueryForObject<Int32>("WEB020_INSERT", dict);
                                else if (dict.GetValue("CMP_NO").IsNotNullOrEmpty() && dataTable.Rows[0]["CNT"].ToInt() > 0)
                                    DataMapper.DefaultMapper.QueryForObject<Int32>("WEB020_UPDATE_EXCEL", dict);
                            

                                // 출입자 테이블 등록, 증빙서류 테이블 등록
                                DataTable dataTable2 = DataMapper.DefaultMapper.QueryForDataTable("WEB020_EMP_COUNT", dict);                            
                                dict.Add("EMP_ID", dataTable2.Rows[0]["EMP_ID"].ToString());                            

                                // 출입자 테이블 등록
                                Int32 ret2 = DataMapper.DefaultMapper.QueryForObject<Int32>("WEB020_INSERT_EMP2", dict);
                            }
                        }
                        if (err_type.IsNotNullOrEmpty())
                        {
                            DataRow errListDict = dataErrTable.NewRow();
                            errListDict["PLANT_NM"] = dict.GetValue("사업장");
                            errListDict["FACT_NM"] = dict.GetValue("공장");
                            errListDict["TA_CODE_NM"] = dict.GetValue("정기보수");
                            errListDict["REL_DEPT_NM"] = dict.GetValue("작업부서");
                            errListDict["TA_SUB"] = dict.GetValue("작업내용");

                            errListDict["CMP_NM"] = dict.GetValue("파트너사명");
                            errListDict["CMP_NO"] = dict.GetValue("사업자번호");                            
                            errListDict["EMP_NM"] = dict.GetValue("성명");
                            errListDict["EMP_TEL_NO"] = dict.GetValue("연락처(HP)");
                            errListDict["ACCESS_GB_NM"] = dict.GetValue("출입구분");
                            errListDict["BIRTH_DT"] = dict.GetValue("생년월일");
                            errListDict["ADDR"] = dict.GetValue("주소");
                            errListDict["ERR_TYPE"] = err_type;

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
        /// 전화번호 유효성 검사 
        /// </summary>
        /// <param name="hp_no"></param>
        /// <returns></returns>
        public static bool IsValidHpNo(string hp_no)
        {
            Regex regex = new Regex(@"^01[016789]-[0-9]{4}-[0-9]{4}$");
            Regex regex2 = new Regex(@"^01[016789][0-9]{4}[0-9]{4}$");

            if (regex.IsMatch(hp_no) || regex.IsMatch(hp_no))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 출입자 상세 조회
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage GetEmpDtlList(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WEB020_SELECT_EMP_FILE", package.KeyValues);
                DataTable dataTable2 = DataMapper.DefaultMapper.QueryForDataTable("WEB020_SELECT_EMP_DTL", package.KeyValues);

                if (dataTable2.Rows.Count > 0) 
                {
                    returnPackage.KeyValues.Add("PLANT_NM", dataTable2.Rows[0]["PLANT_NM"].ToString()); // 1
                    returnPackage.KeyValues.Add("FACT_NM", dataTable2.Rows[0]["FACT_NM"].ToString());   // 2
                    returnPackage.KeyValues.Add("TA_NM", dataTable2.Rows[0]["TA_NM"].ToString());       // 3
                    returnPackage.KeyValues.Add("TA_SUB", dataTable2.Rows[0]["TA_SUB"].ToString());     // 4
                    returnPackage.KeyValues.Add("EMP_NM", dataTable2.Rows[0]["EMP_NM"].ToString());     // 5               
                    returnPackage.KeyValues.Add("EMP_TEL_NO", dataTable2.Rows[0]["EMP_TEL_NO"].ToString()); // 6                   
                    returnPackage.KeyValues.Add("CMP_NM", dataTable2.Rows[0]["CMP_NM"].ToString());         // 7
                    returnPackage.KeyValues.Add("MNGR_TEL_NO", dataTable2.Rows[0]["MNGR_TEL_NO"].ToString());   // 9
                    returnPackage.KeyValues.Add("PRIVACY_CONSENT_DT", dataTable2.Rows[0]["PRIVACY_CONSENT_DT"].ToString()); // 10
                    returnPackage.KeyValues.Add("SAFE_EDU_CMPLT_DT", dataTable2.Rows[0]["SAFE_EDU_CMPLT_DT"].ToString());   // 11
                    returnPackage.KeyValues.Add("PASS_DT", dataTable2.Rows[0]["PASS_DT"].ToString());                       // 12
                    returnPackage.KeyValues.Add("ACCESS_GB_NM", dataTable2.Rows[0]["ACCESS_GB_NM"].ToString());             // 13
                    returnPackage.KeyValues.Add("BP_LOW", dataTable2.Rows[0]["BP_LOW"].ToString());                         // 14
                    returnPackage.KeyValues.Add("BP_HIGHT", dataTable2.Rows[0]["BP_HIGHT"].ToString());                     // 15
                    returnPackage.KeyValues.Add("RMK", dataTable2.Rows[0]["RMK"].ToString());                               // 16

                    string idPhoto = "";
                    if (dataTable2.Rows[0]["ID_PHOTO"].ToString().IsNotNullOrEmpty())
                        idPhoto = string.Format("data:image/jpeg;base64,{0}", E3.Common.Supports.CryptographyUtility.Decrypt(dataTable2.Rows[0]["ID_PHOTO"].ToString())); 

                    returnPackage.KeyValues.Add("ID_PHOTO", idPhoto);   // 17
                    returnPackage.KeyValues.Add("ACCESS_POSSIBLE_YN", dataTable2.Rows[0]["ACCESS_POSSIBLE_YN"].ToString()); // 18
                    returnPackage.KeyValues.Add("UPLOAD_SEQ", dataTable2.Rows[0]["UPLOAD_SEQ"].ToString());                 // 19
                    returnPackage.KeyValues.Add("EMP_ID", dataTable2.Rows[0]["EMP_ID"].ToString());                         // 20
                    returnPackage.KeyValues.Add("CMP_ID", dataTable2.Rows[0]["CMP_ID"].ToString());                         // 21
                    returnPackage.KeyValues.Add("PLANT_ID", dataTable2.Rows[0]["PLANT_ID"].ToString());                     // 22
                    returnPackage.KeyValues.Add("TA_CODE", dataTable2.Rows[0]["TA_CODE"].ToString());                       // 23
                    returnPackage.KeyValues.Add("BIRTH_DT", dataTable2.Rows[0]["BIRTH_DT"].ToString());                     // 24
                    returnPackage.KeyValues.Add("ADDR", dataTable2.Rows[0]["ADDR"].ToString());                       // 25
                    returnPackage.KeyValues.Add("CRTIC_NO", dataTable2.Rows[0]["CRTIC_NO"].ToString());                       // 26

                }                

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
        /// 출입자 상세 정보 수정
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage UpdateEmpDtl(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                List<Dictionary<string, string>> listDict = package.JsonData.ToDictionaryList();

                Dictionary<string, string> param = new Dictionary<string, string>();

                string tmp = package.KeyValues.GetValue("PLANT_ID");
                param.Add("Now", package.KeyValues.GetValue("Now"));
                param.Add("LoginedUserID", package.Token.GetUserIDFromToken());

                param.Add("PLANT_ID", package.KeyValues.GetValue("PLANT_ID"));
                param.Add("EMP_ID", package.KeyValues.GetValue("EMP_ID"));
                param.Add("UPLOAD_SEQ", package.KeyValues.GetValue("UPLOAD_SEQ"));
                param.Add("ACCESS_POSSIBLE_YN", package.KeyValues.GetValue("ACCESS_POSSIBLE_YN"));
                param.Add("BP_LOW", package.KeyValues.GetValue("BP_LOW"));
                param.Add("BP_HIGHT", package.KeyValues.GetValue("BP_HIGHT"));
                param.Add("RMK", package.KeyValues.GetValue("RMK"));
                param.Add("ADDR", package.KeyValues.GetValue("ADDR"));                
                param.Add("BIRTH_DT", Regex.Replace(package.KeyValues.GetValue("BIRTH_DT"), @"\D", ""));
                


                // 체크1. 
                String chkPlantId = param.GetValue("PLANT_ID");
                String chkEmpId = param.GetValue("EMP_ID");
                Int32 ret = 0;

                if (chkPlantId.IsNotNullOrEmpty() && chkEmpId.IsNotNullOrEmpty())
                {
                    // 출입자  테이블 수정
                    ret += DataMapper.DefaultMapper.QueryForObject<Int32>("WEB020_UPDATE_EMP_DTL", param);                    
                }

                // 파일 저장                
                if (listDict.Count > 0)
                {
                    foreach (Dictionary<string, string> dict in listDict)
                    {
                        ret++;
                        dict.Add("Now", package.KeyValues.GetValue("Now"));
                        dict.Add("LoginedUserID", package.Token.GetUserIDFromToken());
                        if(dict.GetValue("FILE_APPR_YN").IsNotNullOrEmpty())
                            DataMapper.DefaultMapper.QueryForObject<Int32>("WEB020_UPDATE_EMP_FILE", dict);
                    }
                }

                returnPackage.KeyValues.Add("Method", "UPDATE");
                returnPackage.KeyValues.Add("Result", ret.ToString());
                returnPackage.KeyValues.Add("UPLOAD_SEQ", package.KeyValues.GetValue("UPLOAD_SEQ").ToString());
                returnPackage.KeyValues.Add("EMP_ID", package.KeyValues.GetValue("EMP_ID").ToString());
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
        /// 파일 다운로드
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public static DataPackage DeSerialize(DataPackage package) 
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                string fileName = package.KeyValues.GetValue("REAL_FILE_NM");
                string fileId = package.KeyValues.GetValue("FILE_ID");

                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WEB020_SELECT_DOWN_FILE", package.KeyValues);
                DataMapper.DefaultMapper.QueryForDataTable("WEB020_UPDATE_READ_YN", package.KeyValues);
                if (dataTable.Rows.Count > 0 )
                    returnPackage.KeyValues.Add("BASE64STR", dataTable.Rows[0]["UPLOAD_FILE"].ToString());
                returnPackage.KeyValues.Add("Method", "SELECT");
                returnPackage.KeyValues.Add("Result", dataTable.Rows.Count.ToString());
                returnPackage.JsonData = dataTable.ToJson<DataTable>();
            }            
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Method", "FILEDOWNLOAD");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }

            return returnPackage;
        }
        
        /// <summary>
        /// 제출서류 보완 SMS 발송
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage InsertSms(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                Dictionary<string, string> param = package.JsonData.ToDictionary();
                param.Add("Now", package.KeyValues.GetValue("Now"));
                param.Add("LoginedUserID", package.Token.GetUserIDFromToken());

                // String chkUploadSeq = package.KeyValues.GetValue("UPLOAD_SEQ");
                String empTelNo = param.GetValue("EMP_TEL_NO").Replace("-", "").ToString();
                if (param.GetValue("EMP_TEL_NO").IsNotNullOrEmpty())
                {
                    String phoneNumber  = param.GetValue("EMP_TEL_NO");
                    String smsMsg       = param.GetValue("SMS_MSG");

                    DataTable dataTableMngrTelNo = DataMapper.DefaultMapper.QueryForDataTable("WEB020_SELECT_MNGR_TEL_NO", param);

                    //WEB_SMS.sendSms(empTelNo, smsMsg, dataTableMngrTelNo.Rows[0]["PLNT_MNGR_TEL_NO"].ToString(), "0");
                        
                    // SMS_SND_DT update
                    DataMapper.DefaultMapper.QueryForObject<Int32>("WEB020_UPDATE_FILE_EMP_SMS_DT", param);                        
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

        public DataPackage ReSendSms(DataPackage package) 
        {
            Dictionary<string, string> param = package.JsonData.ToDictionary();
            DataPackage returnPackage = new DataPackage();
            try
            {   
                param.Add("Now", package.KeyValues.GetValue("Now"));
                param.Add("LoginedUserID", package.Token.GetUserIDFromToken());

                DataTable dataTableSMS = DataMapper.DefaultMapper.QueryForDataTable("WEB020_SELECT_SMS_SND_YN", param);

                if (dataTableSMS.Rows.Count > 0)
                {
                    String phoneNumber = dataTableSMS.Rows[0]["EMP_TEL_NO"].ToString();
                    String empNm = dataTableSMS.Rows[0]["EMP_NM"].ToString();
                    String plantNm = dataTableSMS.Rows[0]["PLANT_NM"].ToString();
                    String taSub = dataTableSMS.Rows[0]["TA_SUB"].ToString();
                    String crticNo = dataTableSMS.Rows[0]["CRTIC_NO"].ToString();

                    String msgTemplet = string.Format("[{0}] {1}님. [{2}]-[{3}] 출입 대상자로 등록되었습니다.", plantNm, empNm, plantNm, taSub); // 사업장, 이름, 사업장, 공사명                            
                    msgTemplet += "모바일 안전교육 사이트에 접속하여 사진 등록 및 안전교육 수강, 서류제출을 진행해주시기 바랍니다.";
                    msgTemplet += Environment.NewLine;
                    msgTemplet += "출입 전까지 각 단계를 완료하여 출입승인을 받으셔야 합니다.";
                    msgTemplet += Environment.NewLine;
                    msgTemplet += Environment.NewLine;
                    msgTemplet += "※ 로그인은 성명, 핸드폰번호, 인증번호로 진행하시면 됩니다.";
                    msgTemplet += Environment.NewLine;
                    msgTemplet += Environment.NewLine;
                    msgTemplet += string.Format("로그인 인증번호: {0}", crticNo);
                    msgTemplet += Environment.NewLine;
                    
                    msgTemplet += Environment.NewLine;
                    DataTable dataTablePath = DataMapper.DefaultMapper.QueryForDataTable("WEB020_SELECT_MANUAL_PATH", param);
                    // msgTemplet += "매뉴얼: " + dataTablePath.Rows[0]["PATH"].ToString();

                    DataTable dataTableMngrTelNo = DataMapper.DefaultMapper.QueryForDataTable("WEB020_SELECT_MNGR_TEL_NO", param);

                    // WEB_SMS.sendSms(phoneNumber, msgTemplet, dataTableMngrTelNo.Rows[0]["PLNT_MNGR_TEL_NO"].ToString(), "0");

                    // SMS_SND_DT update
                    DataMapper.DefaultMapper.QueryForObject<Int32>("WEB020_UPDATE_EMP_SMS_DT", param);
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

        public DataPackage ReSendSmsList(DataPackage package)
        {   
            DataPackage returnPackage = new DataPackage();
            List<Dictionary<string, string>> trgtTable = package.JsonData.ToDictionaryList();

            try
            {
                foreach (Dictionary<string, string> d in trgtTable)
                {
                    d.Add("LoginedUserID", package.Token.GetUserIDFromToken());                    

                    DataTable dataTableSMS = DataMapper.DefaultMapper.QueryForDataTable("WEB020_SELECT_SMS_SND_YN", d);

                    if (dataTableSMS.Rows.Count > 0)
                    {
                        String phoneNumber = dataTableSMS.Rows[0]["EMP_TEL_NO"].ToString();
                        String empNm = dataTableSMS.Rows[0]["EMP_NM"].ToString();
                        String plantNm = dataTableSMS.Rows[0]["PLANT_NM"].ToString();
                        String taSub = dataTableSMS.Rows[0]["TA_SUB"].ToString();
                        String crticNo = dataTableSMS.Rows[0]["CRTIC_NO"].ToString();

                        String msgTemplet = string.Format("[{0}] {1}님. [{2}]-[{3}] 출입 대상자로 등록되었습니다.", plantNm, empNm, plantNm, taSub); // 사업장, 이름, 사업장, 공사명                            
                        msgTemplet += "모바일 안전교육 사이트에 접속하여 사진 등록 및 안전교육 수강, 서류제출을 진행해주시기 바랍니다.";
                        msgTemplet += Environment.NewLine;
                        msgTemplet += "출입 전까지 각 단계를 완료하여 출입승인을 받으셔야 합니다.";
                        msgTemplet += Environment.NewLine;
                        msgTemplet += Environment.NewLine;
                        msgTemplet += "※ 로그인은 성명, 핸드폰번호, 인증번호로 진행하시면 됩니다.";
                        msgTemplet += Environment.NewLine;
                        msgTemplet += Environment.NewLine;
                        msgTemplet += string.Format("로그인 인증번호: {0}", crticNo);
                        msgTemplet += Environment.NewLine;
                        
                        msgTemplet += Environment.NewLine;
                        DataTable dataTablePath = DataMapper.DefaultMapper.QueryForDataTable("WEB020_SELECT_MANUAL_PATH", d);
                        msgTemplet += "매뉴얼: " + dataTablePath.Rows[0]["PATH"].ToString();

                        DataTable dataTableMngrTelNo = DataMapper.DefaultMapper.QueryForDataTable("WEB020_SELECT_MNGR_TEL_NO", d);

                        WEB_SMS.sendSms(phoneNumber, msgTemplet, dataTableMngrTelNo.Rows[0]["PLNT_MNGR_TEL_NO"].ToString(), "0");

                        // SMS_SND_DT update
                        DataMapper.DefaultMapper.QueryForObject<Int32>("WEB020_UPDATE_EMP_SMS_DT", d);
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
        /// 일괄 승인 처리
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage UpdateEmpDtlList(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            List<Dictionary<string, string>> trgtTable = package.JsonData.ToDictionaryList();
            Int32 ret = 0;
            try
            {
                foreach (Dictionary<string, string> d in trgtTable)
                {
                    d.Add("LoginedUserID", package.Token.GetUserIDFromToken());

                    List<Dictionary<string, string>> listDict = package.JsonData.ToDictionaryList();

                    Dictionary<string, string> param = new Dictionary<string, string>();                    

                    // 체크1. 
                    String chkPlantId   = d.GetValue("PLANT_ID");
                    String chkEmpId     = d.GetValue("EMP_ID");

                    ret += DataMapper.DefaultMapper.QueryForObject<Int32>("WEB020_UPDATE_EMP_DTL_LIST", d);
                }
                
                returnPackage.KeyValues.Add("Method", "UPDATE");
                returnPackage.KeyValues.Add("Result", ret.ToString());                
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Method", "INSERT or UPDATE");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        } 
    }
}