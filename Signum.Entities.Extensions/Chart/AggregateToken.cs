﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Entities.DynamicQuery;
using Signum.Entities.Chart;
using Signum.Utilities;
using System.Linq.Expressions;

namespace Signum.Entities.Chart
{
    [Serializable]
    public class AggregateToken : QueryToken
    {
        public AggregateFunction AggregateFunction { get; private set; }

        public AggregateToken(QueryToken parent, AggregateFunction function)
            : base(parent)
        {
            if (function == AggregateFunction.Count)
            {
                if (parent != null)
                    throw new ArgumentException("parent should be null for Count function"); 
            }
            else
            {
                if (parent == null)
                    throw new ArgumentNullException("parent");
            }

            this.AggregateFunction = function;
        }

        public override string ToString()
        {
            return "[{0}]".Formato(AggregateFunction.NiceToString());
        }

        public override string NiceName()
        {
            if(AggregateFunction == AggregateFunction.Count)
                return AggregateFunction.NiceToString();

            return "{0} of {1}".Formato(AggregateFunction.NiceToString(), Parent.ToString());  
        }

        public override string Format
        {
            get
            {
                if (AggregateFunction == AggregateFunction.Count || AggregateFunction == AggregateFunction.Average)
                    return null;
                return Parent.Format;
            }
        }

        public override string Unit
        {
            get
            {
                if (AggregateFunction == AggregateFunction.Count)
                    return null;
                return Parent.Unit;
            }
        }

        public override Type Type
        {
            get
            {
                if (AggregateFunction == AggregateFunction.Count)
                    return typeof(int);

                var pType = Parent.Type;
                var pTypeUn = Parent.Type.UnNullify();

                if (AggregateFunction == AggregateFunction.Average &&
                    (pTypeUn == typeof(int) || pTypeUn == typeof(long) || pTypeUn == typeof(bool)))
                {
                    return pType.IsNullable() ? typeof(double?) : typeof(double);
                }

                if (pTypeUn == typeof(bool))
                {
                    return pType.IsNullable() ? typeof(int?) : typeof(int);
                }

                return pType;
            }
        }

        public override string Key
        {
            get { return AggregateFunction.ToString(); }
        }

        protected override List<QueryToken> SubTokensInternal()
        {
            return new List<QueryToken>();
        }

        protected override Expression BuildExpressionInternal(BuildExpressionContext context)
        {
            throw new InvalidOperationException("AggregateToken does not support this method");
        }

        public override PropertyRoute GetPropertyRoute()
        {
            if (AggregateFunction == AggregateFunction.Count)
                return null;

            return Parent.GetPropertyRoute(); 
        }

        public override Implementations Implementations()
        {
            return null;
        }

        public override bool IsAllowed()
        {
            if (AggregateFunction == Chart.AggregateFunction.Count)
                return true;

            return Parent.IsAllowed();
        }

        public override QueryToken Clone()
        {
            if (AggregateFunction == AggregateFunction.Count)
                return new AggregateToken(null, AggregateFunction.Count);
            else
                return new AggregateToken(Parent.Clone(), AggregateFunction.Count);
        }

        internal Type ConvertTo()
        {
            if (AggregateFunction == Chart.AggregateFunction.Count)
                return null;

            var pu = Parent.Type.UnNullify();

            if (AggregateFunction == Chart.AggregateFunction.Average && (pu == typeof(int) || pu == typeof(long) || pu == typeof(bool)))
                return Parent.Type.IsNullable() ? typeof(double?) : typeof(double);

            if (pu == typeof(bool))
                return Parent.Type.IsNullable() ? typeof(int?) : typeof(int);

            return null;
        }
    }
}
