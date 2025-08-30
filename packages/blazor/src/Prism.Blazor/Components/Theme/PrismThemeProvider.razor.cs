using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Prism.Blazor.Components.Theme
{
public partial class PrismThemeProvider
{
    [Inject] public required IJSRuntime JSRuntime { get; set; }

    [Parameter] public string PrimaryColor { get; set; } = "#3b82f6";
    [Parameter] public bool DarkMode { get; set; } = false;
    [Parameter] public RenderFragment? ChildContent { get; set; }

    private IJSObjectReference? jsModule;
    private string _currentPrimaryColor = string.Empty;
    private bool _currentDarkMode = false;
    private bool _isThemeReady = false;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                Console.WriteLine("📦 [EuroThemeProvider] Importing theme generator module...");
                jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Prism.Blazor/js/theme-generator.js");
                Console.WriteLine("✅ [EuroThemeProvider] Theme generator module imported");
                
                await ApplyTheme();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [EuroThemeProvider] Error importing module: {ex.Message}");
            }
        }
        else if (PrimaryColor != _currentPrimaryColor || DarkMode != _currentDarkMode)
        {
            await ApplyTheme();
        }
    }

    private async Task ApplyTheme()
    {
        if (jsModule == null) return;

        try
        {
            var success = await jsModule.InvokeAsync<bool>("generateAndApplyTheme", PrimaryColor, DarkMode);

            if (success)
            {
                _currentPrimaryColor = PrimaryColor;
                _currentDarkMode = DarkMode;
                _isThemeReady = true;
                StateHasChanged(); // Force re-render to show content
            }
        }
        catch (Exception ex)
        {
            // Show content even if theme fails, but log the error
            Console.WriteLine($"⚠️ [PrismThemeProvider] Theme application failed: {ex.Message}");
            _isThemeReady = true;
            StateHasChanged();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (jsModule != null)
        {
            try
            {
                await jsModule.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // Circuit already disconnected - safe to ignore
                Console.WriteLine("🔄 [PrismThemeProvider] Circuit disconnected during disposal (normal on page refresh)");
            }
            catch (ObjectDisposedException)
            {
                // Component already disposed - safe to ignore
                Console.WriteLine("🔄 [PrismThemeProvider] Component already disposed");
            }
            catch (Exception ex)
            {
                // Log unexpected errors but don't crash
                Console.WriteLine($"⚠️ [PrismThemeProvider] Unexpected disposal error: {ex.Message}");
            }
        }
    }
}
}