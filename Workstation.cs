using System.Management;
using System.Text;

namespace ViberSender2017
{
    public static class Workstation
    {
        public static string GenerateWorkstationId()
        {
            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher();
            StringBuilder stringBuilder = new StringBuilder();
            managementObjectSearcher.Query = new ObjectQuery("select * from Win32_Processor");
            foreach (ManagementObject managementObject in managementObjectSearcher.Get())
            {
                stringBuilder.Append(Workstation.ManagmentObjectPropertyData(managementObject.Properties["ProcessorId"]));
                stringBuilder.Append(',');
            }
            managementObjectSearcher.Query = new ObjectQuery("select * from Win32_BaseBoard");
            foreach (ManagementObject managementObject in managementObjectSearcher.Get())
            {
                stringBuilder.Append(Workstation.ManagmentObjectPropertyData(managementObject.Properties["Product"]));
                stringBuilder.Append(',');
            }
            return stringBuilder.ToString();
        }

        private static string ManagmentObjectPropertyData(PropertyData data)
        {
            string str1 = string.Empty;
            if (data.Value != null && !string.IsNullOrEmpty(data.Value.ToString()))
            {
                string str2 = data.Value.GetType().ToString();
                if (!(str2 == "System.String[]"))
                {
                    if (str2 == "System.UInt16[]")
                    {
                        foreach (ushort num in (ushort[])data.Value)
                            str1 = str1 + (object)num + " ";
                    }
                    else
                        str1 = data.Value.ToString();
                }
                else
                {
                    foreach (string str3 in (string[])data.Value)
                        str1 = str1 + str3 + " ";
                }
            }
            return str1;
        }
    }
}
