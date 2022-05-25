using E3.Common;
using E3.Common.Wcf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E3.SYS.HOME
{
    interface IBase
    {
        DataPackage SelectItem(DataPackage package);
        DataPackage SelectItems(DataPackage package);
        DataPackage InsertItem(DataPackage package);
        DataPackage InsertItems(DataPackage package);
        DataPackage UpdateItem(DataPackage package);
        DataPackage UpdateItems(DataPackage package);
        DataPackage DeleteItem(DataPackage package);
        DataPackage DeleteItems(DataPackage package);
        DataPackage PagingItems(DataPackage package);
    }


    interface IBaseAdd
    {
        DataPackage SelectParents(DataPackage package);
        DataPackage SelectChildren(DataPackage package);
    }

    interface IBaseUser
    {
        DataPackage GetLogin(DataPackage package);
        DataPackage GetMenus(DataPackage package);
        DataPackage ChangePassword(DataPackage package);
        DataPackage GetFavoritMenu(DataPackage package);
        DataPackage SetFavoritMenu(DataPackage package);

    }
}
