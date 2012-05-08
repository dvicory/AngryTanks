using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AngryTanks.Common
{
    class UniqueList<T> : List<T>
    {
        public UniqueList()
            : base()
        { }

        public UniqueList(int capacity)
            : base(capacity)
        { }

        public UniqueList(IEnumerable<T> collection)
            : base(collection)
        { }

        public void AddUnique(T item)
        {
            if (!base.Contains(item))
                base.Add(item);
        }

        public void UnionWith(IList<T> list)
        {
            foreach (T element in list)
            {
                this.AddUnique(element);
            }
        }
    }
}
