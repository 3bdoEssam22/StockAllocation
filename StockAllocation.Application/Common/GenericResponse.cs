namespace StockAllocation.Application.Common
{
    public class GenericResponse<T>
    {
        public int StatusCode { get; set; }
        public string? Code { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
}
