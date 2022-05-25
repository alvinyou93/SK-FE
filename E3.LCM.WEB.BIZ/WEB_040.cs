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

// 퀴즈관리
namespace E3.LCM.WEB.BIZ
{
    public class WEB_040    
    {
        public DataPackage GetQuizList(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                System.Data.DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WEB040_SELECT", package.KeyValues);
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

        public DataPackage GetEduDocList(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                System.Data.DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WEB040_EDU_DOC_SELECT", package.KeyValues);
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
        
        public DataPackage InsertQuizList(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                Dictionary<string, string> param = package.JsonData.ToDictionary();
                param.Add("Now", package.KeyValues.GetValue("Now"));
                param.Add("LoginedUserID", package.Token.GetUserIDFromToken());

                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WEB040_QUIZ_COUNT", param);

                if (dataTable.Rows[0]["CNT"].ToInt() > 0)
                {
                    Int32 ret = DataMapper.DefaultMapper.QueryForObject<Int32>("WEB040_UPDATE", param);
                    returnPackage.KeyValues.Add("Method", "UPDATE");
                    returnPackage.KeyValues.Add("Result", ret.ToString());
                }
                else
                {
                    Int32 ret = DataMapper.DefaultMapper.QueryForObject<Int32>("WEB040_INSERT", param);
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

        public DataPackage DeleteQuiz(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                Dictionary<string, string> param = package.JsonData.ToDictionary();
                param.Add("Now", package.KeyValues.GetValue("Now"));

                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WEB040_DELETE", param);
                returnPackage.KeyValues.Add("Method", "DELETE");
                returnPackage.KeyValues.Add("Result", dataTable.Rows.Count.ToString());
                returnPackage.JsonData = dataTable.ToJson<DataTable>();
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Method", "DELETE");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }

        public DataPackage InserQuizListExcel(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            List<Dictionary<string, string>> dataErrList = new List<Dictionary<string, string>>();
            DataTable dataErrTable = new DataTable();
            dataErrTable.Columns.Add("PLANT_ID", typeof(string));
            dataErrTable.Columns.Add("EDU_MTRL_ID", typeof(string));
            dataErrTable.Columns.Add("SDDN_QUIZ_TIME", typeof(string));
            dataErrTable.Columns.Add("SDDN_QUIZ_ORDR", typeof(string));
            dataErrTable.Columns.Add("QUIZ_SENTENCE", typeof(string));
            dataErrTable.Columns.Add("ANSWER_1", typeof(string));
            dataErrTable.Columns.Add("ANSWER_2", typeof(string));
            dataErrTable.Columns.Add("ANSWER_3", typeof(string));
            dataErrTable.Columns.Add("ANSWER_4", typeof(string));
            dataErrTable.Columns.Add("RIGHT_ANSWER", typeof(string));
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
                        if (err_type.Equals(""))
                        {
                            dict.Add("Now", package.KeyValues.GetValue("Now"));
                            dict.Add("LoginedUserID", package.Token.GetUserIDFromToken());

                            dict.Add("PLANT_ID", dict.GetValue("사업장"));
                            dict.Add("EDU_MTRL_ID", dict.GetValue("교육 동영상 ID"));
                            dict.Add("SDDN_QUIZ_TIME", dict.GetValue("돌발시점(초)"));
                            dict.Add("SDDN_QUIZ_ORDR", dict.GetValue("문제순번"));
                            dict.Add("QUIZ_SENTENCE", dict.GetValue("문제"));
                            dict.Add("ANSWER_1", dict.GetValue("선택지1"));
                            dict.Add("ANSWER_2", dict.GetValue("선택지2"));
                            dict.Add("ANSWER_3", dict.GetValue("선택지3"));
                            dict.Add("ANSWER_4", dict.GetValue("선택지4"));
                            dict.Add("RIGHT_ANSWER", dict.GetValue("정답"));

                            DataTable dataTableEduId = new DataTable();
                            // 유효성 체크 - 사업장별 교육 자료 존재 확인 todo  
                            if (dict.GetValue("교육 동영상 ID").IsNotNullOrEmpty())
                            {
                                dataTableEduId = DataMapper.DefaultMapper.QueryForDataTable("WEB040_EDU_COUNT", dict);
                            }
                            else {
                                if (err_type != "") err_type += ",";
                                err_type += "교육 동영상 ID 미입력";
                            }
                            
                            if (dict.GetValue("교육 동영상 ID").IsNotNullOrEmpty() && dataTableEduId.Rows[0]["CNT"].ToInt() == 0)
                            {
                                if (err_type != "") err_type += ",";
                                err_type += "사업장/교육 동영상 ID 확인필요";
                            }
                            else if (dict.GetValue("돌발시점(초)").IsNullOrEmpty())
                            {
                                if (err_type != "") err_type += ",";
                                err_type += "돌발시점(초) 미입력";
                            }
                            else if (dict.GetValue("문제순번").IsNullOrEmpty())
                            {
                                if (err_type != "") err_type += ",";
                                err_type += "문제순번 미입력";
                            }
                            else if (dict.GetValue("문제").IsNullOrEmpty())
                            {
                                if (err_type != "") err_type += ",";
                                err_type += "문제 미입력";
                            }
                            else if (dict.GetValue("선택지1").IsNullOrEmpty())
                            {
                                if (err_type != "") err_type += ",";
                                err_type += "선택지1 미입력";
                            }
                            else if (dict.GetValue("선택지2").IsNullOrEmpty())
                            {
                                if (err_type != "") err_type += ",";
                                err_type += "선택지2 미입력";
                            }
                            else if (dict.GetValue("선택지3").IsNullOrEmpty())
                            {
                                if (err_type != "") err_type += ",";
                                err_type += "선택지3 미입력";
                            }
                            else if (dict.GetValue("선택지4").IsNullOrEmpty())
                            {
                                if (err_type != "") err_type += ",";
                                err_type += "선택지4 미입력";
                            }
                            else if (dict.GetValue("정답").IsNullOrEmpty())
                            {
                                if (err_type != "") err_type += ",";
                                err_type += "정답 미입력";
                            }
                            else if(err_type == "")
                            {
                                DataMapper.DefaultMapper.QueryForObject<Int32>("WEB040_INSERT", dict);
                            }
                        }

                        if (err_type.IsNotNullOrEmpty())
                        {
                            DataRow errListDict = dataErrTable.NewRow();

                            errListDict.SetField("PLANT_ID", dict.GetValue("사업장"));
                            errListDict.SetField("EDU_MTRL_ID", dict.GetValue("교육 동영상 ID"));
                            errListDict.SetField("SDDN_QUIZ_TIME", dict.GetValue("돌발시점(초)"));
                            errListDict.SetField("SDDN_QUIZ_ORDR", dict.GetValue("문제순번"));
                            errListDict.SetField("QUIZ_SENTENCE", dict.GetValue("문제"));

                            errListDict.SetField("ANSWER_1", dict.GetValue("선택지1"));
                            errListDict.SetField("ANSWER_2", dict.GetValue("선택지2"));
                            errListDict.SetField("ANSWER_3", dict.GetValue("선택지3"));
                            errListDict.SetField("ANSWER_4", dict.GetValue("선택지4"));
                            errListDict.SetField("RIGHT_ANSWER", dict.GetValue("정답"));

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
    }
}
