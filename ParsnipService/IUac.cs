using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web.UI;

namespace ParsnipService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IUac" in both code and config file together.
    [ServiceContract]
    public interface IUac
    {
        [OperationContract]
        User SecurePage(string pUrl, Page pPage, string pDeviceType, string pAccountType);

        [OperationContract]
        User SecurePage(string pUrl, Page pPage, string pDeviceType);
    }
}
