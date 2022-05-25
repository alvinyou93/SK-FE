using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E3.SYS.HOME
{
    public static class SGNBase
    {
        private static string ConnectionName = E3.Common.FrameworkConfigManager.Get_IBATIS_CONNECTION_NAME();

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
    }
}
