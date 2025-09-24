namespace Mottu.Api.Hateoas;

public class Resource<T>
{
    public T Data { get; init; }
    public List<Link> Links { get; } = new();

    public Resource(T data) => Data = data;
}