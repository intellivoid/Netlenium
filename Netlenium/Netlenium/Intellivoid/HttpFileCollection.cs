using System;
using System.Collections.Specialized;

namespace Netlenium.Intellivoid
{
    public class HttpFileCollection : NameObjectCollectionBase
    {
        private string[] allKeys;

        internal HttpFileCollection()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public HttpPostedFile Get(string name)
        {
            return (HttpPostedFile)BaseGet(name);
        }

        public HttpPostedFile this[string name] => Get(name);

        public HttpPostedFile Get(int index)
        {
            return (HttpPostedFile)BaseGet(index);
        }

        public string GetKey(int index)
        {
            return BaseGetKey(index);
        }

        public HttpPostedFile this[int index] => Get(index);

        public string[] AllKeys
        {
            get
            {
                if (allKeys == null)
                    allKeys = BaseGetAllKeys();

                return allKeys;
            }
        }

        internal void AddFile(string name, HttpPostedFile httpPostedFile)
        {
            BaseAdd(name, httpPostedFile);
        }
    }
}
