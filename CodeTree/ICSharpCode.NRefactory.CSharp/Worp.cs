using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.CSharp.Refactoring;

namespace System.Threading.Tasks
{
    public class TaskCompletionSource<T>
    {
        public Task Task { get; set; }
        internal void SetResult(object p)
        {
           
        }
    }
    public class Task
    {
        public bool IsCompleted { get; internal set; }

        internal void ContinueWith(Action action, object p2)
        {
        }
    }
    public class Task<T> : Task
    {
        internal T Result;
    }
    public class TaskScheduler
    {
        internal static object FromCurrentSynchronizationContext()
        {
            throw new NotImplementedException();
        }
    }
}
namespace ICSharpCode.NRefactory.CSharp
{
    public class HashSet0<T> : HashSet<T>, ISet<T>
    {
        public void Add(string name)
        {
        }

        public bool Contains(AstNode node)
        {
            return false;
        }

        public bool Contains(string name)
        {
            return false;
        }


    }
    public interface ISet<T>
    {
        bool Contains(AstNode node);
        bool Contains(string name);
        void Add(string name);
    }
    public class Tuple
    {
        public static Tuple<T,S> Create<T,S>(T t,S s)
        {
            return new Tuple<T, S>(t,s);
        }
    }
    public class Tuple<T, S>: Tuple
    {
        public T Item1;
        public S Item2;
        public Tuple(T t,S s)
        {
            Item1 = t;
            Item2 = s;
        }
    }
}
