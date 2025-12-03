
namespace BlazorClient.Services;

public class ThemeService
{
    public event Action? OnChange;

    public string CurrentTheme { get; private set; } = "light";

    public void SetTheme(string theme)
    {
        if (CurrentTheme == theme) return;
        CurrentTheme = theme;
        OnChange?.Invoke();
    }

    public void Toggle()
    {
        CurrentTheme = (CurrentTheme == "light") ? "dark" : "light";
        OnChange?.Invoke();
    }
}
