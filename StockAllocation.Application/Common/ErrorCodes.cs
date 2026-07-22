namespace StockAllocation.Application.Common
{
    public class ErrorCodes
    {
        public const string InvalidRequest = "INVALID_REQUEST";
        public const string UnknownSku = "UNKNOWN_SKU";
        public const string InsufficientStock = "INSUFFICIENT_STOCK";
        public const string DuplicateOrderNumber = "DUPLICATE_ORDER_NUMBER";
        public const string OrderNotFound = "ORDER_NOT_FOUND";
    }
}
