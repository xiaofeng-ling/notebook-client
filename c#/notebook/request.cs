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

    class UpdateResult: Result
    {
        public class Update
        {
           public  int updated_at;
        }

        public new Update data;
    }

    class LoadNotebookResult: Result
    {
        public class Notebook
        {
            public int id;
            public string content;
        }

        public new Notebook data;
    }

    class NotebookList : Result
    {
        public new List<Notebook> data;

        public class Notebook
        {
            public int id;
            public string title;
            public int updated_at;

            public override string ToString()
            {
                return title;
            }
        }
    }

    class Request
    {
        public static string baseUrl = "http://notebook.test/api/";
        
        public static readonly string login = "login";

        public static readonly string notebookMainList = "notebookMain";
        public static readonly string notebookMainCreate = "notebookMain";
        public static readonly string notebookMainDel = "notebookMain/delete";

        public static readonly string notebookList = "notebook";
        public static readonly string notebookCreate = "notebook";
        public static readonly string SaveNotebook = "notebook/update";
        public static readonly string LoadNotebookContent = "notebook/";
        public static readonly string DeleteNotebook = "notebook/delete";
        public static readonly string ModifyTitleNotebook = "notebook/modifyTitle";

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
