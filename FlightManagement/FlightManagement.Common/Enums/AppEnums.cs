using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightManagement.Common.Enums
{
    public class AppEnums
    {

        public enum StatusCode
        {
            SUCCESS = 200,
            ERROR = 500,
            NOT_FOUND = 404,
            BAD_REQUEST = 400
        }       

    }
}
