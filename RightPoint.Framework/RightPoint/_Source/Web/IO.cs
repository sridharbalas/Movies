using System;
using System.IO;
using System.Net;
using System.Web;

namespace RightPoint.Web
{
    /// <summary>
    /// Summary description for IO.
    /// </summary>
    public sealed class IO
    {
        private IO()
        {
        }

        public static bool HttpFileExists(string url)
        {
            bool returnValue = false;

			HttpWebResponse webResponse = null;
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Timeout = 5000;
                webRequest.Method = "HEAD";
                webRequest.KeepAlive = true;
                webRequest.AllowAutoRedirect = false;

                webResponse = (HttpWebResponse)webRequest.GetResponse();

                if (webResponse.StatusCode == HttpStatusCode.OK)
                {
                    returnValue = true;
                }
            }
            catch (Exception)
            {
               
            }
			finally
			{
				if ( webResponse != null)
				{
					webResponse.Close();
				}
			}
            return returnValue;
        }

        public static byte[] GetHttpFileData(string url)
        {
			HttpWebResponse webResponse = null;
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Timeout = 5000;
                webRequest.Method = "GET";
                webRequest.KeepAlive = true;
                webRequest.AllowAutoRedirect = true;

                webResponse = (HttpWebResponse)webRequest.GetResponse();

                if (webResponse.StatusCode == HttpStatusCode.OK)
                {
                    BinaryReader br = new BinaryReader(webResponse.GetResponseStream());
                    MemoryStream ms = new MemoryStream();
                    BinaryWriter bw = new BinaryWriter(ms);
                    byte[] data = new byte[10240];
                    int count;

                    do
                    {
                        count = br.Read(data, 0, data.Length);

                        if (count > 0)
                        {
                            bw.Write(data, 0, count);
                        }
                    }
                    while (count > 0);

                    ms.Position = 0;
                    return ms.ToArray();
                }

                webResponse.Close();
            }
            catch (Exception)
            {

            }
			finally
			{
				if ( webResponse != null)
				{
					webResponse.Close();
				}
			}
            return null;
        }

        /// <summary>
        /// Uploads a posted file and names it according to the vip publisher raw filename format.
        /// </summary>
        /// <param name="postedFile">Posted File</param>
        /// <param name="destinationPathAndFileName">example: (C:\VipUpload\vip_raw_b132.txt)</param>
        /// <returns>Was the file uploaded?</returns>
        public static Boolean UploadPostedFile(HttpPostedFile postedFile, String destinationPathAndFileName)
        {
            Boolean returnValue = false;

            // Create the directory if it does not exist.
            if (!Directory.Exists(Path.GetDirectoryName(destinationPathAndFileName)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(destinationPathAndFileName));
            }

            // Get size of uploaded file
            Int32 fileLength = postedFile.ContentLength;

            // Allocate a buffer for reading of the file
            byte[] fileData = new byte[fileLength];

            // Read uploaded file from the Stream into the FileData
            postedFile.InputStream.Read(fileData, 0, fileLength);

            #region Write to NewFile and Close it, update returnValue if it succeeds

            try
            {
                // Create a file and override if it already exists
                FileStream newFile = new FileStream(destinationPathAndFileName, FileMode.Create);

                // Write data to the file
                newFile.Write(fileData, 0, fileData.Length);

                // Close the file
                newFile.Close();

                returnValue = true;
            }
            catch (Exception)
            {
                
            }

            #endregion

            return returnValue;
        }
    }
}
