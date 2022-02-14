using System.Collections.Generic;

namespace MFCheck.Utility
{
    class MultiMap<TKey, TValue>
    {
        public object this[TKey key]
        {
            get
            {
                if (m_Container.IndexOfKey(key) >= 0)
                {
                    return m_Container[key];
                }
                else
                {
                    return null;
                }
            }

            set
            {
                if (m_Container.IndexOfKey(key) >= 0)
                {
                    m_Container[key].Add((TValue)value);
                }
                else
                {
                    m_Container[key] = new List<TValue>
                    {
                        (TValue)value
                    };
                }
            }
        }

        public IList<TKey> Keys => m_Container.Keys;

        public int Count => m_Container.Count;

        public bool ContainsKey(TKey key)
        {
            return m_Container.ContainsKey(key);
        }

        public IList<TValue> GetValues(TKey key)
        {
            List<TValue> values = m_Container[key];
            return null != values ? values : new List<TValue>();
        }

        public void Clear()
        {
            m_Container.Clear();
        }

        private SortedList<TKey, List<TValue>> m_Container = new SortedList<TKey, List<TValue>>();
    }
}
