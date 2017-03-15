using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using This4That_library;
using This4That_library.Models.Integration;

namespace This4That_platform.SecurityLayer
{
    public class ServerAuthority
    {
        internal bool GetEncryptedTask(HttpRequest request, out JSONEncryptedTaskDTO encryptedTask, object serverMgrPublicKey)
        {
            string errorMessage = null;
            encryptedTask = null;

            try
            {
                if (!Library.GetEncryptedTask(HttpContext.Current.Request, out encryptedTask, serverMgrPublicKey, ref errorMessage))
                {
                    Global.Log.Error(errorMessage);
                    return false;
                }
                Global.Log.DebugFormat("User ID: [{0}] Encrypted Task: {1}", encryptedTask.UserID, encryptedTask.EncryptedTask);
                return true;
            }
            catch (Exception ex)
            {
                Global.Log.Error(ex.Message);
                return false;
            }
        }
    }
}