using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;

namespace PingApp.Installer.CustomAction
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult MyCustomFunction(Session session)
        {
            session.Log("Begin MyCustomFunction");

            var response = ActionResult.Success;
            if (!CheckForInternetConnection(session))
            {
                response = ActionResult.Failure;
            }

            session.Log("End MyCustomFunction");

            return response;
        }

        public static bool CheckForInternetConnection(Session session)
        {
            try
            {
                var customUrl = session["URL"];
                var url = !string.IsNullOrEmpty(customUrl) ? customUrl : "google.com";
                using (var client = new WebClient())
                using (client.OpenRead($"https://{url}"))
                    return true;
            }
            catch(Exception ex)
            {
                session.Log(ex.ToString());
                return false;
            }
        }
    }
}
