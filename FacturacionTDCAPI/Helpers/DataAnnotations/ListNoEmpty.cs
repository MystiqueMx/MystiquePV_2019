using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FacturacionTDCAPI.Helpers.DataAnnotations
{
    public class ListNoEmpty : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return value is IList list && list.Count > 0;
        }
    }
}