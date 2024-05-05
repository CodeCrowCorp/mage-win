using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MageWin.Enums
{
    public enum UserRoleType
    {
        [Description("Host")]
        Host,

        [Description("Mod")]
        Moderator,

        [Description("Rando")]
        Random,

        [Description("You")]
        You
    }
}
