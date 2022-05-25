using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E3.LCM.WEB.BIZ
{
    public class CMV_Model_User
    {
        /// <summary>
        /// 사용자 모델 
        /// </summary>
        public CMV_Model_User()
        {
            this.user_id = "";
            this.user_ids = ""; // 삭제용
            this.user_name = "";
            this.user_type = "01";
            this.access_allowed = true;
            this.company_id = 0;
            this.cell_phone = "";
            this.date_of_in = DateTime.Now.ToString("yyyyMMdd");
            this.date_of_expired = "99991231";
            this.option_datas = new CMV_Model_Option_Datas();

            this.auth_start_dtm = DateTime.Now.ToString("yyyyMMdd") + "0000";
            this.auth_end_dtm = DateTime.Now.ToString("yyyyMMdd") + "2359";
            this.auth_type = "0";
        }
        public string user_id { get; set; }
        public string user_ids { get; set; }
        public string user_name { get; set; }
        public string user_type { get; set; }
        public bool access_allowed { get; set; }
        public int company_id { get; set; }        
        public string date_of_in { get; set; }
        public string date_of_expired { get; set; }
        public string cell_phone { get; set; }
        public CMV_Model_Option_Datas option_datas { get; set; }

        public string auth_start_dtm { get; set; }
        public string auth_end_dtm { get; set; }
        public string auth_type { get; set; }

    }

    /// <summary>
    /// 사용자의 option_datas 모델 
    /// </summary>
    public class CMV_Model_Option_Datas
    {
        public CMV_Model_Option_Datas() 
        {   
            privileges = new List<string>();
        }        
        public List<string> privileges { get; set; }
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
    }

    // 이미지 변환 및 판독여부 리턴 모델
    public class returnBase64Image
    {
        public string Original { get; set; }
        public string Thumbnail { get; set; }

        public string option_datas { get; set; }
    }

    public class MAPI_CMV2_Models
    {
    }
}
