using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UOrders.DTOModel.V1;
using UOrders.PrintService.Services;

namespace UOrders.PrintService.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PrintController : ControllerBase
    {
        #region Private Fields

        private readonly IPrinterQueue _printerQueue;

        #endregion Private Fields

        #region Public Constructors

        public PrintController(IPrinterQueue printerQueue)
        {
            _printerQueue = printerQueue;
        }

        #endregion Public Constructors

        #region Public Methods

        [HttpPost("order")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<IActionResult> PrintOrder(OrderDTO order)
        {
            await _printerQueue.EnqueueOrderAsync(order);

            return Accepted();
        }

        #endregion Public Methods
    }
}