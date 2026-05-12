using System.Data;
using Microsoft.Data.SqlClient;
using Repository.Data;
using Repository.Models;

namespace Repository.Repositories
{
    public interface IProductRepository
    {
        List<Product> GetAllProducts();
        Product? GetProductById(int productId);
        bool UpdateProductStock(int productId, int quantity);
        List<Product> SearchProducts(string query);
    }

    public class ProductRepository : IProductRepository
    {
        private readonly IDataAccessLayer _dal;

        public ProductRepository(IDataAccessLayer dal)
        {
            _dal = dal;
        }

        public List<Product> GetAllProducts()
        {
            string query = "SELECT Id, Name, Description, Price, Stock, Category, ImageUrl, CreatedAt FROM Products ORDER BY Name";
            DataTable dt = _dal.ExecuteQuery(query);

            List<Product> products = new();
            foreach (DataRow row in dt.Rows)
            {
                products.Add(MapToProduct(row));
            }

            return products;
        }

        public Product? GetProductById(int productId)
        {
            string query = "SELECT Id, Name, Description, Price, Stock, Category, ImageUrl, CreatedAt FROM Products WHERE Id = @Id";
            SqlParameter[] parameters = new[] { new SqlParameter("@Id", productId) };

            DataTable dt = _dal.ExecuteQuery(query, parameters);

            if (dt.Rows.Count > 0)
            {
                return MapToProduct(dt.Rows[0]);
            }

            return null;
        }

        public bool UpdateProductStock(int productId, int quantity)
        {
            string query = "UPDATE Products SET Stock = Stock - @Quantity WHERE Id = @Id AND Stock >= @Quantity";

            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@Id", productId),
                new SqlParameter("@Quantity", quantity)
            };

            int result = _dal.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        public List<Product> SearchProducts(string query)
        {
            string searchQuery = @"SELECT Id, Name, Description, Price, Stock, Category, ImageUrl, CreatedAt
                                   FROM Products
                                   WHERE Name LIKE @Query OR Description LIKE @Query
                                   ORDER BY Name";

            SqlParameter[] parameters = new[] { new SqlParameter("@Query", $"%{query}%") };

            DataTable dt = _dal.ExecuteQuery(searchQuery, parameters);

            List<Product> products = new();
            foreach (DataRow row in dt.Rows)
            {
                products.Add(MapToProduct(row));
            }

            return products;
        }

        private Product MapToProduct(DataRow row)
        {
            return new Product
            {
                Id = (int)row["Id"],
                Name = row["Name"].ToString() ?? string.Empty,
                Description = row["Description"].ToString() ?? string.Empty,
                Price = (decimal)row["Price"],
                Stock = (int)row["Stock"],
                Category = row["Category"].ToString() ?? string.Empty,
                ImageUrl = row["ImageUrl"] != DBNull.Value ? row["ImageUrl"].ToString() : null,
                CreatedAt = (DateTime)row["CreatedAt"]
            };
        }
    }
}
