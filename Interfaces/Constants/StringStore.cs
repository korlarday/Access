using Allprimetech.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Allprimetech.Interfaces.Constants
{
    public class StringStore
    {
        public static string _ErrorOccured = "Sorry, something went wrong.";
        public static string _Success = "Success";
        public static string _Failed = "Failed";
        public static string _IncorrectLogin = "Username or password is incorrect";
        public static string _WaitFor5Minites = "Please wait at least 5mins to resend verification code";
        public static string _InvalidOrder = "Invalid Order";
        public static string CustomerExists = "Customer Exists";
        public static string DiscExists = "Disc Exists";

        #region Production Search
        public static string GetKeyProductionHistory = @"Select 
                                        p.ProductionID, p._Quantity, concat(u._FirstName, ' ', u._LastName) as _CreatedBy, k._Name, k._KeyNumber, 
                                        p._Status, p._CreationDate, o._OrderNumber, k.GroupID
                                        From productions p 
                                        inner join groups k on p._ProductID = k.GroupID 
                                        inner join orders o on o.OrderID = k.OrderID 
                                        inner join aspnetusers u on p.ByUserId = u.Id
                                        where p._ProductType = 1 and k.CustomerID = @customerId";

        public static string GetCylinderProductionHistory = @" Select 
                                        p.ProductionID, p._Quantity, c._DoorName, c._CylinderNumber, 
                                        p._Status, c._Options, c._LengthInside, c._LengthOutside, 
                                        c._Color, p._CreationDate, o._OrderNumber, concat(u._FirstName, ' ', u._LastName) as _CreatedBy
                                        From productions p 
                                        inner join cylinders c on p._ProductID = c.CylinderID 
                                        inner join orders o on o.OrderID = c.OrderID
                                        inner join aspnetusers u on p.ByUserId = u.Id
                                        where p._ProductType = 2 and c.CustomerID = @customerId";

        public static string SearchProductionStatus = " and p._Status = @status";

        public static string SearchOrderNumber = " and o._OrderNumber like @orderNumber";
        public static string SearchDateFrom = " and cast(p._CreationDate as date) >= @from";
        public static string SearchDateTo = " and cast(p._CreationDate as date) <= @to";
        #endregion

        #region Search Customers
        public static string SearchCustomers = @"Select c.CustomerID, c._Name, c._CustomerNumber, c._SystemCode, c._ContactPerson,  
                                        p._Name as Partner, p.PartnerID, count(o.CustomerID) as _Orders, c._CreationDate, c._UpdatedDate,  
                                        concat(u._FirstName, ' ', u._LastName) as CreatedBy, c.CreatedById, c._InstallationCode
                                        from sap_lite.customers c 
                                        left outer join sap_lite.partners p on c.PartnerID = p.PartnerID 
                                        left outer join sap_lite.orders o on o.CustomerID = c.CustomerID 
                                        left outer join sap_lite.aspnetusers u on c.CreatedById = u.Id 
                                        where 1=1";
        public static string SearchWithPartnerId = " and c.PartnerID = @partnerId";
        public static string SearchNameField = " and c._Name like @searchValue"; 

        public static string SearchCustomerNumbField = " and c._CustomerNumber like @searchValue";
        public static string SearchCustomerPartnerField = " and p._Name like @searchValue";
        public static string SearchSystemCodeField = " and c._SystemCode like @searchValue";
        public static string GroupByCustomerId = " group by c.CustomerID";

        #endregion

        public static string InvalidCustomer = "Invalid customer name";
        public static string GetKeyGroupCylinderPairs = @"select cm._Name as _Customer, c._DoorName, g._Name as _KeyName, 
                                                        cg.CustomerID, cg.CylinderID, cg.GroupID  from cylindergroups cg
                                                        inner join cylinders c on cg.CylinderID = c.CylinderID
                                                        inner join groups g on cg.GroupID = g.GroupID
                                                        inner join customers cm on cg.CustomerID = cm.CustomerId
                                                        where cg.CustomerID = @customerId
                                                        group by GroupID";

        public static string OrderNonExists = "Order does not exists";
        public static string MergingQuery = @"select GroupID , group_concat(CylinderID ORDER BY CylinderID asc) as Cylinders from cylindergroups group by GroupID;";
        public static string GetKeyCylinderDefinitions = @"select group_concat(GroupID order by GroupID ASC) as GroupID, Cylinders, count(GroupID) as Occurrence from 
                                                            (select group_concat(CylinderID order by CylinderID ASC) as Cylinders, GroupID from cylindergroups where CustomerID = @customerId group by GroupID)
                                                             cylindergroups group by Cylinders ";
        public static string GetCylinderRelatedGrouping = @"SET SESSION group_concat_max_len = 1000000;
                                                            select cg.CylinderID as _CylinderID, c._DoorName, c._CylinderNumber, group_concat(GroupID separator ' ') as _RelatedGroups 
                                                            from cylindergroups cg inner join cylinders c on c.CylinderID = cg.CylinderID where cg.CustomerID = @customerId 
                                                            group by _CylinderID";
        public static string GetFinalGroupings = @"SELECT f.GroupID as _GroupID, f._RelatedGrouping as _RelatedGrouping, g._Name as _GroupName
                                                    FROM sap_lite.groupfinals f inner join groups g on g.GroupID = f.GroupID where f.CustomerID = @customerId;";
        public static string _InvalidProduct = "Invalid Product Specified";

        public static string InvalidCustomerOrGroup = "Invalid Group or customer supplied";
        public static string InvalidCustomerOrCylinder = "Invalid Customer or Cylinder";

        public static string GetDiscInfo = @"select d.DiscID, d._Name, d._Number, d._Slot, d._Type, 
                                            cy.CylinderID, cy._DoorName, c.CustomerID,
                                            cy._Quantity, cy._LengthInside, cy._LengthOutside, cy._Color, cy._Options,
                                            c._SystemCode, c._InstallationCode, cy._CylinderNumber 
                                            from discs d inner join 
                                            customers c on c.CustomerID = d.CustomerID
                                            inner join cylinders cy on cy.CylinderID = d.CylinderID
                                            where d.CustomerID = @customerId
                                            order by cy.CylinderID";
        public static string GetGroupInfo = @"SELECT gi.GroupsInfoID, gi._Slot, gi._Row, gi.CustomerID, 
                                            gi.GroupID, gi._Value, gi._CreationDate, gi._UpdatedDate,
                                            c._Name as Customer, c._SystemCode, c._InstallationCode,
                                            g._Name as _Group, g._GroupNumber, g._Quantity
                                            FROM sap_lite.groupsinfos gi 
                                            inner join groups g on g.GroupID = gi.GroupID
                                            inner join customers c on c.CustomerID = gi.CustomerID
                                            where gi.CustomerID = @customerId && g._Name like '%top%'";

        public static string GetDiscsTypes = @"SELECT d.DiscID, d._Name, d._Number, d._Slot, d._Type, d.CylinderID, 
                                            count(*) as _Occurrences, c._SystemCode, c._InstallationCode FROM discs d
                                            inner join customers c on c.CustomerID = d.CustomerID
                                            where d.CustomerID = @customerId group by _name;";
        public static string GetDiscStatistics = @"SELECT c.CustomerID, d.DiscID, d._Name, d._Number, d._Slot, d._Type, d.CylinderID, 
                                                    c._SystemCode, c._InstallationCode, cy._Quantity FROM discs d
                                                    inner join customers c on c.CustomerID = d.CustomerID
                                                    inner join cylinders cy on d.CylinderID = cy.CylinderID
                                                    where c.CustomerID = @customerId";
        public static string GetDiscStatisticsBruckner = @"SELECT c.CustomerID, d.DiscID, d._Name, d._Number, d._Slot, d._Type, d.CylinderID, 
                                                    c._SystemCode, c._InstallationCode, cy._Quantity, cy._DoorName FROM discs d
                                                    inner join customers c on c.CustomerID = d.CustomerID
                                                    inner join cylinders cy on d.CylinderID = cy.CylinderID
                                                    where c.CustomerID = @customerId and cy._DoorName like '%top%'";
        public static string GetCylindersWithRelatedGroupIDs = @"SET SESSION group_concat_max_len = 1000000;
                                                            select group_concat(distinct CylinderID separator ' ') 
                                                            as _CylinderIDs, RelatedGroups as _RelatedGroups from 
                                                            (select c.CylinderID, group_concat(distinct c.GroupID separator ' ') 
                                                            as RelatedGroups from cylindergroups c where CustomerID = @customerId group by CylinderID) as tb2
                                                            group by RelatedGroups";

        public static string GetCylindersWithAllRelatedGroups = @"SET SESSION group_concat_max_len = 1000000;
                                                                    select c.CylinderID as _CylinderID , group_concat(f._RelatedGrouping) as _RelatedGroups
                                                                    from cylindergroupsrelations c inner join groupfinals f on f.GroupID = c.GroupID
                                                                    where c.CustomerID = @customerId
                                                                    group by c.CylinderID";
        public static string PartnerExists = "Partner already exist";
        public static string GeyKeyCylinderRelationship = @"SET SESSION group_concat_max_len = 1000000;
                                                            SELECT GroupID as GroupKey, group_concat(CylinderID order by CylinderID asc separator ' ') as Cylinders FROM sap_lite.cylindergroups 
                                                            where CustomerID = @customerId
                                                            group by GroupID
                                                            order by GroupID asc";

        public static int Limit = 100000;
        public static string GetCylindersByGroupId = @"SELECT cg.CustomerID, c.CylinderID, c._DoorName, c._CylinderNumber,  g._GroupNumber, cg.GroupID, g._Name as _GroupName FROM sap_lite.cylindergroups cg
                                                        inner join cylinders c on c.CylinderID = cg.CylinderID
                                                        inner join groups g on g.GroupID = cg.GroupID
                                                        where cg.GroupID = @groupId and cg.CustomerID = @customerId;";

        public static string GetCylinderGrpsWithRelatedGroupIds = @"SET SESSION group_concat_max_len = 1000000;
                                                                    select group_concat(distinct CylinderID separator ' ') 
                                                                    as _CylinderIDs, RelatedGroups from 
                                                                    (select c.CylinderID, group_concat(distinct c.GroupID separator ' ') 
                                                                    as RelatedGroups from cylindergroups c where CustomerID = @customerId group by CylinderID) as tb2
                                                                    group by RelatedGroups;";
        public static string GetCylinderGrpsAndNumWithRelatedGroupIds = @"SET SESSION group_concat_max_len = 1000000;
                                                                        select group_concat(distinct CylinderID separator ' ') 
                                                                        as _CylinderIDs, group_concat(_CylinderNumber separator ',') as _CylinderNumbers, RelatedGroups from 
                                                                        (select c.CylinderID, cyl._CylinderNumber, group_concat(distinct c.GroupID separator ' ') 
                                                                        as RelatedGroups from cylindergroups c inner join cylinders cyl on c.CylinderID = cyl.CylinderID 
                                                                        where c.CustomerID = @customerId group by c.CylinderID) as tb2
                                                                        group by RelatedGroups;";

        public static string GetCylinderGroups = @"SET SESSION group_concat_max_len = 1000000;
                                                    select cg.CylinderID as _CylinderID, group_concat(cg.GroupID separator ' ') as _GroupIDs, 
                                                    group_concat(g._GroupNumber separator ', ') as _GroupNumbers
                                                    from cylindergroups cg inner join groups g on g.GroupID = cg.GroupID
                                                    where cg.CustomerID = @customerId
                                                    group by cg.CylinderID";

        public static string GetGroupCylinders = @"SET SESSION group_concat_max_len = 1000000;
                                                    select cg.GroupID as _GroupID, group_concat(cg.CylinderID separator ' ') as _CylinderIDs, 
                                                    cg.CustomerID as _CustomerID
                                                    from cylindergroupverifications cg 
                                                    where cg.CustomerID = @customerId
                                                    group by cg.GroupID";

        public static string InsertIntoCylinder = @"INSERT into cylinders
                                                    (_DoorName, _CreationDate, _UpdatedDate, 
                                                    _ArticleNumber, _LengthOutside, _LengthInside, 
                                                    _Color, _Options, _Quantity, _QRCodeIssued, _Assembled,
                                                    _Blocked, _Validated, _PositionId, CustomerID, OrderID,
                                                    _Reclaimed, _CylinderNumber, _BatchNumber)
                                                    values ";
        public static string InsertIntoCylinderGroups = @"INSERT into cylindergroups
                                                    (CylinderID, GroupID, CustomerID)
                                                    values ";
        public static string InsertIntoGroup = @"INSERT into groups
                                                (_CreationDate, _UpdatedDate, 
                                                _Name, _Quantity, _KeyNumber, 
                                                _Produced, _Validated, _PositionId, CustomerID, OrderID,
                                                _Reclaimed, _Blocked, _GroupNumber, _BatchNumber)
                                                values ";

        public static string GetGroupsByCylinderId = @"select cg.CylinderID, cg.GroupID as _GroupID, cg.CustomerID, g._Name as _GroupName, g._GroupNumber 
                                                        from cylindergroups cg
                                                        inner join groups g on g.GroupID = cg.GroupID
                                                        where cg.CustomerID = @customerId and cg.CylinderID = @cylinderId";

        public static string GetSortedGroups = @"select count(cg.CylinderID) as _Count, cg.GroupID as _GroupID, cg.CustomerID, g._Name as _GroupName, g._GroupNumber
                                                from cylindergroups cg
                                                inner join groups g on g.GroupID = cg.GroupID
                                                where cg.CustomerID = @customerId group by cg.GroupID";

        public static string GetSortedCylinders = @"select count(cg.GroupID) as _Count, cg.CylinderID as _CylinderID, cg.CustomerID, c._DoorName, c._CylinderNumber
                                                        from cylindergroups cg
                                                        inner join cylinders c on c.CylinderID = cg.CylinderID
                                                        where cg.CustomerID = @customerId group by cg.CylinderID";

        public static string GetFilteredLP = @"SET SESSION group_concat_max_len = 1000000;
                                                    select cg.CustomerID as _CustomerID, count(cg.CylinderID) as _Count, group_concat(cg.CylinderID separator ' ') as 
                                                    _CylinderIDs, cg.GroupID as _GroupID, g._Name, c._DoorName from {{db}} cg
                                                    inner join groups g on cg.GroupID = g.GroupID
                                                    inner join cylinders c on cg.CylinderID = c.CylinderID ";

        public static string FilterTopByGroupName = "and g._Name like '%top%' ";
        public static string FilterTopNotByGroupName = "and g._Name not like '%top%' ";
        public static string FilterTopByCylinderName = "and c._DoorName like '%top%' ";
        public static string FilterTopByNotByCylinderName = "and c._DoorName not like '%top%' ";
        public static string FilterGroupByGroupID = "group by cg.GroupID";

        public static string GetFilteredLPNotByTop = @"SET SESSION group_concat_max_len = 1000000;
                                                    select cg.CustomerID as _CustomerID, group_concat(cg.CylinderID separator ' ') as 
                                                    _CylinderIDs, cg.GroupID as _GroupID, g._Name, c._DoorName from cylindergroupverifications cg
                                                    inner join groups g on cg.GroupID = g.GroupID
                                                    inner join cylinders c on cg.CylinderID = c.CylinderID
                                                    where g._Name not like '%top%' and c._DoorName not like '%top%' and  cg.CustomerID = @customerId
                                                    group by cg.GroupID";

        public static string GetExistingLP = @"SET SESSION group_concat_max_len = 1000000;
                                                select cg.CustomerID as _CustomerID, count(cg.CylinderID) as _Count, group_concat(cg.CylinderID separator ' ') as 
                                                _CylinderIDs, cg.GroupID as _GroupID, g._Name, c._DoorName from cylindergroups cg
                                                inner join groups g on cg.GroupID = g.GroupID
                                                inner join cylinders c on cg.CylinderID = c.CylinderID
                                                where cg.CustomerID = @customerId
                                                group by cg.GroupID";

        public static string NotFound = "Item not found";
        public static string BadRequest = "The request is not well formatted, make sure to provide the required parameters";

        public static string UserExistsResponse = "The user already exists in the system, try with different email or username";

        public static string ExceptionMessage = "An exception has occured during the execution of the process";

        public static string SuccessExecutionMessage = "The operation has been successfully executed!";

        public static string SomethingWentWrong = "Something went wrong!";

        public static string DefaultPassword = "password1";

        public static string EmptyDetailsList = "Order details list is empty";

        public static string InvalidQuantityRequested = "Invalid quantity requested for production";
        public static string CylinderAndKeyProductionPending = "Customer's cylinder and key production is still pending.";
        public static string CylinderProductionPending = "Customer's cylinder production is still pending.";
        public static string KeyProductionPending = "Customer's key production is still pending.";

        public static string SystemAuditDescription(Operation operation, Source source)
        {
            return "item " + operation.ToString() + ", operation done in source - " + source.ToString() + ".";
        }
    }
}
