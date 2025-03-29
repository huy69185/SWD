using GrowthTracking.ShareLibrary.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GrowthTracking.ShareLibrary.Filter
{
    public class ValidationFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(ms => ms.Value?.Errors.Count > 0)
                    .Select(ms => new ValidationErrorDTO
                    {
                        Field = ms.Key,
                        Message = ms.Value?.Errors.Select(e => e.ErrorMessage).ToList()
                    })
                    .ToList();

                var response = new ApiResponse
                {
                    Success = false,
                    Errors = errors,
                    Message = "Invalid input"
                };

                context.Result = new BadRequestObjectResult(response);
            }
        }
    }
}
