using System;
using System.Windows.Forms;
using System.Xml;

namespace CrmDiagTool2011.Helpers
{
    class XmlHelper
    {
        /// <summary>
        /// This function will just load the Web.Config file as XML and return the DevErrors flag value as bool
        /// </summary>
        /// <returns>DevErrors Flag as bool</returns>
        public static bool GetDevErrorFlag()
        {
            bool deverrors = false;     // set the deverrors to false.
            XmlDocument myXmlWebConfig = new XmlDocument();
            string webSitePath = RegistryHelper.EvaluateString("WebSitePath", @"Software\Microsoft\MSCRM");

            try
            {
                // Try to load Web.Config file.
                myXmlWebConfig.Load(webSitePath + @"\Web.Config");
                //Filtering nodes off type "<add key"
                XmlNodeList nodeList = myXmlWebConfig.SelectNodes("//add");

                foreach (XmlNode node in nodeList)
                {
                    // if this node is the DevErrors node, read it and if its value is "On", set the deverrrors bool to true
                    if (node.OuterXml.StartsWith("<add key=\"DevErrors\"") && node.Attributes["value"].Value == "On") deverrors = true;
                }

                return deverrors;
            }
            catch
            {
                return deverrors; // if we are not able to find web config or read it for any reason, we just return false....
            }
        }
      
        /// <summary>
        /// Set the DevErrors flag in the Web.Config file
        /// </summary>
        /// <param name="DevErrorFlag">DevError flag as a bool for On & Off</param>
        public static void SetDevErrorFlag(bool DevErrorFlag)
        {
            XmlDocument myXmlWebConfig = new XmlDocument();
            string webSitePath = RegistryHelper.EvaluateString("WebSitePath", @"Software\Microsoft\MSCRM");

            // Try to load Web.Config file.
            myXmlWebConfig.Load(webSitePath + @"\Web.Config");

            //Filtering nodes off type "<add key"
            XmlNodeList nodeList = myXmlWebConfig.SelectNodes("//add");
            foreach (XmlNode node in nodeList)
            {
                // if this node is the DevErrors node, read it and return the value.
                if (node.OuterXml.StartsWith("<add key=\"DevErrors\""))
                {
                    // Depending on the bool parameter we set it "On" or "Off".
                    if(DevErrorFlag) node.Attributes["value"].Value = "On";
                    else node.Attributes["value"].Value = "Off";
                    // Then we save our changes.
                    myXmlWebConfig.Save(webSitePath + @"\Web.Config");
                }
            }
        }
        
        /// <summary>
        /// DevErrors property of the WebConfig file.
        /// </summary>
        public static bool DevErrors
        {
            get {return GetDevErrorFlag(); }
            set
            {
                try
                {
                    SetDevErrorFlag(value);
                }
                catch(Exception e)
                {
                    MessageBox.Show("An error has occurred while trying to set\r"
                                   + "DevErrors flag in the Web.Config file:"
                                   + e.Message, "CrmDiagTool2011 Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
