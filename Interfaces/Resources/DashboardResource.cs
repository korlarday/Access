using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Resources
{
    public class DashboardResource
    {
        public ProductDoneAndPending _CurrentOrder { get; set; }
        public Quantity _OrderDetail { get; set; }
        public List<ProductDone> _DailyProduction { get; set; }
        public List<ProductDone> _WeeklyProduction { get; set; }
        public List<ProductDone> _MonthlyProduction { get; set; }
        public List<ProductDone> _YearlyProduction { get; set; }
        
        public List<OrderDoneAndPending> _DailyOrder { get; set; }
        public List<OrderDoneAndPending> _WeeklyOrder { get; set; }
        public List<OrderDoneAndPending> _MonthlyOrder { get; set; }
        public List<OrderDoneAndPending> _YearlyOrder { get; set; }

        public OrderDoneAndPending _TotalOrder { get; set; }

        public List<KeyInfo> _DailyKeyInfo { get; set; }
        public List<KeyInfo> _WeeklyKeyInfo { get; set; }
        public List<KeyInfo> _MonthlyKeyInfo { get; set; }
        public List<KeyInfo> _YearlyKeyInfo { get; set; }
        public KeyInfo _TotalKeyInfo { get; set; }

        public List<CylinderInfo> _DailyCylinderInfo { get; set; }
        public List<CylinderInfo> _WeeklyCylinderInfo { get; set; }
        public List<CylinderInfo> _MonthlyCylinderInfo { get; set; }
        public List<CylinderInfo> _YearlyCylinderInfo { get; set; }
        public CylinderInfo _TotalCylinderInfo { get; set; }

        public int _TotalKeys { get; set; }
        public int _TotalCylinders { get; set; }
        public int _CustomerCount { get; set; }
        public int _PartnerCount { get; set; }
    }

    public class ProductDoneAndPending
    {
        public int _KeyDone { get; set; }
        public int _KeyPending { get; set; }
        public int _CylinderDone { get; set; }
        public int _CylinderPending { get; set; }
        public int _TotalKeys { get; set; }
        public int _TotalCylinders { get; set; }
        public string _Customer { get; set; }
        public string _Partner { get; set; }
        public decimal _CurrentOrderPercentage { get; set; }
    }
    public class ProductDone
    {
        public DateTime _StartDate { get; set; }
        public DateTime _EndDate { get; set; }
        public int _CylinderDone { get; set; }
        public int _KeyDone { get; set; }
        public string _DateString { get; set; }
        public int _Position { get; set; }
    }

    public class OrderDoneAndPending
    {
        public DateTime _StartDate { get; set; }
        public DateTime _EndDate { get; set; }
        public int _OrderDone { get; set; }
        public int _OrderPending { get; set; }
        public string _DateString { get; set; }
        public int _Position { get; set; }
    }

    public class KeyInfo
    {
        public int _Produced { get; set; }
        public int _Blocked { get; set; }
        public int _Reclaimed { get; set; }
        public DateTime _StartDate { get; set; }
        public DateTime _EndDate { get; set; }
        public string _DateString { get; set; }
        public int _Position { get; set; }
    }
    public class CylinderInfo
    {
        public int _Assembled { get; set; }
        public int _Blocked { get; set; }
        public int _Reclaimed { get; set; }
        public DateTime _StartDate { get; set; }
        public DateTime _EndDate { get; set; }
        public int _Position { get; set; }
        public string _DateString { get; set; }
    }

    public class Quantity
    {
        public int _OldCylinderQty { get; set; }
        public int _NewCylinderQty { get; set; }
        public int _OldKeyQty { get; set; }
        public int _NewKeyQty { get; set; }
    }
}
