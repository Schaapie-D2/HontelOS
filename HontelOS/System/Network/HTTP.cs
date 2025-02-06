/*
* PROJECT:          HontelOS
* CONTENT:          Http utils class
* PROGRAMMER(S):    Valentin Charbonnier <valentinbreiz@gmail.com>
* MODIFIED BY:      Jort van Dalen
* 
* Copyright (c) 2025 Jort van Dalen
* 
* This code is licensed under the BSD 3-Clause License.
* You may obtain a copy of the License at:
* https://opensource.org/licenses/BSD-3-Clause
*/

using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4.UDP.DNS;
using Cosmos.System.Network.IPv4;
using CosmosHttp.Client;
using System.Net;

namespace HontelOS.System.Network
{
    public static class HTTP
    {
        public static byte[] DownloadRawFile(string url)
        {
            if (url.StartsWith("https://"))
                throw new WebException("HTTPS currently not supported, please use http://");

            string path = ExtractPathFromUrl(url);
            string domainName = ExtractDomainNameFromUrl(url);
            var dnsClient = new DnsClient();

            dnsClient.Connect(DNSConfig.DNSNameservers[0]);
            dnsClient.SendAsk(domainName);
            Address address = dnsClient.Receive();
            dnsClient.Close();

            HttpRequest request = new();
            request.IP = address.ToString();
            request.Domain = domainName;
            request.Path = path;
            request.Method = "GET";
            request.Send();

            return request.Response.GetStream();
        }

        public static string DownloadFile(string url)
        {
            if (url.StartsWith("https://"))
                throw new WebException("HTTPS currently not supported, please use http://");

            string path = ExtractPathFromUrl(url);
            string domainName = ExtractDomainNameFromUrl(url);
            var dnsClient = new DnsClient();

            dnsClient.Connect(DNSConfig.DNSNameservers[0]);
            dnsClient.SendAsk(domainName);
            Address address = dnsClient.Receive();
            dnsClient.Close();

            HttpRequest request = new();
            request.IP = address.ToString();
            request.Domain = domainName;
            request.Path = path;
            request.Method = "GET";
            request.Send();

            return request.Response.Content;
        }

        static string ExtractDomainNameFromUrl(string url)
        {
            int start;
            if (url.Contains("://"))
                start = url.IndexOf("://") + 3;
            else
                start = 0;

            int end = url.IndexOf("/", start);
            if (end == -1)
                end = url.Length;

            return url[start..end];
        }

        static string ExtractPathFromUrl(string url)
        {
            int start;
            if (url.Contains("://"))
                start = url.IndexOf("://") + 3;
            else
                start = 0;

            int indexOfSlash = url.IndexOf("/", start);
            if (indexOfSlash != -1)
                return url.Substring(indexOfSlash);
            else
                return "/";
        }
    }
}
