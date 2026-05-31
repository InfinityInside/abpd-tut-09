using abpd_tut_09.Exceptions;

namespace abpd_tut_09.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync(ex.Message);
        }
        catch (BadHttpRequestException ex)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync(ex.Message);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            Console.WriteLine(ex.StackTrace);
            await context.Response.WriteAsync(ex.Message);
        }


    }
}