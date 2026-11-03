namespace ClickToCart.Marketing.Application;

/// <summary>Marketing formulas in one place, each a tiny pure function.</summary>
public static class Kpi
{
    public static decimal Roas(decimal revenue, decimal spend) => Ratio(revenue, spend);
    public static decimal Cpa(decimal spend, int orders) => Ratio(spend, orders);
    public static decimal ConversionRate(int orders, int clicks) => Ratio(orders, clicks);

    private static decimal Ratio(decimal top, decimal bottom) => bottom == 0 ? 0 : Math.Round(top / bottom, 4);
}
