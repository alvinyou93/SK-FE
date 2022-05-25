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
    public class WEB_060
    {
        public DataPackage GetDocList(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WEB060_SELECT", package.KeyValues);
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

        // popup
        public DataPackage GetEmpDtlList(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WEB060_SELECT_EMP_FILE", package.KeyValues);
                DataTable dataTable2 = DataMapper.DefaultMapper.QueryForDataTable("WEB060_SELECT_EMP_DTL", package.KeyValues);

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
                param.Add("BIRTH_DT", package.KeyValues.GetValue("BIRTH_DT"));

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
                        DataMapper.DefaultMapper.QueryForObject<Int32>("WEB020_UPDATE_EMP_FILE", dict);
                    }
                }
                returnPackage.KeyValues.Add("Method", "UPDATE");
                returnPackage.KeyValues.Add("Result", ret.ToString());
                returnPackage.KeyValues.Add("UPLOAD_SEQ", package.KeyValues.GetValue("UPLOAD_SEQ").ToString());
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Method", "INSERT or UPDATE");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }

        public static DataPackage DeSerialize(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                string fileName = package.KeyValues.GetValue("REAL_FILE_NM");
                byte[] serializedFile = Encoding.UTF8.GetBytes(package.KeyValues.GetValue("UPLOAD_FILE"));

                //string pathString = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                // string pathString = @"c:\\Windows\temp"; 
                string pathString = @"C:\\E3_COMPONENTS\\Files\\Uploads";
                pathString = System.IO.Path.Combine(pathString, fileName);

                using (System.IO.FileStream reader = System.IO.File.Create(pathString))
                {
                    byte[] buffer = Convert.FromBase64String(package.KeyValues.GetValue("UPLOAD_FILE"));
                    reader.Write(buffer, 0, buffer.Length);
                }

                returnPackage.KeyValues.Add("Method", "FILEDOWNLOAD");
                returnPackage.KeyValues.Add("Result", "1");
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Method", "FILEDOWNLOAD");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }

            return returnPackage;
        }
    }

}

