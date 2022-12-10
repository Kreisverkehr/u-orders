using Microsoft.EntityFrameworkCore;

namespace UOrders.Service.Extensions
{
    internal static class IEnumerableExtensions
    {
        #region Public Methods

        /// <summary>
        /// Filter data with the provided filter function. Dynamically use serverside or clientside
        /// evaluation. Use the latter if the first fails.
        /// </summary>
        /// <typeparam name="I"> Entity type of the input elements </typeparam>
        /// <typeparam name="O"> Entity type of the output elements </typeparam>
        /// <param name="data">  
        /// input elements. Must be an <see cref="IQueryable{T}" /> to possibly use serverside evaluation.
        /// </param>
        /// <param name="filter">
        /// Function to filter data. Must return <see cref="IQueryable{T}" /> to possibly use
        /// serverside evaluation
        /// </param>
        /// <returns>
        /// The input enumerable extended by the filter function. Executes while enumerating.
        /// </returns>
        public static IEnumerable<O> DynamicFilterTarget<I, O>(this IEnumerable<I> data, Func<IEnumerable<I>, IEnumerable<O>> filter)
        {
            IEnumerable<O> filteredEntities = filter(data);
            try
            {
                if (filteredEntities is IQueryable<O> query)
                    query.ToQueryString();
                return filteredEntities;
            }
            catch (InvalidOperationException)
            {
                return filter(data.AsEnumerable());
            }
        }

        #endregion Public Methods
    }
}