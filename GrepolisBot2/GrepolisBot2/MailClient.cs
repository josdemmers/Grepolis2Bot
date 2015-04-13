using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;
//using Google.GData.Calendar;
//using Google.GData.Extensions;
//using Google.GData.Client;

namespace GrepolisBot2
{
    class MailClient
    {

//-->Constructors

        public MailClient()
        {

        }

//-->Attributes

//-->Methods
        public void sendMail(string p_Server, int p_Port, bool p_Ssl, string p_User, string p_Pass, string p_Receiver, string p_Subject, string p_Message, bool p_Test)
        {
            IOHandler l_IOHandler = IOHandler.Instance;

            try
            {
                if (isValid(p_Server, p_Port, p_Ssl, p_User, p_Pass, p_Receiver))
                {
                    SmtpClient l_Client = new SmtpClient(p_Server, p_Port);
                    l_Client.EnableSsl = p_Ssl;
                    NetworkCredential l_NetworkCredential = new NetworkCredential(p_User, p_Pass);
                    l_Client.Credentials = l_NetworkCredential;
                    l_Client.Send(p_User, p_Receiver, p_Subject, p_Message);
                }
            }
            catch (Exception ex)
            {
                if(p_Test)
                    MessageBox.Show(ex.Message, "Problem sending mail", MessageBoxButtons.OK);
                l_IOHandler.debug(ex.Message);
            }
        }

        /*
         * Sends a sms using the google api.
         * No longer works, sms reminders seem to be disabled for event created via the api
         */ 
        public void sendSMS(string p_UserName, string p_Password, string p_Message)
        {
            /*CalendarService service = new CalendarService(p_UserName);
            service.setUserCredentials(p_UserName, p_Password);

            EventEntry l_Message = new EventEntry();

            l_Message.Title.Text = p_Message;
            //When l_When = new When(DateTime.Now.AddSeconds(80), DateTime.Now.AddHours(1));
            DateTime l_Start = DateTime.Now.AddMinutes(31);
            DateTime l_End = DateTime.Now.AddMinutes(31);
            l_End = l_End.AddHours(1);
            When l_When = new When(l_Start, l_End);

            l_Message.Times.Add(l_When);

            Reminder l_Sms = new Reminder();
            l_Sms.Minutes = 10;
            l_Sms.Method = Reminder.ReminderMethod.sms;
            l_Message.Reminders.Add(l_Sms);

            Uri l_Uri = new Uri("http://www.google.com/calendar/feeds/default/private/full");

            AtomEntry insertedEntry = service.Insert(l_Uri, l_Message);*/
        }

        private bool isValid(string p_Server, int p_Port, bool p_Ssl, string p_User, string p_Pass, string p_Receiver)
        {
            bool l_IsValid = false;
            if (p_Server.Length > 0 && p_Port >= 0 && p_User.Length > 0 && p_Pass.Length > 0 && p_Receiver.Contains("@"))
            {
                l_IsValid = true;
            }
            return l_IsValid;
        }
    }
}
