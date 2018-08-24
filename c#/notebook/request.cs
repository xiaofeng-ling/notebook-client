using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace notebook
{
    namespace request
    {
        struct LoginResult
        {
            public int code;
            public string message;
            public Object data;
        }

        class Request
        {
            public string baseUrl = "http://notebook.test/api/";

            public string login = "login";
        }
    }
}
