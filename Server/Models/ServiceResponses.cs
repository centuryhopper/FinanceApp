
namespace Server.Models;

public class ServiceResponses
{
    public record class GeneralResponse(bool Flag, string Message);
    public record class GeneralResponseWithPayload(bool Flag, string Message, string Payload);
    public record class LoginResponse(bool Flag, string Token, string Message);
}
