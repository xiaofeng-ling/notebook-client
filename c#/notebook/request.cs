using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace notebook.request
{
    class Result
    {
        public int code;
        public string message;
        public Object data;
    }

    class LoginResult : Result
    {
        public new Login data;

        public class Login
        {
            public string token;
            public int user_id;
        }
    }

    class NotebookMainList: Result
    {
        public new List<NotebookMain> data;

        public class NotebookMain
        {
            public int id;
            public string name;
            public int user_id;

            public override string ToString()
            {
                return name;
            }
        }
    }

    class Request
    {
        public static string baseUrl = "http://notebook.test/api/";

        public static string login = "login";

        public static string notebookMainList = "notebookMain";
        public static string notebookCreate = "notebookMain";
        public static string notebookDel = "notebookMain/delete";

        public static IDictionary<string, string> getParameters()
        {
            IDictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "token", GlobalVar.token }
            };

            return parameters;
        }
    }
}
