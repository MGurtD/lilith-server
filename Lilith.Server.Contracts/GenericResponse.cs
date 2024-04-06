namespace Lilith.Server.Contracts;

public class GenericResponse<ResponseType>
{
    public bool Result { get; }
    public IList<string> Errors { get; }
    public ResponseType? Content { get; }

    public GenericResponse(bool result, ResponseType? content)
    {
        Result = result;
        Errors = [];
        Content = content;
    }

    public GenericResponse(bool result)
    {
        Result = result;
        Errors = [];
    }

    public GenericResponse(bool result, string error)
    {
        Result = result;
        Errors = [error];
    }

    public GenericResponse(bool result, IList<string> errors, ResponseType? content)
    {
        Result = result;
        Errors = errors;
        Content = content;
    }
}
