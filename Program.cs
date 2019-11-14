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
using System.Windows.Forms;

using ImportFromVisio.SecSvc;

namespace ImportFromVisio
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var login = new LoginDialog();
            var result = login.ShowDialog();

            if (result != DialogResult.OK)
                return;

            var mainForm = new MainForm();
            mainForm.ShowDialog();

            var secSvc = ServiceManager.GetSecurityService();
            secSvc.SignOut();
        }
    }
}