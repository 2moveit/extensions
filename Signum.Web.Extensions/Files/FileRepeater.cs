﻿#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Collections;
using System.Linq.Expressions;
using System.Web.Mvc.Html;
using Signum.Entities;
using Signum.Entities.Reflection;
using Signum.Utilities;
using System.Configuration;
using Signum.Web.Properties;
using Signum.Engine;
#endregion

namespace Signum.Web.Files
{
    public class FileRepeater : EntityRepeater
    {
        public Enum FileType { get; set; }

        bool asyncUpload = true;
        public bool AsyncUpload
        {
            get { return asyncUpload; }
            set { asyncUpload = value; }
        }
        
        public FileRepeater(Type type, object untypedValue, Context parent, string controlID, PropertyRoute route)
            : base(type, untypedValue, parent, controlID, route)
        {
            
        }

        public override string ToJS()
        {
            return "new SF.FRep(" + this.OptionsJS() + ")";
        }

        protected override string DefaultCreate()
        {
            return FileRepeater.JsCreate(this, DefaultJsViewOptions()).ToJS();
        }

        public static JsInstruction JsCreate(FileRepeater frep, JsViewOptions viewOptions)
        {
            return new JsInstruction(() => "{0}.create({1})".Formato(frep.ToJS(), viewOptions.TryCC(v => v.ToJS())));
        }
    }
}
