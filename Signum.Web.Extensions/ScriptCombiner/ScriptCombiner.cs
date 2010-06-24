﻿using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Web.Configuration;
using System.Security;
using System.Configuration;
using System.Security.Permissions;
using System.Net;
using Signum.Utilities;

namespace Signum.Web.ScriptCombiner
{
    public class CssScriptCombiner : ScriptCombiner
    {
        public CssScriptCombiner()
        {
            this.contentType = "text/css";
            this.cacheable = true;
            this.gzipable = true;
            this.resourcesFolder = "../Content";
        }

        protected override string Minify(string content)
        {
            content = Regex.Replace(content, "/\\*.+?\\*/", "", RegexOptions.Singleline);
            content = Regex.Replace(content, "(\\s{2,}|\\t+|\\r+|\\n+)", string.Empty);
            content = content.Replace(" {", "{");
            content = content.Replace("{ ", "{");
            content = content.Replace(" :", ":");
            content = content.Replace(": ", ":");
            content = content.Replace(", ", ",");
            content = content.Replace("; ", ";");
            content = content.Replace(";}", "}");
            content = Regex.Replace(content, "/\\*[^\\*]*\\*+([^/\\*]*\\*+)*/", "$1");
            content = Regex.Replace(content, "(?<=[>])\\s{2,}(?=[<])|(?<=[>])\\s{2,}(?=&nbsp;)|(?<=&ndsp;)\\s{2,}(?=[<])", string.Empty);

            content = Regex.Replace(content, "[^\\}]+\\{\\}", string.Empty);  //Eliminamos reglas vacías

            Regex color = new Regex("#([A-Fa-f0-9]{6})");
            foreach (Match CurrentMatch in color.Matches(content))
            {
                string coincidencia = CurrentMatch.Groups[1].Value;
                if (coincidencia[0] == coincidencia[1]
                    && coincidencia[2] == coincidencia[3]
                    && coincidencia[4] == coincidencia[5])
                    content = content.Replace("#" + coincidencia, ("#" + coincidencia[0] + coincidencia[2] + coincidencia[4]).ToLower());
            }
            return content;
        }

        public override string ReadFile(string fileName)
        {
            string file = context.Server.MapPath((!string.IsNullOrEmpty(resourcesFolder) ? (resourcesFolder + "/") : "") + fileName.Replace("%2f", "/"));

            return ReplaceRelativeImg(File.ReadAllText(file), fileName);
        }

        protected override string Extension { get { return "css"; } }
    }

    public class JsScriptCombiner : ScriptCombiner {
        public JsScriptCombiner()
        {
            this.contentType = "application/x-javascript";
            this.cacheable = true;
            this.gzipable = true;
            this.resourcesFolder = "../Scripts";
        }
        protected override string Extension { get { return "js"; } }
        protected override string Minify(string content)
        {
            var minifier = new JavaScriptMinifier();
            return minifier.Minify(content);
        }
    }


    public class AreaJsScriptCombiner : JsScriptCombiner
    {
        public AreaJsScriptCombiner()
        {
            this.cacheable = true;
            this.gzipable = true;
            this.resourcesFolder = null;
        }

        public override DateTime GetLastModifiedDate(string fileName)
        {
            AssemblyResourceStore ars = AssemblyResourceManager.GetResourceStoreFromVirtualPath("~/" + fileName);

            FileInfo fileInfo = new FileInfo(ars.typeToLocateAssembly.Assembly.Location);
            return fileInfo.LastWriteTimeUtc;
        }

        public override string ReadFile(string fileName)
        {
            AssemblyResourceStore ars = AssemblyResourceManager.GetResourceStoreFromVirtualPath("~/" + fileName);
            Stream stream = ars.GetResourceStream("~/" + fileName);
            byte[] bytes  = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(bytes, 0, (int) stream.Length);
            return Encoding.UTF8.GetString(bytes);
        }
    }

    public class AreaCssScriptCombiner : CssScriptCombiner
    {
        public AreaCssScriptCombiner()
        {
            this.cacheable = true;
            this.gzipable = true;
            this.resourcesFolder = null;
        }

        public override DateTime GetLastModifiedDate(string fileName)
        {
            AssemblyResourceStore ars = AssemblyResourceManager.GetResourceStoreFromVirtualPath("~/" + fileName);

            FileInfo fileInfo = new FileInfo(ars.typeToLocateAssembly.Assembly.Location);
            return fileInfo.LastWriteTimeUtc;
        }

        public override string ReadFile(string fileName)
        {
            AssemblyResourceStore ars = AssemblyResourceManager.GetResourceStoreFromVirtualPath("~/" + fileName);
            Stream stream = ars.GetResourceStream("~/" + fileName);
            byte[] bytes = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(bytes, 0, (int)stream.Length);
            string content = Encoding.UTF8.GetString(bytes);

            return ReplaceRelativeImg(content, fileName);           
        }
    }

    public abstract class ScriptCombiner
    {
        /// <summary>
        /// Sets the Content-Type HTTP Header
        /// </summary>
        protected string contentType;

        /// <summary>
        /// The folder where the scripts are stored
        /// </summary>
        public string resourcesFolder;

        /// <summary>
        /// Indicates if the output file should be cached
        /// </summary>
        protected bool cacheable;

        /// <summary>
        /// Indicates if the output stream should be gzipped (depending on client request too)
        /// </summary>
        protected bool gzipable;

        /// <summary>
        /// Current version (increase to get a fresh output file)
        /// </summary>
        protected string version;

        /// <summary>
        /// File extension in order to prevent files reading manipulating the url
        /// </summary>
        protected abstract string Extension{get;}

        protected abstract string Minify(string content);

        private readonly static TimeSpan CACHE_DURATION = TimeSpan.FromDays(2);
        internal HttpContextBase context;
        string lastModifiedDateKey = "-lmd";

        public virtual string ReadFile(string fileName)
        {
            string file = context.Server.MapPath((!string.IsNullOrEmpty(resourcesFolder) ? (resourcesFolder + "/") : "") + fileName.Replace("%2f", "/"));            
            return File.ReadAllText(file);
        }

        public virtual DateTime GetLastModifiedDate(string fileName)
        {
            return File.GetLastWriteTimeUtc(fileName);
        }

        public void Process(string[] files, string path, HttpContextBase context)
        {
            if (path != null) resourcesFolder = "../" + path.Replace("%2f", "/");

            this.context = context;
            DateTime lmServer = DateTime.MinValue;
            foreach (string fileName in files)
            {
                DateTime fileLastModified = GetLastModifiedDate(fileName);
                if (fileLastModified > lmServer) lmServer = fileLastModified;
            }

            //check dates
            if (context.Request["HTTP_IF_MODIFIED_SINCE"] != null)
            {
                DateTime lmBrowser = DateTime.Parse(context.Request["HTTP_IF_MODIFIED_SINCE"].ToString()).ToUniversalTime();

                if (lmServer.Date == lmBrowser.Date && Math.Truncate(lmServer.TimeOfDay.TotalSeconds) <= lmBrowser.TimeOfDay.TotalSeconds)
                {
                    context.Response.Clear();
                    context.Response.StatusCode = (int)HttpStatusCode.NotModified;
                    context.Response.SuppressContent = true;
                    return;
                }
            }

            context.Response.Cache.SetLastModified(lmServer);

            // Decide if browser supports compressed response
            bool isCompressed = this.CanGZip(context.Request);

            // If the set has already been cached, write the response directly from
            // cache. Otherwise generate the response and cache it
            if (cacheable && !this.WriteFromCache(isCompressed, lmServer) || !cacheable)
            {
                using (MemoryStream memoryStream = new MemoryStream(8092))
                {
                    // Decide regular stream or gzip stream based on whether the response can be compressed or not
                    //using (Stream writer = isCompressed ?  (Stream)(new GZipStream(memoryStream, CompressionMode.Compress)) : memoryStream)
                   using (Stream writer = isCompressed ?
                               (Stream)(new GZipStream(memoryStream, CompressionMode.Compress)) :
                               memoryStream)
                          {

                        // Read the files into one big string
                        StringBuilder allScripts = new StringBuilder();
                        foreach (string fileName in files)
                        {
                            if (fileName.EndsWith(Extension.StartsWith(".") ? Extension : "." + Extension))
                            {
                                try
                                {
                                    allScripts.Append(ReadFile(fileName));
                                }
                                catch (Exception) { }
                            }
                        }
                        string minified = allScripts.ToString();
                        minified = Minify(minified);
                        
                        // Send minfied string to output stream
                        byte[] bts = Encoding.UTF8.GetBytes(minified);
                        writer.Write(bts, 0, bts.Length);
                    }

                    // Cache the combined response so that it can be directly written
                    // in subsequent calls 
                    byte[] responseBytes = memoryStream.ToArray();
                    if (cacheable)
                    {
                        context.Cache.Insert(GetCacheKey(version),
                        responseBytes, null, System.Web.Caching.Cache.NoAbsoluteExpiration,
                        CACHE_DURATION);

                        context.Cache.Insert(GetCacheKey(version) + lastModifiedDateKey,
                        lmServer, null, System.Web.Caching.Cache.NoAbsoluteExpiration,
                        CACHE_DURATION);
                    }

                    // Generate the response
                    this.WriteBytes(responseBytes, isCompressed);

                }
            }
        }

        private bool WriteFromCache(bool isCompressed, DateTime lastModificationDate)
        {
            byte[] responseBytes = context.Cache[GetCacheKey()] as byte[];

            if (responseBytes == null || responseBytes.Length == 0)
                return false;

            //Compare with the date of the server cache content
            DateTime lmd = (DateTime) context.Cache[GetCacheKey() + lastModifiedDateKey];
            if (lmd != lastModificationDate) return false;

            this.WriteBytes(responseBytes, isCompressed);
            return true;
        }

        private void WriteBytes(byte[] bytes, bool isCompressed)
        {
            HttpResponseBase response = context.Response;

            response.AppendHeader("Content-Length", bytes.Length.ToString());
            response.ContentType = contentType;

            if (isCompressed)
                response.AppendHeader("Content-Encoding", "gzip");
            else
                response.AppendHeader("Content-Encoding", "utf-8");

            //response.Cache.SetCacheability(HttpCacheability.Public);
            //response.Cache.SetExpires(DateTime.Now.Add(CACHE_DURATION));
            //response.Cache.SetMaxAge(CACHE_DURATION);
            //response.Cache.AppendCacheExtension("must-revalidate, proxy-revalidate");
            response.ContentEncoding = Encoding.Unicode;
            response.OutputStream.Write(bytes, 0, bytes.Length);
            response.Flush();
        }

        private string GetCacheKey(string setName)
        {
            return "HttpCombiner." + setName;
        }

        private string GetCacheKey()
        {
            return "HttpCombiner." + GetUniqueKey(context);
        }

        private bool CanGZip(HttpRequestBase request)
        {
            string acceptEncoding = request.Headers["Accept-Encoding"];
            if (!string.IsNullOrEmpty(acceptEncoding) &&
                 (acceptEncoding.Contains("gzip") || acceptEncoding.Contains("deflate")))
                return true;
            return false;
        }

        public string ReplaceRelativeImg(string content, string fileName)
        {

            string[] parts = fileName.Split('/');
            //replace relative paths

            Match m = Regex.Match(content, "(?<begin>url\\([\"\']?)(?<content>.+?)[\"\'\\)]+");

            StringBuilder sb = new StringBuilder();
            int firstIndex = 0;

            while (m.Success)
            {
                string relativePath = m.Groups["content"].ToString();

                int partsIndex = parts.Length - 1;

                while (relativePath.StartsWith("../"))
                {
                    partsIndex--;
                    relativePath = relativePath.Substring(3);
                }

                if (partsIndex < 0) break;

                StringBuilder sbPath = new StringBuilder();
                for (int i = 0; i < partsIndex; i++)
                {
                    sbPath.Append(parts[i]);
                    sbPath.Append("/");
                }
                sbPath.Append(relativePath);

                sb.Append(content.Substring(firstIndex, m.Index - firstIndex));
                sb.Append("url(\"{0}\")".Formato("../" + sbPath.ToString()));
                firstIndex = m.Index + m.Length;
                m = m.NextMatch();
            }
            sb.Append(content.Substring(firstIndex, content.Length - firstIndex));

            return sb.ToString();
        }



      /*  public static string GetScriptTags(string setName, int version)
        {
            string result = null;
#if (DEBUG)
            foreach (string fileName in GetScriptFileNames(setName))
            {
                result += String.Format("\n<script type=\"text/javascript\" src=\"{0}?v={1}\"></script>", VirtualPathUtility.ToAbsolute(fileName), version);
            }
#else
        result += String.Format("<script type=\"text/javascript\" src=\"ScriptCombiner.axd?s={0}&v={1}\"></script>", setName, version);
#endif
            return result;
        }*/


        public string GetUniqueKey(HttpContextBase context)
        {
            return context.Request.Url.PathAndQuery.ToUpperInvariant().GetHashCode().ToString("x", System.Globalization.CultureInfo.InvariantCulture);
        }    
    }
}