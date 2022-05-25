using E3.Common;
using E3.Common.IBatis;
using E3.Common.Supports;
using E3.Common.Wcf;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace E3.LCM.MBL.BIZ
{
    public class MAPI100
    {
        #region 안전교육 대상자 정보 관련 함수
        /// <summary>
        /// 사용자 정보를 가져온다.
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage SelectAccountInfo(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            returnPackage.KeyValues.Add("Method", "SELECT_ACCOUNT_INFO");
            try
            {
                string[] useridkey = package.Token.GetUserIDFromToken().Split('-');
                package.KeyValues.Add("EMP_ID", useridkey[1]);
                DataTable dataTable = MAPIBase.DefaultMapper().QueryForDataTable("MAPI100_SELECT_ACCOUNT_INFO", package.KeyValues);
                returnPackage.JsonData = dataTable.ToJson<DataTable>();
                returnPackage.KeyValues.Add("Result", dataTable.Rows.Count.ToString());
                returnPackage.KeyValues.Add("ToTalCount", dataTable.Rows.Count.ToString());
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }

        /// <summary>
        /// 안전교육 대상자의 소속업체 정보 관련 함수
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage SelectAccountCompany(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            returnPackage.KeyValues.Add("Method", "SELECT_ACCOUNT_COMPANY");
            try
            {
                string[] useridkey = package.Token.GetUserIDFromToken().Split('-');
                package.KeyValues.Add("EMP_ID", useridkey[1]);
                DataTable dataTable = MAPIBase.DefaultMapper().QueryForDataTable("MAPI100_SELECT_ACCOUNT_COMPANY", package.KeyValues);
                returnPackage.JsonData = dataTable.ToJson<DataTable>();
                returnPackage.KeyValues.Add("Result", dataTable.Rows.Count.ToString());
                returnPackage.KeyValues.Add("ToTalCount", dataTable.Rows.Count.ToString());
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }

        /// <summary>
        /// 개인정보 처리방침 동의.
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage Update4PrivacyAllow(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            returnPackage.KeyValues.Add("Method", "ALLOW_PRIVACY");
            try
            {
                string[] useridkey = package.Token.GetUserIDFromToken().Split('-');
                package.KeyValues.Add("EMP_ID", useridkey[1]);
                Int32 ret = MAPIBase.DefaultMapper().QueryForObject<Int32>("MAPI100_ALLOW_PRIVACY", package.KeyValues);
                returnPackage.KeyValues.Add("CODE", "200");
                returnPackage.KeyValues.Add("Result", "OK");
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("CODE", "500");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }

        /// <summary>
        /// 제출된 파일과 승인 정보 출력
        /// ACHK - 승인된 정보 (1) else 0
        /// FCHK - 업로드된 정보 (1) else 0
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage SelectAllowFileCheck(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            returnPackage.KeyValues.Add("Method", "ALLOW_FILES");
            try
            {
                string[] useridkey = package.Token.GetUserIDFromToken().Split('-');
                package.KeyValues.Add("EMP_ID", useridkey[1]);
                DataTable dataTable = MAPIBase.DefaultMapper().QueryForDataTable("MAPI100_ALLOW_FILES", package.KeyValues);
                returnPackage.JsonData = dataTable.ToJson<DataTable>();
                returnPackage.KeyValues.Add("Result", dataTable.Rows.Count.ToString());
                returnPackage.KeyValues.Add("ToTalCount", dataTable.Rows.Count.ToString());
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }
        #endregion

        #region 사용자 증명사진 정보를 업데이트 한다.
        public DataPackage Update4FacePhoto(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            returnPackage.KeyValues.Add("Method", "ALLOW_FACE_PHOTO");
            try
            {
                string[] useridkey = package.Token.GetUserIDFromToken().Split('-');
                package.KeyValues.Add("EMP_ID", useridkey[1]);
                Int32 ret = MAPIBase.DefaultMapper().QueryForObject<Int32>("MAPI100_ALLOW_FACE_PHOTO", package.KeyValues);
                returnPackage.KeyValues.Add("CODE", "200");
                returnPackage.KeyValues.Add("Result", "OK");
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("CODE", "500");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }
        #endregion

        #region 안전 교육 수강 관련 함수
        /// <summary>
        /// 안전 교육 과목 정보를 가져온다.
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage SelectSafeSubject(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            returnPackage.KeyValues.Add("Method", "SAFE_SUBJECT");
            try
            {
                string[] useridkey = package.Token.GetUserIDFromToken().Split('-');
                package.KeyValues.Add("EMP_ID", useridkey[1]);
                DataTable dataTable = MAPIBase.DefaultMapper().QueryForDataTable("MAPI100_SAFE_SUBJECT", package.KeyValues);
                returnPackage.JsonData = dataTable.ToJson<DataTable>();
                returnPackage.KeyValues.Add("Result", dataTable.Rows.Count.ToString());
                returnPackage.KeyValues.Add("ToTalCount", dataTable.Rows.Count.ToString());
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }

        /// <summary>
        /// 안전 교육 과목이 동영상일 경우 돌발퀴즈를 가져온다.
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage SelectSafeSubjectQuiz(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            returnPackage.KeyValues.Add("Method", "SAFE_SUBJECT_QUIZ");
            try
            {
                string[] useridkey = package.Token.GetUserIDFromToken().Split('-');
                package.KeyValues.Add("EMP_ID", useridkey[1]);
                DataTable dataTable = MAPIBase.DefaultMapper().QueryForDataTable("MAPI100_SAFE_SUBJECT_QUIZ", package.KeyValues);
                returnPackage.JsonData = dataTable.ToJson<DataTable>();
                returnPackage.KeyValues.Add("Result", dataTable.Rows.Count.ToString());
                returnPackage.KeyValues.Add("ToTalCount", dataTable.Rows.Count.ToString());
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }

        /// <summary>
        /// 안전 교육 과목 이수를 완료하였는지 정보를 업데이트 한다.
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage Update4SafeSubjectResult(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            returnPackage.KeyValues.Add("Method", "ALLOW_SAFE_SUBJECT");
            try
            {
                string[] useridkey = package.Token.GetUserIDFromToken().Split('-');
                package.KeyValues.Add("EMP_ID", useridkey[1]);
                Int32 ret = MAPIBase.DefaultMapper().QueryForObject<Int32>("MAPI100_ALLOW_SAFE_SUBJECT", package.KeyValues);
                returnPackage.KeyValues.Add("CODE", "200");
                returnPackage.KeyValues.Add("Result", "OK");
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("CODE", "500");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }
        #endregion

        #region 안전 교육 평가 관련 함수
        /// <summary>
        /// 안전 교육 평가 문제를 10문항을 가져온다.
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage SelectExamSentence(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            returnPackage.KeyValues.Add("Method", "EXAM_SENTENCE");
            try
            {
                string[] useridkey = package.Token.GetUserIDFromToken().Split('-');
                package.KeyValues.Add("EMP_ID", useridkey[1]);
                DataTable dataTable = MAPIBase.DefaultMapper().QueryForDataTable("MAPI100_EXAM_SENTENCE", package.KeyValues);
                returnPackage.JsonData = dataTable.ToJson<DataTable>();
                returnPackage.KeyValues.Add("Result", dataTable.Rows.Count.ToString());
                returnPackage.KeyValues.Add("ToTalCount", dataTable.Rows.Count.ToString());
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }

        /// <summary>
        /// 안전 교육 평가 결과를 사용자 테이블에 저장
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage Update4ExamSentenceResult(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            returnPackage.KeyValues.Add("Method", "ALLOW_EXAM_SENTENCE");
            try
            {
                string[] useridkey = package.Token.GetUserIDFromToken().Split('-');
                package.KeyValues.Add("EMP_ID", useridkey[1]);

                if (package.KeyValues["PASS_YN"].ToString().ToUpper().Equals("Y"))
                {
                    Int32 ret = MAPIBase.DefaultMapper().QueryForObject<Int32>("MAPI100_ALLOW_EXAM_SENTENCE", package.KeyValues);
                }
                else 
                {
                    Int32 ret = MAPIBase.DefaultMapper().QueryForObject<Int32>("MAPI100_DISALLOW_EXAM_SENTENCE", package.KeyValues);
                }
                returnPackage.KeyValues.Add("CODE", "200");
                returnPackage.KeyValues.Add("Result", "OK");
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("CODE", "500");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }
        #endregion

        #region 안전교육 사진 및 첨부 서류 관련 함수

        /// <summary>
        /// 줄입 사용자(작업자) 업로드 파일 유형정보를 가져온다.
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage getUloadFileType(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            returnPackage.KeyValues.Add("Method", "SELECT_FILES_TYPE");
            try
            {
                string[] useridkey = package.Token.GetUserIDFromToken().Split('-');
                package.KeyValues.Add("EMP_ID", useridkey[1]);
                DataTable dataTable = MAPIBase.DefaultMapper().QueryForDataTable("MAPI100_SELECT_FILES_TYPE", package.KeyValues);
                returnPackage.JsonData = dataTable.ToJson<DataTable>();
                returnPackage.KeyValues.Add("CODE", "200");
                returnPackage.KeyValues.Add("Result", "OK");
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("CODE", "500");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }

        /// <summary>
        /// 출입 사용자(작업자) 유형의 필수 첨부파일을 업로드 한다.
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage setUloadFile(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            returnPackage.KeyValues.Add("Method", "INSERT_FILES_TYPE");
            try
            {
                string[] useridkey = package.Token.GetUserIDFromToken().Split('-');
                package.KeyValues.Add("EMP_ID", useridkey[1]);
                Int32 ret = MAPIBase.DefaultMapper().QueryForObject<Int32>("MAPI100_INSERT_FILES_TYPE", package.KeyValues);
                returnPackage.KeyValues.Add("CODE", "200");
                returnPackage.KeyValues.Add("Result", "OK");
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("CODE", "500");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }

        /// <summary>
        /// 저장된 필수 서류 삭제
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage DeleteFile(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            returnPackage.KeyValues.Add("Method", "DELETE_FILE_TYPE");
            try
            {
                string[] useridkey = package.Token.GetUserIDFromToken().Split('-');
                package.KeyValues.Add("EMP_ID", useridkey[1]);
                Int32 ret = MAPIBase.DefaultMapper().QueryForObject<Int32>("MAPI100_DELETE_FILE_TYPE", package.KeyValues);
                returnPackage.KeyValues.Add("CODE", "200");
                returnPackage.KeyValues.Add("Result", "OK");
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("CODE", "500");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }

        /// <summary>
        /// 출입 사용자(작업자)가 업로드한 파일정보를 가져온다.
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage SelectUploadFiles(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            returnPackage.KeyValues.Add("Method", "SELECT_FILES");
            try
            {
                string[] useridkey = package.Token.GetUserIDFromToken().Split('-');
                package.KeyValues.Add("EMP_ID", useridkey[1]);
                DataTable dataTable = MAPIBase.DefaultMapper().QueryForDataTable("MAPI100_SELECT_FILES", package.KeyValues);
                returnPackage.JsonData = dataTable.ToJson<DataTable>();
                returnPackage.KeyValues.Add("CODE", "200");
                returnPackage.KeyValues.Add("Result", "OK");
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("CODE", "500");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }

        /// <summary>
        /// 파일 다운로드 처리
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage DownloadFile (DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            returnPackage.KeyValues.Add("Method", "SELECT_FILE_DOWNLOAD");
            try
            {
                string[] useridkey = package.Token.GetUserIDFromToken().Split('-');
                package.KeyValues.Add("EMP_ID", useridkey[1]);
                DataTable dataTable = MAPIBase.DefaultMapper().QueryForDataTable("MAPI100_SELECT_FILE_DOWNLOAD", package.KeyValues);
                returnPackage.JsonData = dataTable.ToJson<DataTable>();
                returnPackage.KeyValues.Add("CODE", "200");
                returnPackage.KeyValues.Add("Result", "OK");
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("CODE", "500");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }
        #endregion

        #region 사진 파일 처리 로직 
        /// <summary>
        /// 모바일 기긱로부터 사용자의 사진정보를 받아 저장한다.
        /// </summary>
        /// <param name="package"></param>
        /// <returns>DataPackage</returns>
        public DataPackage toImage(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            returnPackage.KeyValues.Add("Method", "toImage");

            try
            {
                string[] useridkey = package.Token.GetUserIDFromToken().Split('-');
                SE_TA_EMP_Model userInfo = getEMPUser(useridkey[1]);    // 사용자정보 및 Device 목록 정보를 가져온다.

                string imageSource = package.KeyValues["img"];
                string imageType = imageSource.Split(',')[0];
                if (imageType.IndexOf("image") > 0)
                {
                    // 안면인식기에 이미지 판독 요청
                    List<CMV_Model_User> cmiduser = new List<CMV_Model_User>();
                    returnBase64Image base64String = MAPIBase.ConvertFile(imageSource, userInfo, out cmiduser);
                    string encryptionString = MAPIBase.Encrypt(base64String.Thumbnail);
                    package.KeyValues.Add("ID_PHOTO", encryptionString);
                    package.KeyValues.Add("OPTIONDATAS", base64String.option_datas);
                    package.KeyValues.Add("EMP_ID", userInfo.EMP_ID.ToString());

                    // 안면인식기에 정보 등록
                    var reagiste4Gate = ifFaceDetect4UserSend(userInfo, cmiduser, base64String.option_datas);

                    Int32 ret = MAPIBase.DefaultMapper().QueryForObject<Int32>("MAPI100_ALLOW_FACE_PHOTO", package.KeyValues);
                    returnPackage.KeyValues.Add("CODE", "200");
                    returnPackage.KeyValues.Add("Result", (1).ToString());
                    returnPackage.KeyValues.Add("Message", "정상 등록되었습니다.");

                    CMV_Model_Faces option_data = new CMV_Model_Faces();
                    option_data = Newtonsoft.Json.JsonConvert.DeserializeObject<CMV_Model_Faces>(base64String.option_datas);
                    /*
                    if (base64String != null && option_data != null && !string.IsNullOrEmpty(option_data.face_feature))
                    {
                        string encryptionString = MAPIBase.Encrypt(base64String.Thumbnail);
                        package.KeyValues.Add("ID_PHOTO", encryptionString);
                        package.KeyValues.Add("OPTIONDATAS", base64String.option_datas);
                        package.KeyValues.Add("EMP_ID", userInfo.EMP_ID.ToString());

                        // 안면인식기에 정보 등록
                        var reagiste4Gate = ifFaceDetect4UserSend(userInfo, cmiduser, base64String.option_datas);

                        Int32 ret = MAPIBase.DefaultMapper().QueryForObject<Int32>("MAPI100_ALLOW_FACE_PHOTO", package.KeyValues);
                        returnPackage.KeyValues.Add("CODE", "200");
                        returnPackage.KeyValues.Add("Result", (1).ToString());
                        returnPackage.KeyValues.Add("Message", "정상 등록되었습니다.");
                        
                        if (reagiste4Gate.result)
                        {
                            Int32 ret = MAPIBase.DefaultMapper().QueryForObject<Int32>("MAPI100_ALLOW_FACE_PHOTO", package.KeyValues);
                            returnPackage.KeyValues.Add("CODE", "200");
                            returnPackage.KeyValues.Add("Result", (1).ToString());
                            returnPackage.KeyValues.Add("Message", "정상 등록되었습니다.");
                        }
                        else
                        {
                            returnPackage.KeyValues.Add("CODE", "531");
                            returnPackage.KeyValues.Add("Result", (-1).ToString());
                            returnPackage.KeyValues.Add("Message", string.Format("인식기에 정보를 등록하지 못하였습니다. 잠시 후 다시 시도해 주세요!{0}", reagiste4Gate.result_desc));
                        }
                    }
                    else 
                    {
                        CMV_Model_Faces_Error errorcheck = new CMV_Model_Faces_Error();
                        errorcheck = Newtonsoft.Json.JsonConvert.DeserializeObject<CMV_Model_Faces_Error>(base64String.option_datas);

                        if (base64String.DeviceCount == 0)
                        {
                            returnPackage.KeyValues.Add("CODE", "521");
                            returnPackage.KeyValues.Add("Result", (-1).ToString());
                            returnPackage.KeyValues.Add("Message", string.Format("안면인식기 장치가 설치되어 있지 않습니다. 장치를 설치 후 진행해 주세요!({0})", errorcheck.error_message));
                        } 
                        else 
                        {
                            returnPackage.KeyValues.Add("CODE", "523");
                            returnPackage.KeyValues.Add("Result", (-1).ToString());
                            returnPackage.KeyValues.Add("Message", string.Format("인식할 수 없는 이미지 입니다. 다른 이미지 또는 재촬영을 해 주세요!({0})", errorcheck.error_message));
                        }
                    }
                    */    
                }
                else {
                    returnPackage.KeyValues.Add("CODE", "511");
                    returnPackage.KeyValues.Add("Result", (-1).ToString());
                    returnPackage.KeyValues.Add("Message", "이미지 파일이 아닙니다. 이미지를 첨부하세요!");
                }
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("CODE", "500");
                returnPackage.KeyValues.Add("Result", (-1).ToString());
                returnPackage.KeyValues.Add("Message", e.Message);
            }
            return returnPackage;
        }

        /// <summary>
        /// 저장된 사용자의 썸네일 사진을 가져온다.
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage fromImage(DataPackage package) 
        {
            DataPackage returnPackage = new DataPackage();
            returnPackage.KeyValues.Add("Method", "fromImage");
            returnPackage.JsonData = "";
            try
            {
                string[] useridkey = package.Token.GetUserIDFromToken().Split('-');
                package.KeyValues.Add("EMP_ID", useridkey[1]);

                // Database Image
                DataTable dataTable = MAPIBase.DefaultMapper().QueryForDataTable("MAPI100_SELECT_FACE_PHOTO", package.KeyValues);
                if (dataTable.Rows.Count > 0 && dataTable.Rows[0][0].ToString().Length > 10)
                {
                    string cryptionString = dataTable.Rows[0][0].ToString();
                    returnPackage.JsonData = string.Format("data:image/jpeg;base64,{0}", MAPIBase.Decrypt(cryptionString));
                }
                // Thumbnail Image
                // returnPackage.JsonData = MAPIBase.readThumbnail(package.Token.GetUserIDFromToken());
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }
            return returnPackage;
        }
        #endregion

        #region 안면인식 시스템 인터페이스
        /// <summary>
        /// 사용자 정보를 안면인식기에 등록 및 변경 요청한다.
        /// </summary>
        /// <param name="EMP_ID"></param>
        /// <returns></returns>
        public CMV_Model_User_Action_Result ifFaceDetect4UserSend(SE_TA_EMP_Model userInfo, List<CMV_Model_User> cmiduser, string base64String_option_datas = "") {
            CMV_Model_User_Action_Result result = new CMV_Model_User_Action_Result();
            result.user_id = userInfo.EMP_ID.ToString();
            try
            {
                // 사용자 이름이 존재하면 작업을 실행한다.
                if (!string.IsNullOrEmpty(userInfo.EMP_NM)) 
                {
                    var client = new E3.LCM.MBL.BIZ.MAPIWebClient();

                    string routerPath = string.Format("/1.0/users");
                    CMV_Model_User p = new CMV_Model_User();
                    p.user_id = userInfo.EMP_ID.ToString(); // row["EMP_ID"].ToString(); <--- row["EMP_TEL_NO"].ToString(); // 사용자 출입 추적이 어렵다고 함
                    p.user_name = userInfo.EMP_NM;
                    p.cell_phone = userInfo.EMP_TEL_NO;

                    p.date_of_in = DateTime.Now.ToString("yyyyMMdd");

                    if (userInfo.ACCESS_POSSIBLE_YN.ToUpper().Equals("APPR_Y")) 
                    {
                        p.auth_start_dtm = DateTime.Now.ToString("yyyyMMdd") + "0000";
                        p.auth_end_dtm = DateTime.Now.AddDays(180).ToString("yyyyMMdd") + "2359";

                        // privileges - 디바이스 권한 추가한다.
                        if (userInfo.Devices.Count > 0)
                        {
                            foreach (string d in userInfo.Devices)
                            {
                                p.option_datas.privileges.Add(d);
                            }
                        }
                    }

                    // 안면 인식 데이타를 추가한다.
                    CMV_Model_Faces faces = new CMV_Model_Faces();
                    if (string.IsNullOrEmpty(base64String_option_datas))
                    {
                        // 안면인시한 데이타가 존재하지 않는다면 기존의 자료가 존재하면 사용한다.
                        if(cmiduser.Count > 0 && cmiduser[0].option_datas.faces.Count > 0)
                        {
                            faces = cmiduser[0].option_datas.faces[0];
                        }
                        if (!string.IsNullOrEmpty(userInfo.OPTIONDATAS))
                        {
                            faces = Newtonsoft.Json.JsonConvert.DeserializeObject<CMV_Model_Faces>(userInfo.OPTIONDATAS);
                        }
                    }
                    else 
                    {
                        faces = Newtonsoft.Json.JsonConvert.DeserializeObject<CMV_Model_Faces>(base64String_option_datas);
                    }
                    p.option_datas.faces.Add(faces);


                    // CMID 사용자 정보를 저장
                    // 기존의 데이타를 지우고 왔음으로 존재여부를 확인하지 않는다.
                    // POST
                    // var r3 = client.callWebRequest<CMV_Model_User>(routerPath, "POST", p);
                    var r3 = string.Empty;
                    result = Newtonsoft.Json.JsonConvert.DeserializeObject<CMV_Model_User_Action_Result>(r3);
                }
            }
            catch (Exception e) 
            {
                e.Data.Clear();
            }

            return result;
        }

        /// <summary>
        /// 안면인식기 프로그램과 API 테스트
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public DataPackage IfFaceDetectTest(DataPackage package)
        {
            DataPackage returnPackage = new DataPackage();
            returnPackage.KeyValues.Add("Method", "INTERFACE_FACE_DETECT_GET_CODE");
            try
            {
                var client = new E3.LCM.MBL.BIZ.MAPIWebClient();

                Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                //keyValuePairs.Add("user_id", "admin");

                //var ret = client.callWebRequest("/1.0/codes", "GET", keyValuePairs);
                var ret = string.Empty;

                returnPackage.JsonData = ret;
                returnPackage.KeyValues.Add("Result", "OK");
                returnPackage.KeyValues.Add("ToTalCount", (1).ToString());
            }
            catch (Exception e)
            {
                returnPackage.ErrorMessage = e.Message;
                returnPackage.KeyValues.Add("Result", (-1).ToString());
            }

            return returnPackage;
        }
        #endregion


        #region
        public SE_TA_EMP_Model getEMPUser(string EMP_ID)
        {
            SE_TA_EMP_Model EMPUser = new SE_TA_EMP_Model();

            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            keyValuePairs.Add("EMP_ID", EMP_ID);
            DataTable dataTable = MAPIBase.DefaultMapper().QueryForDataTable("MAPI100_SELECT_USER", keyValuePairs); // 사용자 정보를 가져온다.

            // 사용자가 존재하면 조회한다.
            if (dataTable.Rows.Count > 0)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    EMPUser.EMP_ID = int.Parse(row["EMP_ID"].ToString());
                    EMPUser.PLANT_ID = row["PLANT_ID"].ToString();
                    EMPUser.EMP_NM = row["EMP_NM"].ToString();
                    EMPUser.EMP_TEL_NO = row["EMP_TEL_NO"].ToString();
                    EMPUser.ACCESS_GB = row["ACCESS_GB"].ToString();
                    EMPUser.SAFE_EDU_CMPLT_YN = row["SAFE_EDU_CMPLT_YN"].ToString();
                    //EMPUser.SAFE_EDU_CMPLT_DT =Convert.ToDateTime(row["SAFE_EDU_CMPLT_DT"].ToString());
                    EMPUser.PASS_YN = row["PASS_YN"].ToString();
                    //EMPUser.PASS_DT = Convert.ToDateTime(row["PASS_DT"].ToString());
                    //EMPUser.PRIVACY_CONSENT_DT = Convert.ToDateTime(row["PRIVACY_CONSENT_DT"].ToString());
                    //EMPUser.SMS_SND_DT = Convert.ToDateTime(row["SMS_SND_DT"].ToString());
                    EMPUser.BLOCK_YN = row["BLOCK_YN"].ToString();
                    EMPUser.ACCESS_POSSIBLE_YN = row["ACCESS_POSSIBLE_YN"].ToString();
                    //EMPUser.ACCESS_POSSIBLE_DT = Convert.ToDateTime(row["ACCESS_POSSIBLE_DT"].ToString());
                    EMPUser.ADDR = row["ADDR"].ToString();
                    EMPUser.BIRTH_DT = row["BIRTH_DT"].ToString();
                    EMPUser.OPTIONDATAS = row["OPTIONDATAS"].ToString();
                    //EMPUser.FILE_SMS_SEND = Convert.ToDateTime(row["FILE_SMS_SEND"].ToString());
                    break;
                }
            }

            Dictionary<string, string> keyValue = new Dictionary<string, string>();
            keyValue.Add("EMP_ID", EMP_ID);
            dataTable = MAPIBase.DefaultMapper().QueryForDataTable("MAPI100_DEVICE_LIST", keyValue);  // DB에서 해당 사업장의 디바이스를 가져온다.
            if (dataTable.Rows.Count > 0)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    EMPUser.Devices.Add(row["IPADDR"].ToString());
                }
            }
            return EMPUser;
        }
        #endregion
    }
}
