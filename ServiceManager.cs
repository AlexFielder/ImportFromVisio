/*=====================================================================
  
  This file is part of the Autodesk Vault API Code Samples.

  Copyright (C) Autodesk Inc.  All rights reserved.

THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
PARTICULAR PURPOSE.
=====================================================================*/

using System;
using System.Collections.Generic;
using System.Text;

using ImportFromVisio.DocExSvc;
using ImportFromVisio.SecSvc;

namespace ImportFromVisio
{
    /// <summary>
	/// A singleton class for managing web service connections
	/// </summary>
    public class ServiceManager
    {
        private static ServiceManager m_mgr = null;

        private SecurityService m_secSvc = null;
        private DocumentServiceExtensions m_docExSvc = null;
        private LoginInfo m_loginInfo;


        private ServiceManager()
        { }

        /// <summary>
        /// Gets the ServiceManager instance
        /// </summary>
        public static ServiceManager GetServiceManager()
        {
            if (m_mgr == null)
                m_mgr = new ServiceManager();
            return m_mgr;
        }

        /// <summary>
        /// Gets the security service object, or creates one if needed.
        /// </summary>
        public static SecurityService GetSecurityService(LoginInfo loginInfo)
        {
            ServiceManager m_mgr = GetServiceManager();

            if (m_mgr.m_secSvc == null)
            {
                m_mgr.m_loginInfo = loginInfo;
                SecurityService secSvc = new SecurityService();

                secSvc.Url = m_mgr.SetSvcUrl(secSvc);
                secSvc.SecurityHeaderValue = new SecSvc.SecurityHeader();
                secSvc.SignIn(loginInfo.Username, loginInfo.Password, loginInfo.Vault);

                m_mgr.m_secSvc = secSvc;
            }

            return m_mgr.m_secSvc;
        }

        /// <summary>
        /// Gets the security service.  It's assumed that the client is aready
        /// logged in
        /// </summary>
        public static SecurityService GetSecurityService()
        {
            ServiceManager m_mgr = GetServiceManager();
            return m_mgr.m_secSvc;
        }

        /// <summary>
        /// Gets the Document service object, or creates one if needed.
        /// </summary>
        public static DocumentServiceExtensions GetDocumentServiceExtensions()
        {
            ServiceManager m_mgr = GetServiceManager();

            if (m_mgr.m_docExSvc == null)
            {
                m_mgr.m_docExSvc = new DocumentServiceExtensions();
                m_mgr.m_docExSvc.Url = m_mgr.SetSvcUrl(m_mgr.m_docExSvc);
                m_mgr.m_docExSvc.SecurityHeaderValue = new DocExSvc.SecurityHeader();
                m_mgr.m_docExSvc.SecurityHeaderValue.Ticket = m_mgr.m_secSvc.SecurityHeaderValue.Ticket;
                m_mgr.m_docExSvc.SecurityHeaderValue.UserId = m_mgr.m_secSvc.SecurityHeaderValue.UserId;
            }

            return m_mgr.m_docExSvc;
        }

        /// <summary>
        /// Set the URL of the web service.  This is how you point the service to a specific server. 
        /// </summary>
        private string SetSvcUrl(System.Web.Services.Protocols.SoapHttpClientProtocol svc)
        {
            UriBuilder url = new UriBuilder(svc.Url);
            url.Host = m_loginInfo.Server;

            if (m_loginInfo.Port != 0)
                url.Port = m_loginInfo.Port;

            if (m_loginInfo.SSL)
                url.Scheme = "https";

            svc.Url = url.Uri.ToString();
            return svc.Url;
        }
    }
}
