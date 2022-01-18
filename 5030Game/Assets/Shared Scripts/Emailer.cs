using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.IO;

public class Emailer : MonoBehaviour
{
    private const string emailAddressStr = "unileedsgamediffanalysis@gmail.com";
    private const string emailSubject = "GameDiffData";
    private string emailBodyText;
    private string fp_attachment;
    private const float delay = 0.0f;
    private GameObject csvHandlerGObj;
    private CSVHandler csvHandler;

    const string kSenderEmailAddress = "unileedsgda@gmail.com";
    const string kSenderPassword = "BMMTamts5030!";
    const string kReceiverEmailAddress = "unileedsgda@gmail.com";

    private void Start()
    {
        csvHandlerGObj = GameObject.Find("CSVHandler");
        csvHandler = csvHandlerGObj.GetComponent<CSVHandler>();

    }

    private bool CheckMissinginfo()
    {
        bool checkEmailStr = emailAddressStr != "";
        bool checkSubject = emailSubject != "";
        bool checkEmailText = emailBodyText != "";
        bool checkfpAttachemt = File.Exists(fp_attachment);
        string errorMsg = "Missing: ";
        bool allFieldsFilled = (checkEmailStr && checkSubject && checkEmailText && checkfpAttachemt);

        if (allFieldsFilled == false)
        {
            if (checkEmailStr == false) { errorMsg += ", email address"; }
            if (checkSubject == false) { errorMsg += ", Subject"; }
            if (checkEmailText == false) { errorMsg += ", Text"; }
            if (checkfpAttachemt == false) { errorMsg += ", Valid Attachment File"; }
            Debug.LogError(errorMsg);
        }
        return allFieldsFilled;
    }

    //Fortesting
    private string MakeAttachmentFp()
    {
        string fp_app = csvHandler.GetDirectoriesFP(CSVHandler.Directories.App);
        string fp_zip = System.IO.Path.Combine(fp_app, "DataCSV.zip");
        return fp_zip;
    }


    public void btnPress()
    {
        //SendAnEmail("testing press");
        SendAnEmail("testing email", csvHandler.GetZipFp()) ;
    }

    public void EmailData(string fp_attachement)
    {
        //string fp_attachemnt = csvHandler.GetZipFp();
        Debug.Log("fp_attachment: " + fp_attachement);
        if (File.Exists(fp_attachement))
        {
            SendAnEmail("testing email", fp_attachement);
        }
        else
        {
            Debug.Log("Attachment file not found");
        }
        
    }

    // Method 1: Direct message
    private static void SendAnEmail(string message, string fp_attachement)
    {
        Debug.Log("Sending email");
        // Create mail
        MailMessage mail = new MailMessage();
        mail.From = new MailAddress(kSenderEmailAddress);
        mail.To.Add(kReceiverEmailAddress);
        mail.Subject = emailSubject;
        mail.Body = message;
        Attachment attachment = new Attachment(fp_attachement);
        mail.Attachments.Add(attachment);

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