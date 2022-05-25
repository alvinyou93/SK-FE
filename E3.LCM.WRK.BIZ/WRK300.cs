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

// 출입회사 관리
namespace E3.LCM.WRK.BIZ
{
    public class WRK300
    {
        public DataPackage selectMainList(DataPackage package)
        {
            package.KeyValues.Add("LOGINID", package.Token.GetUserIDFromToken());
            DataPackage returnPackage = new DataPackage();
            try
            {
                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WRK300_SELECT", package.KeyValues);
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

        public DataPackage InserMainList(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            Int32 ret;
            try
            {
                Dictionary<string, string> param = package.JsonData.ToDictionary();
                param.Add("LOGINID", package.Token.GetUserIDFromToken());

                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WRK300_CHK_CNT", param);

                if (dataTable != null && dataTable.Rows[0]["CNT"].ToInt() > 0)
                {
                    returnPackage.ErrorMessage = "중복된 설비번호입니다. 다시 확인하여 주십시요.";
                    returnPackage.KeyValues.Add("Method", "INSERT");
                    returnPackage.KeyValues.Add("Result", (-1).ToString());

                    return returnPackage;
                }
                else
                {

                    ret = DataMapper.DefaultMapper.QueryForObject<Int32>("WRK300_INSERT_FACILITY_INFO", param);
                }

                returnPackage.KeyValues.Add("Method", "INSERT");
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

        /// <summary>
        /// 수정
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage UpdateMainList(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {

                Int32 ret = 0;

                foreach (Dictionary<string, string> d in package.JsonData.ToDictionaryList())
                {
                    String rowState = d.GetValue("ROW_STATE");
                    if (rowState.Equals("U"))
                    {
                        String ingStep = d.GetValue("ING_STEP");
                        String stepCd = d.GetValue("STEP" + ingStep + "_CD");
                        String status = d.GetValue("STEP" + ingStep + "_STATUS");

                        if (status.Equals("WY"))
                        {
                            d.Add("LOGINID", package.Token.GetUserIDFromToken());
                            d.Add("STEP_CD", stepCd);
                            d.Add("WK_STATUS", status);
                            d.Add("MNGR_ID", package.Token.GetUserIDFromToken());


                            ret = DataMapper.DefaultMapper.QueryForObject<Int32>("WRK300_INSERT_SIGN_SHEET", d);
                        }
                        else
                        {
                            ret = DataMapper.DefaultMapper.QueryForObject<Int32>("WRK300_DELETE_SIGN_SHEET", d);
                        }
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

        public DataPackage DeleteMainList(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                Dictionary<string, string> param = package.JsonData.ToDictionary();

                //                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WRK300_DELETE_FACILITY_INFO", param);
                Int32 ret1 = DataMapper.DefaultMapper.QueryForObject<Int32>("WRK300_DELETE_FACILITY_INFO", param);

                Int32 ret2 = DataMapper.DefaultMapper.QueryForObject<Int32>("WRK300_DELETE_SIGN_SHEET", param);

                returnPackage.KeyValues.Add("Method", "DELETE");
                returnPackage.KeyValues.Add("Result", ret1.ToString());

            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Method", "DELETE");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }

        public DataPackage CancelMainList(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                Int32 ret = 0;
                Dictionary<string, string> param = package.JsonData.ToDictionary();
                param.Add("LOGINID", package.Token.GetUserIDFromToken());

                //취소할 단계의 권한을 체크한다.
                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WRK300_CHECK_FACILITY_INFO", param);

                if (dataTable.Rows.Count > 0)
                {
                    if (dataTable.Rows[0]["AUTH_YN"].ToString() == "Y")
                    {
                        param.Add("STEP_CD", dataTable.Rows[0]["STEP_CD"].ToString());

                        ret = DataMapper.DefaultMapper.QueryForObject<Int32>("WRK300_DELETE_SIGN_SHEET_STEP", param);
                    }
                    else
                    {
                        returnPackage.ErrorMessage = "현재 단계를 취소할 권한이 없습니다. 다시 확인하여 주십시요.";
                        returnPackage.KeyValues.Add("Method", "CANCEL");
                        returnPackage.KeyValues.Add("Result", (-1).ToString());

                        return returnPackage;

                    }
                }

                returnPackage.KeyValues.Add("Method", "CANCEL");
                returnPackage.KeyValues.Add("Result", ret.ToString());

            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Method", "DELETE");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }

        public DataPackage InsertExcelList(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();

            DataTable errorTable = new DataTable();
            errorTable.Columns.Add("PLANT_ID", typeof(string));
            errorTable.Columns.Add("TA_ID", typeof(string));
            errorTable.Columns.Add("FACILITY_KND", typeof(string));
            errorTable.Columns.Add("PROCESS_CD", typeof(string));
            errorTable.Columns.Add("FACILITY_NO", typeof(string));
            errorTable.Columns.Add("FACILITY_NM", typeof(string));
            errorTable.Columns.Add("ERR_INFO", typeof(string));

            string userId = package.Token.GetUserIDFromToken();

            try
            {
                Int32 ret = 0;
                List<Dictionary<string, string>> listDict = package.JsonData.ToDictionaryList();

                if (listDict.Count > 0)
                {
                    foreach (Dictionary<string, string> dict in listDict)
                    {
                        string errInfo = "";
                        string checkYn;

                        string plantId = dict.GetValue("사업장");
                        string taId = dict.GetValue("정기보수");
                        string facilityKnd = dict.GetValue("설비종류");
                        string processCd = dict.GetValue("공정");
                        string facilityNo = dict.GetValue("설비번호");
                        string facilityNm = dict.GetValue("상세설명");

                        dict.Add("LOGINID", package.Token.GetUserIDFromToken());
                        dict.Add("PLANT_ID", plantId);
                        dict.Add("TA_ID", taId);
                        dict.Add("FACILITY_KND", facilityKnd);
                        dict.Add("PROCESS_CD", processCd);
                        dict.Add("FACILITY_NO", facilityNo);
                        dict.Add("FACILITY_NM", facilityNm);

                        //사업장 체크
                        if (plantId.IsNullOrEmpty())
                        {
                            errInfo += "사업장 필수, ";
                        }
                        else
                        {
                            checkYn = checkComCode(userId, "PLANTID", plantId);
                            if (checkYn.Equals("N"))
                            {
                                errInfo += "사업장 코드 오류(" + plantId + "), ";
                            }

                        }

                        //정기보수 체크
                        if (taId.IsNullOrEmpty())
                        {
                            errInfo += "정기보수 필수, ";
                        }
                        else
                        {
                            checkYn = checkComCode(userId, "TA_MNG", taId, "Y");
                            if (checkYn.Equals("N"))
                            {
                                errInfo += "정기보수 코드 오류(" + taId + "), ";
                            }
                        }

                        //설비종류 체크
                        if (facilityKnd.IsNullOrEmpty())
                        {
                            errInfo += "설비종류 필수, ";
                        }
                        else
                        {
                            checkYn = checkComCode(userId, "FACT_KND", facilityKnd, "Y");
                            if (checkYn.Equals("N"))
                            {
                                errInfo += "설비종류 코드 오류(" + facilityKnd + "), ";
                            }

                        }

                        //공정 체크
                        if (processCd.IsNullOrEmpty())
                        {
                            errInfo += "공정 필수, ";
                        }
                        else
                        {
                            checkYn = checkComCode(userId, "PROCESS_CD", processCd, "Y");
                            if (checkYn.Equals("N"))
                            {
                                errInfo += "공정 코드 오류(" + processCd + "), ";
                            }

                        }

                        //설비번호 체크
                        if (facilityNo.IsNullOrEmpty())
                        {
                            errInfo += "설비번호 필수, ";
                        }

                        //중복체크
                        DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WRK300_CHK_CNT", dict);
                        if (dataTable.Rows[0]["CNT"].ToInt() > 0)
                        {
                            errInfo += "중복 오류 ";
                        }

                        if (errInfo.IsNullOrEmpty())
                        {
                            DataMapper.DefaultMapper.QueryForObject<Int32>("WRK300_INSERT_FACILITY_INFO", dict);
                        }
                        else
                        {
                            DataRow drError = errorTable.NewRow();
                            drError["PLANT_ID"] = plantId;
                            drError["TA_ID"] = taId;
                            drError["FACILITY_KND"] = facilityKnd;
                            drError["PROCESS_CD"] = processCd;
                            drError["FACILITY_NO"] = facilityNo;
                            drError["FACILITY_NM"] = facilityNm;
                            drError["ERR_INFO"] = errInfo;

                            errorTable.Rows.Add(drError);
                        }
                    }
                }
                if (errorTable.Rows.Count > 0)
                {
                    returnPackage.KeyValues.Add("Method", "ERROR");
                    returnPackage.JsonData = errorTable.ToJson<DataTable>();
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

        public string checkComCode(string loginId, string parentId, string codeId, string plantCheck = "")
        {
            string returnValue = "Y";

            DataPackage param = new DataPackage();

            param.KeyValues.Add("LOGINID", loginId);
            param.KeyValues.Add("PARENT_ID", parentId);
            param.KeyValues.Add("CODE_ID", codeId);
            param.KeyValues.Add("PLANT_CHECK", plantCheck);

            DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WRK300_CHECK_COMCODE", param.KeyValues);

            if (dataTable != null && dataTable.Rows[0]["CNT"].ToInt() > 0)
            {
                returnValue = "Y";
            }
            else
            {
                returnValue = "N";
            }
            return returnValue;
        }
    }
}
