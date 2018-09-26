
using ServerBackupUtility.Logging;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ServerBackupUtility.Services
{
    public class TransferService : ITransferService
    {
        private readonly string _path = AppDomain.CurrentDomain.BaseDirectory;
        private readonly string _dateTime = DateTime.Now.ToLocalTime().ToString("yy-MM-dd");
        private readonly string _url = ConfigurationManager.AppSettings["FtpUrl"].Trim();
        private readonly string _port = ConfigurationManager.AppSettings["FtpPort"].Trim();
        private readonly string _mode = ConfigurationManager.AppSettings["FtpMode"].Trim();
        private readonly bool _ssl = Convert.ToBoolean(ConfigurationManager.AppSettings["FtpSsl"].Trim());
        private readonly string _userName = ConfigurationManager.AppSettings["FtpUserName"].Trim();
        private readonly string _password = ConfigurationManager.AppSettings["FtpPassword"].Trim();

        public async Task<Boolean> InitializeFtpAsync()
        {
            LogService.LogEvent("Contacting FTP Server For Login");

            Uri baseUri = new Uri("ftp://" + _url + ':' + _port + '/');

            NetworkCredential networkCredential = new NetworkCredential();
            networkCredential.UserName = _userName;
            networkCredential.Password = _password;

            X509Certificate2 certificate2 = new X509Certificate2(_path + "\\localhost.pfx", "secret");

            FtpWebResponse response = null;

            try
            {
                Uri requestUri = new Uri(baseUri, _dateTime);
                FtpWebRequest request = (FtpWebRequest) WebRequest.Create(requestUri);

                request.Credentials = networkCredential;
                request.ClientCertificates.Add(certificate2);
                request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
                request.EnableSsl = _ssl;
                request.KeepAlive = true;
                request.UsePassive = String.Equals(_mode, "passive", StringComparison.OrdinalIgnoreCase);
                request.UseBinary = true;
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                request.ContentLength = 0;

                response = (FtpWebResponse) await request.GetResponseAsync();
                LogService.LogEvent("FTP Server Response: " + response.StatusDescription);

                if (response.StatusCode == FtpStatusCode.PathnameCreated)
                {
                    return true;
                }

                return false;
            }
            catch (WebException ex)
            {
                response = (FtpWebResponse) ex.Response;
                LogService.LogEvent("Error: FtpService.InitializeFtpAsync - " + response.StatusDescription);

                return false;
            }
            catch (Exception ex)
            {
                LogService.LogEvent("Error: FtpService.InitializeFtpAsync - " + ex.Message);
                return false;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
        }

        public async Task<Boolean> UploadFileAsync(string filePath)
        {
            Uri baseUri = new Uri("ftp://" + _url + ':' + _port + '/');

            NetworkCredential networkCredential = new NetworkCredential();
            networkCredential.UserName = _userName;
            networkCredential.Password = _password;

            X509Certificate2 certificate2 = new X509Certificate2(_path + "\\localhost.pfx", "secret");

            FileStream fileStream = null;
            FtpWebResponse response = null;

            try
            {
                Uri fileNameUri = new Uri(baseUri, _dateTime + "/" + Path.GetFileName(filePath));
                FtpWebRequest request = (FtpWebRequest) WebRequest.Create(fileNameUri);

                request.Credentials = networkCredential;
                request.ClientCertificates.Add(certificate2);
                request.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);
                request.EnableSsl = _ssl;
                request.KeepAlive = true;
                request.UsePassive = String.Equals(_mode, "passive", StringComparison.OrdinalIgnoreCase);
                request.UseBinary = true;
                request.Method = WebRequestMethods.Ftp.UploadFile;

                fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                request.ContentLength = fileStream.Length;

                Stream requestStream = await request.GetRequestStreamAsync();

                byte[] buffer = new byte[32768];
                int readBytes = 0;

                do
                {
                    readBytes = await fileStream.ReadAsync(buffer, 0, buffer.Length);
                    await requestStream.WriteAsync(buffer, 0, readBytes);
                }
                while (readBytes != 0);

                requestStream.Close();

                response = (FtpWebResponse) await request.GetResponseAsync();
                LogService.LogEvent("FTP Server Response: " + response.StatusDescription);

                if (response.StatusCode == FtpStatusCode.ClosingData)
                {
                    return true;
                }

                return false;
            }
            catch (WebException ex)
            {
                response = (FtpWebResponse) ex.Response;
                LogService.LogEvent("Error: FtpService.UploadFileAsync - " + response.StatusDescription);

                return false;
            }
            catch (Exception ex)
            {
                LogService.LogEvent("Error: FtpService.InitializeFtpAsync - " + ex.Message);
                return false;
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
