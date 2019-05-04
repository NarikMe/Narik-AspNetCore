using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Narik.Common.Data
{
    public class CustomTypeSqlQuery<T> where T : class
    {
        public DatabaseFacade DatabaseFacade { get; set; }
        public string SqlQuery { get; set; }

        public object[] Parameters  { get; set; }

       

        private static IList<T> MapToList(DbDataReader dr,bool isFirst=false)
        {
            var objList = new List<T>();
            var props = typeof(T).GetRuntimeProperties().Where(x=>x.CanWrite).ToList();

            var colMapping = dr.GetColumnSchema()
                .Where(x => props.Any(y => y.Name.ToLower() == x.ColumnName.ToLower()))
                .ToDictionary(key => key.ColumnName.ToLower());

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    T obj = Activator.CreateInstance<T>();
                    foreach (var prop in props.Where(prop => colMapping.ContainsKey(prop.Name.ToLower())))
                    {
                        var val = dr.GetValue(colMapping[prop.Name.ToLower()].ColumnOrdinal.Value);
                        prop.SetValue(obj, val == DBNull.Value ? null : val);
                    }
                    objList.Add(obj);
                    if (isFirst)
                        return objList;
                }
            }
            return objList;
        }
        public async Task<IList<T>> ToListAsync()
        {
            return await GetList();
        }

        public async Task<T> FirstOrDefaultAsync()
        {
            var list = await GetList(true);
            return list.FirstOrDefault();
        }

        public async Task<IList<T>> GetList(bool isFirst=false)
        {
            var conn = DatabaseFacade.GetDbConnection();
            try
            {
                await conn.OpenAsync();
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = GetQuery();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        return MapToList(reader,isFirst);
                    }
                }
            }
            finally
            {
                conn.Close();
            }

        }

        private string GetQuery()
        {
            if (Parameters != null)
            {
                for (int i = 0; i < Parameters.Length; i++)
                {
                    if (Parameters[i] == null)
                        Parameters[i] = "null";
                    else if (Parameters[i] is string || Parameters[i] is DateTime)
                        Parameters[i] = $"'{Parameters[i]}'";
                }
                return string.Format(SqlQuery, Parameters);
            }

            return SqlQuery;

        }
    }
    
}
