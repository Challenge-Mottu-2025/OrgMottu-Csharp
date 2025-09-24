using Microsoft.AspNetCore.Http;

namespace Mottu.Api.Hateoas;

public class LinkBuilder
{
    private readonly IHttpContextAccessor _ctx;

    public LinkBuilder(IHttpContextAccessor ctx) => _ctx = ctx;

    private string BaseUrl =>
        $"{_ctx.HttpContext!.Request.Scheme}://{_ctx.HttpContext!.Request.Host}";

    public Link Self(string path, string method = "GET") =>
        new("self", $"{BaseUrl}{path}", method);

    public Link Action(string rel, string path, string method) =>
        new(rel, $"{BaseUrl}{path}", method);
}