using System;
using System.Configuration;
using System.Net;
using System.Xml;
using System.Xml.Linq;

namespace AzureFrameWork
{
    public static class ManageVM
    {
        /// <summary>
        /// 
        /// </summary>
        /// 

        private static string subscriberID = ConfigurationSettings.AppSettings[Constants.FieldNameConstants.SubscriptionID].ToString();

        public static bool AddRole(string serviceName, string deploymentName, AddRoleRequest requestObject)
        {
            bool result = false;
            string version = VMOperations.RoleVersionConstants.VER2012;
            string uriFormat = AzureFrameWork.Util.UrlConstants.AddRole;
            //Uri uri = new Uri(String.Format(uriFormat, subscriberID, "testvmdns", "testvmsep12"));
            Uri uri = new Uri(String.Format(uriFormat, subscriberID, serviceName, deploymentName));

            string requestObject1 = @"<PersistentVMRole xmlns=""http://schemas.microsoft.com/windowsazure"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
               <RoleName>test1strole</RoleName>
               <RoleType>PersistentVMRole</RoleType>   
            <ConfigurationSets>   
            <ConfigurationSet>
                <ConfigurationSetType>WindowsProvisioningConfiguration</ConfigurationSetType>
                     <ComputerName>mytestcomputer</ComputerName>
                     <AdminPassword>CloudTeam1</AdminPassword> 
                     <ResetPasswordOnFirstLogon>false</ResetPasswordOnFirstLogon> 
                     <EnableAutomaticUpdates>true</EnableAutomaticUpdates>
                     <TimeZone>Pacific Standard Time</TimeZone>
                     <DomainJoin>
                        <Credentials>
                           <Domain>cloudapp.net</Domain>
                           <Username>testuser</Username>
                           <Password>testpassword</Password>
                        </Credentials>
                        <JoinDomain>NULL</JoinDomain>
                        <MachineObjectOU>MyOu</MachineObjectOU>
                     </DomainJoin>
                     <StoredCertificateSettings>
                        <CertificateSetting>
                           <StoreLocation>LocalMachine</StoreLocation>
                           <StoreName>My</StoreName>
                           <Thumbprint>275FE85811F5C40D33B395AC225BFDEBA0CBD7BA</Thumbprint>
                        </CertificateSetting>
                     </StoredCertificateSettings>
             </ConfigurationSet>  
            </ConfigurationSets>   
                <AvailabilitySetName>NULL</AvailabilitySetName>
               <DataVirtualHardDisks/>
                <OSVirtualHardDisk>
                    <HostCaching>ReadOnly</HostCaching>
                    <DiskLabel/>
                    <DiskName>mydisk</DiskName>
                    <MediaLink></MediaLink>
                    <SourceImageName>MSFT__Win2K8R2SP1-Datacenter-201208.01-en.us-30GB.vhd</SourceImageName>
                </OSVirtualHardDisk>      
                <RoleSize>ExtraSmall</RoleSize>      
            </PersistentVMRole>";

            //string requestObject1 = Util.CreateXml(requestObject);
            try
            {
                HttpWebRequest request = AzureFrameWork.Util.CreateWebRequest(uri, version, APIVersions.MethodType.POST.ToString(), requestObject1);
                //HttpWebRequest request = AzureFrameWork.Util.CreateWebRequest(uri, version, APIVersions.MethodType.POST.ToString(), requestObject); 

                XDocument responseBody = null;

                HttpWebResponse response;
                TrustAllCert trust = new TrustAllCert();
                ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(trust.OnValidationCallback);
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                    //request.BeginGetResponse(new AsyncCallback(FinishWebrequest), request);
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


                if (response.StatusCode == HttpStatusCode.Created)
                {
                    result = true;
                }
                else
                {
                    //TODO: Need to write the logic
                    //makeing some changes
                    //Conflict check
                }
            }
            catch (WebException ex)
            {
                throw ex.InnerException;
            }
            finally
            {

            }

            return result;
        }

        static void FinishWebrequest(IAsyncResult result)
        {
            HttpWebResponse response = (result.AsyncState as HttpWebRequest).EndGetResponse(result) as HttpWebResponse;
            if (response.ContentLength > 0)
            {
                using (XmlReader reader = XmlReader.Create(response.GetResponseStream()))
                {
                    XDocument responseBody = XDocument.Load(reader);
                }
            }
            response.Close();
        }

        /// <summary>
        /// Method used to create the Deployment for the Virtual Machine.
        /// </summary>
        /// <param name="requestObject">Windows Request Object</param>
        /// <param name="serviceName">Name of the Service.</param>
        public static bool CreateVMDeployment(WinVMDeploymentRequest requestObject, string serviceName)
        {
            bool result = false;
            string version = VMOperations.RoleVersionConstants.VER2012;
            string uriFormat = AzureFrameWork.Util.UrlConstants.CreateVMDeployment;
            Uri uri = new Uri(String.Format(uriFormat, subscriberID, serviceName));

            try
            {
                string requestObj = @"<Deployment xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"" xmlns=""http://schemas.microsoft.com/windowsazure"">
  <Name>test1stdep</Name>
  <Label>test1strole</Label>
  <DeploymentSlot>Production</DeploymentSlot>
  <RoleList>
    <Role>
      <RoleName>test1strole</RoleName>
      <RoleType>PersistentVMRole</RoleType>
      <ConfigurationSets>
        <ConfigurationSet>
          <ConfigurationSetType>WindowsProvisioningConfiguration</ConfigurationSetType>
          <ComputerName>Administrator</ComputerName>
          <AdminPassword>Tm1lZGlhMTAh</AdminPassword>
          <StoredCertificateSettings>
            <CertificateSetting>
              <StoreLocation>LocalMachine</StoreLocation>
              <StoreName>My</StoreName>
              <Thumbprint>275FE85811F5C40D33B395AC225BFDEBA0CBD7BA</Thumbprint>
            </CertificateSetting>
          </StoredCertificateSettings>
        </ConfigurationSet>
        <ConfigurationSet>
          <ConfigurationSetType>NetworkConfiguration</ConfigurationSetType>
          <InputEndpoints>
            <InputEndpoint>
              <LocalPort>8080</LocalPort>
              <Name>AzureService</Name>
              <Port>8080</Port>
              <Protocol>TCP</Protocol>
            </InputEndpoint>
          </InputEndpoints>
        </ConfigurationSet>
      </ConfigurationSets>
      <OSVirtualHardDisk>
        <SourceImageName>testimage11sep</SourceImageName>
      </OSVirtualHardDisk>
      <RoleSize>ExtraSmall</RoleSize>
    </Role>
  </RoleList>
  <VirtualNetworkName>minevnetwork</VirtualNetworkName>
</Deployment>";

                HttpWebRequest request = AzureFrameWork.Util.CreateWebRequest(uri, version, APIVersions.MethodType.POST.ToString(), requestObj); //requestObject);

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
                

                if (response.StatusCode == HttpStatusCode.Created)
                {
                    result = true;
                }
                else
                {
                    //TODO: Need to write the logic
                }
            }
            catch (WebException ex)
            {
                throw ex.InnerException;
            }
            finally
            {
               
            }

            return result;
        }

        /// <summary>
        /// Method used to get the details of the existing Role.
        /// </summary>
        /// <param name="serviceName">Name of the Service.</param>
        /// <param name="deploymentName">Deployment Name.</param>
        /// <param name="roleName">Role Name.</param>
        /// <returns>True/False</returns>
        public static RoleResponse GetVMRole(string serviceName, string deploymentName, string roleName)
        {
            RoleResponse roleResponse = null;
            string version = VMOperations.RoleVersionConstants.VER2012;
            string uriFormat = AzureFrameWork.Util.UrlConstants.GetVMRole;
            Uri uri = new Uri(String.Format(uriFormat, subscriberID, serviceName, deploymentName, roleName));

            try
            {
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
                if (response.ContentLength > 0 && response.StatusCode == HttpStatusCode.OK)
                {
                    using (XmlReader reader = XmlReader.Create(response.GetResponseStream()))
                    {
                        //responseBody = XDocument.Load(reader);
                        System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(RoleResponse));
                        roleResponse = (RoleResponse)serializer.Deserialize(reader);
                    }
                }
                else
                {
                    //TODO: Need to write the logic
                }
                response.Close();
            }
            catch (WebException ex)
            {
                throw ex.InnerException;
            }
            finally{}

            return roleResponse;
        }

        /// <summary>
        /// Method use to Start the VM Role.
        /// </summary>
        /// <param name="serviceName">Name of the Service.</param>
        /// <param name="deploymentName">Deployment Name.</param>
        /// <param name="roleName">Role Name.</param>
        /// <returns>True/False</returns>
        public static bool StartVMRole(string serviceName, string deploymentName, string roleName)
        {
            bool result = false;
            string version = VMOperations.RoleVersionConstants.VER2012;
            string uriFormat = AzureFrameWork.Util.UrlConstants.StartStopVMRole;
            //Uri uri = new Uri(String.Format(uriFormat, subscriberID, serviceName, deploymentName, roleName));
            Uri uri = new Uri(String.Format(uriFormat, subscriberID, "autovm", "autovm13sep", roleName));
            StartRoleRequest startRoleRequest = new StartRoleRequest() { OperationType = "StartRoleOperation" };
            HttpWebRequest request = AzureFrameWork.Util.CreateWebRequest(uri, version, APIVersions.MethodType.POST.ToString(), startRoleRequest);

            HttpWebResponse response = null;
            TrustAllCert trust = new TrustAllCert();
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(trust.OnValidationCallback);

            try
            {
                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.Accepted)
                    result = true;
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
                        XDocument responseBody = XDocument.Load(reader);
                    }
                }
            response.Close(); 
            return result;
        }

        /// <summary>
        /// Method use to Shutdown the VM Role.
        /// </summary>
        /// <param name="serviceName">Name of the Service.</param>
        /// <param name="deploymentName">Deployment Name.</param>
        /// <param name="roleName">Role Name.</param>
        /// <returns>True/False</returns>
        public static bool ShutDownVMRole(string serviceName, string deploymentName, string roleName)
        {
            bool result = false;
            string version = VMOperations.RoleVersionConstants.VER2012;
            string uriFormat = AzureFrameWork.Util.UrlConstants.StartStopVMRole;
            //Uri uri = new Uri(String.Format(uriFormat, subscriberID, serviceName, deploymentName, roleName));
            Uri uri = new Uri(String.Format(uriFormat, subscriberID, "autovm", "autovm13sep", roleName));
            ShutDownRoleRequest shutdownRoleRequest = new ShutDownRoleRequest() { OperationType = "ShutdownRoleOperation" };
            HttpWebRequest request = AzureFrameWork.Util.CreateWebRequest(uri, version, APIVersions.MethodType.POST.ToString(), shutdownRoleRequest);

            HttpWebResponse response = null;
            TrustAllCert trust = new TrustAllCert();
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(trust.OnValidationCallback);

            try
            {
                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.Accepted)
                    result = true;
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
                    XDocument responseBody = XDocument.Load(reader);
                }
            }
            response.Close(); 
            return result;
        }

        /// <summary>
        /// Method use to Restart the VM Role.
        /// </summary>
        /// <param name="serviceName">Name of the Service.</param>
        /// <param name="deploymentName">Deployment Name.</param>
        /// <param name="roleName">Role Name.</param>
        /// <returns>True/False</returns>
        public static bool RestartVMRole(string serviceName, string deploymentName, string roleName)
        {
            bool result = false;
            string version = VMOperations.RoleVersionConstants.VER2012;
            string uriFormat = AzureFrameWork.Util.UrlConstants.StartStopVMRole;
            //Uri uri = new Uri(String.Format(uriFormat, subscriberID, serviceName, deploymentName, roleName));
            Uri uri = new Uri(String.Format(uriFormat, subscriberID, "autovm", "autovm13sep", roleName));
            RestartRoleRequest restartRoleRequest = new RestartRoleRequest() { OperationType = "RestartRoleOperation" };
            HttpWebRequest request = AzureFrameWork.Util.CreateWebRequest(uri, version, APIVersions.MethodType.POST.ToString(), restartRoleRequest);

            HttpWebResponse response = null;
            TrustAllCert trust = new TrustAllCert();
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(trust.OnValidationCallback);

            try
            {
                response = (HttpWebResponse)request.GetResponse();
                if(response.StatusCode == HttpStatusCode.Accepted)
                    result = true;
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
                    XDocument responseBody = XDocument.Load(reader);
                }
            }
            response.Close(); 
            return result;
        }

        /// <summary>
        /// Method use to Delete the VM Role.
        /// </summary>
        /// <param name="serviceName">Name of the Service.</param>
        /// <param name="deploymentName">Deployment Name.</param>
        /// <param name="roleName">Role Name.</param>
        /// <returns>True/False</returns>
        public static bool DeleteVMRole(string serviceName, string deploymentName, string roleName)
        {
            bool result = false;
            string version = VMOperations.RoleVersionConstants.VER2012;
            string uriFormat = AzureFrameWork.Util.UrlConstants.StartStopVMRole;
            Uri uri = new Uri(String.Format(uriFormat, subscriberID, serviceName, deploymentName, roleName));
            HttpWebRequest request = AzureFrameWork.Util.CreateWebRequest(uri, version, APIVersions.MethodType.DELETE.ToString(), null);

            HttpWebResponse response = null;
            TrustAllCert trust = new TrustAllCert();
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(trust.OnValidationCallback);

            try
            {
                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                    result = true;
            }
            catch (WebException ex)
            {
                throw ex.InnerException;
            }
            finally { response.Close(); }
            return result;
        }

        /// <summary>
        /// Method use to Download the VM Role Instance Rdp file.
        /// </summary>
        /// <param name="serviceName">Name of the Service.</param>
        /// <param name="deploymentName">Deployment Name.</param>
        /// <param name="roleInstanceName">Role Instance Name.</param>
        /// <returns>True/False</returns>
        public static bool DownloadVMRdpFile(string serviceName, string deploymentName, string roleInstanceName)
        {
            bool result = false;
            string version = VMOperations.RoleVersionConstants.VER2012;
            string uriFormat = AzureFrameWork.Util.UrlConstants.DownloadRdpFile;
            Uri uri = new Uri(String.Format(uriFormat, subscriberID, serviceName, deploymentName, roleInstanceName));
            HttpWebRequest request = AzureFrameWork.Util.CreateWebRequest(uri, version, APIVersions.MethodType.GET.ToString(), null);

            HttpWebResponse response = null;
            TrustAllCert trust = new TrustAllCert();
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(trust.OnValidationCallback);

            try
            {
                response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                    result = true;
            }
            catch (WebException ex)
            {
                throw ex.InnerException;
            }
            finally { response.Close(); }
            return result;
        }

        /// <summary>
        /// Method used to create the Deployment for the Virtual Machine.
        /// </summary>
        /// <param name="requestObject">Windows Request Object</param>
        /// <param name="serviceName">Name of the Service.</param>
        public static bool CaptureVMRole(string serviceName)
        {
            bool result = false;
            string version = VMOperations.RoleVersionConstants.VER2012;
            string uriFormat = AzureFrameWork.Util.UrlConstants.CaptureRole;
            Uri uri = new Uri(String.Format(uriFormat, subscriberID, "autovm", "autovm13sep", "autovm13sep"));

            string requestObject = @"<CaptureRoleOperation xmlns=""http://schemas.microsoft.com/windowsazure"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
                                    <OperationType>CaptureRoleOperation</OperationType>
   <PostCaptureAction>Reprovision</PostCaptureAction>
   <ProvisioningConfiguration> 
      <ConfigurationSetType>WindowsProvisioningConfiguration<ConfigurationSetType>
      <ComputerName>capturename</ComputerName>
      <AdminPassword>admin123</AdminPassword> 
      <ResetPasswordOnFirstLogon>false</ResetPasswordOnFirstLogon > 
      <EnableAutomaticUpdate>false</EnableAutomationUpdate>  
      <TimeZone>Pacific Standard Time</TimeZone>
      <DomainJoin>
         <Credentials>
            <Domain>domain-to-join</Domain>
            <Username>user-name-in-the-domain</Username>
            <Password>password-for-the-user-name</Password>
         </Credentials>
         <JoinDomain>domain-to-join</JoinDomain>
         <MachineObjectOU>distinguished-name-of-the-ou<MachineObjectOU>
      </DomainJoin>
      <StoredCertificateSettings>
         <CertificateSetting>
            <StoreLocation>LocalMachine</StoreLocation>
            <StoreName>My</StoreName>
            <Thumbprint>275fe85811f5c40d33b395ac225bfdeba0cbd7ba</Thumbprint>
         </CertificateSetting>
      </StoredCertificateSettings>
   </ProvisioningConfiguration> 
   <TargetImageLabel>captureimage1</TargetImageLabel>
   <TargetImageName>captureimage1</TargetImageName>
</CaptureRoleOperation>";


            try
            {
                HttpWebRequest request = AzureFrameWork.Util.CreateWebRequest(uri, version, APIVersions.MethodType.POST.ToString(), requestObject);

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


                if (response.StatusCode == HttpStatusCode.Created)
                {
                    result = true;
                }
                else
                {
                    //TODO: Need to write the logic
                }
            }
            catch (WebException ex)
            {
                throw ex.InnerException;
            }
            finally
            {

            }

            return result;
        }

        /// <summary>
        /// Method used to reimage  the role.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="deploymentSlot">Name of Deployment Slot.</param>
        /// <param name="roleName">Name of the Role.</param>
        /// <returns>bool</returns>
        public static bool ReImageRole(string serviceName, string deploymentSlot, string roleName)
        {
            bool result = false;
            string version = VMOperations.RoleVersionConstants.VER2010;
            string uriFormat = AzureFrameWork.Util.UrlConstants.ReImageRole;
            Uri uri = new Uri(String.Format(uriFormat, subscriberID, serviceName, deploymentSlot.ToLower(), roleName));

            try
            {
                HttpWebRequest request = AzureFrameWork.Util.CreateWebRequest(uri, version, APIVersions.MethodType.POST.ToString(), null);
                request.ContentLength = 0;
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
                if (response.ContentLength > 0)
                {
                    using (XmlReader reader = XmlReader.Create(response.GetResponseStream()))
                    {
                        responseBody = XDocument.Load(reader);
                    }
                }
                else
                {
                    //TODO: Need to write the logic
                }
                response.Close();
            }
            catch (WebException ex)
            {
                throw ex.InnerException;
            }
            finally { }

            return result;
        }
    }
}
