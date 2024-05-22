using System;
using System.Collections;
using System.Collections.Generic;

namespace MystiqueMC.Controllers
{
    internal class InsumosVisibles : IEnumerable<object>
    {
        internal static IEnumerable<object> Where(Func<object, object> value)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<object> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}