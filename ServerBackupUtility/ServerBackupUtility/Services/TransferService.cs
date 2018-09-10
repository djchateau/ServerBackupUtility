﻿
using ServerBackupUtility.Logging;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Security.Cryptography.X509Certificates;

namespace ServerBackupUtility.Services
{
    public class TransferService : ITransferService
    {
        private readonly string _path = AppDomain.CurrentDomain.BaseDirectory;
        private readonly string _dateTime = DateTime.Now.ToLocalTime().ToString("yy-MM-dd");
        private readonly string _url = ConfigurationManager.AppSettings["FtpUrl"];
        private readonly string _port = ConfigurationManager.AppSettings["FtpPort"];
        private readonly string _mode = ConfigurationManager.AppSettings["FtpMode"];
        private readonly bool _ssl = Convert.ToBoolean(ConfigurationManager.AppSettings["FtpSsl"]);
        private readonly string _userName = ConfigurationManager.AppSettings["FtpUserName"];
        private readonly string _password = ConfigurationManager.AppSettings["FtpPassword"];

        public void InitializeFtp()
        {
            LogService.LogEvent("Contacting FTP Server For Login");

            Uri baseUri = _port == "21" ? new Uri("ftp://" + _url + '/') : new Uri("ftp://" + _url + ':' + _port + '/');

            NetworkCredential networkCredential = new NetworkCredential();
            networkCredential.UserName = _userName;
            networkCredential.Password = _password;

            X509Certificate2 certificate2 = new X509Certificate2(_path + "\\localhost.pfx", "secret");

            FtpWebResponse response = null;

            try
            {
                Uri requestUri = new Uri(baseUri, _dateTime);
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(requestUri);

                request.Credentials = networkCredential;
                if (_ssl) { request.ClientCertificates.Add(certificate2); }
                request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
                request.EnableSsl = _ssl;
                request.KeepAlive = true;
                request.UsePassive = String.Equals(_mode, "passive", StringComparison.OrdinalIgnoreCase);
                request.UseBinary = true;
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                request.ContentLength = 0;

                response = (FtpWebResponse) request.GetResponse();
                LogService.LogEvent("FTP Server Response: " + response.StatusDescription);
            }
            catch (WebException ex)
            {
                response = (FtpWebResponse)ex.Response;
                LogService.LogEvent("Error: FtpService.InitializeFtp - " + response.StatusDescription);
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
        }

        public void UploadFile(string filePath)
        {
            Uri baseUri = _port == "21" ? new Uri("ftp://" + _url + '/') : new Uri("ftp://" + _url + ':' + _port + '/');

            NetworkCredential networkCredential = new NetworkCredential();
            networkCredential.UserName = _userName;
            networkCredential.Password = _password;

            X509Certificate2 certificate2 = new X509Certificate2(_path + "\\localhost.pfx", "secret");

            FileStream fileStream = null;
            FtpWebResponse response = null;

            try
            {
                Uri fileNameUri = new Uri(baseUri, _dateTime + "/" + Path.GetFileName(filePath));
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(fileNameUri);

                request.Credentials = networkCredential;
                if (_ssl) { request.ClientCertificates.Add(certificate2); }
                request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
                request.EnableSsl = _ssl;
                request.KeepAlive = true;
                request.UsePassive = String.Equals(_mode, "passive", StringComparison.OrdinalIgnoreCase);
                request.UseBinary = true;
                request.Method = WebRequestMethods.Ftp.UploadFile;

                fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                Stream requestStream = request.GetRequestStream();

                byte[] buffer = new byte[32768];
                int readBytes = 0;

                do
                {
                    readBytes = fileStream.Read(buffer, 0, buffer.Length);
                    requestStream.Write(buffer, 0, readBytes);
                }
                while (readBytes != 0);

                requestStream.Close();

                response = (FtpWebResponse) request.GetResponse();
                LogService.LogEvent("FTP Server Response: " + response.StatusDescription);
            }
            catch (WebException ex)
            {
                response = (FtpWebResponse) ex.Response;
                LogService.LogEvent("Error: FtpService.UploadFile - " + response.StatusDescription);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }

                if (response != null)
                {
                    response.Close();
                }
            }
        }
    }
}