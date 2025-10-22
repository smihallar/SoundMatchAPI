using System.Net;

namespace SoundMatchAPI.Data.DTOs.Responses
{
    // Used for returning a response without an object
    public class ReturnResponse
    {
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = [];
        public HttpStatusCode StatusCode { get; set; }
    }

    // Used for returning an object along with the response
    public class ReturnResponse<T> : ReturnResponse
    {
        public T? Data { get; set; }
    }
}
