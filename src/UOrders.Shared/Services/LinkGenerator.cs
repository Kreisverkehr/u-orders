using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UOrders.Shared.Options;

namespace UOrders.Shared.Services;

public interface ILinkGenerator
{
    Uri GetCreateReviewLink(int orderId, Guid reviewToken);
}

public class LinkGenerator : ILinkGenerator
{
    private readonly WebContextOptions _webContext;

    public LinkGenerator(IOptions<WebContextOptions> webContext)
    {
        _webContext = webContext.Value;
    }

    public Uri GetCreateReviewLink(int orderId, Guid reviewToken) =>
        new($"{_webContext.WebUiBaseUri}/AddReview/{orderId}?token={reviewToken}");
}
