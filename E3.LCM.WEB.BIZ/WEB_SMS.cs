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

// 출입회사 관리
namespace E3.LCM.WEB.BIZ
{
    public class WEB_SMS
    {
        private static string ConnectionName = "SMS";
        /*
         * LCC      롯데케미칼(주)
           LAM     롯데첨단소재(주)
           LFC      롯데정밀화학(주)
           LBP      비피화학
        */
        private static string companyType = "LCC";

        //EPROC 고정(?)
        private static string sysType = "EPROC";
        //SendPhoneNumber
        // private static string sendPhoneNumber = "000000000";



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

        /*
         * phoneNumber : SMS를 보낼 대상 Phone번호
         * msgData : SMS나 LMS를 보낼 내용
         * msgType : 0 = SMS(90Byte 이하), 5 = LMS(2000Byte 이하) Default(0 : SMS) 
         * 
         */
        //public static string sendSms(string phoneNumber, string msgData, string msgType = "0")
        public static string sendSms(string phoneNumber, string msgData, string sendPhoneNumber, string msgType = "5")
        {
            try
            {
                var mapper = Common.IBatis.DataMapper.CreateMapper(ConnectionName);

                DataPackage package = new DataPackage();
                package.KeyValues.Add("companyType", companyType);
                package.KeyValues.Add("sysType", sysType);
                package.KeyValues.Add("msgType", "5");
                package.KeyValues.Add("phoneNumber", phoneNumber);
                package.KeyValues.Add("sendPhoneNumber", sendPhoneNumber);
                package.KeyValues.Add("msgData", "롯데케미칼 " + msgData);

                Int32 ret = mapper.QueryForObject<Int32>("WEB_SMS_INSERT", package.KeyValues);
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return "S";
        }
    }
}
