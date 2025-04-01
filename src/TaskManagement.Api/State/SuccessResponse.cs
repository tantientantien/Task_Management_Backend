public class SuccessResponse<T>
{
    public string Code { get; private set; }
    public T Data { get; private set; }
    private SuccessResponse(string code, T data)
    {
        Code = code ?? "SUCCESS";
        Data = data;
    }

    public static SuccessResponse<T> Create(T data, string code = null)
    {
        return new SuccessResponse<T>(code, data);
    }
}
