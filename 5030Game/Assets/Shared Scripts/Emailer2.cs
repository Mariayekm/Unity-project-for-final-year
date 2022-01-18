//  Emailer.cs
//  http://www.mrventures.net/all-tutorials/sending-emails
using System.Collections;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Emailer2 : MonoBehaviour
{
    [SerializeField] bool sendDirect;

/*    const string kSenderEmailAddress = "unileedsgamediffanalysis@gmail.com";
    const string kSenderPassword = "BMMTamts5030!";
    const string kReceiverEmailAddress = "unileedsgamediffanalysis@gmail.com";*/
    const string txtData = "testing123";
    const string kSenderEmailAddress = "unileedsgda@gmail.com";
    const string kSenderPassword = "BMMTamts5030!";
    const string kReceiverEmailAddress = "unileedsgda@gmail.com";


    void Start()
    { 
    }

    public void btnPress()
    {
        Debug.Log("Button pressed");
        SendAnEmail("testing press");
    }

    // Method 1: Direct message
    private static void SendAnEmail(string message)
    {
        // Create mail
        Debug.Log("Sendign email");
        MailMessage mail = new MailMessage();
        mail.From = new MailAddress(kSenderEmailAddress);
        mail.To.Add(kReceiverEmailAddress);
        mail.Subject = "Email Title";
        mail.Body = message;
        

        // Setup server 
        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
        smtpServer.Port = 587;
        smtpServer.Credentials = new NetworkCredential(
            kSenderEmailAddress, kSenderPassword) as ICredentialsByHost;
        smtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate,
            X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                Debug.Log("Email success!");
                return true;
            };

        // Send mail to server, print results
        try
        {
            smtpServer.Send(mail);
        }
        catch (System.Exception e)
        {
            Debug.Log("Email error: " + e.Message);
        }
        finally
        {
            Debug.Log("Email sent!");
        }
    }

}
