using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AngryTanks.Common
{
    class UniqueList<T> : List<T>
    {
        public void AddUnique(T t)
        {
            if (!base.Contains(t))
                base.Add(t);
        }

        public void UnionWith(IList<T> l)
        {
            foreach (T element in l)
            {
                this.AddUnique(element);
            }
        }
    }
}
