using System;
using System.Configuration;
using System.Data;
using System.Net;
using System.Xml;
using System.Xml.Linq;

namespace AzureFrameWork
{
    public static class ManageVMDisks
    {
        //Branch testing
        //Testing 
        private static string subscriberID = ConfigurationSettings.AppSettings[Constants.FieldNameConstants.SubscriptionID].ToString();
        private static string attachmsg = "Attched To Disk";
        private static string message = "Message";

        /// <summary>
        /// Method used to list of all the available disks.
        /// </summary>
        /// <returns>Table contain disks information.</returns>
        public static DataSet GetAvailableVMDisks()
        {
            string version = VMOperations.RoleVersionConstants.VER2012;
            string uriFormat = AzureFrameWork.Util.UrlConstants.GetAvailableDisks;
            Uri uri = new Uri(String.Format(uriFormat, subscriberID));

            HttpWebRequest request = AzureFrameWork.Util.CreateWebRequest(uri, version, APIVersions.MethodType.GET.ToString(), null);

            HttpWebResponse response;
            TrustAllCert trust = new TrustAllCert();
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(trust.OnValidationCallback);
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                // GetResponse throws a WebException for 400 and 500 status codes
                response = (HttpWebResponse)ex.Response;
            }
            ListVMDisksResponse disksResponse = null;
            HttpStatusCode statusCode = response.StatusCode;
            if (response.ContentLength > 0 && statusCode == HttpStatusCode.OK)
            {
                using (XmlReader reader = XmlReader.Create(response.GetResponseStream()))
                {
                    //responseBody = XDocument.Load(reader);
                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(ListVMDisksResponse));
                    disksResponse = (ListVMDisksResponse)serializer.Deserialize(reader);
                }
            }

            DataSet dsDisks = CreateDisksInfoTable();
            FillDataSet(dsDisks, disksResponse);
            return dsDisks;
        }

        private static void FillDataSet(DataSet dsDisks, ListVMDisksResponse disksResponse)
        {
            if (disksResponse != null)
            {
                foreach (ListVMDisksResponse.Disk  item in disksResponse.DiskCollection)
                {
                    DataRow drow = dsDisks.Tables[Constants.TableNameConstants.Parent].NewRow();
                    drow[Constants.FieldNameConstants.DiskName] = item.Name;
                    drow[Constants.FieldNameConstants.DiskURL] = item.MediaLink;
                    drow[Constants.FieldNameConstants.DiskSize] = item.LogicalSizeInGB;
                    drow[Constants.FieldNameConstants.Location] = item.Location;
                    drow[Constants.FieldNameConstants.OS] = item.OS;
                    drow[Constants.FieldNameConstants.IsAttached] = item.AttachedTo != null ? true : false;

                    dsDisks.Tables[Constants.TableNameConstants.Parent].Rows.Add(drow);

                    if (item.AttachedTo != null)
                    {
                        DataRow dCrow = dsDisks.Tables[Constants.TableNameConstants.Child].NewRow();
                        dCrow[Constants.FieldNameConstants.DiskName] = item.Name;
                        dCrow[Constants.FieldNameConstants.DeploymentName] = item.Name;
                        dCrow[Constants.FieldNameConstants.HostedService] = item.Name;
                        dCrow[Constants.FieldNameConstants.RoleName] = item.Name;

                        dsDisks.Tables[Constants.TableNameConstants.Child].Rows.Add(dCrow);
                    }

                }
            }
        }


        /// <summary>
        /// Method used to create the table for the disk information.
        /// </summary>
        /// <returns>DataTable.</returns>
        private static DataSet CreateDisksInfoTable()
        {
            DataSet ds = new DataSet();
            DataTable dt = ds.Tables.Add(Constants.TableNameConstants.Parent);
            dt.Columns.Add(Constants.FieldNameConstants.DiskName, typeof(string));
            dt.Columns.Add(Constants.FieldNameConstants.DiskURL, typeof(string));
            dt.Columns.Add(Constants.FieldNameConstants.DiskSize, typeof(string));
            dt.Columns.Add(Constants.FieldNameConstants.Location, typeof(string));
            dt.Columns.Add(Constants.FieldNameConstants.OS, typeof(string));
            dt.Columns.Add(Constants.FieldNameConstants.IsAttached, typeof(bool));
            dt.Columns[Constants.FieldNameConstants.IsAttached].ReadOnly = true;

            DataTable dtChild = ds.Tables.Add(Constants.TableNameConstants.Child); ;
            dtChild.Columns.Add(Constants.FieldNameConstants.DiskName, typeof(string));
            dtChild.Columns.Add(Constants.FieldNameConstants.DeploymentName, typeof(string));
            dtChild.Columns.Add(Constants.FieldNameConstants.HostedService, typeof(string));
            dtChild.Columns.Add(Constants.FieldNameConstants.RoleName, typeof(string));



            ds.Tables[Constants.TableNameConstants.Child].ParentRelations.Add(attachmsg, ds.Tables[Constants.TableNameConstants.Parent].Columns[Constants.FieldNameConstants.DiskName], ds.Tables[Constants.TableNameConstants.Child].Columns[Constants.FieldNameConstants.DiskName]);
                     
            return ds;
        }

        /// <summary>
        /// Method used to add the disk.
        /// </summary>
        /// <param name="diskRequestBody">AddDiskRequest object.</param>
        /// <returns>True/False</returns>
        public static bool AddImage(AddDiskRequest diskRequestBody)
        {
            bool result = false;
            XDocument responseBody = null;
            string version = VMOperations.RoleVersionConstants.VER2012;
            string uriFormat = AzureFrameWork.Util.UrlConstants.AddDisk;
            Uri uri = new Uri(String.Format(uriFormat, subscriberID));

            HttpWebRequest request = AzureFrameWork.Util.CreateWebRequest(uri, version, APIVersions.MethodType.POST.ToString(), diskRequestBody);

            HttpWebResponse response;
            TrustAllCert trust = new TrustAllCert();
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(trust.OnValidationCallback);
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                // GetResponse throws a WebException for 400 and 500 status codes
                response = (HttpWebResponse)ex.Response;
            }
            HttpStatusCode statusCode = response.StatusCode;
            if (statusCode == HttpStatusCode.OK) result = true;
            if (response.ContentLength > 0)
            {
                using (XmlReader reader = XmlReader.Create(response.GetResponseStream()))
                {
                    responseBody = XDocument.Load(reader);
                    //System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(ListVMDisksResponse));
                    //locat = (ListVMDisksResponse)serializer.Deserialize(reader);
                    XElement element = responseBody.Element(message);
                    
                }
            }
            return result;
        }
    }
}
