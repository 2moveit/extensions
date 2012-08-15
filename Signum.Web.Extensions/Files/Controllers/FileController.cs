﻿#region usings
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using Signum.Services;
using Signum.Utilities;
using Signum.Entities;
using Signum.Web;
using Signum.Engine;
using Signum.Engine.Operations;
using Signum.Entities.Operations;
using Signum.Engine.Basics;
using Signum.Web.Extensions.Properties;
using System.IO;
using Signum.Entities.Files;
using Signum.Entities.Basics;
using System.Text;
using System.Net;
using Signum.Engine.Files;
using System.Reflection;
using Signum.Web.Controllers;
using Signum.Web.PortableAreas;
#endregion

namespace Signum.Web.Files
{
    public class FileController : Controller
    {
        [HttpPost]
        public PartialViewResult PartialView(
            string prefix,
            string fileType,
            int? sfId)
        {
            Type type = typeof(FilePathDN);
            FilePathDN entity = null;
            if (entity == null || entity.GetType() != type || sfId != (entity as IIdentifiable).TryCS(e => e.IdOrNull))
            {
                if (sfId.HasValue)
                    entity = Database.Retrieve<FilePathDN>(sfId.Value);
                else
                {
                    entity = new FilePathDN(EnumLogic<FileTypeDN>.ToEnum(fileType));
                }
            }
            ViewData["IdValueField"] = prefix;
            ViewData["FileType"] = fileType;

            string url = Navigator.Manager.EntitySettings[type].OnPartialViewName(entity);

            return Navigator.PartialView(this, entity, prefix, url);
        }

        public ActionResult Upload()
        {
            bool shouldSaveFilePath = !RuntimeInfo.FromFormValue((string)Request["fileParentRuntimeInfo"]).IsNew;

            string fileName = Request.Files.Cast<string>().Single();

            string prefix = fileName.Substring(0, fileName.IndexOf(FileLineKeys.File) - 1);

            RuntimeInfo info = RuntimeInfo.FromFormValue((string)Request.Form[TypeContextUtilities.Compose(prefix, EntityBaseKeys.RuntimeInfo)]);

            IFile file;

            if (info.RuntimeType == typeof(FilePathDN))
            {
                string fileType = (string)Request.Form[TypeContextUtilities.Compose(prefix, FileLineKeys.FileType)];
                if (!fileType.HasText())
                    throw new InvalidOperationException("Couldn't create FilePath with unknown FileType for file '{0}'".Formato(fileName));

                file = new FilePathDN(EnumLogic<FileTypeDN>.ToEnum(fileType));
            }
            else
            {
                file = (IFile)Activator.CreateInstance(info.RuntimeType);
            }

            HttpPostedFileBase hpf = Request.Files[fileName] as HttpPostedFileBase;

            file.FileName = Path.GetFileName(hpf.FileName);
            file.BinaryFile = hpf.InputStream.ReadAllBytes();

            if (shouldSaveFilePath)
                ((IdentifiableEntity)file).Save();
            else
                Session[Request.Form[ViewDataKeys.TabId] + prefix] = file;


            return UploadResult(prefix, file, shouldSaveFilePath);
        }

        public ContentResult UploadDropped()
        {
            bool shouldSaveFilePath = !RuntimeInfo.FromFormValue((string)Request.Headers["X-" + EntityBaseKeys.RuntimeInfo]).IsNew;

            string fileName = Request.Headers["X-FileName"];

            string prefix = Request.Headers["X-Prefix"];

            RuntimeInfo info = RuntimeInfo.FromFormValue((string)Request.Headers["X-" + TypeContextUtilities.Compose(prefix, EntityBaseKeys.RuntimeInfo)]);
            IFile file;
            if (info.RuntimeType == typeof(FilePathDN))
            {
                string fileType = (string)Request.Headers["X-" + FileLineKeys.FileType];
                if (!fileType.HasText())
                    throw new InvalidOperationException("Couldn't create FilePath with unknown FileType for file '{0}'".Formato(prefix));

                file = new FilePathDN(EnumLogic<FileTypeDN>.ToEnum(fileType));
            }
            else
            {
                file = (IFile)Activator.CreateInstance(info.RuntimeType);
            }

            file.FileName = fileName;
            file.BinaryFile = Request.InputStream.ReadAllBytes();

            if (shouldSaveFilePath)
                ((IdentifiableEntity)file).Save();
            else
                Session[Request.Headers["X-" + ViewDataKeys.TabId] + prefix] = file;

            return UploadResult(prefix, file, shouldSaveFilePath);
        }

        private ContentResult UploadResult(string prefix, IFile file, bool shouldHaveSaved)
        {
            StringBuilder sb = new StringBuilder();
            //Use plain javascript not to have to add also the reference to jquery in the result iframe
            sb.AppendLine("<html><head><title>-</title></head><body>");
            sb.AppendLine("<script type='text/javascript'>");
            sb.AppendLine("var parDoc = window.parent.document;");

            if (/*file.TryCS(f => f.IdOrNull) != null ||*/ !shouldHaveSaved)
            {
                RuntimeInfo ri = file is EmbeddedEntity ? new RuntimeInfo((EmbeddedEntity)file) : new RuntimeInfo((IIdentifiable)file);

                sb.AppendLine("parDoc.getElementById('{0}loading').style.display='none';".Formato(prefix));
                sb.AppendLine("parDoc.getElementById('{0}').innerHTML='{1}';".Formato(TypeContextUtilities.Compose(prefix, EntityBaseKeys.ToStrLink), file.FileName));
                sb.AppendLine("parDoc.getElementById('{0}').value='{1}';".Formato(TypeContextUtilities.Compose(prefix, EntityBaseKeys.RuntimeInfo), ri.ToString()));
                sb.AppendLine("parDoc.getElementById('{0}').style.display='none';".Formato(TypeContextUtilities.Compose(prefix, "DivNew")));
                sb.AppendLine("parDoc.getElementById('{0}').style.display='block';".Formato(TypeContextUtilities.Compose(prefix, "DivOld")));
                sb.AppendLine("parDoc.getElementById('{0}').style.display='block';".Formato(TypeContextUtilities.Compose(prefix, "btnRemove")));
                sb.AppendLine("var frame = parDoc.getElementById('{0}'); frame.parentNode.removeChild(frame);".Formato(TypeContextUtilities.Compose(prefix, "frame")));
            }
            else
            {
                sb.AppendLine("parDoc.getElementById('{0}loading').style.display='none';".Formato(prefix));
                sb.AppendLine("window.parent.alert('{0}');".Formato(Resources.ErrorSavingFile));
            }

            sb.AppendLine("</script>");
            sb.AppendLine("</body></html>");

            return Content(sb.ToString());
        }

        public FileResult Download(int? filePathID)
        {
            if (filePathID == null)
                throw new ArgumentException("filePathID");

            FilePathDN fp = Database.Retrieve<FilePathDN>(filePathID.Value);

            return File(fp.FullPhysicalPath, MimeType.FromFileName(fp.FullPhysicalPath), fp.FileName);
        }

        public ActionResult DownloadFile(int? fileID)
        {
            if (fileID == null)
                throw new ArgumentNullException("fileID");

            FileDN file = Database.Retrieve<FileDN>(fileID.Value);

            return new StaticContentResult(file.BinaryFile, file.FileName);
        }
    }
}
