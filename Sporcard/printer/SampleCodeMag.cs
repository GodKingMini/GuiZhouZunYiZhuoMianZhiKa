// CSharpSDKSampleCode for Magnetic Encoding
// **********************************************************************************************************

using System;
using System.Text;

namespace Sporcard
{
    class SampleCodeMag
    {
        #region Constructor

        // Class Initialization
        //     gets the ZBRPrinter.dll version --------------------------------------------------------------

        public SampleCodeMag()
        {
        }

        #endregion

        #region Get SDK Version

        // Gets the DLL version for the SDK -----------------------------------------------------------------
        public string GetSDKVersion()
        {
            ZBRPrinter prn = null;

            int engLevel, major, minor;

            string version = "";

            try
            {
                prn = new ZBRPrinter();

                prn.GetSDKVer(out major, out minor, out engLevel);

                if ((major + minor + engLevel) > 0)
                    version = major.ToString() + "." + minor.ToString() + "." + engLevel.ToString();
            }
            catch (Exception ex)
            {
                version = ex.ToString();
            }
            finally
            {
                prn = null;
            }
            return version;
        }

        #endregion
    }
}
