using Microsoft.AspNetCore.Components;

namespace LoanServicingSystem.Components.Shared;

public partial class StatCard
{
    [Parameter]
    public string Title { get; set; } = string.Empty;

    [Parameter]
    public string Value { get; set; } = string.Empty;

    [Parameter]
    public string SubTitle { get; set; } = string.Empty;

    [Parameter]
    public string Trend { get; set; } = string.Empty;

    [Parameter]
    public string FooterText { get; set; } = string.Empty;

    [Parameter]
    public bool IsLoading { get; set; }

    [Parameter]
    public RenderFragment? Icon { get; set; }

    [Parameter]
    public string TrendBackground { get; set; } = "bg-emerald-100";

    [Parameter]
    public string TrendColor { get; set; } = "text-emerald-700";

    [Parameter]
    public EventCallback OnClick { get; set; }

    [Parameter] public string? CardColor { get; set; }





    [Parameter] 
    public bool ShowTrend { get; set; }
    [Parameter]
    public string ValueColor { get; set; } = "text-slate-800";

    [Parameter]
    public string IconBackground { get; set; } = "bg-indigo-100";

    [Parameter]
    public string IconColor { get; set; } = "text-indigo-600";

    protected async Task HandleClick()
    {
        if (OnClick.HasDelegate)
        {
            await OnClick.InvokeAsync();
        }
    }
}