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
    public class WRK400
    {
        public DataPackage getFacilityKnd(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WRK400_SELECT_TOTAL_CNT", package.KeyValues);

                returnPackage.KeyValues.Add("TOTAL_CNT", dataTable.Rows[0]["TOTAL_CNT"].ToString());
                returnPackage.KeyValues.Add("ING_RATE", dataTable.Rows[0]["ING_RATE"].ToString());
                returnPackage.KeyValues.Add("COMP_RATE", dataTable.Rows[0]["COMP_RATE"].ToString());

                DataTable dataTable2 = DataMapper.DefaultMapper.QueryForDataTable("WRK400_GET_FACILITY_KND", package.KeyValues);
                returnPackage.JsonData = dataTable2.ToJson<DataTable>();
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Method", "SELECT");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }

        public DataPackage getJobList(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WRK400_GET_JOB_LIST", package.KeyValues);

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

        public DataPackage selectMainList(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WRK400_SELECT", package.KeyValues);
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

                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WRK400_CHK_CNT", param);

                if (dataTable != null && dataTable.Rows[0]["CNT"].ToInt() > 0)
                {
                    returnPackage.ErrorMessage = "중복된 설비번호입니다. 다시 확인하여 주십시요.";
                    returnPackage.KeyValues.Add("Method", "INSERT");
                    returnPackage.KeyValues.Add("Result", (-1).ToString());

                    return returnPackage;
                }
                else
                {

                    ret = DataMapper.DefaultMapper.QueryForObject<Int32>("WRK400_INSERT_FACILITY_INFO", param);
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


                            ret = DataMapper.DefaultMapper.QueryForObject<Int32>("WRK400_INSERT_SIGN_SHEET", d);
                        }
                        else
                        {
                            ret = DataMapper.DefaultMapper.QueryForObject<Int32>("WRK400_DELETE_SIGN_SHEET", d);
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

                //                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WRK400_DELETE_FACILITY_INFO", param);
                Int32 ret1 = DataMapper.DefaultMapper.QueryForObject<Int32>("WRK400_DELETE_FACILITY_INFO", param);

                Int32 ret2 = DataMapper.DefaultMapper.QueryForObject<Int32>("WRK400_DELETE_SIGN_SHEET", param);

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

        public DataPackage InsertExcelList(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();


            try
            {
                Int32 ret = 0;
                List<Dictionary<string, string>> listDict = package.JsonData.ToDictionaryList();

                if (listDict.Count > 0)
                {
                    foreach (Dictionary<string, string> dict in listDict)
                    {
                        dict.Add("LOGINID", package.Token.GetUserIDFromToken());

                        dict.Add("PLANT_ID", dict.GetValue("사업장"));
                        dict.Add("TA_ID", dict.GetValue("정기보수"));
                        dict.Add("FACILITY_KND", dict.GetValue("설비종류"));
                        dict.Add("PROCESS_CD", dict.GetValue("공정"));
                        dict.Add("FACILITY_NO", dict.GetValue("설비번호"));
                        dict.Add("FACILITY_NM", dict.GetValue("상세설명"));

                        DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WRK400_CHK_CNT", dict);

                        /*if (!dict.GetValue("PLANT_ID").IsNotNullOrEmpty() &&
                            !dict.GetValue("TA_ID").IsNotNullOrEmpty() &&
                            !dict.GetValue("FACILITY_KND").IsNotNullOrEmpty() &&
                            !dict.GetValue("PROCESS_CD").IsNotNullOrEmpty() &&
                            !dict.GetValue("FACILITY_NO").IsNotNullOrEmpty() &&
                            dataTable.Rows[0]["CNT"].ToInt() == 0)
                        */
                        if (dict.GetValue("PLANT_ID").IsNotNullOrEmpty() &&
                            dict.GetValue("TA_ID").IsNotNullOrEmpty() &&
                            dict.GetValue("FACILITY_KND").IsNotNullOrEmpty() &&
                            dict.GetValue("PROCESS_CD").IsNotNullOrEmpty() &&
                            dataTable.Rows[0]["CNT"].ToInt() == 0)
                        {
                            DataMapper.DefaultMapper.QueryForObject<Int32>("WRK400_INSERT_MAIN_LIST", dict);
                        }
                            
                    }
                }

                returnPackage.KeyValues.Add("Method", "INSERT");
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
    }
}
