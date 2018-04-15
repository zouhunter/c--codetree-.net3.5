using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.CSharp.Refactoring;


namespace ICSharpCode.NRefactory.CSharp
{
    public class HeshSet<T> : ISet<T>
    {
        public List<string> keys = new List<string>();
        public void Add(string name)
        {
            if(!keys.Contains(name))
            {
                keys.Add(name);
            }
        }

        public bool Contains(AstNode node)
        {
            return keys.Find(x=>x == node.ToString()) != null;
        }

        public bool Contains(string name)
        {
            return keys.Contains(name);
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
