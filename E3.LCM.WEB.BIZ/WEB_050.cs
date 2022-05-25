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

// 시험문제 관리
namespace E3.LCM.WEB.BIZ
{
    public class WEB_050
    {
        public DataPackage GetExamList(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                System.Data.DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WEB050_SELECT", package.KeyValues);
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

        public DataPackage InsertExamList(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                Dictionary<string, string> param = package.JsonData.ToDictionary();
                param.Add("Now", package.KeyValues.GetValue("Now"));
                param.Add("LoginedUserID", package.Token.GetUserIDFromToken());

                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WEB050_EXAM_COUNT", param);

                if (dataTable.Rows[0]["CNT"].ToInt() > 0)
                {
                    Int32 ret = DataMapper.DefaultMapper.QueryForObject<Int32>("WEB050_UPDATE", param);
                    returnPackage.KeyValues.Add("Method", "UPDATE");
                    returnPackage.KeyValues.Add("Result", ret.ToString());
                }
                else
                {
                    Int32 ret = DataMapper.DefaultMapper.QueryForObject<Int32>("WEB050_INSERT", param);
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

        public DataPackage DeleteExam(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            try
            {
                Dictionary<string, string> param = package.JsonData.ToDictionary();
                param.Add("Now", package.KeyValues.GetValue("Now"));

                DataTable dataTable = DataMapper.DefaultMapper.QueryForDataTable("WEB050_DELETE", param);
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

        public DataPackage InserExamListExcel(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            List<Dictionary<string, string>> dataErrList = new List<Dictionary<string, string>>();
            DataTable dataErrTable = new DataTable();
            dataErrTable.Columns.Add("PLANT_ID", typeof(string));
            dataErrTable.Columns.Add("EXAM_TYPE", typeof(string));            
            dataErrTable.Columns.Add("EXAM_SENTENCE", typeof(string));
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
                            dict.Add("EXAM_TYPE", dict.GetValue("문제유형"));                            
                            dict.Add("EXAM_SENTENCE", dict.GetValue("문제"));
                            dict.Add("ANSWER_1", dict.GetValue("선택지1"));
                            dict.Add("ANSWER_2", dict.GetValue("선택지2"));
                            dict.Add("ANSWER_3", dict.GetValue("선택지3"));
                            dict.Add("ANSWER_4", dict.GetValue("선택지4"));
                            dict.Add("RIGHT_ANSWER", dict.GetValue("정답"));

                            if (dict.GetValue("사업장").IsNullOrEmpty())
                            {
                                if (err_type != "") err_type += ",";
                                err_type += "사업장 미입력";
                            }
                            else if (dict.GetValue("문제유형").IsNullOrEmpty())
                            {
                                if (err_type != "") err_type += ",";
                                err_type += "문제유형 미입력";
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
                            else
                            {
                                DataMapper.DefaultMapper.QueryForObject<Int32>("WEB050_INSERT", dict);
                            }
                        }

                        if (err_type.IsNotNullOrEmpty())
                        {
                            DataRow errListDict = dataErrTable.NewRow();

                            errListDict.SetField("PLANT_ID", dict.GetValue("사업장"));
                            errListDict.SetField("EXAM_TYPE", dict.GetValue("문제유형"));                            
                            errListDict.SetField("EXAM_SENTENCE", dict.GetValue("문제"));

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
