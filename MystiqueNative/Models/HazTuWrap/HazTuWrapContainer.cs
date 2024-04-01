using MystiqueNative.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MystiqueNative.Models.HazTuWrap
{
    public class HazTuWrapContainer : BaseContainer
    {

    }

    public enum TortillaWrap
    {
        NoDefinida = 0,

        [Description("Tortilla de brocoli")]
        Brocoli = 1,

        [Description("Tortilla de chipotle")]
        Chipotle = 2,

        [Description("Tortilla normal")]
        Normal = 2,
    }
}
