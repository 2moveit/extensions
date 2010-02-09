﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Utilities;
using Signum.Entities.Basics;
using Signum.Entities.Operations;

namespace Signum.Entities.Authorization
{
    [Serializable]
    public class RuleQueryDN : IdentifiableEntity
    {
        RoleDN role;
        [NotNullValidator]
        public RoleDN Role
        {
            get { return role; }
            set { Set(ref role, value, () => Role); }
        }

        QueryDN query;
        [NotNullValidator]
        public QueryDN Query
        {
            get { return query; }
            set { Set(ref query, value, () => Query); }
        }

        bool allowed;
        public bool Allowed
        {
            get { return allowed; }
            set { Set(ref allowed, value, () => Allowed); }
        }
    }

    [Serializable]
    public class RuleFacadeMethodDN : IdentifiableEntity
    {
        RoleDN role;
        [NotNullValidator]
        public RoleDN Role
        {
            get { return role; }
            set { Set(ref role, value, () => Role); }
        }

        FacadeMethodDN serviceOperation;
        [NotNullValidator]
        public FacadeMethodDN ServiceOperation
        {
            get { return serviceOperation; }
            set { Set(ref serviceOperation, value, () => ServiceOperation); }
        }

        bool allowed;
        public bool Allowed
        {
            get { return allowed; }
            set { Set(ref allowed, value, () => Allowed); }
        }
    }

    [Serializable]
    public class RulePermissionDN : IdentifiableEntity
    {
        RoleDN role;
        [NotNullValidator]
        public RoleDN Role
        {
            get { return role; }
            set { Set(ref role, value, () => Role); }
        }

        PermissionDN permission;
        [NotNullValidator]
        public PermissionDN Permission
        {
            get { return permission; }
            set { Set(ref permission, value, () => Permission); }
        }

        bool allowed;
        public bool Allowed
        {
            get { return allowed; }
            set { Set(ref allowed, value, () => Allowed); }
        }
    }

    [Serializable]
    public class RuleOperationDN : IdentifiableEntity
    {
        RoleDN role;
        [NotNullValidator]
        public RoleDN Role
        {
            get { return role; }
            set { Set(ref role, value, () => Role); }
        }

        OperationDN operation;
        [NotNullValidator]
        public OperationDN Operation
        {
            get { return operation; }
            set { Set(ref operation, value, () => Operation); }
        }

        bool allowed;
        public bool Allowed
        {
            get { return allowed; }
            set { Set(ref allowed, value, () => Allowed); }
        }
    }

    [Serializable]
    public class RulePropertyDN : IdentifiableEntity
    {
        RoleDN role;
        [NotNullValidator]
        public RoleDN Role
        {
            get { return role; }
            set { Set(ref role, value, () => Role); }
        }

        PropertyDN property;
        [NotNullValidator]
        public PropertyDN Property
        {
            get { return property; }
            set { Set(ref property, value, () => Property); }
        }

        Access access;
        public Access Access
        {
            get { return access; }
            set { Set(ref access, value, () => Access); }
        }
    }

    [Serializable]
    public class RuleEntityGroupDN : IdentifiableEntity
    {
        RoleDN role;
        [NotNullValidator]
        public RoleDN Role
        {
            get { return role; }
            set { Set(ref role, value, () => Role); }
        }

        EntityGroupDN group;
        [NotNullValidator]
        public EntityGroupDN Group
        {
            get { return group; }
            set { Set(ref group, value, () => Group); }
        }

        bool allowedIn;
        public bool AllowedIn
        {
            get { return allowedIn; }
            set { Set(ref allowedIn, value, () => AllowedIn); }
        }

        bool allowedOut;
        public bool AllowedOut
        {
            get { return allowedOut; }
            set { Set(ref allowedOut, value, () => AllowedOut); }
        }
    }

    public enum Access
    {
        None,
        Read,
        Modify,
    }

    [Serializable]
    public class RuleTypeDN : IdentifiableEntity
    {
        RoleDN role;
        [NotNullValidator]
        public RoleDN Role
        {
            get { return role; }
            set { Set(ref role, value, () => Role); }
        }

        TypeDN type;
        [NotNullValidator]
        public TypeDN Type
        {
            get { return type; }
            set { Set(ref type, value, () => Type); }
        }

        TypeAccess access;
        public TypeAccess Access
        {
            get { return access; }
            set { Set(ref access, value, () => Access); }
        }        
    }

    public enum TypeAccess
    {
        None = 0,
        Read = 1,
        ModifyOnly = 3,
        CreateOnly = 5,
        FullAccess =7,
    }
}