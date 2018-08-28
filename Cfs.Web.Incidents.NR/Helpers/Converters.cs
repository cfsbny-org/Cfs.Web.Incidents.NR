using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Cfs.Web.Incidents.NR.Helpers
{
    public class Converters
    {

        public static string GetComputerName(string ipAddressString)
        {
            try
            {
                IPAddress ipAddress = IPAddress.Parse(ipAddressString);
                IPHostEntry ipHostEntry = Dns.GetHostEntry(ipAddress);

                return ipHostEntry.HostName.ToString();
            }
            catch
            {
                return "Unknown host";
            }
        }





    }




    public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        public CustomMultipartFormDataStreamProvider(string path) : base(path)
        {
        }



        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            var name = !string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName) ? headers.ContentDisposition.FileName : "UnnamedFile";
            return name.Replace("\"", string.Empty);
        }
    }


    public class UploadedFileInformation
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        
        public UploadedFileInformation(string name, string path)
        {
            FileName = name;
            FilePath = path;
        }
    }


}