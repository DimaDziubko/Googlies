using System;
using UnityEngine;

namespace _Game.Utils
{
    public class OpenEmailAppUtill
    {
        public static void OpenEmailApp(string emailBody, string emailAddress, string subjectMail)
        {
            string emailTo = "mailto:" + emailAddress +
                             "?subject=" + Uri.EscapeDataString(subjectMail) +
                             "&body=" + Uri.EscapeDataString(emailBody);
            Application.OpenURL(emailTo);
        }
    }
}
