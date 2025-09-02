using System.Data;

namespace RecipeBook.ServiceLibrary.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}