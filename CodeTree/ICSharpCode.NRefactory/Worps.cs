using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.TypeSystem;

namespace System.Collections.Concurrent
{
    public class ConcurrentDictionary<T, S> : Dictionary<T, S>
    {
        ICSharpCode.NRefactory.Utils.ReferenceComparer Instance;
        public ConcurrentDictionary(ICSharpCode.NRefactory.Utils.ReferenceComparer instence)
        {
            this.Instance = instence;
        }
        internal S GetOrAdd(T key, S value)
        {
            if (!ContainsKey(key))
            {
                this[key] = value;
            }
            return value;
        }

        internal bool TryAdd(T key, S value)
        {
            if (!ContainsKey(key))
            {
                this[key] = value;
                return true;
            }
            return false;
        }
    }

}
namespace System.Threading
{
    public class CancellationToken
    {
        public static CancellationToken None { get; set; }
        public void ThrowIfCancellationRequested()
        {
        }
    }
}
namespace ICSharpCode.NRefactory.Contracts
{

}
namespace System.Diagnostics.Contracts
{

}
namespace ICSharpCode.NRefactory.TypeSystem
{
    public static class LazyInitializer
    {
        internal static IList<T> EnsureInitialized<T>(ref IList<T> implementedInterfaceMembers, Func<IList<T>> findImplementedInterfaceMembers)
        {
            if (implementedInterfaceMembers == null)
            {
                implementedInterfaceMembers = findImplementedInterfaceMembers();
            }
            return implementedInterfaceMembers;
        }
    }
    public class Lazy<T>
    {
        public T Value { get { return getFunc(); } }
        private Func<T> getFunc;
        public Lazy(Func<T> action)
        {
            getFunc = action;
        }
    }
}
