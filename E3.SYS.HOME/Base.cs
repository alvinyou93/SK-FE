using E3.Common;
using E3.Common.Wcf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E3.SYS.HOME
{
    public abstract class Base : IBase
    {
        public abstract DataPackage DeleteItem(DataPackage package);
        public abstract DataPackage DeleteItems(DataPackage package);
        public abstract DataPackage InsertItem(DataPackage package);
        public abstract DataPackage InsertItems(DataPackage package);
        public abstract DataPackage PagingItems(DataPackage package);
        public abstract DataPackage SelectItem(DataPackage package);
        public abstract DataPackage SelectItems(DataPackage package);
        public abstract DataPackage UpdateItem(DataPackage package);
        public abstract DataPackage UpdateItems(DataPackage package);
    }
}
