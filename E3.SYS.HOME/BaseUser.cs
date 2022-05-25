using E3.Common;
using E3.Common.Wcf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E3.SYS.HOME
{
    public abstract class BaseUser : IBaseUser
    {
        public abstract DataPackage GetLogin(DataPackage package);
        public abstract DataPackage GetMenus(DataPackage package);
        public abstract DataPackage ChangePassword(DataPackage package);
        public abstract DataPackage SetFavoritMenu(DataPackage package);
        public abstract DataPackage GetFavoritMenu(DataPackage package);
    }
}
