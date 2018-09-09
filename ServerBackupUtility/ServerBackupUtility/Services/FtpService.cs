
using ServerBackupUtility.Logging;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Security.Cryptography.X509Certificates;

namespace ServerBackupUtility.Services
{
    public class FtpService : IFtpService
    {
        private readonly string _path = AppDomain.CurrentDomain.BaseDirectory;
        private readonly string _dateTime = DateTime.Now.ToString("yy-MM-dd");
        private readonly string _url = ConfigurationManager.AppSettings["FtpUrl"];
        private readonly string _port = ConfigurationManager.AppSettings["FtpPort"];
        private readonly string _mode = ConfigurationManager.AppSettings["FtpMode"];
        private readonly bool _ssl = Convert.ToBoolean(ConfigurationManager.AppSettings["FtpSsl"]);
        private readonly string _userName = ConfigurationManager.AppSettings["FtpUserName"];
        private readonly string _password = ConfigurationManager.AppSettings["FtpPassword"];

        public void InitializeFtp(bool status = false)
        {
            LogService.LogEvent("Contacting FTP Server For Login");

            Uri baseUri = _port == "21" ? new Uri("ftp://" + _url) : new Uri("ftp://" + _url + ':' + _port);

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
                if (_ssl)
                { request.ClientCertificates.Add(certificate2); }
                request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
                request.EnableSsl = _ssl;
                request.KeepAlive = true;
                request.UsePassive = String.Equals(_mode, "passive", StringComparison.OrdinalIgnoreCase);
                request.UseBinary = true;
                request.ContentLength = 0;
                request.Method = WebRequestMethods.Ftp.MakeDirectory;

                response = (FtpWebResponse) request.GetResponse();
                LogService.LogEvent("FTP Server Response: " + response.StatusDescription);
            }
            catch (WebException ex)
            {
                response = (FtpWebResponse)ex.Response;
                LogService.LogEvent("Error: FtpService.InitializeFtpAsync - " + response.StatusDescription);

                if (!status)
                {
                    DeleteCurrentFtpFolder();
                    InitializeFtp(true);
                }
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
            Uri baseUri = _port == "21" ? new Uri("ftp://" + _url) : new Uri("ftp://" + _url + ':' + _port);

            NetworkCredential networkCredential = new NetworkCredential();
            networkCredential.UserName = _userName;
            networkCredential.Password = _password;

            X509Certificate2 certificate2 = new X509Certificate2(_path + "\\localhost.pfx", "secret");

            FileStream fileStream = null;
            Stream requestStream = null;
            FtpWebResponse response = null;

            try
            {
                Uri fileNameUri = new Uri(baseUri, _dateTime + "/" + Path.GetFileName(filePath));
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(fileNameUri);

                request.Credentials = networkCredential;
                if (_ssl)
                { request.ClientCertificates.Add(certificate2); }
                request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
                request.EnableSsl = _ssl;
                request.KeepAlive = true;
                request.UsePassive = String.Equals(_mode, "passive", StringComparison.OrdinalIgnoreCase);
                request.UseBinary = true;
                request.Method = WebRequestMethods.Ftp.UploadFile;

                byte[] buffer = new byte[32768];
                int readBytes = 0;

                requestStream = request.GetRequestStream();
                fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                do
                {
                    readBytes = fileStream.Read(buffer, 0, buffer.Length);
                    requestStream.Write(buffer, 0, readBytes);
                }
                while (readBytes != 0);

                response = (FtpWebResponse) request.GetResponse();
                LogService.LogEvent("FTP Server Response: " + response.StatusDescription);
            }
            catch (WebException ex)
            {
                response = (FtpWebResponse) ex.Response;
                LogService.LogEvent("Error: FtpService.UploadFileAsync - " + response.StatusDescription);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }

                if (requestStream != null)
                {
                    requestStream.Close();
                }

                if (response != null)
                {
                    response.Close();
                }
            }
        }

        public void DeleteCurrentFtpFolder()
        {
            LogService.LogEvent("Deleting Current FTP Folder");

            Uri baseUri = _port == "21" ? new Uri("ftp://" + _url) : new Uri("ftp://" + _url + ':' + _port);

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
                if (_ssl)
                { request.ClientCertificates.Add(certificate2); }
                request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
                request.EnableSsl = _ssl;
                request.KeepAlive = true;
                request.UsePassive = String.Equals(_mode, "passive", StringComparison.OrdinalIgnoreCase);
                request.UseBinary = true;
                request.ContentLength = 0;
                request.Method = WebRequestMethods.Ftp.RemoveDirectory;

                response = (FtpWebResponse) request.GetResponse();
                LogService.LogEvent("FTP Server Response: " + response.StatusDescription);
            }
            catch (WebException ex)
            {
                response = (FtpWebResponse) ex.Response;
                LogService.LogEvent("Error: FtpService.InitializeFtpAsync - " + response.StatusDescription);
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
        }
    }
}
