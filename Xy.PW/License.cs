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

        public static string GetHardwareID()
        {
            var processorIDs = new ManagementClass("win32_processor")
                .GetInstances()
                .Cast<ManagementObject>()
                .Select(x => x.GetPropertyValue("ProcessorID"));
            var volumeSNs = new ManagementClass("Win32_LogicalDisk")
                .GetInstances()
                .Cast<ManagementObject>()
                .Select(x => x.GetPropertyValue("VolumeSerialNumber"));

            var hwid = Enumerable.Concat(processorIDs, volumeSNs)
                .Aggregate(17, (accumulate, x) => accumulate * 31 + x?.GetHashCode() ?? 0);

            return Regex.Replace(hwid.ToString("X"), ".{4}(?!$)", "$0-");
        }

        public static bool CheckLicense()
        {
            var hwid = GetHardwareID();
            return CQ.CreateFromUrl(LicenseDataUrl)["td"]
                .Map(x => x.Cq().Text())
                .Any(x => x == hwid);
        }
    }
}
