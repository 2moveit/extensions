﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Signum.Engine.Operations;
using Signum.Entities.Operations;
using Signum.Utilities;
using Signum.Entities;
using System.Web;
using Signum.Entities.Basics;
using System.Reflection;
using Signum.Entities.Files;

namespace Signum.Web.Files
{
    public static class FilesClient
    {
        //public static string ViewPrefix = "files/Views";

        public static void Start()
        {
            if (Navigator.Manager.NotDefined(MethodInfo.GetCurrentMethod()))
            {
                FileRepositoryDN.OverridenPhisicalCurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

                Navigator.AddSetting(new EntitySettings<FilePathDN>(EntityType.Default));
            }
        }
    }
}
