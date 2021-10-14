//-----------------------------------------------------------------------
// <copyright file="DC4100_ResourceManager.cs" company="Thorlabs GmbH">
//     Copyright (c) Thorlabs GmbH. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Thorlabs
{
    using System;
    using Ivi.Visa.Interop;
    using System.Collections.Generic;

    /// <summary>
    /// This class let the VISA Runtime search for connected devices
    /// </summary>
    public class DC4100_ResourceManager
    {
        /// <summary>
        /// The computer is parsed for connected devices which match the given expression
        /// </summary>
        /// <param name="expression">search parameter for a device</param>
        /// <returns>list of connected devices with its resource string which matches the expression</returns>
        public static string[] FindRsc(string expression)
        {
            IResourceManager3 rscMng = new ResourceManager();
            return rscMng.FindRsrc(expression);
        }

        /// <summary>
        /// The computer is parsed for connected <c>Thorlabs DC4100 or DC4101 instruments</c>
        /// </summary>
        /// <returns>list of connected DC4100 instruments and its resource strings</returns>
        public static string[] FindRscDC4100()
        {
            List<string> devList = new List<string>();
            IResourceManager3 rscMng = new ResourceManager();
            string[] devRes = rscMng.FindRsrc("ASRL?*");
            foreach (string devItem in devRes)
            {
                // get alias
                IVisaSession session = rscMng.Open(devItem);
                string instrName = session.HardwareInterfaceName;
                if (instrName.Contains("DC4100") || instrName.Contains("DC4104"))
                {
                    devList.Add(devItem);
                }
                session.Close();
            }

            return devList.ToArray();
        }
    }
}
