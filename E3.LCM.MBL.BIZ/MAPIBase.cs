using E3.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E3.LCM.MBL.BIZ
{
    /// <summary>
    /// 모바일 안전 교육 관련 Class
    /// </summary>
    public static class MAPIBase
    {
        private static string ConnectionName = E3.Common.FrameworkConfigManager.Get_IBATIS_CONNECTION_NAME();
        private static int THUMBNAIL_MAX_PIXELS = 1024;
        public static string BasicDirectory = E3.Common.FrameworkConfigManager.GetAppSetting("FACE_IMAGE_PATH", @"C:\E3_COMPONENTS\STORAGE\FACE_IMAGE");

        public static IBatisNet.DataMapper.ISqlMapper Mapper()
        {
            var mapper = Common.IBatis.DataMapper.CreateMapper(ConnectionName);
            return mapper;
        }
        public static IBatisNet.DataMapper.ISqlMapper Mapper(string connectionname)
        {
            var mapper = Common.IBatis.DataMapper.CreateMapper(connectionname);
            return mapper;
        }
        public static IBatisNet.DataMapper.ISqlMapper DefaultMapper()
        {
            var mapper = Common.IBatis.DataMapper.DefaultMapper;
            return mapper;
        }

        #region E3 암/복호화 기능 
        /// <summary>
        /// 암호화를 적용한다
        /// </summary>
        /// <param name="value">원문 문자열</param>
        /// <returns>암호화된 문자열</returns>
        public static string Encrypt(string value)
        {
            return E3.Common.Supports.CryptographyUtility.Encrypt(value);
        }
        /// <summary>
        /// 복호화를 적용한다.
        /// </summary>
        /// <param name="value">암호된 문자열</param>
        /// <returns>복호화된 문자열</returns>
        public static string Decrypt(string value)
        {
            return E3.Common.Supports.CryptographyUtility.Decrypt(value);
        }
        #endregion

        #region Handle Base64String to File -> Make thumbnail to Base64String 
        /// <summary>
        /// 원본 이미지를 받아서 파일에 저장하고 저장한 파일을 이용하여
        /// thumbnail 파일을 만들고 만들어진 파일을 읽어 Base64로 변환한다.
        /// 변환이 완료된 파일을 모두 삭제한다.
        /// </summary>
        /// <param name="Base64String">base64 이미지 파일</param>
        /// <param name="tokenID">사용자 ID 정보</param>
        /// <returns>thumbnail 이미지의 base64 문자열</returns>
        public static returnBase64Image ConvertFile(string Base64String, SE_TA_EMP_Model userInfo, out List<CMV_Model_User> cmiduser)
        {
            returnBase64Image returnValue = new returnBase64Image();
            bool isUseThumbnail = true; // 이미지 템플릿을 요청할 자료 선택 (true 이면 thumbnail, false 이면 original 전송)
            string extention = "png";
            string tokenID = string.Format("{0}-{1}", userInfo.EMP_TEL_NO, userInfo.EMP_ID.ToString());
            cmiduser = new List<CMV_Model_User>();
            try
            {
                string imageSource = Base64String;
                string imageType = imageSource.Split(',')[0];
                if (imageType.IndexOf("image") > 0)
                {
                    extention = imageType.Split('/')[1].Split(';')[0];
                    string imageString = imageSource.Split(',')[1];

                    byte[] data = Convert.FromBase64String(imageString);
                    if (!System.IO.Directory.Exists(BasicDirectory))
                    {
                        Directory.CreateDirectory(BasicDirectory);
                    }

                    string ImageFilePath = string.Format(@"{0}{1}{2}"
                                                    , BasicDirectory
                                                    , System.IO.Path.DirectorySeparatorChar
                                                    , string.Format("{0}.{1}", tokenID, extention));
                    using (var imageFile = new FileStream(ImageFilePath, FileMode.Create))
                    {
                        imageFile.Write(data, 0, data.Length);
                        imageFile.Flush();

                        makeThumbnail(imageFile, tokenID);  // 암호화 저장되는 자료의 크기를 줄이기 위함
                    }

                    // 카메라 기본 설정은 jpeg로 설정됨
                    // 저장된 사진이 PNG이면 JPEG로 변환
                    // 안면인식기가 JPEG 24bit 이미지만 인식 가능하다고 함
                    if (extention.ToLower().Equals("png"))
                    {
                        using (Image png = Image.FromFile(ImageFilePath))
                        {
                            png.Save(string.Format(@"{0}{1}{2}"
                                                    , BasicDirectory
                                                    , System.IO.Path.DirectorySeparatorChar
                                                    , string.Format("{0}.{1}", tokenID, "jpg")), ImageFormat.Jpeg);
                            png.Dispose();
                        }
                        returnValue.Original = readOriginal(tokenID, "jpg");
                    }
                    else 
                    {
                        returnValue.Original = readOriginal(tokenID, extention);
                    }

                    
                    returnValue.Thumbnail = readThumbnail(tokenID);

                    try
                    {
                        var client = new E3.LCM.MBL.BIZ.MAPIWebClient();

                        returnValue.option_datas = "";
                        returnValue.DeviceCount = userInfo.Devices.Count;


                        // CMID REST API 사용자정보 수집
                        string routerPath = string.Format("/1.0/users/{0}", userInfo.EMP_ID.ToString());
                        // var check_cmid_user_data = client.callWebRequest(routerPath, "GET");
                        var check_cmid_user_data = string.Empty;
                        cmiduser = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CMV_Model_User>>(check_cmid_user_data);

                        /*
                        // 기존의 사용자 정보가 존재하면 지우고 진행해야 한다고 함
                        if (cmiduser.Count > 0)
                        {
                            // 사용자 정보를 삭제한다.
                            // var delete_cmid_user_data = client.callWebRequest(routerPath, "DELETE");
                            var delete_cmid_user_data = string.Empty;
                            System.Threading.Thread.Sleep(100); // 잠시 대기

                            // 영구 삭제 적용 (디바이스에서 정보를 지운다.)
                            CMV_Model_User_Remove removeId = new CMV_Model_User_Remove();
                            removeId.user_ids.Add(userInfo.EMP_ID.ToString());
                            routerPath = string.Format("/1.0/users/remove");
                            string removeJson = Newtonsoft.Json.JsonConvert.SerializeObject(removeId);
                            // var remove_cmid_user_data = client.callWebRequest(routerPath, "POST", removeJson);
                            var remove_cmid_user_data = string.Empty;
                            System.Threading.Thread.Sleep(500); // 잠시 대기
                        }
                        */
                        /*
                        // 기준정보에 저장된 안면인식기 장치가 존재한다면
                        if (returnValue.DeviceCount > 0)
                        {
                            for (int i = 0; i < userInfo.Devices.Count; i++)
                            {
                                // [SERIALNO], [IPADDR] - 디바이스의 연결 정보를 확인한다.
                                routerPath = string.Format("/1.0/devices/{0}", userInfo.Devices[i]);
                                // var deviceCheckResponse = client.callWebRequest(routerPath, "GET");
                                var deviceCheckResponse = string.Empty;
                                List<DeviceModel> devicelist = new List<DeviceModel>();
                                devicelist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DeviceModel>>(deviceCheckResponse);

                                foreach (DeviceModel d in devicelist)
                                {
                                    if (d.device_connected == true)
                                    {
                                        Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                                        keyValuePairs.Add("device", d.device_sn); // KH1205A003167, 192.168.0.101 테스트 디바이스 정보
                                        // Thumbnail 이미지를 전송할 지 원본 이미지를 전송할 지 결정
                                        if(isUseThumbnail) keyValuePairs.Add("image", returnValue.Thumbnail);
                                        else keyValuePairs.Add("image", returnValue.Original);

                                        // var ret = client.callWebRequest(keyValuePairs);
                                        var ret = string.Empty;

                                        returnValue.option_datas = ret;
                                        break;
                                    }
                                }

                                if (!string.IsNullOrEmpty(returnValue.option_datas)) break;
                            }
                        }
                        else
                        {
                            // 기준정보에 저장된 안면인식기 장치가 존재하지 않는다면
                            // var deviceCheckResponse = client.callWebRequest("/1.0/devices", "GET");
                            var deviceCheckResponse = string.Empty;
                            List<DeviceModel> devicelist = new List<DeviceModel>();
                            devicelist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DeviceModel>>(deviceCheckResponse);

                            foreach (DeviceModel d in devicelist)
                            {
                                if (d.device_connected == true)
                                {
                                    Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                                    keyValuePairs.Add("device", d.device_sn); // KH1205A003167, 192.168.0.101 테스트 디바이스 정보
                                    // Thumbnail 이미지를 전송할 지 원본 이미지를 전송할 지 결정
                                    if (isUseThumbnail) keyValuePairs.Add("image", returnValue.Thumbnail);
                                    else keyValuePairs.Add("image", returnValue.Original);
                                    
                                    // var ret = client.callWebRequest(keyValuePairs);
                                    var ret = string.Empty;

                                    returnValue.option_datas = ret;
                                    break;
                                }
                            }
                        }
                        */
                    }
                    catch (IOException ioe) 
                    {
                        E3.Common.Loggers.FileLogger.Error(ioe);
                        ioe.Data.Clear();
                        returnValue.option_datas = "";
                    }
                    catch (Exception e)
                    {
                        E3.Common.Loggers.FileLogger.Error(e);
                        e.Data.Clear();
                        returnValue.option_datas = "";
                    }
                }
            }
            catch (Exception e)
            {
                E3.Common.Loggers.FileLogger.Error(e);
                e.Data.Clear();
            }
            finally 
            {
                // 파일 삭제 처리
                deleteFile(tokenID, extention);
            }
            return returnValue;
        }

        public static bool UserFaceCheck(string user_id, bool isDelete = true)
        {
            bool ret = false;
            var client = new E3.LCM.MBL.BIZ.MAPIWebClient();
            List<CMV_Model_User_Face_Check> faceCheck = new List<CMV_Model_User_Face_Check>();
            // var retget = client.callWebRequest(string.Format("/1.0/users/face/{0}", user_id), "GET");   // 기존에 존재여부 확인
            var retget = string.Empty;
            faceCheck = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CMV_Model_User_Face_Check>>(retget);
            if (faceCheck.Count > 0) 
            {
                for (int i = 0; i < faceCheck.Count; i++)
                {
                    if (faceCheck[i].faces.Count > 0) 
                    {
                        for (int j = 0; j < faceCheck[i].faces.Count; j++) 
                        {
                            if (!string.IsNullOrEmpty(faceCheck[i].faces[j].face_feature))
                            {
                                ret = true;
                                break;
                            }
                        }
                    }
                    if (ret) break;
                }
            }
            // 삭제 할까
            if (ret && isDelete)
            {
                // var retdel = client.callWebRequest(string.Format("/1.0/users/face/{0}", user_id), "DELETE");   // 기존에 존재하면 삭제
                var retdel = string.Empty;

            }
            return ret;
        }

        /// <summary>
        /// 썸네일 파일을 읽어온다.
        /// </summary>
        /// <param name="tokenID">사용자 ID 정보</param>
        /// <returns>base64String</returns>
        public static string readThumbnail(string tokenID)
        {
            string returnValue = "";
            try
            {
                string src = Path.Combine(BasicDirectory, "Thumbnail", string.Format("{0}.{1}", tokenID, "jpg"));

                if (File.Exists(src))
                {
                    using (Image image = Image.FromFile(src))
                    {
                        using (MemoryStream m = new MemoryStream())
                        {
                            image.Save(m, image.RawFormat);
                            byte[] imageBytes = m.ToArray();

                            returnValue = Convert.ToBase64String(imageBytes);
                        }
                    }
                }
            }
            catch (IOException ioe)
            {
                E3.Common.Loggers.FileLogger.Error(ioe);
                ioe.Data.Clear();
            }
            catch (Exception e)
            {
                E3.Common.Loggers.FileLogger.Error(e);
                e.Data.Clear();
            }
            return returnValue;
        }

        /// <summary>
        /// 썸네일 파일을 읽어온다.
        /// </summary>
        /// <param name="tokenID">사용자 ID 정보</param>
        /// <returns>base64String</returns>
        public static string readOriginal(string tokenID, string extention)
        {
            string returnValue = "";
            try
            {
                string src = Path.Combine(BasicDirectory, string.Format("{0}.{1}", tokenID, extention));

                if (File.Exists(src))
                {
                    long length = new FileInfo(src).Length / 1024;
                    if (length >= 600)
                    {
                        Image image = Image.FromFile(src);
                        Size thumbnailSize = GetThumbnailSize(image, 1920);
                        Image thumbnailImage = image.GetThumbnailImage(thumbnailSize.Width, thumbnailSize.Height, null, IntPtr.Zero);
                        image.Dispose();
                        thumbnailImage.Save(src, ImageFormat.Jpeg);
                        thumbnailImage.Dispose();
                    }
                    System.Threading.Thread.Sleep(100);

                    using (Image image = Image.FromFile(src))
                    {
                        using (MemoryStream m = new MemoryStream())
                        {
                            image.Save(m, image.RawFormat);
                            byte[] imageBytes = m.ToArray();

                            returnValue = Convert.ToBase64String(imageBytes);
                        }
                    }
                }
            }
            catch (IOException ioe)
            {
                E3.Common.Loggers.FileLogger.Error(ioe);
                ioe.Data.Clear();
            }
            catch (Exception e)
            {
                E3.Common.Loggers.FileLogger.Error(e);
                e.Data.Clear();
            }
            return returnValue;
        }

        /// <summary>
        /// 사진을 저장할 경우 썸네일을 만든다.
        /// </summary>
        /// <param name="imageFile">저장된 원본 이미지</param>
        /// <param name="tokenID">사용자 ID 정보</param>
        private static void makeThumbnail(FileStream imageFile, string tokenID)
        {
            string ThumbnailDirectory = Path.Combine(BasicDirectory, "Thumbnail");
            if (!System.IO.Directory.Exists(ThumbnailDirectory))
            {
                Directory.CreateDirectory(ThumbnailDirectory);
            }

            string output = Path.Combine(ThumbnailDirectory, string.Format("{0}.{1}", tokenID, "jpg"));

            Image image = Image.FromStream(imageFile);
            Size thumbnailSize = GetThumbnailSize(image, THUMBNAIL_MAX_PIXELS);
            Image thumbnail = image.GetThumbnailImage(thumbnailSize.Width, thumbnailSize.Height, null, IntPtr.Zero);

            thumbnail.Save(output, ImageFormat.Jpeg);
            image.Dispose();
            thumbnail.Dispose();
        }

        /// <summary>
        /// 썸네일을 만들 경우 사진의 크기 정보를 가져온다.
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        private static Size GetThumbnailSize(Image original, double thumbnailsize)
        {
            int originalWidth = original.Width;
            int originalHeight = original.Height;

            double factor;
            if (originalWidth > originalHeight)
            {
                factor = (double)thumbnailsize / originalWidth;
            }
            else
            {
                factor = (double)thumbnailsize / originalHeight;
            }

            return new Size((int)(originalWidth * factor), (int)(originalHeight * factor));
        }

        /// <summary>
        /// base64 변환 작업을 완료하면 파일을 삭제합니다.
        /// </summary>
        /// <param name="tokenID">사용자 ID 정보</param>
        /// <param name="thumbnailDelete">thumbnail 이미지도 함께 삭제 함 (Default: true)</param>
        private static void deleteFile(string tokenID, string extention, bool thumbnailDelete = true)
        {
            try
            {
                string originalFile = Path.Combine(BasicDirectory, string.Format("{0}.{1}", tokenID, "jpg"));
                string originalExtentionFile = Path.Combine(BasicDirectory, string.Format("{0}.{1}", tokenID, extention));
                string thumbnailFile = Path.Combine(BasicDirectory, "Thumbnail", string.Format("{0}.{1}", tokenID, "jpg"));

                if (File.Exists(originalFile))
                {
                    File.Delete(originalFile);
                }
                if (File.Exists(originalExtentionFile))
                {
                    File.Delete(originalExtentionFile);
                }
                if (thumbnailDelete && File.Exists(thumbnailFile))
                {
                    File.Delete(thumbnailFile);
                }
            }
            catch (IOException ioe)
            {
                E3.Common.Loggers.FileLogger.Error(ioe);
                ioe.Data.Clear();
            }
            catch (Exception ex)
            {
                E3.Common.Loggers.FileLogger.Error(ex);
                ex.Data.Clear();
            }
        }
        #endregion
    }
}
