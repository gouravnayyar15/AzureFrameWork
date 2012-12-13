using System;
using System.Configuration;
using System.Data;
using System.Net;
using System.Xml;
using System.Xml.Linq;

namespace AzureFrameWork
{
    public static class ManageVMImage
    {
        /// <summary>
        /// Method Use to Add Virual Machine Image.
        /// </summary>
        /// <param name="imageRequest">Imagerequest object</param>
        /// <returns>Bool</returns>
        public static bool AddImage(AddImageRequest imageRequest, string subscriptionID)
        {
            bool result= false;
            string version = VMOperations.RoleVersionConstants.VER2012;
            string uriFormat = AzureFrameWork.Util.UrlConstants.AddViewVMImageUrl;
            Uri uri = new Uri(String.Format(uriFormat, subscriptionID));
            
            HttpWebRequest request = AzureFrameWork.Util.CreateWebRequest(uri, version, APIVersions.MethodType.POST.ToString(), imageRequest);

            XDocument responseBody = null;
            
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
            if (response.ContentLength > 0)
            {
                using (XmlReader reader = XmlReader.Create(response.GetResponseStream()))
                {
                    responseBody = XDocument.Load(reader);
                }
            }
            response.Close();
            if (response.StatusCode == HttpStatusCode.OK)
                result = true;

            return result;
        }

        /// <summary>
        /// Method used to Delete the existing Image.
        /// </summary>
        /// <param name="imageName">Name of the Image.</param>
        /// <param name="imageLabel">Label of the Image.</param>
        public static void DeleteImage(string imageName, string imageLabel, string subscriptionID)
        {
            string version = VMOperations.RoleVersionConstants.VER2012;
            string uriFormat = AzureFrameWork.Util.UrlConstants.UpdateDeleteVMImageUrl;
            Uri uri = new Uri(String.Format(uriFormat, subscriptionID, imageName));

            DeleteImageRequest deleteImageRequest = new DeleteImageRequest(){ Label = imageLabel };

            HttpWebRequest request = AzureFrameWork.Util.CreateWebRequest(uri, version, APIVersions.MethodType.DELETE.ToString(), deleteImageRequest);

            XDocument responseBody = null;

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
            if (response.ContentLength > 0)
            {
                using (XmlReader reader = XmlReader.Create(response.GetResponseStream()))
                {
                    responseBody = XDocument.Load(reader);
                }
            }
            response.Close();
        }

        /// <summary>
        /// Method Use to List all the available images.
        /// </summary>
        public static DataTable ListImages(string subscriptionID)
        {
            string version = VMOperations.RoleVersionConstants.VER2012;
            ListImagesResponse imagesDescription = new ListImagesResponse();
            string uriFormat = AzureFrameWork.Util.UrlConstants.AddViewVMImageUrl;
            Uri uri = new Uri(String.Format(uriFormat, subscriptionID));

            HttpWebRequest request = AzureFrameWork.Util.CreateWebRequest(uri, version, APIVersions.MethodType.GET.ToString(), null);

            XDocument responseBody = null;

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
            //HttpStatusCode statusCode = response.StatusCode;
            if (response.ContentLength > 0 && response.StatusCode == HttpStatusCode.OK)
            {
                using (XmlReader reader = XmlReader.Create(response.GetResponseStream()))
                {
                    //responseBody = XDocument.Load(reader);
                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(ListImagesResponse));
                    imagesDescription = (ListImagesResponse)serializer.Deserialize(reader);
                }
            }
            response.Close();

            DataTable dt = CreateImagesTable();

            foreach (ListImagesResponse.OSImage  image in imagesDescription.images)
            {
                DataRow row = dt.NewRow();
                //row["AffinityGroup"] = image.AffinityGroup;
                row[Constants.FieldNameConstants.Category] = image.Category;
                //row["Location"] = image.Location;
                row[Constants.FieldNameConstants.LogicalSizeInGB] = image.Size;
                row[Constants.FieldNameConstants.Label] = image.Label;
                row[Constants.FieldNameConstants.Description] = image.Description;
                row[Constants.FieldNameConstants.AssociatedVHD] = image.Name;
                row[Constants.FieldNameConstants.OS] = image.OSName;

                dt.Rows.Add(row);
            }

            return dt;
        }

        /// <summary>
        /// Method Use to Add Virual Machine Image.
        /// </summary>
        /// <param name="imageLabel">Label of the Image.</param>
        /// <param name="mediaLink">VHD Link.</param>
        /// <param name="imageName">Name of the Image.</param>
        /// <param name="osName">Operating Sysytem Name.</param>
        public static void UpdateImage(string imageName, string imageLabel, string mediaLink, string osName, string subscriptionID)
        {
            string version = "";
            string uriFormat = AzureFrameWork.Util.UrlConstants.UpdateDeleteVMImageUrl;
            Uri uri = new Uri(String.Format(uriFormat, subscriptionID));

            AddImageRequest imageRequest = new AddImageRequest() { Label = imageLabel, MediaLink = mediaLink, Name = imageName, OSName = osName };
            HttpWebRequest request = AzureFrameWork.Util.CreateWebRequest(uri, version, APIVersions.MethodType.PUT.ToString(), imageRequest);

            XDocument responseBody = null;

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
            if(statusCode == HttpStatusCode.OK)
                //TODO:Need to add the code 

            if (response.ContentLength > 0)
            {
                using (XmlReader reader = XmlReader.Create(response.GetResponseStream()))
                {
                    responseBody = XDocument.Load(reader);
                }
            }
            response.Close();
        }

        private static DataTable CreateImagesTable()
        {
            DataTable dt = new DataTable();
            dt.TableName = Constants.TableNameConstants.Images;
            dt.Columns.Add(Constants.FieldNameConstants.Category, typeof(string));
            dt.Columns.Add(Constants.FieldNameConstants.Label, typeof(string));
            dt.Columns.Add(Constants.FieldNameConstants.LogicalSizeInGB, typeof(string));
            dt.Columns.Add(Constants.FieldNameConstants.AssociatedVHD, typeof(string));
            dt.Columns.Add(Constants.FieldNameConstants.OS, typeof(string));
            dt.Columns.Add(Constants.FieldNameConstants.Description, typeof(string));

            return dt;
        }
    }
}
