using Allprimetech.GeneralUtils;
using Allprimetech.Interfaces;
using Allprimetech.Interfaces.Models;
using Allprimetech.Interfaces.Resources;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allprimetech.DAL
{
    public class StatsRepository : IStatsRepository
    {
        private ApplicationDbContext _Context;

        public StatsRepository(ApplicationDbContext context)
        {
            _Context = context;
        }

        public async Task<DashboardResource> DashboardStats(string userId, int customerId, int orderId)
        {
            try
            {
                var user = await _Context.ApplicationUsers.FindAsync(userId);

                #region declarations
                List<ProductDone> dailyProduction = new List<ProductDone>();
                List<ProductDone> weeklyProduction = new List<ProductDone>();
                List<ProductDone> monthlyProduction = new List<ProductDone>();
                List<ProductDone> yearlyProduction = new List<ProductDone>();

                List<OrderDoneAndPending> dailyOrder = new List<OrderDoneAndPending>();
                List<OrderDoneAndPending> weeklyOrder = new List<OrderDoneAndPending>();
                List<OrderDoneAndPending> monthlyOrder = new List<OrderDoneAndPending>();
                List<OrderDoneAndPending> yearlyOrder = new List<OrderDoneAndPending>();
                OrderDoneAndPending totalOrder = new OrderDoneAndPending();

                List<KeyInfo> dailyKeyInfo = new List<KeyInfo>();
                List<KeyInfo> weeklyKeyInfo = new List<KeyInfo>();
                List<KeyInfo> monthlyKeyInfo = new List<KeyInfo>();
                List<KeyInfo> yearlyKeyInfo = new List<KeyInfo>();
                KeyInfo totalKeyInfo = new KeyInfo();

                List<CylinderInfo> dailyCylinderInfo = new List<CylinderInfo>();
                List<CylinderInfo> weeklyCylinderInfo = new List<CylinderInfo>();
                List<CylinderInfo> monthlyCylinderInfo = new List<CylinderInfo>();
                List<CylinderInfo> yearlyCylinderInfo = new List<CylinderInfo>();
                CylinderInfo totalCylinderInfo = new CylinderInfo();

                Quantity orderDetail = new Quantity();

                var thisWeek = ThisWeek(DateTime.UtcNow);
                var thisMonth = ThisMonth(DateTime.UtcNow);
                var thisYear = ThisYear(DateTime.UtcNow);
                #endregion

                var _TotalCylinders = await _Context.Cylinders.Where(x => x.CustomerID == customerId).ToListAsync();
                var _TotalKeys = await _Context.Groups.Where(x => x.CustomerID == customerId).ToListAsync();
                var _CustomerOrders = await _Context.Orders.Where(x => x.CustomerID == customerId).OrderByDescending(x => x._CreationDate).ToListAsync();

                #region Current Order
                var currOrder = await _Context.Orders.Include(x => x._OrderDetails)
                                                    .Include(x => x.Customer)
                                                        .ThenInclude(x => x.Partner)
                                                    .Where(x => x.CustomerID == customerId && x.OrderID == orderId)
                                                    .FirstOrDefaultAsync();
                var currentProduction = await _Context.Productions
                                                    .Where(x => x.OrderID == orderId)
                                                    .FirstOrDefaultAsync();

                ProductDoneAndPending currentOrder = new ProductDoneAndPending();

                if (currOrder != null)
                {
                    // get the done and pending for cylinder and key
                    // starting with cylinder
                    int cylinderQty = currOrder._CylinderQty;
                    var cylindersToProduce = await _Context.Cylinders.Where(x => x.OrderID == currOrder.OrderID && x.CustomerID == customerId).ToListAsync();
                    int cylinderDone = cylindersToProduce.Sum(x => x._Validated);
                    currentOrder._CylinderDone = cylinderDone;
                    currentOrder._CylinderPending = _TotalCylinders.Count() - cylinderDone;

                    // then keygroup
                    int keyQty = currOrder._GroupKeyQty;
                    var keysToProduce = await _Context.Groups.Where(x => x.OrderID == currOrder.OrderID).ToListAsync();
                    int keyDone = keysToProduce.Sum(x => x._Validated);
                    currentOrder._KeyDone = keyDone;
                    currentOrder._KeyPending = _TotalKeys.Count() - keyDone;

                    var cylinderDetail = currOrder._OrderDetails
                                                    .Where(x => x._ProductType == ProductType.Cylinder)
                                                    .OrderByDescending(x => x._Date)
                                                    .FirstOrDefault();
                    var keyDetail = currOrder._OrderDetails
                                                    .Where(x => x._ProductType == ProductType.Key)
                                                    .OrderByDescending(x => x._Date)
                                                    .FirstOrDefault();

                    var lengthOfOrders = _CustomerOrders.Count();
                    
                    if(lengthOfOrders < 1)
                    {
                        orderDetail._NewCylinderQty = 0;
                        orderDetail._OldCylinderQty = 0;
                        orderDetail._NewKeyQty = 0;
                        orderDetail._OldKeyQty = 0;
                    }
                    else if(lengthOfOrders == 1)
                    {
                        var latestOrder = _CustomerOrders.FirstOrDefault();
                        orderDetail._NewCylinderQty = latestOrder._CylinderQty;
                        orderDetail._OldCylinderQty = 0;
                        orderDetail._NewKeyQty = latestOrder._GroupKeyQty;
                        orderDetail._OldKeyQty = 0;
                    }
                    else if (lengthOfOrders > 1)
                    {
                        var totalCylinderQty = _CustomerOrders.Sum(x => x._CylinderQty);
                        var totalGroupQty = _CustomerOrders.Sum(x => x._GroupKeyQty);
                        var previousOrder = _CustomerOrders.FirstOrDefault();

                        orderDetail._NewCylinderQty = totalCylinderQty;
                        orderDetail._OldCylinderQty = totalCylinderQty - previousOrder._CylinderQty;
                        orderDetail._NewKeyQty = totalGroupQty;
                        orderDetail._OldKeyQty = totalGroupQty - previousOrder._GroupKeyQty;
                    }

                    // Total Cylinders And Keys Of the current Order
                    currentOrder._TotalCylinders = cylindersToProduce.Count();
                    currentOrder._TotalKeys = keysToProduce.Count();
                    // customer and partner
                    currentOrder._Customer = currOrder.Customer._Name;
                    currentOrder._Partner = currOrder.Customer.Partner != null ? currOrder.Customer.Partner._Name : null;
                    // curr order percent
                    decimal keyPercentage = 0;
                    decimal cylPercentage = 0;

                    if(currentOrder._KeyDone != 0 && currentOrder._TotalKeys != 0)
                    {
                        keyPercentage = (Decimal.Divide(currentOrder._KeyDone,currentOrder._TotalKeys)) * 100;
                    }
                    if(currentOrder._CylinderDone != 0 && currentOrder._TotalCylinders != 0)
                    {
                        cylPercentage = (Decimal.Divide(currentOrder._CylinderDone, currentOrder._TotalCylinders)) * 100;
                    }
                    if((keyPercentage + cylPercentage) > 0)
                        currentOrder._CurrentOrderPercentage = Decimal.Divide((keyPercentage + cylPercentage), 2);
                }

                #endregion

                var productions = new List<Production>();
                var groups = new List<Group>();
                var cylinders = new List<Cylinder>();
                var orders = new List<Order>();
                var keys = new List<Group>();
                var AllCylinders = new List<Cylinder>();

                //if(user.PartnerID == null)
                if (true)
                {
                    productions = await _Context.Productions.Include(x => x.Order).Where(x => x.Order.CustomerID == customerId).ToListAsync();
                    groups = _TotalKeys;
                    cylinders = _TotalCylinders;
                    orders = await _Context.Orders.Where(x => x.CustomerID == customerId).ToListAsync();
                    keys = groups;
                }
                else
                {
                    //productions = await _Context.Productions.Include(x => x.ByUser).Where(x => x.ByUser.PartnerID == user.PartnerID).ToListAsync();
                    //groups = await _Context.Groups.Include(x => x.Customer).Where(x => x.Customer.PartnerID == user.PartnerID).ToListAsync();
                    //cylinders = await _Context.Cylinders.Include(x => x.Customer).Where(x => x.Customer.PartnerID == user.PartnerID).ToListAsync();
                    //orders = await _Context.Orders.Include(x => x.Customer).Where(x => x.Customer.PartnerID == user.PartnerID).ToListAsync();
                    //keys = groups;
                }

                //var keys = await _Context.Groups.ToListAsync();

                #region Daily Information
                for (int i = 0; i < 7; i++)
                {
                    // production info
                    var date = DateTime.UtcNow.AddDays(-i).Date;
                    ProductDone product = new ProductDone();
                    product._KeyDone = groups.Where(x => x._CreationDate.Date == date &&
                                                        x._Validated >= 1).Count();
                    product._CylinderDone = cylinders.Where(x => x._CreationDate.Date == date &&
                                                        x._Validated >= 1).Count();
                    product._StartDate = date;
                    product._EndDate = date;
                    product._DateString = date.DayOfWeek.ToString();
                    product._Position = i;
                    dailyProduction.Add(product);


                    // daily order info
                    OrderDoneAndPending dayOrder = new OrderDoneAndPending()
                    {
                        _OrderDone = orders.Where(x => x._CreationDate.Date == date &&
                                                            x._Status == Status.Done).Count(),
                        _OrderPending = orders.Where(x => x._CreationDate.Date == date &&
                                                            x._Status == Status.OnProgress || x._Status == Status.NotStarted).Count(),
                        _EndDate = date,
                        _StartDate = date,
                        _DateString = date.DayOfWeek.ToString(),
                        _Position = i
                    };
                    dailyOrder.Add(dayOrder);

                    // daily key info
                    var keyInfo = new KeyInfo
                    {
                        _Produced = productions.Where(x => x._CreationDate.Date == date && x._ProductType == ProductType.Key && x._Status == ProductionStatus.Produced)
                                                .Sum(x => x._Quantity),
                        _Blocked = productions.Where(x => x._CreationDate.Date == date && x._ProductType == ProductType.Key && x._Status == ProductionStatus.Blocked)
                                                .Sum(x => x._Quantity),
                        _Reclaimed = productions.Where(x => x._CreationDate.Date == date && x._ProductType == ProductType.Key && x._Status == ProductionStatus.Recliamed)
                                                .Sum(x => x._Quantity),
                        _DateString = date.DayOfWeek.ToString(),
                        _StartDate = date,
                        _EndDate = date,
                        _Position = i
                    };
                    dailyKeyInfo.Add(keyInfo);

                    // daily cylinder info
                    var cylinderInfo = new CylinderInfo()
                    {
                        _Assembled = productions.Where(x => x._CreationDate.Date == date && x._ProductType == ProductType.Cylinder && x._Status == ProductionStatus.Assembled)
                                                .Sum(x => x._Quantity),
                        _Blocked = productions.Where(x => x._CreationDate.Date == date && x._ProductType == ProductType.Cylinder && x._Status == ProductionStatus.Blocked)
                                                .Sum(x => x._Quantity),
                        _Reclaimed = productions.Where(x => x._CreationDate.Date == date && x._ProductType == ProductType.Cylinder && x._Status == ProductionStatus.Recliamed)
                                                .Sum(x => x._Quantity),
                        _StartDate = date,
                        _EndDate = date,
                        _DateString = date.DayOfWeek.ToString(),
                        _Position = i
                    };
                    dailyCylinderInfo.Add(cylinderInfo);
                }
                #endregion

                #region Weekly Information
                int weekDays = 0;
                int weekCount = 6;
                for (int i = 0; i < 5; i++)
                {
                    weekCount -= 1;
                    var weekStart = thisWeek.Start.AddDays(-(weekDays));
                    var weekEnd = thisWeek.End.AddDays(-(weekDays));
                    ProductDone product = new ProductDone();
                    product._KeyDone = groups.Where(x => x._CreationDate >= weekStart &&
                                                            x._CreationDate <= weekEnd &&
                                                            x._Validated == 1).Count();
                    product._CylinderDone = cylinders.Where(x => x._CreationDate >= weekStart &&
                                                            x._CreationDate <= weekEnd &&
                                                            x._Validated == 1).Count();
                    product._StartDate = weekStart;
                    product._EndDate = weekEnd;
                    product._DateString = "Week " + weekCount;
                    product._Position = i;
                    weeklyProduction.Add(product);

                    // weekly order info
                    var weekOrder = new OrderDoneAndPending
                    {
                        _OrderDone = orders.Where(x => x._CreationDate >= weekStart &&
                                                                x._CreationDate <= weekEnd &&
                                                                x._Status == Status.Done).Count(),
                        _OrderPending = orders.Where(x => x._CreationDate >= weekStart &&
                                                                x._CreationDate <= weekEnd &&
                                                                x._Status == Status.OnProgress || x._Status == Status.NotStarted).Count(),
                        _StartDate = weekStart,
                        _DateString = "Week " + weekCount,
                        _EndDate = weekEnd,
                        _Position = i
                    };
                    weeklyOrder.Add(weekOrder);

                    // weekly key info
                    var weekKeyInfo = new KeyInfo
                    {
                        _Produced = productions.Where(x => x._CreationDate >= weekStart &&
                                                                x._CreationDate <= weekEnd && x._ProductType == ProductType.Key && x._Status == ProductionStatus.Produced)
                                                                .Sum(x => x._Quantity),
                        _Blocked = productions.Where(x => x._CreationDate >= weekStart &&
                                                                x._CreationDate <= weekEnd && x._ProductType == ProductType.Key && x._Status == ProductionStatus.Blocked)
                                                                .Sum(x => x._Quantity),
                        _Reclaimed = productions.Where(x => x._CreationDate >= weekStart &&
                                                                x._CreationDate <= weekEnd && x._ProductType == ProductType.Key && x._Status == ProductionStatus.Recliamed)
                                                                .Sum(x => x._Quantity),
                        _StartDate = weekStart,
                        _DateString = "Week " + weekCount,
                        _EndDate = weekEnd,
                        _Position = i
                    };
                    weeklyKeyInfo.Add(weekKeyInfo);

                    // Weekly cylinder info
                    var cylInfo = new CylinderInfo
                    {
                        _Assembled = productions.Where(x => x._CreationDate >= weekStart &&
                                                                x._CreationDate <= weekEnd && x._ProductType == ProductType.Cylinder && x._Status == ProductionStatus.Assembled)
                                                                .Sum(x => x._Quantity),
                        _Blocked = productions.Where(x => x._CreationDate >= weekStart &&
                                                                x._CreationDate <= weekEnd && x._ProductType == ProductType.Cylinder && x._Status == ProductionStatus.Blocked)
                                                                .Sum(x => x._Quantity),
                        _Reclaimed = productions.Where(x => x._CreationDate >= weekStart &&
                                                                x._CreationDate <= weekEnd && x._ProductType == ProductType.Cylinder && x._Status == ProductionStatus.Recliamed)
                                                                .Sum(x => x._Quantity),
                        _StartDate = weekStart,
                        _DateString = "Week " + weekCount,
                        _EndDate = weekEnd,
                        _Position = i
                    };
                    weeklyCylinderInfo.Add(cylInfo);

                    weekDays += 7;
                }

                #endregion

                #region Monthly Info
                // monthly production info
                for (int i = 0; i < 12; i++)
                {
                    var date = DateTime.UtcNow;
                    var monthStart = (new DateTime(date.Year, date.Month, 1)).AddMonths(-(i));
                    var monthEnd = monthStart.AddMonths(1).AddSeconds(-1);
                    var product = new ProductDone();
                    product._KeyDone = groups.Where(x => x._CreationDate >= monthStart &&
                                                            x._CreationDate <= monthEnd &&
                                                            x._Validated == 1).Count();
                    product._CylinderDone = cylinders.Where(x => x._CreationDate >= monthStart &&
                                                            x._CreationDate <= monthEnd &&
                                                            x._Validated == 1).Count();
                    product._StartDate = monthStart;
                    product._EndDate = monthEnd;
                    product._DateString = monthStart.ToString("MMMM");
                    product._Position = i;
                    monthlyProduction.Add(product);


                    // monthly order info
                    var monthOrder = new OrderDoneAndPending
                    {
                        _OrderDone = orders.Where(x => x._CreationDate >= monthStart &&
                                                                x._CreationDate <= monthEnd &&
                                                                x._Status == Status.Done).Count(),
                        _OrderPending = orders.Where(x => x._CreationDate >= monthStart &&
                                                                x._CreationDate <= monthEnd &&
                                                                x._Status == Status.OnProgress || x._Status == Status.NotStarted).Count(),
                        _StartDate = monthStart,
                        _DateString = monthStart.ToString("MMMM"),
                        _EndDate = monthEnd,
                        _Position = i
                    };
                    monthlyOrder.Add(monthOrder);

                    // monthly key info
                    var monthKeyInfo = new KeyInfo
                    {
                        _Produced = productions.Where(x => x._CreationDate >= monthStart &&
                                                                x._CreationDate <= monthEnd && x._ProductType == ProductType.Key && x._Status == ProductionStatus.Produced)
                                                                .Sum(x => x._Quantity),
                        _Blocked = productions.Where(x => x._CreationDate >= monthStart &&
                                                                x._CreationDate <= monthEnd && x._ProductType == ProductType.Key && x._Status == ProductionStatus.Blocked)
                                                                .Sum(x => x._Quantity),
                        _Reclaimed = productions.Where(x => x._CreationDate >= monthStart &&
                                                                x._CreationDate <= monthEnd && x._ProductType == ProductType.Key && x._Status == ProductionStatus.Recliamed)
                                                                .Sum(x => x._Quantity),
                        _StartDate = monthStart,
                        _EndDate = monthEnd,
                        _DateString = monthStart.ToString("MMMM"),
                        _Position = i
                    };
                    monthlyKeyInfo.Add(monthKeyInfo);


                    // monthly cylinder info
                    var cylInfo = new CylinderInfo
                    {
                        _Assembled = productions.Where(x => x._CreationDate >= monthStart &&
                                                                x._CreationDate <= monthEnd && x._ProductType == ProductType.Cylinder && x._Status == ProductionStatus.Assembled)
                                                                .Sum(x => x._Quantity),
                        _Blocked = productions.Where(x => x._CreationDate >= monthStart &&
                                                                x._CreationDate <= monthEnd && x._ProductType == ProductType.Cylinder && x._Status == ProductionStatus.Blocked)
                                                                .Sum(x => x._Quantity),
                        _Reclaimed = productions.Where(x => x._CreationDate >= monthStart &&
                                                                x._CreationDate <= monthEnd && x._ProductType == ProductType.Cylinder && x._Status == ProductionStatus.Recliamed)
                                                                .Sum(x => x._Quantity),
                        _StartDate = monthStart,
                        _EndDate = monthEnd,
                        _DateString = monthStart.ToString("MMMM"),
                        _Position = i
                    };
                    monthlyCylinderInfo.Add(cylInfo);
                }

                #endregion

                #region Yearly Information
                // yearly production info
                for (int i = 0; i < 5; i++)
                {
                    var date = DateTime.UtcNow;
                    var yearStart = new DateTime(date.Year - (i), 1, 1);
                    var yearEnd = yearStart.AddYears(1).AddSeconds(-1);
                    var product = new ProductDone();
                    product._KeyDone = groups.Where(x => x._CreationDate >= yearStart &&
                                                            x._CreationDate <= yearEnd &&
                                                            x._Validated == 1).Count();
                    product._CylinderDone = cylinders.Where(x => x._CreationDate >= yearStart &&
                                                            x._CreationDate <= yearEnd &&
                                                            x._Validated == 1).Count();
                    product._StartDate = yearStart;
                    product._EndDate = yearEnd;
                    product._DateString = yearStart.ToString("yyyy");
                    product._Position = i;
                    yearlyProduction.Add(product);

                    // yearly order info
                    var yearOrder = new OrderDoneAndPending
                    {
                        _OrderDone = orders.Where(x => x._CreationDate >= yearStart &&
                                                                x._CreationDate <= yearEnd &&
                                                                x._Status == Status.Done).Count(),
                        _OrderPending = orders.Where(x => x._CreationDate >= yearStart &&
                                                                x._CreationDate <= yearEnd &&
                                                                x._Status == Status.OnProgress || x._Status == Status.NotStarted).Count(),
                        _StartDate = yearStart,
                        _EndDate = yearEnd,
                        _DateString = yearStart.ToString("yyyy"),
                        _Position = i
                    };
                    yearlyOrder.Add(yearOrder);


                    // yearly key info
                    var yearKeyInfo = new KeyInfo
                    {
                        _Produced = productions.Where(x => x._CreationDate >= yearStart &&
                                                                x._CreationDate <= yearEnd && x._ProductType == ProductType.Key && x._Status == ProductionStatus.Produced)
                                                                .Sum(x => x._Quantity),
                        _Blocked = productions.Where(x => x._CreationDate >= yearStart &&
                                                                x._CreationDate <= yearEnd && x._ProductType == ProductType.Key && x._Status == ProductionStatus.Blocked)
                                                                .Sum(x => x._Quantity),
                        _Reclaimed = productions.Where(x => x._CreationDate >= yearStart &&
                                                                x._CreationDate <= yearEnd && x._ProductType == ProductType.Key && x._Status == ProductionStatus.Recliamed)
                                                                .Sum(x => x._Quantity),
                        _StartDate = yearStart,
                        _EndDate = yearEnd,
                        _DateString = yearStart.ToString("yyyy"),
                        _Position = i
                    };
                    yearlyKeyInfo.Add(yearKeyInfo);

                    // yearly cylinder info
                    var cylInfo = new CylinderInfo
                    {
                        _Assembled = productions.Where(x => x._CreationDate >= yearStart &&
                                                                x._CreationDate <= yearEnd && x._ProductType == ProductType.Cylinder && x._Status == ProductionStatus.Assembled)
                                                                .Sum(x => x._Quantity),
                        _Blocked = productions.Where(x => x._CreationDate >= yearStart &&
                                                                x._CreationDate <= yearEnd && x._ProductType == ProductType.Cylinder && x._Status == ProductionStatus.Blocked)
                                                                .Sum(x => x._Quantity),
                        _Reclaimed = productions.Where(x => x._CreationDate >= yearStart &&
                                                                x._CreationDate <= yearEnd && x._ProductType == ProductType.Cylinder && x._Status == ProductionStatus.Recliamed)
                                                                .Sum(x => x._Quantity),
                        _StartDate = yearStart,
                        _EndDate = yearEnd,
                        _DateString = yearStart.ToString("yyyy"),
                        _Position = i
                    };
                    yearlyCylinderInfo.Add(cylInfo);
                }

                #endregion

                #region order info

                // total order info
                totalOrder._OrderDone = orders.Where(x => x._Status == Status.Done).Count();
                totalOrder._OrderPending = orders.Where(x => x._Status == Status.OnProgress || x._Status == Status.NotStarted).Count();
                #endregion

                #region keyInfo
                
                // this total
                totalKeyInfo._Produced = keys.Sum(x => x._Produced);
                totalKeyInfo._Blocked = keys.Sum(x => x._Produced);
                totalKeyInfo._Reclaimed = keys.Sum(x => x._Reclaimed);

                #endregion

                #region cylinderInfo
                
                // this total
                totalCylinderInfo._Assembled = cylinders.Sum(x => x._Assembled);
                totalCylinderInfo._Blocked = cylinders.Sum(x => x._Blocked);
                totalCylinderInfo._Reclaimed = cylinders.Sum(x => x._Validated);
                #endregion

                // customer count && partner count
                int customerCount = 0;
                if(user.PartnerID == null)
                {
                    customerCount = await _Context.Customers.CountAsync();
                }
                else
                {
                    customerCount = await _Context.Customers.Where(x => x.PartnerID == user.PartnerID).CountAsync();
                }
                int partnerCount = await _Context.Partners.CountAsync();

                DashboardResource resource = new DashboardResource
                {
                    #region response
                    _CurrentOrder = currentOrder,
                    _OrderDetail = orderDetail,
                    _DailyProduction = dailyProduction,
                    _WeeklyProduction = weeklyProduction,
                    _MonthlyProduction = monthlyProduction,
                    _YearlyProduction = yearlyProduction,
                    _DailyOrder = dailyOrder,
                    _WeeklyOrder = weeklyOrder,
                    _MonthlyOrder = monthlyOrder,
                    _YearlyOrder = yearlyOrder,
                    _TotalOrder = totalOrder,
                    _CustomerCount = customerCount,
                    _PartnerCount = partnerCount,

                    _DailyKeyInfo = dailyKeyInfo,
                    _WeeklyKeyInfo = weeklyKeyInfo,
                    _MonthlyKeyInfo = monthlyKeyInfo,
                    _YearlyKeyInfo = yearlyKeyInfo,
                    _TotalKeyInfo = totalKeyInfo,

                    _DailyCylinderInfo = dailyCylinderInfo,
                    _WeeklyCylinderInfo = weeklyCylinderInfo,
                    _MonthlyCylinderInfo = monthlyCylinderInfo,
                    _YearlyCylinderInfo = yearlyCylinderInfo,
                    _TotalCylinderInfo = totalCylinderInfo,

                    _TotalCylinders = cylinders.Count(),
                    _TotalKeys = groups.Count()
                    #endregion
                };
                return resource;
            }
            catch (Exception ex)
            {
                Logs.logError("StatsRepository", "DashboardStats", ex.Message);
                Console.WriteLine(ex.Message);
                throw;
            }
            
        }


        #region date ranges
        public struct DateRange
        {
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
        }

        public static DateRange ThisYear(DateTime date)
        {
            DateRange range = new DateRange();

            range.Start = new DateTime(date.Year, 1, 1);
            range.End = range.Start.AddYears(1).AddSeconds(-1);

            return range;
        }

        public static DateRange LastYear(DateTime date)
        {
            DateRange range = new DateRange();

            range.Start = new DateTime(date.Year - 1, 1, 1);
            range.End = range.Start.AddYears(1).AddSeconds(-1);

            return range;
        }

        public static DateRange ThisMonth(DateTime date)
        {
            DateRange range = new DateRange();

            range.Start = new DateTime(date.Year, date.Month, 1);
            range.End = range.Start.AddMonths(1).AddSeconds(-1);

            return range;
        }

        public static DateRange LastMonth(DateTime date)
        {
            DateRange range = new DateRange();

            range.Start = (new DateTime(date.Year, date.Month, 1)).AddMonths(-1);
            range.End = range.Start.AddMonths(1).AddSeconds(-1);

            return range;
        }

        public static DateRange ThisWeek(DateTime date)
        {
            DateRange range = new DateRange();

            range.Start = date.Date.AddDays(-(int)date.DayOfWeek);
            range.End = range.Start.AddDays(7).AddSeconds(-1);

            return range;
        }

        public static DateRange LastWeek(DateTime date)
        {
            DateRange range = ThisWeek(date);

            range.Start = range.Start.AddDays(-7);
            range.End = range.End.AddDays(-7);

            return range;
        }

        public static DateRange TwoWeeksAgo(DateTime date)
        {
            DateRange range = ThisWeek(date);

            range.Start = range.Start.AddDays(-14);
            range.End = range.End.AddDays(-14);

            return range;
        }
        public static DateRange ThreeWeeksAgo(DateTime date)
        {
            DateRange range = ThisWeek(date);

            range.Start = range.Start.AddDays(-21);
            range.End = range.End.AddDays(-21);

            return range;
        }
        public static DateRange FourWeeksAgo(DateTime date)
        {
            DateRange range = ThisWeek(date);

            range.Start = range.Start.AddDays(-28);
            range.End = range.End.AddDays(-28);

            return range;
        }
        #endregion
    }
}
