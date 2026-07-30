using ClosedXML.Excel;
using CoreData.LoanOrigination;
using CoreData.Servicing;

namespace CoreData.Export;

public static class LoanStatementExporter
{
    public static XLWorkbook GenerateWorkbook(
    Loan loan,
    List<Payment> payments,
    List<RepaymentSchedule> schedules)
    {
        var workbook = new XLWorkbook();

        GenerateSummarySheet(workbook, loan);
        GeneratePaymentSheet(workbook, payments);
        GenerateScheduleSheet(workbook, schedules);

        return workbook;
    }

    private static void GenerateSummarySheet(
        XLWorkbook workbook,
        Loan loan)
    {
        var ws = workbook.Worksheets.Add("Loan Summary");

        ws.Range("A1:B1").Merge();
        ws.Cell("A1").Value = "LOAN STATEMENT";

        ws.Cell("A1").Style.Font.Bold = true;
        ws.Cell("A1").Style.Font.FontSize = 18;
        ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Cell("A1").Style.Fill.BackgroundColor = XLColor.DarkBlue;
        ws.Cell("A1").Style.Font.FontColor = XLColor.White;

        int row = 3;

        AddRow(ws, row++, "Loan Number", loan.LoanNumber);
        AddRow(ws, row++, "Customer", loan.CustomerName);
        AddRow(ws, row++, "Product", loan.ProductName);
        AddRow(ws, row++, "Principal", loan.Principal);
        AddRow(ws, row++, "Interest Rate", $"{loan.InterestRate}%");
        AddRow(ws, row++, "Tenure", $"{loan.Tenure} Months");
        AddRow(ws, row++, "Repayment Frequency", loan.RepaymentFrequency);
        AddRow(ws, row++, "Status", loan.Status);
        AddRow(ws, row++, "Start Date", loan.StartDate?.ToString("dd MMM yyyy"));
        AddRow(ws, row++, "End Date", loan.EndDate?.ToString("dd MMM yyyy"));

        ws.Column(1).Style.Font.Bold = true;

        ws.Columns().AdjustToContents();
    }

    private static void GeneratePaymentSheet(
    XLWorkbook workbook,
    List<Payment> payments)
    {
        var ws = workbook.Worksheets.Add("Payment History");

        ws.Cell("A1").Value = "PAYMENT HISTORY";
        ws.Range("A1:F1").Merge();

        ws.Cell("A1").Style.Font.Bold = true;
        ws.Cell("A1").Style.Font.FontSize = 16;
        ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Cell("A1").Style.Fill.BackgroundColor = XLColor.DarkBlue;
        ws.Cell("A1").Style.Font.FontColor = XLColor.White;

        int row = 3;

        ws.Cell(row, 1).Value = "Payment Date";
        ws.Cell(row, 2).Value = "Amount";
        ws.Cell(row, 3).Value = "Payment Type";
        ws.Cell(row, 4).Value = "Mode";
        ws.Cell(row, 5).Value = "Reference No.";
        ws.Cell(row, 6).Value = "Remarks";

        ws.Range(row, 1, row, 6).Style.Font.Bold = true;
        ws.Range(row, 1, row, 6).Style.Fill.BackgroundColor = XLColor.LightGray;

        row++;

        foreach (var payment in payments)
        {
            ws.Cell(row, 1).Value = payment.PaymentDate;
            ws.Cell(row, 1).Style.DateFormat.Format = "dd-MMM-yyyy";

            ws.Cell(row, 2).Value = payment.Amount;
            ws.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";

            ws.Cell(row, 3).Value = payment.PaymentType;
            ws.Cell(row, 4).Value = payment.Mode;
            ws.Cell(row, 5).Value = payment.ReferenceNumber;
            ws.Cell(row, 6).Value = payment.Remarks;

            row++;
        }

        ws.Columns().AdjustToContents();
    }

    private static void GenerateScheduleSheet(
    XLWorkbook workbook,
    List<RepaymentSchedule> schedules)
    {
        var ws = workbook.Worksheets.Add("Repayment Schedule");

        ws.Range("A1:F1").Merge();
        ws.Cell("A1").Value = "REPAYMENT SCHEDULE";

        ws.Cell("A1").Style.Font.Bold = true;
        ws.Cell("A1").Style.Font.FontSize = 16;
        ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Cell("A1").Style.Fill.BackgroundColor = XLColor.DarkBlue;
        ws.Cell("A1").Style.Font.FontColor = XLColor.White;

        int row = 3;

        ws.Cell(row, 1).Value = "EMI No";
        ws.Cell(row, 2).Value = "Due Date";
        ws.Cell(row, 3).Value = "Principal";
        ws.Cell(row, 4).Value = "Interest";
        ws.Cell(row, 5).Value = "Outstanding";
        ws.Cell(row, 6).Value = "Status";

        ws.Range(row, 1, row, 6).Style.Font.Bold = true;
        ws.Range(row, 1, row, 6).Style.Fill.BackgroundColor = XLColor.LightGray;

        row++;

        foreach (var schedule in schedules)
        {
            ws.Cell(row, 1).Value = schedule.EmiNo;

            ws.Cell(row, 2).Value = schedule.DueDate;
            ws.Cell(row, 2).Style.DateFormat.Format = "dd-MMM-yyyy";

            ws.Cell(row, 3).Value = schedule.Principal;
            ws.Cell(row, 3).Style.NumberFormat.Format = "#,##0.00";

            ws.Cell(row, 4).Value = schedule.Interest;
            ws.Cell(row, 4).Style.NumberFormat.Format = "#,##0.00";

            ws.Cell(row, 5).Value = schedule.Outstanding;
            ws.Cell(row, 5).Style.NumberFormat.Format = "#,##0.00";

            ws.Cell(row, 6).Value = schedule.Status;

            row++;
        }

        ws.Columns().AdjustToContents();
    }

    private static void AddRow(
        IXLWorksheet ws,
        int row,
        string title,
        object? value)
    {
        ws.Cell(row, 1).Value = title;
        ws.Cell(row, 2).Value = value?.ToString() ?? "";
    }
}