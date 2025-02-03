using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace SalonSync.Api.Common.Api;

public static class MethodsExtensions
{
    public static async Task<bool> CheckFieldInUse<T>(
        DbSet<T> dbSet, 
        Expression<Func<T, bool>> predicate) where T : class
    {
        try
        {
            return await dbSet.AsNoTracking().AnyAsync(predicate);
        }
        catch (Exception)
        {
            return false;
        }
    }
}