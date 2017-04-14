using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Develandoo.Infrastructure
{
    /// <summary>
    /// An interface for manipulating data from Entity Framework
    /// 
    /// </summary>
    /// <typeparam name="T">Type T must be a class type</typeparam>
    public interface IRepository<T> where T:class
    {
        void Create(T user);
        void Update(T user);
        T GetWithPredict(Func<T, bool> predicate);
        int CountByRule(Func<T, bool> predicate);
        T Get(int userId);
    }
}
