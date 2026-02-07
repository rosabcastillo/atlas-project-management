using System.Text.RegularExpressions;
using ProjectManagement.Domain.Enums;

namespace ProjectManagement.Web.Helpers;

public static class DisplayHelpers
{
    public static string GetReasonDisplayName(UnavailabilityReason reason) => reason switch
    {
        UnavailabilityReason.PTO => "PTO",
        UnavailabilityReason.Holiday => "Holiday",
        UnavailabilityReason.SickLeave => "Sick Leave",
        UnavailabilityReason.Other => "Other",
        _ => reason.ToString()
    };

    public static string GetEnumDisplayName<T>(T value) where T : struct, Enum
    {
        var name = value.ToString();
        return Regex.Replace(name, "(?<!^)([A-Z])", " $1");
    }

    public static string GetAriaSort(string sortColumn, bool sortAscending, string column) =>
        sortColumn != column ? "none" : (sortAscending ? "ascending" : "descending");
}
