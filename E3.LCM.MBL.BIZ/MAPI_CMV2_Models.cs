using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E3.LCM.MBL.BIZ
{
    public class CMV_Model_User
    {
        /// <summary>
        /// 사용자 모델 
        /// </summary>
        public CMV_Model_User()
        {
            this.user_id = "";
            this.user_name = "";
            this.user_type = "01";
            this.auth_type = "1";
            this.access_allowed = true;
            this.company_id = 0;
            this.depart_id = "1";
            this.company_phone = "";
            this.cell_phone = "";
            this.auth_start_dtm = DateTime.Now.AddDays(-1).ToString("yyyyMMddHHmm");
            this.auth_end_dtm = DateTime.Now.AddDays(-1).ToString("yyyyMMddHHmm");
            this.title_id = "";
            this.email = "";
            this.password = "";
            this.pin_no = "";
            this.individual_id = 0;
            this.bypass_card = false;
            this.option_datas = new CMV_Model_Option_Datas();
        }
        public string user_id { get; set; }
        public string user_name { get; set; }
        public string user_type { get; set; }
        public string auth_type { get; set; }
        public bool access_allowed { get; set; }
        public int company_id { get; set; }
        public string depart_id { get; set; }
        public string company_phone { get; set; }
        public string cell_phone { get; set; }
        public string date_of_in { get; set; }
        public string date_of_expired { get; set; }
        public string auth_start_dtm { get; set; }
        public string auth_end_dtm { get; set; }
        public string title_id { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string pin_no { get; set; }
        public int individual_id { get; set; }
        public bool bypass_card { get; set; }
        public CMV_Model_Option_Datas option_datas { get; set; }
    }

    public class CMV_Model_User_Action_Result
    { 
        public string user_id { get; set; }
        public bool result { get; set; }
        public string result_desc { get; set; }
    }

    public class CMV_Model_User_Remove
    {
        public CMV_Model_User_Remove()
        {
            user_ids = new List<string>();
        }
        public List<string> user_ids { get; set; }
    }

    public class CMV_Model_Faces_Data
    {
        public CMV_Model_Faces_Data()
        {
            faces = new List<CMV_Model_Faces>();
        }
        public string user_id { get; set; }
        public string user_name { get; set; }
        public List<CMV_Model_Faces> faces { get; set; }
    }

    /// <summary>
    /// 사용자의 option_datas 모델 
    /// </summary>
    public class CMV_Model_Option_Datas
    {
        public CMV_Model_Option_Datas() 
        {
            faces = new List<CMV_Model_Faces>();
            privileges = new List<string>();
        }
        public List<CMV_Model_Faces> faces { get; set; }
        public List<string> privileges { get; set; }
    }

    public class CMV_Model_User_Face_Check
    {
        public CMV_Model_User_Face_Check()
        {
            faces = new List<CMV_Model_Faces>();
        }

        public string user_id { get; set; }
        public string user_name { get; set; }

        public List<CMV_Model_Faces> faces { get; set; }
    }

    /// <summary>
    /// 사용자의 option_datas > faces 모델 
    /// </summary>
    public class CMV_Model_Faces
    { 
        public int face_eye_width { get; set; }
        public int face_feature_size { get; set; }
        public double face_frontal_score { get; set; }
        public int face_left_eye_x { get; set; }
        public int face_right_eye_x { get; set; }
        public double face_roll { get; set; }
        public double face_score { get; set; }
        public int face_LED_state { get; set; }
        public string face_feature { get; set; }
        public string user_uuid { get; set; }
        public int sub_id { get; set; }

    }

    public class CMV_Model_Faces_Error
    { 
        public decimal error_no { get; set; }
        public string error_message { get; set; }
    }

    // 이미지 변환 및 판독여부 리턴 모델
    public class returnBase64Image
    {
        public string Original { get; set; }
        public string Thumbnail { get; set; }

        public string option_datas { get; set; }

        public int DeviceCount { get; set; }
    }

    public class DeviceListModel
    {
        public DeviceListModel()
        {
            DeviceList = new List<DeviceModel>();
        }
        public List<DeviceModel> DeviceList { get; set; }
    }

    public class DeviceModel
    {
        public string device_ip { get; set; }
        public string device_model { get; set; }
        public int device_port { get; set; }
        public string device_sn { get; set; }
        public string device_name { get; set; }
        public bool device_enable { get; set; }
        public bool device_connected { get; set; }
        public string door_name { get; set; }
        public string door_direction { get; set; }
    }

    public class MAPI_CMV2_Models
    {
    }

    public class SE_TA_EMP_Model
    {
        public SE_TA_EMP_Model()
        {
            Devices = new List<string>();
        }
        public int EMP_ID { get; set; }
        public string PLANT_ID { get; set; }
        public int CRTIC_NO { get; set; }
        public string ACCESS_GB { get; set; }
        public string SAFE_EDU_CMPLT_YN { get; set; }
        public DateTime SAFE_EDU_CMPLT_DT { get; set; }
        public string PASS_YN { get; set; }
        public int PASS_SCORE { get; set; }
        public DateTime PASS_DT { get; set; }
        public DateTime PRIVACY_CONSENT_DT { get; set; }
        public DateTime SMS_SND_DT { get; set; }
        public string BLOCK_YN { get; set; }
        public string ID_PHOTO { get; set; }
        public int BP_HIGHT { get; set; }
        public int BP_LOW { get; set; }
        public string ACCESS_POSSIBLE_YN { get; set; }
        public DateTime ACCESS_POSSIBLE_DT { get; set; }
        public string RMK { get; set; }
        public string ADDR { get; set; }
        public string BIRTH_DT { get; set; }
        public string OPTIONDATAS { get; set; }
        public string EMP_TEL_NO { get; set; }
        public string EMP_NM { get; set; }
        public DateTime FILE_SMS_SEND { get; set; }

        public List<string> Devices { get; set; }
    }
}
