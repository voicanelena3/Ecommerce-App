using System.Data;
using Microsoft.Data.SqlClient;
using Repository.Data;
using Repository.Models;

namespace Repository.Repositories
{
    public interface IOrderRepository
    {
        int CreateOrder(Order order);
        void CreateOrderItem(OrderItem orderItem);
        Order? GetOrderById(int orderId);
        List<Order> GetOrdersByUserId(int userId);
    }

    public class OrderRepository : IOrderRepository
    {
        private readonly IDataAccessLayer _dal;

        public OrderRepository(IDataAccessLayer dal)
        {
            _dal = dal;
        }

        public int CreateOrder(Order order)
        {
            string query = @"INSERT INTO Orders (UserId, TotalPrice, ShippingAddress, City, State, ZipCode, Status, CreatedAt)
                             VALUES (@UserId, @TotalPrice, @ShippingAddress, @City, @State, @ZipCode, @Status, @CreatedAt);
                             SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@UserId", order.UserId),
                new SqlParameter("@TotalPrice", order.TotalPrice),
                new SqlParameter("@ShippingAddress", order.ShippingAddress),
                new SqlParameter("@City", order.City),
                new SqlParameter("@State", string.IsNullOrEmpty(order.State) ? DBNull.Value : (object)order.State),
                new SqlParameter("@ZipCode", order.ZipCode),
                new SqlParameter("@Status", order.Status),
                new SqlParameter("@CreatedAt", order.CreatedAt)
            };

            object? result = _dal.ExecuteScalar(query, parameters);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public void CreateOrderItem(OrderItem orderItem)
        {
            string query = @"INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice, Subtotal)
                             VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice, @Subtotal)";

            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@OrderId", orderItem.OrderId),
                new SqlParameter("@ProductId", orderItem.ProductId),
                new SqlParameter("@Quantity", orderItem.Quantity),
                new SqlParameter("@UnitPrice", orderItem.UnitPrice),
                new SqlParameter("@Subtotal", orderItem.Subtotal)
            };

            _dal.ExecuteNonQuery(query, parameters);
        }

        public Order? GetOrderById(int orderId)
        {
            string query = @"SELECT Id, UserId, TotalPrice, ShippingAddress, City, State, ZipCode, Status, CreatedAt
                             FROM Orders WHERE Id = @Id";

            SqlParameter[] parameters = new[] { new SqlParameter("@Id", orderId) };
            DataTable dt = _dal.ExecuteQuery(query, parameters);

            if (dt.Rows.Count == 0)
                return null;

            Order order = MapToOrder(dt.Rows[0]);

            
            string itemsQuery = @"SELECT Id, OrderId, ProductId, Quantity, UnitPrice, Subtotal
                                  FROM OrderItems WHERE OrderId = @OrderId";
            SqlParameter[] itemParams = new[] { new SqlParameter("@OrderId", orderId) };
            DataTable itemsDt = _dal.ExecuteQuery(itemsQuery, itemParams);

            foreach (DataRow row in itemsDt.Rows)
            {
                order.OrderItems.Add(MapToOrderItem(row));
            }

            return order;
        }

        public List<Order> GetOrdersByUserId(int userId)
        {
            string query = @"SELECT Id, UserId, TotalPrice, ShippingAddress, City, State, ZipCode, Status, CreatedAt
                             FROM Orders WHERE UserId = @UserId ORDER BY CreatedAt DESC";

            SqlParameter[] parameters = new[] { new SqlParameter("@UserId", userId) };
            DataTable dt = _dal.ExecuteQuery(query, parameters);

            List<Order> orders = new();
            foreach (DataRow row in dt.Rows)
            {
                Order order = MapToOrder(row);

                
                string itemsQuery = @"SELECT Id, OrderId, ProductId, Quantity, UnitPrice, Subtotal
                                      FROM OrderItems WHERE OrderId = @OrderId";
                SqlParameter[] itemParams = new[] { new SqlParameter("@OrderId", order.Id) };
                DataTable itemsDt = _dal.ExecuteQuery(itemsQuery, itemParams);

                foreach (DataRow itemRow in itemsDt.Rows)
                {
                    order.OrderItems.Add(MapToOrderItem(itemRow));
                }

                orders.Add(order);
            }

            return orders;
        }

        private Order MapToOrder(DataRow row)
        {
            return new Order
            {
                Id = (int)row["Id"],
                UserId = (int)row["UserId"],
                TotalPrice = (decimal)row["TotalPrice"],
                ShippingAddress = row["ShippingAddress"].ToString() ?? string.Empty,
                City = row["City"].ToString() ?? string.Empty,
                State = row["State"] != DBNull.Value ? row["State"].ToString() ?? string.Empty : string.Empty,
                ZipCode = row["ZipCode"].ToString() ?? string.Empty,
                Status = row["Status"].ToString() ?? string.Empty,
                CreatedAt = (DateTime)row["CreatedAt"]
            };
        }

        private OrderItem MapToOrderItem(DataRow row)
        {
            return new OrderItem
            {
                Id = (int)row["Id"],
                OrderId = (int)row["OrderId"],
                ProductId = (int)row["ProductId"],
                Quantity = (int)row["Quantity"],
                UnitPrice = (decimal)row["UnitPrice"],
                Subtotal = (decimal)row["Subtotal"]
            };
        }
    }
}
