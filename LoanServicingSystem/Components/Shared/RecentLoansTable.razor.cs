using CoreData.Dashboard.Models;
using Microsoft.AspNetCore.Components;

namespace LoanServicingSystem.Components.Shared;

public partial class RecentLoansTable
{
    [Parameter]
    public List<RecentLoanModel> Loans { get; set; } = new();

    [Parameter]
    public string Title { get; set; } = "Recent Loans";
    protected string GetStatusClass(string status)
    {
        return status switch
        {
            "Active" =>
                "px-3 py-1 rounded-full bg-emerald-100 text-emerald-700 text-xs font-semibold",

            "Draft" =>
                "px-3 py-1 rounded-full bg-amber-100 text-amber-700 text-xs font-semibold",

            "Closed" =>
                "px-3 py-1 rounded-full bg-slate-100 text-slate-700 text-xs font-semibold",

            _ =>
                "px-3 py-1 rounded-full bg-red-100 text-red-700 text-xs font-semibold"
        };
    }
}