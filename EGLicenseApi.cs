using System.Text;
using System.Runtime.InteropServices;
using RGiesecke.DllExport;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System;

public class EGLicenseAPI
{
    [DllExport("RVExtensionVersion", CallingConvention = CallingConvention.Winapi)]
    public static void RvExtensionVersion(StringBuilder output, int outputSize)
    {
        output.Append("Life-Codes.net API v1.0");
    }
    [DllExport("RVExtension", CallingConvention = CallingConvention.Winapi)]
    public static void RvExtension(StringBuilder output, int outputSize,
    [MarshalAs(UnmanagedType.LPStr)] string licKey)
    {
        bool result = false;

        try
        {

            string communityUrl = "https://YOURDOMAINHERE.COM/api/index.php?/nexus/lkey/";
            string licenseKey = licKey;
            string endpoint = "&key=MY_IPBOARD_APIKEY";

            string html = string.Empty;
            string url = communityUrl + licenseKey + endpoint;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }

            JObject json = JObject.Parse(html);
            string productName = json.SelectToken("name").ToString();
            string expires = json.SelectToken("expires").ToString();
            string canceled = json.SelectToken("canceled").ToString();
            string isActive = json.SelectToken("active").ToString();
            string steam64ID = json.SelectToken("customFields.1").ToString();
            result = true;
            output.Append(result);
        }
        catch
        {
            output.Append(result);
        }
    }

    [DllExport("RVExtensionArgs", CallingConvention = CallingConvention.Winapi)]
    public static int RvExtensionArgs(StringBuilder output, int outputSize,
        [MarshalAs(UnmanagedType.LPStr)] string function,
        [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 4)] string[] args, int argCount)
    {

        bool result = false;
        string lickey = "";
        string playerUID = "";
        int index = 0;

        foreach (var arg in args)
        {
            switch (index)
            {
                case 0:
                    lickey = arg;
                    break;
                case 1:
                    playerUID = arg;
                    break;
            }
            index++;
        }

        using (WebClient wc = new WebClient())
        {
            try
            {
                string communityUrl = "https://YOURDOMAINHERE.COM/api/index.php?/nexus/lkey/";
                string licenseKey = lickey;
                string endpoint = "&key=MY_IPBOARD_APIKEY";

                string url = communityUrl + licenseKey + endpoint;
                var jsonStr = wc.DownloadString(url);

                JObject json = JObject.Parse(jsonStr);
                //string productName = json.SelectToken("name").ToString();
                //string expires = json.SelectToken("expires").ToString();
                string canceled = json.SelectToken("canceled").ToString();
                string isActive = json.SelectToken("active").ToString();
                string steam64ID = json.SelectToken("customFields.1").ToString();

                if (steam64ID == playerUID)
                {
                    if (isActive == "true" && canceled == "false")
                    {
                        //License is okey
                        result = true;
                        output.Append(result);
                        return 1;
                    }
                    else
                    {
                        output.Append(result);
                        return 0;
                    }
                }
                else
                {
                    output.Append(result);
                    return 0;
                }
            } catch (Exception er)
            {
                output.Append(result);
                return 0;
            } 
        }
    }
}