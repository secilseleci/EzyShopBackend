namespace Core.Interfaces;

public interface IRazorViewRenderer
{
    Task<string?> RenderViewToStringAsync(string viewName, object model);

}
