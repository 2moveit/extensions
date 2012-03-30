﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Engine.Maps;
using Signum.Engine.DynamicQuery;
using System.Reflection;
using Signum.Entities.Chart;
using Signum.Entities;
using Signum.Utilities.Reflection;
using Signum.Utilities;
using System.Drawing;
using Signum.Entities.Basics;

namespace Signum.Engine.Chart
{
    public static class ChartColorLogic
    {
        public static readonly Lazy<Dictionary<Type, Dictionary<int, Color>>> Colors = GlobalLazy.Create(() =>
              Database.Query<ChartColorDN>()
              .Select(cc => new { cc.Related.RuntimeType, cc.Related.Id, cc.Color.Argb })
              .AgGroupToDictionary(a => a.RuntimeType, gr => gr.ToDictionary(a => a.Id, a => Color.FromArgb(a.Argb))))
        .InvalidateWith(typeof(ChartColorDN));

        public static readonly int Limit = 360; 

        public static void Start(SchemaBuilder sb, DynamicQueryManager dqm)
        {
            if (sb.NotDefined(MethodInfo.GetCurrentMethod()))
            {
                sb.Include<ChartColorDN>();

                dqm[typeof(ChartColorDN)] = (from cc in Database.Query<ChartColorDN>()
                                             select new
                                             {
                                                 Entity = cc.ToLite(),
                                                 cc.Related,
                                                 cc.Color,
                                             }).ToDynamic();
            }
        }

        public static void CreateNewPalette(Type type)
        {
            AssertFewEntities(type);

            var dic = Database.RetrieveAllLite(type).Select(l => new ChartColorDN { Related = l.ToLite<IdentifiableEntity>() }).ToDictionary(a => a.Related);

            dic.SetRange(Database.Query<ChartColorDN>().Where(c => c.Related.RuntimeType == type).ToDictionary(a=>a.Related));

            double[] bright = dic.Count < 18 ? new double[]{.80}:
                            dic.Count < 72 ? new double[]{.90, .50}:
                            new double[]{1, .60, .30};

            var hues = (dic.Count / bright.Length);

            var hueStep = 360 / hues;

            var values = dic.Values.ToList();

            for (int i = 0; i < bright.Length; i++)
            {
                for (int h = 0; h < hues; h++)
                {
                    values[i * hues + h].Color = new ColorDN { Argb = ColorExtensions.FromHsv(h * hueStep, .8, bright[i]).ToArgb() };
                }
            }

            values.SaveList();
        }

        public static void AssertFewEntities(Type type)
        {
            int count = giCount.GetInvoker(type)();

            if (count > Limit)
                throw new ApplicationException("Too many {0} ({1}), maximum is {2}".Formato(type.NicePluralName(), count, Limit));
        }

        public static void SavePalette(ChartPaletteModel model)
        {
            using (Transaction tr = new Transaction())
            {
                Type type = model.Type.ToType();

                giDeleteColors.GetInvoker(type)();

                model.Colors.Where(a => a.Color != null).SaveList();
                tr.Commit();
            }
        }

        static readonly GenericInvoker<Func<int>> giCount = new GenericInvoker<Func<int>>(() => Count<IdentifiableEntity>());
        static int Count<T>() where T : IdentifiableEntity
        {
            return Database.Query<T>().Count();
        }

        static readonly GenericInvoker<Func<int>> giDeleteColors = new GenericInvoker<Func<int>>(() => DeleteColors<IdentifiableEntity>());
        static int DeleteColors<T>() where T : IdentifiableEntity
        {
            return (from t in Database.Query<T>() // To filter by type conditions
                    join cc in Database.Query<ChartColorDN>() on t.ToLite<IdentifiableEntity>() equals cc.Related
                    select cc).UnsafeDelete();
        }

        public static ChartPaletteModel GetPalette(Type type)
        {
            AssertFewEntities(type);

            var dic = ChartColorLogic.Colors.Value.TryGetC(type);

            return new ChartPaletteModel
            {
                Type = type.ToTypeDN(),
                Colors = Database.RetrieveAllLite(type).Select(l => new ChartColorDN
                {
                    Related = l.ToLite<IdentifiableEntity>(),
                    Color = dic.TryGetS(l.Id).TrySC(c => new ColorDN { Argb = c.ToArgb() })
                }).ToMList()
            };
        }

        public static Color? ColorFor(Lite lite)
        {
            return Colors.Value.TryGetC(lite.RuntimeType).TryGetS(lite.Id);
        }
    }
}