using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ElectionMonitoring.Repository
{
    /// <summary>
    /// The i base repository.
    /// </summary>
    /// <typeparam name="T">
    /// A class 
    /// </typeparam>
    public interface IRepository<T>
        where T : class
    {
        #region Public Methods and Operators

        /// <summary>
        /// The apply changes.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        void ApplyChanges(T entity);

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        void Add(T entity);

        /// <summary>
        /// The as queryable.
        /// </summary>
        /// <returns>
        /// An IQueryable collection
        /// </returns>
        IQueryable<T> AsQueryable();

        /// <summary>
        /// The attach.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        void Attach(T entity);

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        void Delete(T entity);

        /// <summary>
        /// The find.
        /// </summary>
        /// <param name="where">
        /// The where.
        /// </param>
        /// <returns>
        /// An IEnumerable list of T type objects
        /// </returns>
        IQueryable<T> Find(Expression<Func<T, bool>> where);

        /// <summary>
        /// The first.
        /// </summary>
        /// <param name="where">
        /// The where.
        /// </param>
        /// <returns>
        /// A single T type obbject based on the condictions specified
        /// </returns>
        T First(Expression<Func<T, bool>> where);

        /// <summary>
        /// The first.
        /// </summary>
        /// <param name="where">
        /// The where.
        /// </param>
        /// <returns>
        /// A single T type obbject based on the condictions specified
        /// </returns>
        T FirstOrDefault(Expression<Func<T, bool>> where);

        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>
        /// A IEnumerable list of T entities
        /// </returns>
        IQueryable<T> GetAll();

        /// <summary>
        /// The single.
        /// </summary>
        /// <param name="where">
        /// The where.
        /// </param>
        /// <returns>
        /// A single T type instance that satisfies the where condition
        /// </returns>
        T Single(Expression<Func<T, bool>> where);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        IQueryable<T> GetById(int entityId);

        #endregion
    }
}
