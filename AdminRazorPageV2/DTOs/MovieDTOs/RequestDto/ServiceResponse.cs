namespace APIS.DTOs.AuthenticationDTOs.ResponseDto
{
    public class ServiceResponse<T>
    {
        public T? Data { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
        public int TotalDataList { get; set; }
    }
}
