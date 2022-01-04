using Pedigree.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic.Core;
using Pedigree.Core.Data.Entity;

namespace Pedigree.Infrastructure.Database
{
    public static class DBExtensions
    {
        public static Core.Extensions.PagedResult<T> GetPagedHorses<T>(this IQueryable<T> query,
                                        int page, int pageSize) where T : Horse
        {
            var result = new Core.Extensions.PagedResult<T>();
            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.RowCount = query.Count(c=> c.Id > 0);


            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (page - 1) * pageSize;
            result.Results = query.Skip(skip).Take(pageSize).ToList();

            return result;
        }

        public static Core.Extensions.PagedResult<T> GetPaged<T>(this IQueryable<T> query,
                                        int page, int pageSize) where T : class
        {
            var result = new Core.Extensions.PagedResult<T>();
            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.RowCount = query.Count();


            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (page - 1) * pageSize;
            result.Results = query.Skip(skip).Take(pageSize).ToList();

            return result;
        }
    }
}
