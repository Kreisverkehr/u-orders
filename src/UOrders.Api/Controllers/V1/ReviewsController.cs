using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UOrders.Api.Services;
using UOrders.DTOModel.V1;
using UOrders.EFModel;

namespace UOrders.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly ILogger<ReviewsController> _logger;
    private readonly UOrdersDbContext _dbContext;
    private readonly IMapper _mapper;

    public ReviewsController(
    ILogger<ReviewsController> logger,
    UOrdersDbContext dbContext,
    IMapper mapper)
    {
        _logger = logger;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    [HttpGet("latest")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(IEnumerable<ReviewDTO>))]
    public IActionResult GetOverview() => 
        Ok(_dbContext
            .OrderReviews
            .OrderByDescending(r => r.CreatedOn)
            .Take(10)
            .ProjectTo<ReviewDTO>(_mapper.ConfigurationProvider));
}
