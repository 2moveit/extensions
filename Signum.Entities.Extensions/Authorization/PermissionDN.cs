﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Utilities;
using Signum.Entities.Basics;

namespace Signum.Entities.Authorization
{
    [Serializable]
    public class PermissionDN : EnumDN
    {

    }

    public enum BasicPermissions
    {
        AdminRules,
        EntitiesWithNoGroup,
    }
}
