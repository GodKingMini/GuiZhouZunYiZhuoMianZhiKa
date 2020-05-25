// CSharpSDKSampleCode for Printing
// **********************************************************************************************************

using System;
using System.Text;
using System.Threading;
using System.IO;
using System.Drawing;


namespace Sporcard
{
    class SampleCodeGraphics
    {
        #region Declarations

        // Constants ----------------------------------------------------------------------------------------

        const int FONT_NORMAL       = 0x00;
        const int FONT_BOLD         = 0x01;
        const int FONT_ITALIC       = 0x02;
        const int FONT_UNDERLINE    = 0x04;
        const int FONT_STRIKETHRU   = 0x08;

        #endregion

        #region Constructor

        // Class Initialization
        //     gets the ZBRGraphics.dll version -------------------------------------------------------------

        public SampleCodeGraphics()
        {
        }

        #endregion

        #region Get SDL DLL Version

        // Gets the DLL version for the SDK -----------------------------------------------------------------

        public string GetSDKVersion()
        {
            ZBRGraphics graphics = null;

            int engLevel, major, minor;

            string version = "";

            try
            {
                graphics = new ZBRGraphics();

                graphics.GetSDKVer(out major, out minor, out engLevel);
                
                if ((major + minor + engLevel) > 0)
                    version = major.ToString() + "." + minor.ToString() + "." + engLevel.ToString();
            }
            catch(Exception ex)
            {
                version = ex.ToString();
            }
            finally
            {
                graphics = null;
            }
            return version;
        }

        #endregion

        #region Is Printer Ready

        // Checks to see if any jobs are in the print spooler -----------------------------------------------

        public bool IsPrinterReady(string drvName, int seconds, out string errMsg)
        {
            errMsg = string.Empty;

            bool    ready = false;
            int     errValue = 0;

            ZBRGraphics     g;
            try
            {
                g = new ZBRGraphics();

                for (int i = 0; i < seconds; i++)
                {
                    if (g.IsPrinterReady(ASCIIEncoding.ASCII.GetBytes(drvName), out errValue) != 0)
                    {
                        ready = true;
                        break;
                    }
                    else if (errValue != 0)
                    {
                        errMsg = "Error: " + errValue.ToString();
                        break;
                    }
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                ready = false;
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "IsPrinterReady");   
            }
            finally
            {
                g = null;
            }
            return ready;
        }

        #endregion

        #region Printing Example Code

        // Printing on Both Sides ---------------------------------------------------------------------------

        public void PrintBothSides(string drvName, string frontText, string imgPath,
                                   string backText, out int jobID, out string msg)
        {
            jobID = 0;
            msg = string.Empty;

            int     errValue;   // value of 0 indicates no errors
            ZBRGraphics graphics = null;
            
            try
            {
                // Creates a Graphics Buffer
                graphics = new ZBRGraphics();

                if (graphics.InitGraphics(ASCIIEncoding.ASCII.GetBytes(drvName), "job", out jobID, out errValue) == 0)
                {
                    msg = "Printing InitGraphics Error: " + errValue.ToString();
                    return;
                }

                // Draws Text into the Graphics Buffer
                int fontStyle = FONT_BOLD | FONT_ITALIC | FONT_UNDERLINE | FONT_STRIKETHRU;

                if (graphics.DrawText(35, 575, ASCIIEncoding.ASCII.GetBytes(frontText), ASCIIEncoding.ASCII.GetBytes("Arial"), 12, fontStyle,
                                      0xFF0000, out errValue) == 0)
                {
                    msg = "Printing DrawText Error: " + errValue.ToString();
                    return;
                }

                // Draws a line into the Graphics Buffer
                if (graphics.DrawLine(35, 300, 300, 300, 0xFF0000, (float)5.0, out errValue) == 0)
                {
                    msg = "Printing DrawLine Error: " + errValue.ToString();
                    return;
                }

                // Places an Image from a File into the Graphics Buffer
                if (graphics.DrawImage(ASCIIEncoding.ASCII.GetBytes(imgPath + "\\Zebra.bmp"), 30, 30, 200, 150, out errValue) == 0)
                {
                    msg = "Printing DrawImage Error: " + errValue.ToString();
                    return;
                }

                // Sends Barcode Data to the Monochrome Buffer
                if (graphics.DrawBarcode(35, 500, 0, 0, 1, 3, 30, 1, ASCIIEncoding.ASCII.GetBytes("123456789"), out errValue) == 0)
                {
                    msg = "Printing DrawBarcode Error: " + errValue.ToString();
                    return;
                }

                // Prints the Graphics Buffer (Front Side)
                if (graphics.PrintGraphics(out errValue) == 0)
                {
                    msg = "Printing PrintGraphics Error: " + errValue.ToString();
                    return;
                }

                // Clears the Graphics Buffer
                if (graphics.ClearGraphics(out errValue) == 0)
                {
                    msg = "Printing ClearGraphics Error: " + errValue.ToString();
                    return;
                }

                // Draws Text into the Graphics Buffer
                if (graphics.DrawText(30, 575, ASCIIEncoding.ASCII.GetBytes(backText), ASCIIEncoding.ASCII.GetBytes("Arial"), 12, fontStyle, 0,
                                      out errValue) == 0)
                {
                    msg = "Printing DrawText Error: " + errValue.ToString();
                    return;
                }

                // Prints the Graphics Buffer (Back Side)
                if (graphics.PrintGraphics(out errValue) == 0)
                {
                    msg = "Printing PrintGraphics Error: " + errValue.ToString();
                    return;
                }
            }
            catch (Exception ex)
            {
                msg += ex.ToString();
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "PrintBothSides");
            }
            finally
            {
                if (graphics != null)
                {
                    // Starts the printing process and releases the Graphics Buffer
                    if (graphics.CloseGraphics(out errValue) == 0)
                    {
                        msg = "Printing CloseGraphics Error: " + errValue.ToString();
                    }
                    graphics = null;
                }
            }
        }
               

        public void PrintFrontSideOnly(PrintParameter pParas, string drvName, out int jobID, out string msg)
        {
            PrintFormatView pfv = new PrintFormatView();
            jobID = 0;
            msg = string.Empty;
                        
            int     errValue;    // value of 0 indicates no errors
            ZBRGraphics graphics = graphics = new ZBRGraphics();
            try
            {
                if (graphics.InitGraphics(ASCIIEncoding.ASCII.GetBytes(drvName), "job", out jobID, out errValue) == 0)
                {
                    msg = "Printing InitGraphics Error: " + errValue.ToString();
                    return;
                }

                LogHelper.WriteLog(typeof(ZBRPrinter), "InitGraphics 成功");

                //数据位置    
                pfv.DrawPersonInfo(pParas, ref graphics);
                

                LogHelper.WriteLog(typeof(ZBRPrinter), "DrawPersonInfo 成功");

                //头像位置    
                if (!pfv.DrawPhoto(pParas, ref graphics))
                {
                    msg = "DrawPhoto 失败";
                    LogHelper.WriteLog(typeof(ZBRPrinter), "DrawPhoto 失败");
                    return;
                }

                LogHelper.WriteLog(typeof(ZBRPrinter), "DrawPhoto 成功");

                if (graphics.PrintGraphics(out errValue) == 0)
                {
                    msg = "Printing PrintGraphics Error: " + errValue.ToString();
                    return;
                }
                LogHelper.WriteLog(typeof(ZBRPrinter), "PrintGraphics 成功");
            }
            catch (Exception ex)
            {
                msg += ex.ToString();
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "PrintFrontSideOnly");
            }
            finally
            {
                if (graphics != null)
                {
                    // Starts the printing process and releases the Graphics Buffer
                    if (graphics.CloseGraphics(out errValue) == 0)
                    {
                        msg = "Printing CloseGraphics Error: " + errValue.ToString();
                    }
                    graphics = null;
                }

            }
        }

        public void CancelJob(string printerName, int jobID, out string errMsg)
        {
            errMsg = string.Empty;

            int errValue;
            ZBRGraphics graphics = null;
            try
            {
                graphics = new ZBRGraphics();
                
                if (graphics.CancelJob(printerName, jobID, out errValue) == 0)
                {
                    errMsg = "CancelJob failed. Error: " + errValue.ToString();
                }
            }
            catch (Exception ex)
            {
                errMsg = "CancelJob exception: " + ex.Message;
            }
            finally
            {
                graphics = null;
            }
        }
                        
        #endregion




        

    }
}
