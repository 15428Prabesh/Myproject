using Azure;
using Microsoft.AspNetCore.StaticFiles;
using MimeKit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helper.FTP
{
    public class FTPHelper
    {
        public FTPResponse UploadFile(string ftpServer, string ftpUsername, string ftpPassword, string remoteFileName, byte[] data)
        {
            FTPResponse fTPResponse= new FTPResponse();
            try
            {
                // Create the FTP request
                ftpServer = "ftp://" + ftpServer;
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"{ftpServer}/{remoteFileName}");
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                request.UseBinary = true;
                request.UsePassive = true;
                // Write the byte array to the request stream
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(data, 0, data.Length);
                }

                // Get the response from the FTP server
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    fTPResponse.Status = 1;
                    fTPResponse.StatusMessage = $"Upload File Complete, status {response.StatusDescription}";
                }
            }
            catch (WebException ex)
            {
                fTPResponse.Status = -1;
                fTPResponse.StatusMessage = ex.ToString();
            }
            return fTPResponse;
        }
        
    }
}
