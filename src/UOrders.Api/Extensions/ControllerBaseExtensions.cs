using Microsoft.AspNetCore.Mvc;

namespace UOrders.Api.Extensions;

public static class ControllerBaseExtensions
{
    #region Public Methods

    public static IQueryable<T> PreparePagedResponse<T>(this ControllerBase controller, IQueryable<T> source)
    {
        var pageIdx = Convert.ToInt32(controller.Request.Headers["X-Pagination-PageIdx"].SingleOrDefault() ?? "0");
        var pageSize = Convert.ToInt32(controller.Request.Headers["X-Pagination-PageSize"].SingleOrDefault() ?? "10");

        var totalRecords = source.Count();
        var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        var hasNext = pageIdx < totalPages - 1;
        var hasPrev = pageIdx > 0;

        controller.Response.Headers.Add("X-Pagination-PageIdx", pageIdx.ToString());
        controller.Response.Headers.Add("X-Pagination-PageSize", pageSize.ToString());
        controller.Response.Headers.Add("X-Pagination-TotalRecords", totalRecords.ToString());
        controller.Response.Headers.Add("X-Pagination-TotalPages", totalPages.ToString());
        controller.Response.Headers.Add("X-Pagination-HasNext", hasNext.ToString());
        controller.Response.Headers.Add("X-Pagination-HasPrev", hasPrev.ToString());

        return source
            .Skip(pageIdx * pageSize)
            .Take(pageSize);
    }

    #endregion Public Methods
}