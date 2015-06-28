using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CsQuery;

namespace Xy.PW
{
    public static class License
    {
        private const string LicenseDataUrl= "https://docs.google.com/spreadsheets/d/1kXuUUcXc8XRTwjn35W1cm0QCqGK4moyD8GnM9K5mXwY/pubhtml";

        public static string GetCpuID()
        {
            return new ManagementClass("win32_processor")
                .GetInstances()
                .Cast<ManagementObject>()
                .Select(x => x.Properties["processorID"].Value.ToString())
                .Select(x => Regex.Replace(x, ".{4}(?!$)", "$0-"))
                .FirstOrDefault() ?? "404-CpuID-Not-Found";
        }

        public static bool CheckLicense()
        {
            var cpuID = GetCpuID();
            return CQ.CreateFromUrl(LicenseDataUrl)["td"]
                .Map(x => x.Cq().Text())
                .Any(x => x == cpuID);
        }
    }
}
