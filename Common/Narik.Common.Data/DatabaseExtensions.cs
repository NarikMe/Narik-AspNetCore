using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Narik.Common.Data
{
    public static class DatabaseExtensions
    {
        public static CustomTypeSqlQuery<T> SqlQuery<T>(
            this DatabaseFacade database,
            string sqlQuery,
            params object[] parameters) where T : class
        {
            return new CustomTypeSqlQuery<T>
            {
                DatabaseFacade = database,
                SqlQuery = sqlQuery,
                Parameters = parameters
            };
        }
    }
}
