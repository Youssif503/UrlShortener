namespace UrlShortener.Common;

public class Result<T>
{
    public T? Data { get; }
    public List<string> Errors { get; }
    public bool IsSuccess { get; }

    private Result(T data)
    {
        Data = data;
        Errors = [];
        IsSuccess = true;
    }

    private Result(List<string> errors)
    {
        Errors = errors;
        IsSuccess = false;
    }

    public static Result<T> Success(T data)
        => new(data);

    public static Result<T> Fail(List<string> errors)
        => new(errors);
}