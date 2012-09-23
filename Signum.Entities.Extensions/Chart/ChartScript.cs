﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Signum.Utilities;
using Signum.Entities.DynamicQuery;
using Signum.Entities.Files;
using System.Xml.Linq;
using System.Collections;
using System.Text.RegularExpressions;
using Signum.Entities.Reflection;
using Signum.Utilities.Reflection;
using System.Reflection;

namespace Signum.Entities.Chart
{
    [Serializable]
    public class ChartScriptDN : Entity
    {
        [NotNullable, SqlDbType(Size = 100), UniqueIndex]
        string name;
        [StringLengthValidator(AllowNulls = false, Min = 3, Max = 100)]
        public string Name
        {
            get { return name; }
            set { SetToStr(ref name, value, () => Name); }
        }

        Lite<FileDN> icon;
        public Lite<FileDN> Icon
        {
            get { return icon; }
            set { Set(ref icon, value, () => Icon); }
        }

        [NotNullable, SqlDbType(Size = int.MaxValue)]
        string script;
        [StringLengthValidator(AllowNulls = false, Min = 3)]
        public string Script
        {
            get { return script; }
            set { Set(ref script, value, () => Script); }
        }

        GroupByChart groupBy;
        public GroupByChart GroupBy
        {
            get { return groupBy; }
            set { Set(ref groupBy, value, () => GroupBy); }
        }

        [NotifyCollectionChanged, ValidateChildProperty]
        MList<ChartScriptColumnDN> columns = new MList<ChartScriptColumnDN>();
        public MList<ChartScriptColumnDN> Columns
        {
            get { return columns; }
            set { Set(ref columns, value, () => Columns); }
        }

        static Expression<Func<ChartScriptDN, string>> ToStringExpression = e => e.name;
        public override string ToString()
        {
            return ToStringExpression.Evaluate(this);
        }

        public string ColumnsToString()
        {
            return Columns.ToString(a => a.ColumnType.ToString(), "|");
        }

        protected override string ChildPropertyValidation(ModifiableEntity sender, System.Reflection.PropertyInfo pi)
        {
            var column = sender as ChartScriptColumnDN;

            if (column != null && pi.Is(() => column.IsGroupKey))
            {
                if (column.IsGroupKey)
                {
                    if (!ChartUtils.Flag(ChartColumnType.Groupable, column.ColumnType))
                        return "{0} can not be true for {1}".Formato(pi.NiceName(), column.ColumnType.NiceToString());
                }
            }

            return base.ChildPropertyValidation(sender, pi);
        }

        protected override string PropertyValidation(System.Reflection.PropertyInfo pi)
        {
            if (pi.Is(() => GroupBy))
            {
                if (GroupBy == GroupByChart.Always || GroupBy == GroupByChart.Optional)
                {
                    if (!Columns.Any(a => a.IsGroupKey))
                        return "{0} {1} requires some key columns".Formato(pi.NiceName(), groupBy.NiceToString());
                }
                else
                {
                    if (Columns.Any(a => a.IsGroupKey))
                        return "{0} {1} should not have key".Formato(pi.NiceName(), groupBy.NiceToString());
                }
            }

            if (pi.Is(() => Script))
            {
                if (!Regex.IsMatch(Script, @"^\s*function\s+DrawChart\s*\(\s*chart\s*,\s*data\s*\)\s*{.*}\s*$", RegexOptions.Singleline))
                {
                    return "{0} should be a definition of function DrawChart(chart, data)".Formato(pi.NiceName());
                }
            }

            return base.PropertyValidation(pi);
        }

        protected override void PreSaving(ref bool graphModified)
        {
            Columns.ForEach((c, i) => c.Index = i);

            base.PreSaving(ref graphModified);
        }

        protected override void PostRetrieving()
        {
            Columns.Sort(c => c.Index);
            
            base.PostRetrieving();
        }

        public XDocument ExportXml()
        {
            var icon = Icon == null? null: Icon.Entity;

            return new XDocument(new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("ChartScript",
                    new XAttribute("GroupBy", GroupBy.ToString()),
                    new XElement("Columns",
                        Columns.Select(c => new XElement("Column",
                            new XAttribute("DisplayName", c.DisplayName),
                            new XAttribute("ColumnType", c.ColumnType.ToString()),
                            c.IsGroupKey ? new XAttribute("IsGroupKey", true) : null,
                            c.IsOptional ? new XAttribute("IsOptional", true) : null,
                            c.Parameter1 != null ? c.Parameter1.ExportXml(1): null,  
                            c.Parameter2 != null ? c.Parameter2.ExportXml(2): null,  
                            c.Parameter3 != null ? c.Parameter3.ExportXml(3): null  
                         ))),
                    icon == null ? null :
                    new XElement("Icon",
                        new XAttribute("FileName", icon.FileName),
                        new XCData(Convert.ToBase64String(Icon.Entity.BinaryFile))),
                    new XElement("Script", new XCData(Script))));
                    
        }

        public void ImportXml(XDocument doc, string name, bool force = false)
        {
            XElement script = doc.Root;

            GroupByChart groupBy = script.Attribute("GroupBy").Value.ToEnum<GroupByChart>();

            List<ChartScriptColumnDN> columns = script.Element("Columns").Elements("Column").Select(c => new ChartScriptColumnDN
            {
                DisplayName = c.Attribute("DisplayName").Value,
                ColumnType = c.Attribute("ColumnType").Value.ToEnum<ChartColumnType>(),
                IsGroupKey = c.Attribute("IsGroupKey").Let(a => a != null && a.Value == "true"),
                IsOptional = c.Attribute("IsOptional").Let(a => a != null && a.Value == "true"),
                Parameter1 = ChartScriptParameterDN.ImportXml(c, 1),
                Parameter2 = ChartScriptParameterDN.ImportXml(c, 2),
                Parameter3 = ChartScriptParameterDN.ImportXml(c, 3)
            }).ToList();

            if (!IsNew && !force)
                AsssertColumns(columns);

            this.Name = name;
            this.GroupBy = groupBy;

            if (this.Columns.Count == columns.Count)
            {
                this.Columns.ZipForeach(columns, (o, n) =>
                {
                    o.ColumnType = n.ColumnType;
                    o.DisplayName = n.DisplayName;
                    o.IsGroupKey = n.IsGroupKey;
                    o.IsOptional = n.IsOptional;
                }); 
            }
            else
            {
                this.Columns = columns.ToMList();
            }

            this.Script = script.Elements("Script").Nodes().OfType<XCData>().Single().Value;

            var newFile = script.Element("Icon").TryCC(icon => new FileDN
            {
                FileName = icon.Attribute("FileName").Value,
                BinaryFile = Convert.FromBase64String(icon.Nodes().OfType<XCData>().Single().Value),
            });

            if (newFile == null)
            {
                Icon = null;
            }
            else
            {
                if (icon == null || icon.Entity.FileName != newFile.FileName || !AreEqual(icon.Entity.BinaryFile, newFile.BinaryFile))
                    Icon = newFile.ToLiteFat();
            }
        }

        static bool AreEqual(byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length)
                return false;

            for (int i = 0; i < a1.Length; i++)
            {
                if (a1[i] != a2[i])
                    return false;
            }

            return true;
        }

        private void AsssertColumns(List<ChartScriptColumnDN> columns)
        {
            string errors = Columns.ZipOrDefault(columns, (o, n) =>
            {
                if (o == null)
                {
                    if (!n.IsOptional)
                        return "Adding non optional column {0}".Formato(n.DisplayName);
                }
                else if (n == null)
                {
                    if (o.IsOptional)
                        return "Removing non optional column {0}".Formato(o.DisplayName);
                }
                else if (n.ColumnType != o.ColumnType)
                {
                    return "The column type of '{0}' ({1}) does not match with '{2}' ({3})".Formato(
                        o.DisplayName, o.ColumnType,
                        n.DisplayName, n.ColumnType);
                }

                return null;
            }).NotNull().ToString("\r\n");

            if (errors.HasText())
                throw new FormatException("The columns doesn't match: \r\n" + errors);
        }

        public bool IsCompatibleWith(IChartBase chartBase)
        {
            if (GroupBy == GroupByChart.Always && !chartBase.GroupResults)
                return false;

            if (GroupBy == GroupByChart.Never && chartBase.GroupResults)
                return false;

            return Columns.ZipOrDefault(chartBase.Columns, (s, c) =>
            {
                if (s == null)
                    return false;

                if (c == null)
                    return s.IsOptional;

                return ChartUtils.IsChartColumnType(c.Token, s.ColumnType);
            }).All(a => a);
        }

        public bool HasChanges()
        {
            var graph = GraphExplorer.FromRoot(this);
            return graph.Any(a => a.SelfModified);
        }
    }

    public enum ChartScriptOperations
    {
        Clone,
        Delete
    }

    public enum GroupByChart
    {
        Always,
        Optional,
        Never
    }

    [Serializable]
    public class ChartScriptColumnDN : EmbeddedEntity       
    {
        int index;
        public int Index
        {
            get { return index; }
            set { Set(ref index, value, () => Index); }
        }

        [NotNullable, SqlDbType(Size = 80)]
        string displayName;
        [StringLengthValidator(AllowNulls = false, Min = 3, Max = 80)]
        public string DisplayName
        {
            get { return displayName; }
            set { Set(ref displayName, value, () => DisplayName); }
        }

        bool isOptional;
        public bool IsOptional
        {
            get { return isOptional; }
            set { Set(ref isOptional, value, () => IsOptional); }
        }
     
        [ForceForeignKey]
        ChartColumnType columnType;
        public ChartColumnType ColumnType
        {
            get { return columnType; }
            set { Set(ref columnType, value, () => ColumnType); }
        }

        bool isGroupKey;
        public bool IsGroupKey
        {
            get { return isGroupKey; }
            set { Set(ref isGroupKey, value, () => IsGroupKey); }
        }

        ChartScriptParameterDN parameter1;
        public ChartScriptParameterDN Parameter1
        {
            get { return parameter1; }
            set { Set(ref parameter1, value, () => Parameter1); }
        }

        ChartScriptParameterDN parameter2;
        public ChartScriptParameterDN Parameter2
        {
            get { return parameter2; }
            set { Set(ref parameter2, value, () => Parameter2); }
        }

        ChartScriptParameterDN parameter3;
        public ChartScriptParameterDN Parameter3
        {
            get { return parameter3; }
            set { Set(ref parameter3, value, () => Parameter3); }
        }
    }

    [Flags]
    public enum ChartColumnType
    {
        [Code("i")] Integer = 1,
        [Code("r")] Real = 2,
        [Code("d")] Date = 4,
        [Code("dt")] DateTime = 8,
        [Code("s")] String = 16, //Guid
        [Code("s")] Lite = 32,
        [Code("e")] Enum = 64, // Boolean 

        [Code("G")] Groupable = ChartColumnTypeUtils.GroupMargin | Integer | Date | String | Lite | Enum,
        [Code("M")] Magnitude = ChartColumnTypeUtils.GroupMargin | Integer | Real,
        [Code("P")] Positionable = ChartColumnTypeUtils.GroupMargin | Integer | Real | Date | DateTime | Enum
    }


    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public sealed class CodeAttribute : Attribute
    {
        string code;
        public CodeAttribute(string code)
        {
            this.code = code;
        }

        public string Code
        {
            get { return code; }
        }
    }

    public static class ChartColumnTypeUtils
    {
       public const int GroupMargin = 0x10000000;

       static Dictionary<ChartColumnType, string> codes = EnumFieldCache.Get(typeof(ChartColumnType)).ToDictionary(
           a => (ChartColumnType)a.Key,
           a => a.Value.SingleAttribute<CodeAttribute>().Code);

       public static string GetCode(this ChartColumnType columnType)
       {
           return codes[columnType];
       }

       public static string GetComposedCode(this ChartColumnType columnType)
       {
           var result = columnType.GetCode();

           if (result.HasText())
               return result;

           return EnumExtensions.GetValues<ChartColumnType>()
               .Where(a => (int)a < ChartColumnTypeUtils.GroupMargin && columnType.HasFlag(a))
               .ToString(GetCode, ",");
       }

       static Dictionary<string, ChartColumnType> fromCodes = EnumFieldCache.Get(typeof(ChartColumnType)).ToDictionary(
           a => a.Value.SingleAttribute<CodeAttribute>().Code,
           a => (ChartColumnType)a.Key);

       public static string TryParse(string code, out ChartColumnType type)
       {
           if(fromCodes.TryGetValue(code, out type))
               return null;
                
           return "{0} is not a valid type code, use {1} instead".Formato(code, fromCodes.Keys.CommaOr());
       }

       public static string TryParseComposed(string code, out ChartColumnType type)
       {
           type = default(ChartColumnType);
           foreach (var item in code.Split(','))
	       {
               ChartColumnType temp;
                string error = TryParse(item,   out temp);

               if(error.HasText())
                   return error;

               type |= temp;
           }
           return null;
       }

       //public static string GetDescription(this ChartColumnType columnType)
       //{
       //    switch (columnType)
       //    {
       //        case ChartColumnType.Integer: return "Number with no fractional part";
       //        case ChartColumnType.Real: return "Number with fractional part";
       //        case ChartColumnType.Date: return "Date with no time part";
       //        case ChartColumnType.DateTime: return "Date with time part";
       //        case ChartColumnType.String: return "Sequence of characters";
       //        case ChartColumnType.Lite: return "Reference to an entity";
       //        case ChartColumnType.Enum: return "Set of pre-defined identifiers";
       //        case ChartColumnType.Groupable: return "Can be grouped (Integer | Date | String | Lite | Enum)";
       //        case ChartColumnType.Magnitude: return "Can be added up (Integer | Real)";
       //        case ChartColumnType.Positionable: return "Can be positioned (Integer | Real | Date | DateTime | Enum)";
       //        default: throw new ArgumentException("Unexpected columnType");
       //    }
       //}
    }

    public class ChartScriptParameterDN : EmbeddedEntity
    {
        [NotNullable, SqlDbType(Size = 50)]
        string name;
        [StringLengthValidator(AllowNulls = false, Min = 3, Max = 50)]
        public string Name
        {
            get { return name; }
            set { Set(ref name, value, () => Name); }
        }

        ChartParameterType type;
        public ChartParameterType Type
        {
            get { return type; }
            set
            {
                if (Set(ref type, value, () => Type))
                {
                    ValueDefinition = null;
                }
            }
        }

        [NotNullable, SqlDbType(Size = 150)]
        string valueDefinition;
        [StringLengthValidator(AllowNulls = false, Min = 3, Max = 150)]
        public string ValueDefinition
        {
            get { return valueDefinition; }
            set
            {
                if (Set(ref valueDefinition, value, () => ValueDefinition))
                {
                    enumValues = null;
                    numberInterval = null;
                }
            }
        }

       
        protected override string PropertyValidation(PropertyInfo pi)
        {
            if (pi.Is(() => Name))
            {
                if (!Reflector.ValidIdentifier(Name))
                    return "{0} should be a vaid interval".Formato(pi.NiceName());
            }

            if (pi.Is(() => ValueDefinition) && ValueDefinition != null)
            {
                if (Type == ChartParameterType.Enum)
                    return EnumValueList.TryParse(valueDefinition, out enumValues);
                else
                    return NumberInterval.TryParse(valueDefinition, out  numberInterval);
            }

            return base.PropertyValidation(pi);
        }

        public string DefaultValue(QueryToken token)
        {
            switch (Type)
            {
                case ChartParameterType.Enum:return GetEnumValues().DefaultValue(token);
                case ChartParameterType.Number:return GetNumberInterval().DefaultValue.ToString();
                case ChartParameterType.String: return ValueDefinition;
                default: throw new InvalidOperationException();
            }
        }


        public string Valdidate(string parameter, QueryToken token)
        {
            switch (Type)
            {
                case ChartParameterType.Enum: return GetEnumValues().Validate(parameter, token);
                case ChartParameterType.Number: return GetNumberInterval().Validate(parameter);
                case ChartParameterType.String: return null;
                default: throw new InvalidOperationException();
            }
        }

        [Ignore]
        EnumValueList enumValues;
        public EnumValueList GetEnumValues()
        {
            if (Type != ChartParameterType.Enum)
                throw new InvalidOperationException("Type is not Enum");

            if (enumValues != null)
                return enumValues;

            string error = EnumValueList.TryParse(valueDefinition, out enumValues);
            if (error.HasText())
                throw new FormatException(error);

            return enumValues;
        }

        [Ignore]
        NumberInterval numberInterval;
        public NumberInterval GetNumberInterval()
        {
            if (Type != ChartParameterType.Number)
                throw new InvalidOperationException("Type is not Number");

            if (numberInterval != null)
                return numberInterval;
            string error = NumberInterval.TryParse(valueDefinition, out numberInterval);
            if (error.HasText())
                throw new FormatException(error);

            return numberInterval;
        }


        public class NumberInterval
        {
            public decimal DefaultValue; 
            public decimal? MinValue;
            public decimal? MaxValue;

            public static string TryParse(string valueDefinition, out NumberInterval interval)
            {
                interval = null;
                var m = Regex.Match(valueDefinition, @"^\s*(?<def>.+)\[(?<min>.+)?\s*,\s*(?<max>.+)?\s*\]\s*$");

                if (!m.Success)
                    return "Invalid number interval, [min?, max?]";

                interval = new NumberInterval();

                if (!ReflectionTools.TryParse<decimal>(m.Groups["def"].Value, out interval.DefaultValue))
                    return "Invalid default value";

                if (!ReflectionTools.TryParse<decimal?>(m.Groups["min"].Value, out interval.MinValue))
                    return "Invalid min value";

                if (!ReflectionTools.TryParse<decimal?>(m.Groups["max"].Value, out interval.MaxValue))
                    return "Invalid max value";

                return null;
            }

            public override string ToString()
            {
                return "{0}[{1},{2}]".Formato(DefaultValue, MinValue, MaxValue);
            }

            public string Validate(string parameter)
            {
                decimal value;
                if (!decimal.TryParse(parameter, out value))
                    return "{0} is not a valid number".Formato(parameter);

                if (MinValue.HasValue && value < MinValue)
                    return "{0} is lesser than the minimum {1}".Formato(value, MinValue);

                if (MaxValue.HasValue && MaxValue < value)
                    return "{0} is grater than the maximum {1}".Formato(value, MinValue);

                return null;
            }
        }

        public class EnumValueList : List<EnumValue> 
        {
            public static string TryParse(string valueDefinition, out EnumValueList list)
            {
                list = new EnumValueList();
                foreach (var item in valueDefinition.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    EnumValue val;
                    string error = EnumValue.TryParse(item, out val);
                    if (error.HasText())
                        return error;

                    list.Add(val);
                }
                return null;
            }

            internal string Validate(string parameter, QueryToken token)
            {
                var enumValue = this.SingleOrDefault(a=>a.Name == parameter);

                if (enumValue == null)
                    return "{0} is not in the list".Formato(parameter);

                if (!enumValue.CompatibleWith(token))
                    return "{0} is not compatible with {1}".Formato(parameter, token.NiceName());

                return null;
            }

            internal string DefaultValue(QueryToken token)
            {
                return this.Where(a => a.CompatibleWith(token)).SingleEx(() => "No default parameter value for {0} found".Formato(token.NiceName())).Name;
            }
        }

        public class EnumValue
        {
            public string Name;
            public ChartColumnType? TypeFilter;

            public override string ToString()
            {
                if (TypeFilter == null)
                    return Name;

                return "{0} ({1})".Formato(Name, TypeFilter.Value.GetComposedCode());
            }

            public static string TryParse(string value, out EnumValue enumValue)
            {
                var m = Regex.Match(value, @"^\s*(?<name>[^\]\(]*)\s*(?<filter>\([^\)]*\))?\s*\$");

                if (!m.Success)
                {
                    enumValue = null;
                    return "Invalid ChartSciptParameterValue";
                }

                enumValue = new EnumValue()
                {
                    Name = m.Groups["name"].Value.Trim()
                };

                if (string.IsNullOrEmpty(enumValue.Name))
                    return "Parameter has no name";

                string composedCode = m.Groups["filter"].Value;
                if (!composedCode.HasText())
                    return null;

                ChartColumnType filter;

                string error = ChartColumnTypeUtils.TryParseComposed(composedCode, out filter);
                if (error.HasText())
                    return enumValue.Name + ": " + error;

                enumValue.TypeFilter = filter;

                return null;
            }

            public bool CompatibleWith(QueryToken token)
            {
                return TypeFilter == null || ChartUtils.IsChartColumnType(token, TypeFilter.Value);
            }
        }

        internal XElement ExportXml(int index)
        {
            return new XElement("Parameter" + index,
                new XAttribute("Name", Name),
                new XAttribute("Type", Type),
                new XAttribute("ValueDefinition", ValueDefinition));
        }

        internal static ChartScriptParameterDN ImportXml(XElement c, int index)
        {
            var element = c.Element("Parameter" + index);

            if (element == null)
                return null;

            return new ChartScriptParameterDN
            {
                Name = c.Attribute("Name").Value,
                Type = c.Attribute("Type").Value.ToEnum<ChartParameterType>(),
                ValueDefinition = c.Attribute("ValueDefinition").Value,
            }; 
        }
    }

   

    public enum ChartParameterType
    {
        Enum, 
        Number,
        String,
    }
}
