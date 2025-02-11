using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircleApp.Data.Helpers.Constants
{
    public static class AppRoles
    {
        public const string Admin = "admin";
        public const string User = "user";

        public static readonly IReadOnlyList<string> All = new[] { Admin, User };
    }
}
