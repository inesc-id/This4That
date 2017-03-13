using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace This4That_library
{
    public class Library
    {
        public static bool GetJsonTaskFromHttpRequest(HttpRequest request, out string encryptedTask, ref string errorMessage)
        {
            try
            {
                encryptedTask = new StreamReader(request.InputStream).ReadToEnd();
                if (String.IsNullOrEmpty(encryptedTask))
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                encryptedTask = null;
                return false;
            }
            
        }
    }
}
