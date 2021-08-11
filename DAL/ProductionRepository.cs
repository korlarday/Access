using Allprimetech.GeneralUtils;
using Allprimetech.Interfaces;
using Allprimetech.Interfaces.Constants;
using Allprimetech.Interfaces.Models;
using Allprimetech.Interfaces.Resources;
using Allprimetech.ServiceRestAPI.Metadatas;
using Allprimetech.ServiceRestAPI.Proxy;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allprimetech.DAL
{ 
    public class ProductionRepository : IProductionRepository
    {
        private ApplicationDbContext _Context;
        private DbContextOptions<ApplicationDbContext> _Options;
        private IMapper _Mapper { get; set; }

        public ProductionRepository(ApplicationDbContext context, DbContextOptions<ApplicationDbContext> options, IMapper mapper)
        {
            _Context = context;
            _Options = options;
            _Mapper = mapper;
        }

        #region Production CRUD
        public async Task<List<Production>> AllProduction()
        {
            try
            {
                return await _Context.Productions.ToListAsync();
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "AllProduction", ex.Message);
                throw;
            }
        }

        public async Task<Production> GetProduction(int? productionId)
        {
            try
            {
                return await _Context.Productions.FindAsync(productionId);
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "GetProduction", ex.Message);
                throw;
            }
        }
        public async Task<object> GetProduct(Production production)
        {
            try
            {
                var order = await _Context.Orders.FindAsync(production.OrderID);

                if (production._ProductType == ProductType.Key)
                {
                    var key = await _Context.Groups.FindAsync(production._ProductID);
                    KeyProductionResource keyProduction = new KeyProductionResource()
                    {
                        ProductionID = production.ProductionID,
                        _CreationDate = production._CreationDate,
                        _OrderNumber = order._OrderNumber,
                        _Status = production._Status.ToString(),
                        _KeyName = key != null ? key._Name : "",
                        _KeyNumber = key != null ? key._KeyNumber : ""
                    };
                    return keyProduction;
                }
                else
                {
                    var cylinder = await _Context.Cylinders.FindAsync(production._ProductID);
                    CylinderProductionResource cylinderProduction = new CylinderProductionResource()
                    {
                        ProductionID = production.ProductionID,
                        _CreationDate = production._CreationDate,
                        _OrderNumber = order._OrderNumber,
                        _Status = production._Status.ToString(),
                        _CylinderNumber = cylinder != null ? cylinder._CylinderNumber : "",
                        _Description = cylinder != null ? cylinder._Options.ToString() + " " + cylinder._LengthInside.ToString() + " " + cylinder._Color.ToString() : "",
                        _DoorName = cylinder != null ? cylinder._DoorName : ""
                    };
                    return cylinderProduction;
                }
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "GetProduct", ex.Message);
                throw;
            }
        }
        #region Create Production
        public async Task<ProductionResponse> CreateProduction(Production production, int produced, string userId)
        {
            try
            {
                var response = new ProductionResponse { _ProductType = production._ProductType, _ProductID = production._ProductID, _Succeeded = false };

                var order = await GetOrder(production.OrderID);
                var responseMessage = "";
                var user = await _Context.Users.Include(x => x.Country).SingleOrDefaultAsync(x => x.Id == userId);


                // first check if the production of an order exist
                Group group = null;
                Cylinder cylinder = null;

                if (production._ProductType == ProductType.Key)
                {
                    group = await _Context.Groups.FindAsync(production._ProductID);
                    if (group == null)
                    {
                        response._Message = StringStore._InvalidProduct;
                        return response;
                    }
                    production.OrderID = 0;
                    if (group.OrderID != null) production.OrderID = (int)group.OrderID;

                    // check if quantity requested is not greater than the group quantity
                    if(produced > group._Quantity || produced < 1)
                    {
                        response._Message = StringStore.InvalidQuantityRequested;
                        return response;
                    }
                    // APQ => Absolute Production Quantity i.e Quantity - (blocked + reclamed + produced)
                    // PN => Production Number i.e the number of items to produce
                    // Produceable => number of quantities that can still be produced 

                    var APQ = group._Quantity - (group._Blocked + group._Reclaimed + group._Produced);
                    var PN = produced;

                    if (production._Status == ProductionStatus.Produced)
                    {
                        int unreclaimCount = 0;
                        int unblockedCount = 0;

                        var Produceable = group._Quantity - group._Produced;
                        if(PN <= Produceable)
                        {
                            if(PN <= APQ)
                            {
                                
                                responseMessage = "Produce Successful";
                                group._Produced += PN;
                            }
                            else
                            {
                                

                                // let SN => PN - APQ
                                var SN = PN - APQ;
                                int added = PN - APQ;
                                if (group._Blocked > 0)
                                {
                                    if(group._Blocked >= SN)
                                    {
                                        unblockedCount = SN;
                                        group._Blocked -= SN;
                                        SN = 0;
                                    }
                                    else
                                    {
                                        unblockedCount = group._Blocked;
                                        SN = SN - group._Blocked;
                                        group._Blocked = 0;
                                    }
                                }

                                if(SN > 0)
                                {
                                    if(group._Reclaimed > 0)
                                    {
                                        if(group._Reclaimed >= SN)
                                        {
                                            unreclaimCount = SN;
                                            group._Reclaimed -= SN;
                                            SN = 0;
                                        }
                                        else
                                        {
                                            unreclaimCount = group._Reclaimed;
                                            SN = SN - group._Reclaimed;
                                            group._Reclaimed = 0;
                                        }
                                    }
                                }

                                group._Produced += added;
                                group._Produced += APQ;
                            }

                            if(group._Produced >= group._Quantity)
                            {
                                group._Validated = 1;
                            }
                            else
                            {
                                group._Validated = 0;
                            }
                            //generate response message 
                            if(unblockedCount != 0 && unreclaimCount != 0)
                            {
                                responseMessage = "Produce Successful, " + unblockedCount + " Key(s) unblocked, and " + unreclaimCount + " Key(s) unreclamed"; 
                            }
                            else if(unblockedCount != 0 && unreclaimCount == 0)
                            {
                                responseMessage = "Produce Successful, " + unblockedCount + " Key(s) unblocked";
                            }
                            else if (unblockedCount == 0 && unreclaimCount != 0)
                            {
                                responseMessage = "Produce Successful, " + unreclaimCount + " Key(s) unreclamed";
                            }
                            else
                            {
                                responseMessage = "Produce Successful";
                            }
                        }
                        else
                        {
                            response._Message = StringStore.InvalidQuantityRequested;
                            return response;
                        }
                    }
                    else if(production._Status == ProductionStatus.Recliamed)
                    {
                        int unproduced = 0;
                        int unblockedCount = 0;

                        if (PN <= APQ)
                        {
                            responseMessage = "Reclam Successful";
                            group._Reclaimed += PN;
                        }
                        else
                        {
                            // let SN => PN - APQ
                            var SN = PN - APQ;
                            int added = PN - APQ;
                            if(group._Produced > 0)
                            {
                                if (group._Produced >= SN)
                                {
                                    unproduced = SN;
                                    group._Produced -= SN;
                                    SN = 0;
                                }
                                else
                                {
                                    unproduced = group._Produced;
                                    SN = SN - group._Produced;
                                    group._Produced = 0;
                                }
                            }

                            if (SN > 0)
                            {
                                if (group._Blocked > 0)
                                {
                                    if (group._Blocked >= SN)
                                    {
                                        unblockedCount = SN;
                                        group._Blocked -= SN;
                                        SN = 0;
                                    }
                                    else
                                    {
                                        unblockedCount = group._Blocked;
                                        SN = SN - group._Blocked;
                                        group._Blocked = 0;
                                    }
                                }
                            }

                            group._Reclaimed += added;
                            group._Reclaimed += APQ;
                        }
                        if (group._Produced >= group._Quantity)
                        {
                            group._Validated = 1;
                        }
                        else
                        {
                            group._Validated = 0;
                        }

                        //generate response message 
                        if (unproduced != 0 && unblockedCount != 0)
                        {
                            responseMessage = "Reclam Successful, " + unproduced + " Key(s) Nullified, and " + unblockedCount + " Key(s) unblocked";
                        }
                        else if (unproduced != 0 && unblockedCount == 0)
                        {
                            responseMessage = "Reclam Successful, " + unproduced + " Key(s) Nullified";
                        }
                        else if (unproduced == 0 && unblockedCount != 0)
                        {
                            responseMessage = "Reclam Successful, " + unblockedCount + " Key(s) unblocked";
                        }
                        else
                        {
                            responseMessage = "Reclam Successful";
                        }

                    }
                    else if (production._Status == ProductionStatus.Blocked)
                    {
                        var Blockable = group._Quantity - (group._Produced + group._Blocked);
                        if(PN <= Blockable)
                        {
                            int unreclaimed = 0;

                            if (PN <= APQ)
                            {
                                responseMessage = "Block Successful";
                                group._Blocked += PN;
                            }
                            else
                            {
                                // let SN => PN - APQ
                                var SN = PN - APQ;
                                int added = PN - APQ;
                                if (group._Reclaimed > 0)
                                {
                                    if (group._Reclaimed >= SN)
                                    {
                                        unreclaimed = SN;
                                        group._Reclaimed -= SN;
                                        SN = 0;
                                    }
                                    else
                                    {
                                        unreclaimed = group._Reclaimed;
                                        SN = SN - group._Reclaimed;
                                        group._Reclaimed = 0;
                                    }
                                }

                                group._Blocked += added;
                                group._Blocked += APQ;
                            }

                            if (group._Produced >= group._Quantity)
                            {
                                group._Validated = 1;
                            }
                            else
                            {
                                group._Validated = 0;
                            }

                            // generate response message
                            if (unreclaimed != 0)
                            {
                                responseMessage = "Block Successful, " + unreclaimed + " Key(s) unreclamed";
                            }
                            else
                            {
                                responseMessage = "Block Successful";
                            }
                        }
                        else
                        {
                            response._Message = StringStore.InvalidQuantityRequested;
                            return response;
                        }
                    }
                    
                }
                else
                {
                    cylinder = await _Context.Cylinders.FindAsync(production._ProductID);
                    if (cylinder == null)
                    {
                        response._Message = StringStore._InvalidProduct;
                        return response;
                    }

                    production.OrderID = 0;
                    if (cylinder.OrderID != null) production.OrderID = (int)cylinder.OrderID;

                    // check if quantity requested is not greater than the group quantity
                    if (produced > cylinder._Quantity || produced < 1)
                    {
                        response._Message = StringStore.InvalidQuantityRequested;
                        return response;
                    }

                    // APQ => Absolute Production Quantity i.e Quantity - (blocked + reclamed + produced)
                    // PN => Production Number i.e the number of items to produce
                    // Produceable => number of quantities that can still be produced 

                    var APQ = cylinder._Quantity - (cylinder._Blocked + cylinder._Reclaimed + cylinder._Assembled);
                    var PN = produced;

                    if (production._Status == ProductionStatus.Assembled)
                    {
                        int unreclaimCount = 0;
                        int unblockedCount = 0;

                        var Produceable = cylinder._Quantity - cylinder._Assembled;
                        if (PN <= Produceable)
                        {
                            if (PN <= APQ)
                            {
                                responseMessage = "Assembly Successful";
                                cylinder._Assembled += PN;
                            }
                            else
                            {
                                // let SN => PN - APQ
                                var SN = PN - APQ;
                                int added = PN - APQ;
                                if (cylinder._Blocked > 0)
                                {
                                    if (cylinder._Blocked >= SN)
                                    {
                                        unblockedCount = SN;
                                        cylinder._Blocked -= SN;
                                        SN = 0;
                                    }
                                    else
                                    {
                                        unblockedCount = cylinder._Blocked;
                                        SN = SN - cylinder._Blocked;
                                        cylinder._Blocked = 0;
                                    }
                                }

                                if (SN > 0)
                                {
                                    if (cylinder._Reclaimed > 0)
                                    {
                                        if (cylinder._Reclaimed >= SN)
                                        {
                                            unreclaimCount = SN;
                                            cylinder._Reclaimed -= SN;
                                            SN = 0;
                                        }
                                        else
                                        {
                                            unreclaimCount = cylinder._Reclaimed;
                                            SN = SN - cylinder._Reclaimed;
                                            cylinder._Reclaimed = 0;
                                        }
                                    }
                                }

                                cylinder._Assembled += added;
                                cylinder._Assembled += APQ;
                            }

                            if (cylinder._Assembled >= cylinder._Quantity)
                            {
                                cylinder._Validated = 1;
                            }
                            else
                            {
                                cylinder._Validated = 0;
                            }


                            if (unblockedCount != 0 && unreclaimCount != 0)
                            {
                                responseMessage = "Assembly Successful, " + unblockedCount + " Cylinder(s) unblocked, and " + unreclaimCount + " Cylinder(s) unreclamed";
                            }
                            else if (unblockedCount != 0 && unreclaimCount == 0)
                            {
                                responseMessage = "Assembly Successful, " + unblockedCount + " Cylinder(s) unblocked";
                            }
                            else if (unblockedCount == 0 && unreclaimCount != 0)
                            {
                                responseMessage = "Assembly Successful, " + unreclaimCount + " Cylinder(s) unreclamed";
                            }
                            else
                            {
                                responseMessage = "Assembly Successful";
                            }
                        }
                        else
                        {
                            response._Message = StringStore.InvalidQuantityRequested;
                            return response;
                        }
                    }
                    else if (production._Status == ProductionStatus.Recliamed)
                    {
                        int unproduced = 0;
                        int unblockedCount = 0;

                        if (PN <= APQ)
                        {
                            responseMessage = "Reclam Successful";
                            cylinder._Reclaimed += PN;
                        }
                        else
                        {
                            // let SN => PN - APQ
                            var SN = PN - APQ;
                            int added = PN - APQ;

                            if (cylinder._Assembled > 0)
                            {
                                if (cylinder._Assembled >= SN)
                                {
                                    unproduced = SN;
                                    cylinder._Assembled -= SN;
                                    SN = 0;
                                }
                                else
                                {
                                    unproduced = cylinder._Assembled;
                                    SN = SN - cylinder._Assembled;
                                    cylinder._Assembled = 0;
                                }
                            }

                            if (SN > 0)
                            {
                                if (cylinder._Blocked > 0)
                                {
                                    if (cylinder._Blocked >= SN)
                                    {
                                        unblockedCount = SN;
                                        cylinder._Blocked -= SN;
                                        SN = 0;
                                    }
                                    else
                                    {
                                        unblockedCount = cylinder._Blocked;
                                        SN = SN - cylinder._Blocked;
                                        cylinder._Blocked = 0;
                                    }
                                }
                            }

                            cylinder._Reclaimed += added;
                            cylinder._Reclaimed += APQ;
                        }
                        if (cylinder._Assembled >= cylinder._Quantity)
                        {
                            cylinder._Validated = 1;
                        }
                        else
                        {
                            cylinder._Validated = 0;
                        }



                        //generate response message 
                        if (unproduced != 0 && unblockedCount != 0)
                        {
                            responseMessage = "Reclam Successful, " + unproduced + " Cylinder(s) Nullified, and " + unblockedCount + " Cylinder(s) unblocked";
                        }
                        else if (unproduced != 0 && unblockedCount == 0)
                        {
                            responseMessage = "Reclam Successful, " + unproduced + " Cylinder(s) Nullified";
                        }
                        else if (unproduced == 0 && unblockedCount != 0)
                        {
                            responseMessage = "Reclam Successful, " + unblockedCount + " Cylinder(s) unblocked";
                        }
                        else
                        {
                            responseMessage = "Reclam Successful";
                        }
                    }
                    else if (production._Status == ProductionStatus.Blocked)
                    {
                        var Blockable = cylinder._Quantity - (cylinder._Assembled + cylinder._Blocked);
                        if (PN <= Blockable)
                        {
                            int unreclaimed = 0;
                            if (PN <= APQ)
                            {
                                responseMessage = "Block Successful";
                                cylinder._Blocked += PN;
                            }
                            else
                            {
                                // let SN => PN - APQ
                                var SN = PN - APQ;
                                int added = PN - APQ;
                                if (cylinder._Reclaimed > 0)
                                {
                                    if (cylinder._Reclaimed >= SN)
                                    {
                                        unreclaimed = SN;
                                        cylinder._Reclaimed -= SN;
                                        SN = 0;
                                    }
                                    else
                                    {
                                        unreclaimed = cylinder._Reclaimed;
                                        SN = SN - cylinder._Reclaimed;
                                        cylinder._Reclaimed = 0;
                                    }
                                }

                                cylinder._Blocked += added;
                                cylinder._Blocked += APQ;
                            }

                            if (cylinder._Assembled >= cylinder._Quantity)
                            {
                                cylinder._Validated = 1;
                            }
                            else
                            {
                                cylinder._Validated = 0;
                            }

                            // generate response message
                            if (unreclaimed != 0)
                            {
                                responseMessage = "Block Successful, " + unreclaimed + " Cylinder(s) unreclamed";
                            }
                            else
                            {
                                responseMessage = "Block Successful";
                            }
                        }
                        else
                        {
                            response._Message = StringStore.InvalidQuantityRequested;
                            return response;
                        }
                    }
                }
                int cylinderAssembled = await _Context.Cylinders.Where(x => x.OrderID == order.OrderID).SumAsync(x => x._Assembled);
                int keyProduced = await _Context.Groups.Where(x => x.OrderID == order.OrderID).SumAsync(x => x._Produced);
                if(keyProduced >= order._GroupKeyQty && cylinderAssembled >= order._CylinderQty)
                {
                    order._Status = Status.Done;
                }
                else
                {
                    order._Status = Status.OnProgress;
                }
                var currentUtcTime = DateTime.UtcNow;

                TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById(user.Country._TimeZoneName);
                DateTime dateNow = TimeZoneInfo.ConvertTimeFromUtc(currentUtcTime, cstZone);

                production._CreationDate = dateNow;
                production._UpdatedDate = dateNow;

                await _Context.Productions.AddAsync(production);
                await _Context.SaveChangesAsync();
                if(production._ProductType == ProductType.Key)
                {
                    
                    return new ProductionResponse {
                        _Succeeded = true, 
                        _Message = responseMessage, 
                        _ProductType = ProductType.Key, 
                        _ProductID = production._ProductID, 
                        _Group = _Mapper.Map<Group, ReadGroupResource>(group)
                        
                    };
                }
                else
                {
                    return new ProductionResponse { 
                        _Succeeded = true, 
                        _Message = responseMessage,
                        _ProductType = ProductType.Cylinder, 
                        _ProductID = production._ProductID, 
                        _Cylinder = _Mapper.Map<Cylinder, ReadCylinderResource>(cylinder) 
                    };
                }
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "CreateProduction", ex.Message);
                throw;
            }
        }

        #endregion
        public void DeleteProduction(Production production)
        {
            try
            {
                _Context.Productions.Remove(production);
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "DeleteProduction", ex.Message);
                throw;
            }
        }
        
        #endregion

        public async Task<Order> GetOrder(int orderId)
        {
            try
            {
                return await _Context.Orders.FindAsync(orderId);
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "GetOrder", ex.Message);
                throw;
            }
        }
        public async Task<IEnumerable<Production>> SearchProductions(ProductionSearchResource model)
        {
            try
            {
                var productions = await _Context.Productions.ToListAsync();
                productions = productions.Where(x => x._CreationDate.Date >= model._StartDate.Date &&
                                                    x._CreationDate.Date <= model._EndDate.Date)
                                            .ToList();
                return productions;
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "SearchProductions", ex.Message);
                throw;
            }
        }

        public async Task<List<ReadCylinderGroupResource>> StoreCylinderGroups(List<CreateCylinderGroupResource> keysCylinders)
        {
            ArrayList arrayList = new ArrayList(keysCylinders);
            

            //List<int> Tmp = new List<int>();  
            //ArrayList TmpOb = new ArrayList();

             
            //DateTime dt = DateTime.Now;
            //double Elapsedms = 0;
            //int Counter = 0;
 
           
            //for (int k = 0; k < 1000; k++)
            //{
            //    for (int z = 0; z < 1000; z++)
            //    { 
            //        TmpOb.Add(Counter);
            //        Tmp.Add(Counter++);
            //    }

            //}

            //Elapsedms = ((TimeSpan)(DateTime.Now - dt)).TotalMilliseconds;
            //Console.WriteLine($"Elaspsed Ms : {Elapsedms}");
            //dt = DateTime.Now;
            //Counter = 0;

            //for (int k = 0; k < 1000; k++)
            //{
            //    for (int z = 0; z < 1000; z++)
            //    { 
            //        var t = TmpOb[Counter++];
            //    }

            //}


            //Elapsedms = ((TimeSpan)(DateTime.Now - dt)).TotalMilliseconds;
            //Console.WriteLine($"Elaspsed Ms : {Elapsedms}");
            //dt = DateTime.Now;
            //Counter = 0;

            //for (int k = 0; k < 1000; k++)
            //{
            //    for (int z = 0; z < 1000; z++)
            //    {
            //      var t =  Tmp[Counter++];

            //    }

            //}


            //Elapsedms = ((TimeSpan)(DateTime.Now - dt)).TotalMilliseconds;
            //Console.WriteLine($"Elaspsed Ms : {Elapsedms}");

            /* 1000 * 1000 =  1M */




            List<ReadCylinderGroupResource> cylinderGroups = new List<ReadCylinderGroupResource>();
            string insertQuery = "";

            ArrayList TmpArr = new ArrayList();
            StringBuilder S = new StringBuilder();
            StringBuilder S2 = new StringBuilder();

            StringBuilder[] Sb = new StringBuilder[1000];

            //for (int i = 0; i < sb.Length; i++)
            //{
            //    sb[i] = new StringBuilder("");
            //}

            S.Append(StringStore.InsertIntoCylinderGroups);
            S2.Append(StringStore.InsertIntoCylinderGroups);
            int Counter = 0;
            int ArrayCount = 0;

            Sb[ArrayCount] = new StringBuilder();

            try
            {
                if(keysCylinders != null && keysCylinders.Count() > 0)
                {
                    var groupCylItem = keysCylinders.FirstOrDefault();
                    var customer = await _Context.Customers.FindAsync(groupCylItem._CustomerID);

                    //optimizations = optimizations.OrderByDescending(x => x.Cylinders.Length).ToList();
                    if (customer != null)
                    {
                        int customerId = customer.CustomerID;

                        //var context = new ApplicationDbContext(_Options);

                        int count = 1;
                        for (int i = 0; i < keysCylinders.Count; i++)
                        { 
                            for (int j = 0; j < keysCylinders[i]._CylinderIDs.Length; j++)
                            {
                                Counter++;
                                //CylinderGroup cylinderGroup = new CylinderGroup
                                //{
                                //    CylinderID = keysCylinders[i]._CylinderIDs[j],
                                //    GroupID = keysCylinders[i]._GroupID,
                                //    CustomerID = customerId
                                //};
                                //_Context.CylinderGroups.Add(cylinderGroup);

                                //ReadCylinderGroupResource cylinderGroupResource = new ReadCylinderGroupResource
                                //{
                                //    _CylinderID = cylinderId,
                                //    _GroupID = groupId,
                                //    CustomerID = customerId
                                //};

                                if(keysCylinders[i]._GroupID != 0)
                                {
                                    Sb[ArrayCount].Append("(" + keysCylinders[i]._CylinderIDs[j] + "," + keysCylinders[i]._GroupID + "," + customerId + "),");

                                    if (Counter == 35000)
                                    {
                                        Counter = 0;
                                        ++ArrayCount;
                                        Sb[ArrayCount] = new StringBuilder();
                                    }

                                }



                            }
                            ++count;
                            
                            #region sql query
                            //insertQuery = insertQuery.Remove(insertQuery.Length - 1);  
                            //var sqlScript = StringStore.InsertIntoCylinderGroups + insertQuery +";";
                            //int r = await context.Database..ExecuteSqlRawAsync(sqlScript);
                             

                            //List<CylinderGroup> cylGrps = new List<CylinderGroup>();
                            //await Task.Run(() =>
                            //{
                            //    using ()
                            //    {
                                   

                            //    }
                            //});
                            #endregion

                            //insertQuery = "";
                        }
                         

                        string ss = S.ToString();
                        string ss2 = S2.ToString();



                        //int r = await context.Database.ExecuteSqlRawAsync(ss.Remove(ss.Length - 1)) ;
                        // r = await context.Database.ExecuteSqlRawAsync(ss2.Remove(ss2.Length - 1));
                        string sql = "";

                        //await Task.Run(() =>
                        //{
                        for (int i = 0; i < Sb.Length; i++)
                        {
                            string query = Sb[i] != null ? Sb[i].ToString() : String.Empty;
                            if (!String.IsNullOrEmpty(query))
                            {
                                var context = new ApplicationDbContext(_Options);
                                //var r = await context.Database.ExecuteSqlRawAsync(StringStore.InsertIntoCylinderGroups + query.Remove(query.Length - 1));

                                using (var command = context.Database.GetDbConnection().CreateCommand())
                                {
                                    command.CommandTimeout = 600000;
                                    command.CommandText = StringStore.InsertIntoCylinderGroups + query.Remove(query.Length - 1);
                                    command.CommandType = CommandType.Text;
                                    context.Database.OpenConnection();
                                    context.Database.SetCommandTimeout(600000);
                                    var result = command.ExecuteReader();
                                }
                            }

                        }

                        //string ConnectionString = "server=localhost;port=3306;user=root;password=admin;Database=sap_lite;";
                        //using (MySqlConnection mConnection = new MySqlConnection(ConnectionString))
                        //{
                        //    mConnection.Open();
                        //    using (MySqlTransaction trans = mConnection.BeginTransaction())
                        //    {
                        //        for (int i = 0; i < Sb.Length; i++)
                        //        {
                        //            string query = Sb[i] != null ? Sb[i].ToString() : String.Empty;
                        //            if (!String.IsNullOrEmpty(query))
                        //            {
                        //                using (MySqlCommand myCmd = new MySqlCommand(StringStore.InsertIntoCylinderGroups + query.Remove(query.Length - 1), mConnection, trans))
                        //                {
                        //                    myCmd.CommandType = CommandType.Text;
                        //                    myCmd.CommandTimeout = 600000;
                        //                    myCmd.ExecuteNonQuery();
                        //                //trans.Commit();
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                        //});
                        //await Task.Run(() =>
                        //{

                        //});


                        await _Context.SaveChangesAsync();



                        //await _Context.SaveChangesAsync();

                        // delete all previous groupings
                        #region Delete Previous Groupings
                        // remove cylinder group relations
                        var cylGrpsRelations = await _Context.CylinderGroupsRelations
                                                            .Where(x => x.CustomerID == customer.CustomerID)
                                                            .ToListAsync();
                        if(cylGrpsRelations != null && cylGrpsRelations.Count > 0)
                        {
                            for (int i = 0; i < cylGrpsRelations.Count; i++)
                            {
                                var cylGrpRel = cylGrpsRelations[i];
                                _Context.CylinderGroupsRelations.Remove(cylGrpRel);
                            }
                        }
                        // remove group finals
                        var grpFinals = await _Context.GroupFinals.Where(x => x.CustomerID == customer.CustomerID)
                                                                    .ToListAsync();
                        if (grpFinals != null && grpFinals.Count > 0)
                        {
                            for (int i = 0; i < grpFinals.Count; i++)
                            {
                                var grpFinal = grpFinals[i];
                                _Context.GroupFinals.Remove(grpFinal);
                            }
                        }
                        // remove group summaries
                        var grpSummaries = await _Context.GroupSummaries.Where(x => x.CustomerID == customer.CustomerID)
                                                                    .ToListAsync();
                        if (grpSummaries != null && grpSummaries.Count > 0)
                        {
                            for (int i = 0; i < grpSummaries.Count; i++)
                            {
                                var grpSummary = grpSummaries[i];
                                _Context.GroupSummaries.Remove(grpSummary);
                            }
                        }
                        // remove KeyGroupSylinderAnalyses
                        var keyGrpCylAnalysis = await _Context.KeyGroupCylinderAnalyses.Where(x => x.CustomerID == customer.CustomerID)
                                                                    .ToListAsync();
                        if (keyGrpCylAnalysis != null && keyGrpCylAnalysis.Count > 0)
                        {
                            for (int i = 0; i < keyGrpCylAnalysis.Count; i++)
                            {
                                var keyGrpAnalysis = keyGrpCylAnalysis[i];
                                _Context.KeyGroupCylinderAnalyses.Remove(keyGrpAnalysis);
                            }
                        }
                        // remove KeyGroupSylinderDetails
                        var keyGrpCylDetails = await _Context.KeyGroupCylinderDetails.Where(x => x.CustomerID == customer.CustomerID)
                                                                    .ToListAsync();
                        if (keyGrpCylDetails != null && keyGrpCylDetails.Count > 0)
                        {
                            for (int i = 0; i < keyGrpCylDetails.Count; i++)
                            {
                                var keyGrpDetail = keyGrpCylDetails[i];
                                _Context.KeyGroupCylinderDetails.Remove(keyGrpDetail);
                            }
                        }
                        await _Context.SaveChangesAsync();
                        #endregion

                        // fetch the updated relationships from the database
                        var context1 = new ApplicationDbContext(_Options);
                        List<GroupsOptimization> optimizations = new List<GroupsOptimization>();

                        #region Get Updated Relationships from the database
                        await Task.Run(() =>
                        {
                            using (var command = context1.Database.GetDbConnection().CreateCommand())
                            {

                                var sqlScript = StringStore.GeyKeyCylinderRelationship;

                                command.CommandText = sqlScript;
                                command.CommandType = CommandType.Text;
                                command.Parameters.Add(new MySqlParameter("@customerId", customerId));

                                context1.Database.OpenConnection();

                                using (var result = command.ExecuteReader())
                                {
                                    while (result.Read())
                                    {
                                        GroupsOptimization grpOptimization = new GroupsOptimization();
                                        grpOptimization.Cylinders = result["Cylinders"].ToString();
                                        grpOptimization.GroupKey = result["GroupKey"].ToString();

                                        optimizations.Add(grpOptimization);
                                    }
                                }
                            }

                        });

                        #endregion
                        // then save to new relationship to the database
                        await MergeRelationship(optimizations, customer.CustomerID);

                        await SaveDiscGroupInfo(customerId);
                    }
                    else
                    {
                        return null;
                    }
                }
                return cylinderGroups;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Message => " + ex.Message);
                Logs.logError("ProductionRepository", "StoreCylinderGroups", ex.Message);
                throw;
            }
        }

        public async Task<List<ReadCylinderGroupResource>> ModifyCylinderGroups(List<ModifyCylinderGroup> keysCylinders)
        {
            List<ReadCylinderGroupResource> cylinderGroups = new List<ReadCylinderGroupResource>();
            try
            {
                if (keysCylinders != null && keysCylinders.Count() > 0)
                {
                    var groupCylItem = keysCylinders.FirstOrDefault();
                    var customer = await _Context.Customers.FindAsync(groupCylItem._CustomerID);

                    //optimizations = optimizations.OrderByDescending(x => x.Cylinders.Length).ToList();
                    if (customer != null)
                    {
                        int customerId = customer.CustomerID;
                        var existingCylinderGroups = await _Context.CylinderGroups.Where(x => x.CustomerID == customerId).ToListAsync();

                        var cylinders = await _Context.Cylinders.Where(x => x.CustomerID == customerId).OrderBy(x => x.CustomerID).ToListAsync();
                        var groups = await _Context.Groups.Where(x => x.CustomerID == customerId).ToListAsync();

                        int count = 1;
                        for (int i = 0; i < keysCylinders.Count; i++)
                        {
                            //var groupId = keysCylinders[i]._GroupId;
                            for (int j = 0; j < keysCylinders[i]._CylinderPosNumbers.Length; j++)
                            {
                                //var positionId = keysCylinders[i]._CylinderPosNumbers[j];
                                //var cylinderId = cylinders.Where(x => x.CylinderID == positionId).FirstOrDefault().CylinderID;
                                

                                //ReadCylinderGroupResource cylinderGroupResource = new ReadCylinderGroupResource
                                //{
                                //    _CylinderID = cylinderId,
                                //    _GroupID = keysCylinders[i]._GroupId,
                                //    CustomerID = customerId
                                //};

                                //cylinderGroups.Add(cylinderGroupResource);
                                if(!existingCylinderGroups.Any(x => x.CylinderID == keysCylinders[i]._CylinderPosNumbers[j] && x.GroupID == keysCylinders[i]._GroupId))
                                {
                                    CylinderGroup cylinderGroup = new CylinderGroup
                                    {
                                        CylinderID = keysCylinders[i]._CylinderPosNumbers[j],
                                        GroupID = keysCylinders[i]._GroupId,
                                        CustomerID = customerId
                                    };
                                    await _Context.CylinderGroups.AddAsync(cylinderGroup);
                                }
                            }
                        }
                        

                        await _Context.SaveChangesAsync();

                        // delete all previous groupings
                        #region Delete Previous Groupings
                        // remove cylinder group relations
                        var cylGrpsRelations = await _Context.CylinderGroupsRelations
                                                            .Where(x => x.CustomerID == customer.CustomerID)
                                                            .ToListAsync();
                        if (cylGrpsRelations != null && cylGrpsRelations.Count > 0)
                        {
                            for (int i = 0; i < cylGrpsRelations.Count; i++)
                            {
                                var cylGrpRel = cylGrpsRelations[i];
                                _Context.CylinderGroupsRelations.Remove(cylGrpRel);
                            }
                        }
                        // remove group finals
                        var grpFinals = await _Context.GroupFinals.Where(x => x.CustomerID == customer.CustomerID)
                                                                    .ToListAsync();
                        if (grpFinals != null && grpFinals.Count > 0)
                        {
                            for (int i = 0; i < grpFinals.Count; i++)
                            {
                                var grpFinal = grpFinals[i];
                                _Context.GroupFinals.Remove(grpFinal);
                            }
                        }
                        // remove group summaries
                        var grpSummaries = await _Context.GroupSummaries.Where(x => x.CustomerID == customer.CustomerID)
                                                                    .ToListAsync();
                        if (grpSummaries != null && grpSummaries.Count > 0)
                        {
                            for (int i = 0; i < grpSummaries.Count; i++)
                            {
                                var grpSummary = grpSummaries[i];
                                _Context.GroupSummaries.Remove(grpSummary);
                            }
                        }
                        // remove KeyGroupSylinderAnalyses
                        var keyGrpCylAnalysis = await _Context.KeyGroupCylinderAnalyses.Where(x => x.CustomerID == customer.CustomerID)
                                                                    .ToListAsync();
                        if (keyGrpCylAnalysis != null && keyGrpCylAnalysis.Count > 0)
                        {
                            for (int i = 0; i < keyGrpCylAnalysis.Count; i++)
                            {
                                var keyGrpAnalysis = keyGrpCylAnalysis[i];
                                _Context.KeyGroupCylinderAnalyses.Remove(keyGrpAnalysis);
                            }
                        }
                        // remove KeyGroupSylinderDetails
                        var keyGrpCylDetails = await _Context.KeyGroupCylinderDetails.Where(x => x.CustomerID == customer.CustomerID)
                                                                    .ToListAsync();
                        if (keyGrpCylDetails != null && keyGrpCylDetails.Count > 0)
                        {
                            for (int i = 0; i < keyGrpCylDetails.Count; i++)
                            {
                                var keyGrpDetail = keyGrpCylDetails[i];
                                _Context.KeyGroupCylinderDetails.Remove(keyGrpDetail);
                            }
                        }
                        await _Context.SaveChangesAsync();
                        #endregion

                        // fetch the updated relationships from the database
                        var context = new ApplicationDbContext(_Options);
                        List<GroupsOptimization> optimizations = new List<GroupsOptimization>();

                        #region Get Updated Relationships from the database
                        await Task.Run(() =>
                        {
                            using (var command = context.Database.GetDbConnection().CreateCommand())
                            {

                                var sqlScript = StringStore.GeyKeyCylinderRelationship;

                                command.CommandText = sqlScript;
                                command.CommandType = CommandType.Text;
                                command.Parameters.Add(new MySqlParameter("@customerId", customerId));

                                context.Database.OpenConnection();

                                using (var result = command.ExecuteReader())
                                {
                                    while (result.Read())
                                    {
                                        GroupsOptimization grpOptimization = new GroupsOptimization();
                                        grpOptimization.Cylinders = result["Cylinders"].ToString();
                                        grpOptimization.GroupKey = result["GroupKey"].ToString();

                                        optimizations.Add(grpOptimization);
                                    }
                                }
                            }

                        });

                        #endregion
                        // then save to new relationship to the database
                        await MergeRelationship(optimizations, customer.CustomerID);

                        await SaveDiscGroupInfo(customerId);
                    }
                    else
                    {
                        return null;
                    }
                }
                return cylinderGroups;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Message => " + ex.Message);
                Logs.logError("ProductionRepository", "StoreCylinderGroups", ex.Message);
                throw;
            }
        }


        public async Task<List<ReadCylinderGroupResource>> StoreCylinderGroupVerification(List<CreateCylinderGroupVerification> keysCylinders)
        {
            List<ReadCylinderGroupResource> cylinderGroups = new List<ReadCylinderGroupResource>();
            try
            {
                if (keysCylinders != null && keysCylinders.Count() > 0)
                {
                    var groupCylItem = keysCylinders.FirstOrDefault();
                    var customer = await _Context.Customers.FindAsync(groupCylItem._CustomerID);

                    if (customer != null)
                    {
                        int customerId = customer.CustomerID;

                        int count = 1;
                        foreach (var groupItem in keysCylinders)
                        {
                            foreach (var cylinderId in groupItem._CylinderIDs)
                            {
                                int groupId = groupItem._GroupID;
                                CylinderGroupVerification cylinderGroup = new CylinderGroupVerification
                                {
                                    CylinderID = cylinderId,
                                    GroupID = groupId,
                                    CustomerID = customerId
                                };

                                ReadCylinderGroupResource cylinderGroupResource = new ReadCylinderGroupResource
                                {
                                    _CylinderID = cylinderId,
                                    _GroupID = groupId,
                                    CustomerID = customerId
                                };

                                cylinderGroups.Add(cylinderGroupResource);
                                await _Context.CylinderGroupVerifications.AddAsync(cylinderGroup);
                                //if(!_Context.CylinderGroups.Any(x => x.CylinderID == cylinderId && x.GroupID == cylinderId))
                                //{
                                //}
                                if (count == 1000)
                                {
                                    await _Context.SaveChangesAsync();
                                };
                            }
                        }

                        await _Context.SaveChangesAsync();
                    }
                    else
                    {
                        return null;
                    }
                }
                return cylinderGroups;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Message => " + ex.Message);
                Logs.logError("ProductionRepository", "StoreCylinderGroups", ex.Message);
                throw;
            }
        }

        public async Task<List<CylinderProduction>> GetCylinderProductionHistory(int customerId)
        {
            try
            {
                var context = new ApplicationDbContext(_Options);
                List<CylinderProduction> cylinderProductions = new List<CylinderProduction>();

                await Task.Run(() =>
                {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {

                        var sqlScript = StringStore.GetCylinderProductionHistory;

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;

                        command.Parameters.Add(new MySqlParameter("@customerId", customerId));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {

                            while (result.Read())
                            {
                                CylinderProduction production = new CylinderProduction();
                                production.ProductionID = result["ProductionID"] is DBNull ? 0 : Convert.ToInt32(result["ProductionID"]);
                                production._DoorName = result["_DoorName"].ToString();
                                production._CylinderNumber = result["_CylinderNumber"].ToString();
                                production._Status = (ProductionStatus)Convert.ToInt32(result["_Status"]);
                                production._LengthInside = result["_LengthInside"].ToString();
                                production._LengthOutside = result["_LengthOutside"].ToString();
                                production._Options = (Options)Convert.ToInt32(result["_Options"]);
                                production._Quantity = Convert.ToInt32(result["_Quantity"]);
                                production._Color = result["_Color"].ToString();
                                production._OrderNumber = result["_OrderNumber"].ToString();
                                production._CreatedBy = result["_CreatedBy"].ToString();
                                production._CreationDate = (DateTime)result["_CreationDate"];

                                cylinderProductions.Add(production);
                            }
                        
                        }
                    }


                });
                return cylinderProductions;
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "GetCylinderProductionHistory", ex.Message);
                throw;
            }
        }

        public async Task<List<KeyProduction>> GetKeyProductionHistory(int customerId)
        {
            try
            {
                var context = new ApplicationDbContext(_Options);
                List<KeyProduction> keyProductions = new List<KeyProduction>();

                await Task.Run(() =>
                {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {
                        var sqlScript = StringStore.GetKeyProductionHistory;
                    
                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;

                        command.Parameters.Add(new MySqlParameter("@customerId", customerId));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {

                            while (result.Read())
                            {
                                KeyProduction production = new KeyProduction();
                                production.ProductionID = result["ProductionID"] is DBNull ? 0 : Convert.ToInt32(result["ProductionID"]);
                                production._KeyName = result["_Name"].ToString();
                                production._KeyNumber = result["_KeyNumber"].ToString();
                                production._Status = (ProductionStatus)Convert.ToInt32(result["_Status"]);
                                production._OrderNumber = result["_OrderNumber"].ToString();
                                production._CreationDate = (DateTime)result["_CreationDate"];
                                production._CreatedBy = result["_CreatedBy"].ToString();
                                production._Quantity = Convert.ToInt32(result["_Quantity"]);

                                keyProductions.Add(production);
                            }

                        }
                    }


                });
                return keyProductions;

            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "GetKeyProductionHistory", ex.Message);
                throw;
            }
        }

        public async Task<List<CylinderProduction>> SearchCylinderProduction(ProductionProdSearchResource model)
        {
            try
            {
                //ProductionSearchEnum selectedFieldType = (ProductionSearchEnum)Enum.Parse(typeof(ProductionSearchEnum), model._SearchTerm);

                var from = DateTime.UtcNow;
                var to = DateTime.UtcNow;

                var context = new ApplicationDbContext(_Options);
                #region Cylinder Search
                List<CylinderProduction> cylinderProductions = new List<CylinderProduction>();
                await Task.Run(() => {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {
                        var sqlScript = StringStore.GetCylinderProductionHistory;

                        if (model._ProductionStatus != 0)
                        {
                            sqlScript += StringStore.SearchProductionStatus;
                        }
                        if (model._OrderNumbers != null && model._OrderNumbers.Count > 0)
                        {
                            if(model._OrderNumbers.Count() == 1)
                            {
                                sqlScript += $" and o._OrderNumber = @orderNumber1";
                            }
                            else
                            {
                                for (int i = 0; i < model._OrderNumbers.Count; i++)
                                {
                                    var item = model._OrderNumbers[i];
                                    var label = i + 1;
                                    if(i == 0)
                                    {
                                        sqlScript += $" and (o._OrderNumber = @orderNumber{label}";
                                    }
                                    else
                                    {
                                        sqlScript += $" or o._OrderNumber = @orderNumber{label}";
                                        if (i == model._OrderNumbers.Count() - 1)
                                        {
                                            sqlScript += ")";
                                        }
                                    }
                                }
                            }
                        }

                        if (model._From != null)
                        {
                            from = (DateTime)model._From;
                            sqlScript += StringStore.SearchDateFrom;
                        }
                        if (model._To != null)
                        {
                            to = (DateTime)model._To;
                            sqlScript += StringStore.SearchDateTo;
                        }

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;

                        if (model._OrderNumbers != null && model._OrderNumbers.Count > 0)
                        {
                            //sqlScript += StringStore.SearchOrderNumber;
                            for (int i = 0; i < model._OrderNumbers.Count; i++)
                            {
                                var item = model._OrderNumbers[i];
                                var label = i + 1;
                                if (!String.IsNullOrWhiteSpace(item))
                                {
                                    command.Parameters.Add(new MySqlParameter($"@orderNumber{label}", item));
                                }
                            }
                        }


                        //command.Parameters.Add(new MySqlParameter("@orderNumber", "%" + model._OrderNumber + "%"));
                        command.Parameters.Add(new MySqlParameter("@status", model._ProductionStatus));
                        command.Parameters.Add(new MySqlParameter("@from", from));
                        command.Parameters.Add(new MySqlParameter("@to", to));
                        command.Parameters.Add(new MySqlParameter("@customerId", model._CustomerID));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {
                            while (result.Read())
                            {
                                CylinderProduction production = new CylinderProduction();
                                production.ProductionID = result["ProductionID"] is DBNull ? 0 : Convert.ToInt32(result["ProductionID"]);
                                production._DoorName = result["_DoorName"].ToString();
                                production._CylinderNumber = result["_CylinderNumber"].ToString();
                                production._Status = (ProductionStatus)Convert.ToInt32(result["_Status"]);
                                production._LengthInside = result["_LengthInside"].ToString();
                                production._LengthOutside = result["_LengthOutside"].ToString();
                                production._Options = (Options)Convert.ToInt32(result["_Options"]);
                                production._Color = result["_Color"].ToString();
                                production._OrderNumber = result["_OrderNumber"].ToString();
                                production._CreationDate = (DateTime)result["_CreationDate"];
                                production._CreatedBy = result["_CreatedBy"].ToString();
                                production._Quantity = Convert.ToInt32(result["_Quantity"]);

                                cylinderProductions.Add(production);
                            }
                        }
                    }
                });
                return cylinderProductions;
                #endregion
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "SearchCylinderProduction", ex.Message);
                throw;
            }
        }

        public async Task<List<KeyProduction>> SearchKeyProduction(ProductionProdSearchResource model)
        {
            try
            {
                //ProductionSearchEnum selectedFieldType = (ProductionSearchEnum)Enum.Parse(typeof(ProductionSearchEnum), model._SearchTerm);

                var from = DateTime.UtcNow;
                var to = DateTime.UtcNow;

                var context = new ApplicationDbContext(_Options);

                #region Key Search
                List<KeyProduction> keyProductions = new List<KeyProduction>();
                await Task.Run(() => {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {
                        var sqlScript = StringStore.GetKeyProductionHistory;

                        if(model._ProductionStatus != 0)
                        {
                            sqlScript += StringStore.SearchProductionStatus;
                        }

                        if (model._OrderNumbers != null && model._OrderNumbers.Count > 0)
                        {
                            if(model._OrderNumbers.Count() == 1)
                            {
                                sqlScript += $" and o._OrderNumber = @orderNumber1";
                            }
                            else
                            {
                                for (int i = 0; i < model._OrderNumbers.Count; i++)
                                {
                                    var item = model._OrderNumbers[i];
                                    var label = i + 1;
                                    if (i == 0)
                                    {
                                        sqlScript += $" and (o._OrderNumber = @orderNumber{label}";
                                    }
                                    else
                                    {
                                        sqlScript += $" or o._OrderNumber = @orderNumber{label}";
                                        if (i == model._OrderNumbers.Count() - 1)
                                        {
                                            sqlScript += ")";
                                        }
                                    }
                                }
                            

                            }
                        }

                        if (model._From != null)
                        {
                            from = (DateTime)model._From;
                            sqlScript += StringStore.SearchDateFrom;
                        }
                        if (model._To != null)
                        {
                            to = (DateTime)model._To;
                            sqlScript += StringStore.SearchDateTo;
                        }

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;


                        if (model._OrderNumbers != null && model._OrderNumbers.Count > 0)
                        {
                            //sqlScript += StringStore.SearchOrderNumber;
                            for (int i = 0; i < model._OrderNumbers.Count; i++)
                            {
                                var item = model._OrderNumbers[i];
                                var label = i + 1;
                                if (!String.IsNullOrWhiteSpace(item))
                                {
                                    command.Parameters.Add(new MySqlParameter($"@orderNumber{label}", item));
                                }
                            }
                        }

                        //command.Parameters.Add(new MySqlParameter("@orderNumber", "%" + model._OrderNumber + "%"));
                        command.Parameters.Add(new MySqlParameter("@status", model._ProductionStatus));
                        command.Parameters.Add(new MySqlParameter("@from", from));
                        command.Parameters.Add(new MySqlParameter("@to", to));
                        command.Parameters.Add(new MySqlParameter("@customerId", model._CustomerID));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {

                            while (result.Read())
                            {
                                KeyProduction production = new KeyProduction();
                                production.ProductionID = result["ProductionID"] is DBNull ? 0 : Convert.ToInt32(result["ProductionID"]);
                                production._KeyName = result["_Name"].ToString();
                                production._KeyNumber = result["_KeyNumber"].ToString();
                                production._Status = (ProductionStatus)Convert.ToInt32(result["_Status"]);
                                production._OrderNumber = result["_OrderNumber"].ToString();
                                production._CreationDate = (DateTime)result["_CreationDate"];
                                production._CreatedBy = result["_CreatedBy"].ToString();
                                production._Quantity = Convert.ToInt32(result["_Quantity"]);

                                keyProductions.Add(production);
                            }
                        }
                    }
                });
                return keyProductions;
                #endregion
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "SearchKeyProduction", ex.Message);
                throw;
            }
        }

        public async Task<List<ReadCylinderGroupResource>> GetKeyGroupCylinderPairs(int customerId)
        {
            try
            {
                var context = new ApplicationDbContext(_Options);
                #region Get Key Cylinders pairs
                List<ReadCylinderGroupResource> keysCylinders = new List<ReadCylinderGroupResource>();
                await Task.Run(() => {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {
                        var sqlScript = StringStore.GetKeyGroupCylinderPairs;


                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;

                        command.Parameters.Add(new MySqlParameter("@customerId", customerId ));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {

                            while (result.Read())
                            {
                                ReadCylinderGroupResource keyCylinder = new ReadCylinderGroupResource();
                                keyCylinder._DoorName = result["_DoorName"].ToString();
                                keyCylinder._Customer = result["_Customer"].ToString();
                                keyCylinder._KeyName = result["_KeyName"].ToString();
                                keyCylinder._CylinderID = Convert.ToInt32(result["CylinderID"]);
                                keyCylinder._GroupID = Convert.ToInt32(result["GroupID"]);
                                keyCylinder.CustomerID = Convert.ToInt32(result["CustomerID"]);

                                keysCylinders.Add(keyCylinder);
                            }
                        }
                    }
                });
                return keysCylinders;
                #endregion
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "GetKeyGroupCylinderPairs", ex.Message);
                throw;
            }
        }

        public async Task<object> MergeRelationship(List<GroupsOptimization> lockingPlan, int customerId)
        {
            try
            {
                var context = new ApplicationDbContext(_Options);
                #region Merge Relationship
                
                try
                {
                    List<MergingResource> mergings = new List<MergingResource>();

                    //List<GroupsOptimization> FromDB = new List<GroupsOptimization>()
                    //{
                    //    new GroupsOptimization { GroupKey ="G5", Cylinders ="Z1 Z2 Z4" },
                    //    new GroupsOptimization { GroupKey ="G1", Cylinders ="Z1 Z2" },
                    //    new GroupsOptimization { GroupKey ="G2", Cylinders ="Z3 Z4" },
                    //    new GroupsOptimization { GroupKey ="G3", Cylinders ="Z1 Z2" },
                    //    new GroupsOptimization { GroupKey ="G4", Cylinders ="Z1 Z4" }
                    //};

                    //List<GroupsOptimization> FromDB = new List<GroupsOptimization>()
                    //{
                    //    new GroupsOptimization { GroupKey ="5", Cylinders ="1,2,4,6,7,8" },
                    //    new GroupsOptimization { GroupKey ="1", Cylinders ="1,2,3,4,5" },
                    //    new GroupsOptimization { GroupKey ="2", Cylinders ="3,4" },
                    //    new GroupsOptimization { GroupKey ="3", Cylinders ="1,2" },
                    //    new GroupsOptimization { GroupKey ="4", Cylinders ="1,4" }
                    //};

                    //List<GroupsOptimization> FromDB = new List<GroupsOptimization>()
                    //{
                    //    new GroupsOptimization { GroupKey = "1", Cylinders = "1 2 3 4 5 7 9 10 11 12 13" },
                    //    new GroupsOptimization { GroupKey = "2", Cylinders = "1 2 3 4 11 12 13"},
                    //    new GroupsOptimization { GroupKey = "3", Cylinders = "1 7 9 10 11 12 13"},
                    //    new GroupsOptimization { GroupKey = "4", Cylinders = "1 2 3 4 5 7 9 13"},
                    //    new GroupsOptimization { GroupKey = "5", Cylinders = "1 3 4 5 7 9 10 11 12 13"},
                    //    new GroupsOptimization { GroupKey = "6", Cylinders = "1 3 4 5 12 13"},
                    //    new GroupsOptimization { GroupKey = "7", Cylinders = "1 10 11 12 13"},
                    //    new GroupsOptimization { GroupKey = "8", Cylinders = "1 3 4 5 7 9 10 11 12 13"},
                    //    new GroupsOptimization { GroupKey = "9", Cylinders = "1 10 11 12 13"},
                    //    new GroupsOptimization { GroupKey = "10", Cylinders = "1 2 3 4 12 13"}
                    //};

                    //List<GroupsOptimization> FromDB = new List<GroupsOptimization>()
                    //{
                    //    new GroupsOptimization { GroupKey ="G1", Cylinders ="Z1 Z2 Z6 Z10 Z17 Z18 Z16" },
                    //    new GroupsOptimization { GroupKey ="G50", Cylinders ="Z1 Z2 Z3 Z4 Z5 Z6 Z7 Z8 Z9 Z10 Z11 Z12 Z13 Z15" },
                    //    new GroupsOptimization { GroupKey ="G7", Cylinders ="Z1 Z2 Z3 Z4 Z30 Z20 Z21" },
                    //    new GroupsOptimization { GroupKey ="G9", Cylinders ="Z1 Z2 Z3 Z4 Z5 Z6 Z7 Z8" },
                    //    new GroupsOptimization { GroupKey ="G8", Cylinders ="Z1 Z2 Z3 Z4 Z5 Z6" }

                    //};
                    /*

                     GroupKey G8 || Cylinders SG3 (SG0 + SG1 )
                     GroupKey G7 || Cylinders Z30 Z20 Z21 SG2
                     GroupKey G9 || Cylinders SG4
                     GroupKey G1 || Cylinders SG0 VZ10 Z17 Z18 Z16
                     GroupKey G50 || Cylinders Z9 VZ10 Z11 Z12 Z13 Z15 SG4

                    GROUP ADDITION or SPLITTING

                     * G8 -> SG3 
                     * G7 -> SG5 Z3 Z4 Z30 Z20 Z21  
                     * G9 -> SG4 
                     * G1 -> SG5 Z6 Z10 Z17 Z18 Z16   
                     * G50 -> SG4 Z9 Z10 Z11 Z12 Z13 Z15
                 


                     * 
                     * 
                     * 
                     * 
                    */

                    /*
                     */

                    /*  * G2 -> Z3 Z4
                     * G3 -> Z1 Z2
                     * G4 -> Z1 Z4
                     * G5 -> Z1 Z2 Z4
                     * G6 -> Z1 Z2 Z3
                     * 
                     * 
                     * G1 -> Z1 Z2 Z6      
                     * G7 -> Z1 Z2 Z3 Z4                    
                     * G8 -> Z1 Z2 Z3 Z4 Z5 Z6               
                     * G9 -> Z1 Z2 Z3 Z4 Z5 Z6 Z7 Z8          
                     * G50 -> Z1 Z2 Z3 Z4 Z5 Z6 Z7 Z8 Z9 Z10
                     * 
                     * 
                     * G1 -> Sg1 Z6      
                     * G7 -> SG1 Z3 Z4                    
                     * G8 -> Sg1 Z3 Z4 Z5 Z6               
                     * G9 -> Sg1 Z3 Z4 Z5 Z6 Z7 Z8          
                     * G50 -> Sg1 Z3 Z4 Z5 Z6 Z7 Z8 Z9 Z10
                     * 
                     * G1 -> Sg2     
                     * G7 -> SG1 Z3 Z4                    
                     * G8 -> Sg2 Z3 Z4 Z5              
                     * G9 -> Sg2 Z3 Z4 Z5 Z7 Z8          
                     * G50 -> Sg2 Z3 Z4 Z5 Z7 Z8 Z9 Z10
                 
                     * G1 -> Sg2     
                     * G7 -> SG1 Z3 Z4                    
                     * G8 -> Sg2 Z3 Z4 Z5              
                     * G9 -> Sg2 Z3 Z4 Z5 Z7 Z8          
                     * G50 -> Sg2 Z3 Z4 Z5 Z7 Z8 Z9 Z10
                 
                     * G1 -> Sg2     
                     * G7 -> SG1 SG3                    
                     * G8 -> SG4              
                     * G9 -> SG5          
                     * G50 -> SG5 Z9 Z10 Z11, Z12, Z13, Z15
                     * 
                     * 
                      G1 -> Sg2     
                     * G7 -> SG1 Z3 Z4                    
                     * G8 -> Sg2 Z3 Z4 Z5              
                     * G9 -> Sg2 Z3 Z4 Z5 Z7 Z8          
                     * G50 -> Sg2 Z3 Z4 Z5 Z7 Z8 Z9 Z10
                     * 
                     * 
                     * G1 -> Sg2 Z11                        | Sg1 + Z6
                     * G7 -> SG1 Z3 Z4                     | G1-3 
                     * G8 -> Sg2 Z3 Z4 Z5               | G1-3 
                     * G9 -> Sg3 Z3 Z4 Z5 Z7 Z8         | G1-5  
                     * G50 -> Sg2 Z3 Z4 Z5 Z7 Z8 Z9 Z10 | G1-7
                     * 
             
                     * Cylinder sorting (sort the ID when added he list is populated 1 2 3 4......
                     * object Lenght sorting (based on Cylinders) 
                     * 
                     * Z1
                     * Z2
                     * Z3
                     * Z4
                     * 
                     * 
                     * G5 -> Z1 Z2 Z4 | 1
                     * G1 -> Z1 Z2    | 0 
                     * G2 -> Z3 Z4    | 0
                     * G3 -> Z1 Z2    | 0
                     * G4 -> Z1 Z4    | 
                     * 
                     * Z1 -> G4,G3,G1,G5
                     * Z2 -> G3,G1,G5
                     * Z3 -> G2
                     * Z4 -> G4,G2,G5
                     * 
                     * 
                     * 
                     * 
                     * var match = G4.All(x => G3.Contains(x));
                     * 
                     * G4 -> G5
                     * G3 -> G1 G5 => vG3
                     * G1 -> G5 
                     * -----------------------------------
                     * if you want, create another array object for the replacements
                     * 
                     * G5 -> vG3 Z4 |-1
                     * G1 -> vG3    | 1 
                     * G2 -> Z3 Z4    | 0
                     * G3 -> vG3    | 2
                     * G4 -> Z1 Z4    | 1
                     * 
                     * 
                     * vertical replacement result
                     * 
                     * G5 -> vG3 Z4 |-1
                     * G1 -> vG3    | 1 
                     * G2 -> Z3 Z4    | 0
                     * G3 -> vG3    | 2
                     * G4 -> Z1 Z4    | 1
                     * ---------------------
                     * Z4 -> G2 G5 => vZ4
                     * 
                     * ---------------------
                     * 
                     * 
                     * G5 -> vG3 vZ4 |-1
                     * G1 -> vG3    | 1 
                     * G2 -> Z3 vZ4    | 0
                     * G3 -> vG3    | 2
                     * G4 -> Z1 vZ4    | 1
                     * 
                     */

                    //var FromDB = keysCylinders;

                    //new GroupsOptimization { GroupKeys = "G5", Cylinders = "G1,G2,Z4" },
                    //new GroupsOptimization { GroupKeys = "G1,G2", Cylinders = "Z1,Z2" },
                    //new GroupsOptimization { GroupKeys = "G2", Cylinders = "Z3,Z4" }, 
                    //new GroupsOptimization { GroupKeys = "G4", Cylinders = "Z1,Z4" },

                    List<KeyGroupCylinderAnalysis> OptLevel = new List<KeyGroupCylinderAnalysis>();
                    /*sort Main data from DB*/
                    int count = lockingPlan.Count;
                    //List<GroupsOptimization> originalData = new List<GroupsOptimization>(FromDB);
                    List<GroupsOptimization> originalData = lockingPlan.ConvertAll(x => new GroupsOptimization(x));
                    List<GroupsOptimization> optimizations = new List<GroupsOptimization>();
                    List<Occurrence> occurrences = new List<Occurrence>();
                    List<OccurrenceSummary> occurrenceSummaries = new List<OccurrenceSummary>();
                    List<VerticalOccurrenceSummary> vertOccSummaries = new List<VerticalOccurrenceSummary>();
                    List<KeyGroupCylinderDetail> keyGroupCylinderDetails = new List<KeyGroupCylinderDetail>();

                    for (int i = 0; i < lockingPlan.Count; i++)
                    {
                        var item = lockingPlan[i];
                        var newItem = new KeyGroupCylinderDetail
                        {
                            GroupID = Convert.ToInt32(item.GroupKey),
                            _Cylinders = item.Cylinders,
                            CustomerID = customerId
                        };
                        _Context.KeyGroupCylinderDetails.Add(newItem);
                    }
                    await _Context.SaveChangesAsync();

                    var optimizedObjs = lockingPlan;

                    #region Horizontal Grouping

                    var groupSummaries = new List<GroupsOptimization>();
                    var subGroups = new List<SubGroups>();

                    // get the intersect between the 2 most smallest groups
                    optimizedObjs = optimizedObjs.OrderBy(x => x.Cylinders.Length).ToList();
                    var completeData = optimizedObjs.OrderBy(x => x.Cylinders.Length).ToList();
                    var rowsShift = 0;
                    var objCounts = optimizedObjs.Count(); 

                    bool isGroupAble = (optimizedObjs.Count > 1);
                    if (isGroupAble)
                    {
                        int groupId = 1;
                        var vn = "SG";
                        var ss = "SS";
                        var intersect = new List<string>();

                        do
                        {
                            if(rowsShift < objCounts)
                            {
                                var cylinders1 = optimizedObjs[rowsShift].Cylinders.Split(" ");
                                var cyl = optimizedObjs[rowsShift];

                                for (int i = 0; i < optimizedObjs.Count; i++)
                                {
                                    if(i != rowsShift)
                                    {
                                        var cylinders2 = optimizedObjs[i].Cylinders.Split(" ");
                                        intersect = cylinders1.Intersect(cylinders2).ToList();
                                        if(intersect.Count > 1)
                                        {
                                            break;
                                        }
                                    }
                                }


                                if(cylinders1.Length == 1 || intersect.Count() <= 1)
                                {
                                    ++rowsShift;
                                }
                                else
                                {
                                    //groupId = optimizedObjs[rowsShift].GroupKey;
                                    ++groupId;
                                    bool anyMatch = false;

                                    var containsSubGroup = intersect.Any(x => x.Contains("S"));
                                    SubGroups subGroup = null;
                                    if (containsSubGroup)
                                    {
                                        string itemSubgroups = "";
                                        var otherStrings = new List<string>(intersect);

                                        // seperate the group from others
                                        for (int i = 0; i < intersect.Count; i++)
                                        {
                                            if (intersect[i].Contains("S"))
                                            {
                                                itemSubgroups += intersect[i] + " ";
                                                otherStrings.Remove(intersect[i]);
                                            }
                                        }

                                        var cylinders = new List<string>();
                                        // remove rows of other groups
                                        for (int i = 0; i < otherStrings.Count; i++)
                                        {
                                            if (optimizedObjs[rowsShift].Cylinders.Contains(otherStrings[i]))
                                            {
                                                cylinders.Add(otherStrings[i]);
                                            }
                                        }

                                        var subGrpName = ss + groupId;
                                        var groupName = vn + (groupId + 1);
                                        ++groupId;
                                        subGroup = new SubGroups()
                                        {
                                            InnerGroups = itemSubgroups.Trim(),
                                            Cylinders = string.Join(" ", cylinders),
                                            GroupName = groupName,
                                            SubGroupName = subGrpName,
                                            IsSubGroup = true
                                        };
                                        subGroups.Add(subGroup);
                                    }
                                    else
                                    {
                                        var cylinders = new List<string>();
                                        // remove rows of other groups
                                        for (int i = 0; i < intersect.Count; i++)
                                        {
                                            if (optimizedObjs[rowsShift].Cylinders.Contains(intersect[i]))
                                            {
                                                cylinders.Add(intersect[i]);
                                            }
                                        }
                                        subGroup = new SubGroups()
                                        {
                                            InnerGroups = "",
                                            Cylinders = string.Join(" ", cylinders),
                                            GroupName = vn + groupId,
                                            SubGroupName = "",
                                            IsSubGroup = false
                                        };
                                        subGroups.Add(subGroup);
                                    }

                                    for (int i = 0; i < optimizedObjs.Count; i++)
                                    {
                                        var match = intersect.All(x => optimizedObjs[i].Cylinders.Contains(x));
                                        var cylinders = optimizedObjs[i].Cylinders.Split(" ");
                                        var cylindersCopy = new List<string>(cylinders);
                                        if (match)
                                        {
                                            anyMatch = true;
                                            // replace with virtual name
                                            for (int j = 0; j < intersect.Count(); j++)
                                            {
                                                if (cylinders.Contains(intersect[j]))
                                                {
                                                    cylindersCopy.Remove(intersect[j]);
                                                }
                                            }

                                            optimizedObjs[i].Cylinders = (string.Join(" ", cylindersCopy) + " " + subGroup.GroupName).Trim();
                                    
                                        }
                                    }
                                
                                    if(!anyMatch)
                                        ++rowsShift;

                                }

                            }
                            else
                            {
                                ++rowsShift;
                            }

                        } while (rowsShift <= objCounts);
                    }
                    // horizontal replacement

                    // Regroup subgroups
                    subGroups = subGroups.OrderBy(x => x.Cylinders.Length).ToList();
                    optimizedObjs = optimizedObjs.OrderByDescending(x => x.Cylinders.Length).ToList();

                    for (int i = 0; i < subGroups.Count; i++)
                    {
                        var subItems = subGroups[i].Cylinders.Trim().Split(" ");

                        if (subGroups[i].IsSubGroup)
                        {
                            for (int j = 0; j < optimizedObjs.Count; j++)
                            {
                                var cylinders = optimizedObjs[j].Cylinders.Split(" ");
                                var cylindersCopy = new List<string>(cylinders);

                                var match = subItems.All(x => cylinders.Contains(x));
                                if (match)
                                {
                                    for (int k = 0; k < subItems.Length; k++)
                                    {
                                        if (cylinders.Contains(subItems[k]))
                                        {
                                            cylindersCopy.Remove(subItems[k]);
                                        }
                                    }

                                    optimizedObjs[j].Cylinders = (string.Join(" ", cylindersCopy) + " " + subGroups[i].SubGroupName).Trim();
                                }
                            }
                        }
                    }

                    for (int i = 0; i < optimizedObjs.Count; i++)
                    {
                        var item = optimizedObjs[i];
                        string output = "GroupKey " + item.GroupKey + " || Cylinders " + item.Cylinders;
                        Console.WriteLine(output);

                    }

                    #endregion

                    #region Vertical
                    // Vertical Occurences
                    var verticalOccurrences = new List<VerticalOccurrence>();
                    for (int i = 0; i < count; i++)
                    {

                        var cylindersSuper = lockingPlan[i].Cylinders.Split(" ");
                        var groupKeySuper = lockingPlan[i].GroupKey;

                        for (int j = count - 1; j >= 0; j--)
                        {
                            if (j != i)
                            {
                                var cylindersSub = lockingPlan[j].Cylinders.Split(" ");
                                var groupKeySub = lockingPlan[j].GroupKey;
                                var occurrence = new VerticalOccurrence();
                                occurrence.NumberOccured = new Dictionary<string, int>();

                                for (int c = 0; c < cylindersSub.Length; c++)
                                {
                                    if (!cylindersSub[c].Contains("G") && !string.IsNullOrWhiteSpace(cylindersSub[c]))
                                    {
                                        var match = cylindersSuper.Contains(cylindersSub[c]);
                                        var isCylinderExists = verticalOccurrences.Any(x => x.Cylinder == cylindersSub[c].ToString());
                                        if (match)
                                        {
                                            if (isCylinderExists)
                                            {
                                                occurrence = verticalOccurrences.Where(x => x.Cylinder == cylindersSub[c].ToString()).FirstOrDefault();
                                            }
                                            else
                                            {
                                                occurrence.Cylinder = cylindersSub[c].ToString();
                                                occurrence.NumberOccured = new Dictionary<string, int>();
                                            }
                                            if (!occurrence.NumberOccured.ContainsKey(groupKeySub))
                                            {
                                                occurrence.NumberOccured.Add(groupKeySub, 0);
                                            }
                                            occurrence.NumberOccured[groupKeySub] = 1;

                                            if (!verticalOccurrences.Any(x => x.Cylinder == cylindersSub[c].ToString()))
                                            {
                                                verticalOccurrences.Add(occurrence);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }


                    //Console.WriteLine("Vertical Occurrences-------------------");

                    // print Occurences
                    for (int i = 0; i < verticalOccurrences.Count; i++)
                    {
                        var item = verticalOccurrences[i];
                        if (item.NumberOccured.Count > 0)
                        {
                            var occurrenceSummary = new VerticalOccurrenceSummary
                            {
                                Cylinder = item.Cylinder,
                                Occurrence = item.NumberOccured.Select(x => x.Key.ToString()).ToList(),
                                VirtualName = "V" + item.Cylinder
                            };
                            vertOccSummaries.Add(occurrenceSummary);
                        }


                        Console.WriteLine("Cylinder => " + item.Cylinder + "-------------------");

                        //foreach (var ocur in item.NumberOccured)
                        //{
                        //    Console.WriteLine("Group = " + ocur.Key + " / value = " + ocur.Value + "-------------------");
                        //}
                    }
                    //foreach (var item in verticalOccurrences)
                    //{

                    //}

                    // Vertical Occurence Summmary
                    //Console.WriteLine("------------------ Vertical Occurrence Summary -------------------------");
                    //foreach (var item in vertOccSummaries)
                    //{
                    //    var summaryOutput = item.Cylinder + " ===> ";

                    //    foreach (var summary in item.Occurrence)
                    //    {
                    //        summaryOutput += " " + summary + ", ";
                    //    }

                    //    Console.WriteLine(summaryOutput);

                    //    Console.WriteLine("");
                    //    Console.WriteLine("");
                    //}


                    // vertical replacement
                    for (int i = 0; i < vertOccSummaries.Count; i++)
                    {
                        var item = vertOccSummaries[i];
                        for (int k = 0; k < item.Occurrence.Count; k++)
                        {
                            var groupKey = item.Occurrence[k];
                            var optizedRow = optimizedObjs.Where(x => x.GroupKey == groupKey).FirstOrDefault();
                            //optizedRow.Cylinders = optizedRow.Cylinders.Replace(item.Cylinder, item.VirtualName);

                            var itemsToReplce = item.Cylinder.Split(" ").ToList();
                            var cylinders = optizedRow.Cylinders.Split(" ").ToList();
                            var cylindersCopy = new List<string>(cylinders);
                            var holdStrings = optizedRow.Cylinders;
                            for (int j = 0; j < itemsToReplce.Count; j++)
                            {
                                if (cylindersCopy.Contains(itemsToReplce[j]))
                                {
                                    cylindersCopy.Remove(itemsToReplce[j]);
                                }
                            }
                            if (cylindersCopy == null || cylindersCopy.Count < 1)
                            {
                                optizedRow.Cylinders = item.VirtualName;
                            }
                            else
                            {
                                optizedRow.Cylinders = string.Join(" ", cylindersCopy) + " " + item.VirtualName;
                            }
                        }

                        //var verticalGroup = new SubGroups()
                        //{
                        //    InnerGroups = "",
                        //    Cylinders = item.Cylinder,
                        //    GroupName = item.VirtualName,
                        //    SubGroupName = "",
                        //    IsSubGroup = false
                        //};
                        //subGroups.Add(verticalGroup);
                    }


                    #endregion

                    // Replace unique groups with no Groups
                    for (int i = 0; i < optimizedObjs.Count; i++)
                    {
                        var item = optimizedObjs[i];
                        if (!item.Cylinders.Contains("G") && !item.Cylinders.Contains("V"))
                        {
                            var obj = new GroupSummary()
                            {
                                GroupName = "UG" + item.GroupKey,
                                Cylinders = item.Cylinders,
                                IsSubGroup = false
                            };
                            var uniqueGroup = new SubGroups()
                            {
                                InnerGroups = "",
                                Cylinders = item.Cylinders,
                                GroupName = "UG" + item.GroupKey,
                                SubGroupName = "",
                                IsSubGroup = false
                            };
                            subGroups.Add(uniqueGroup);
                            item.Cylinders = "UG" + item.GroupKey;
                        }
                    }
                    

                    // replacement of uncommon groups
                    //optimizedObjs

                    for (int i = 0; i < optimizedObjs.Count; i++)
                    {
                        var cylinders = optimizedObjs[i].Cylinders.Trim().Split(" ");
                        var uniqueCyls = new List<string>();
                        var innerGroups = new List<string>();
                        var group = optimizedObjs[i].GroupKey;

                        await Task.Run(() =>
                        {
                            for (int j = 0; j < cylinders.Length; j++)
                            {
                                if (cylinders[j].Contains("V") || cylinders[j].Contains("G") || cylinders[j].Contains("S"))
                                {
                                    innerGroups.Add(cylinders[j]);
                                }
                                else
                                {
                                    uniqueCyls.Add(cylinders[j]);
                                }
                            }

                        });

                        if (uniqueCyls.Count() > 0)
                        {
                            var newUniqName = "UUSG" + group;
                            optimizedObjs[i].Cylinders = newUniqName + " " + string.Join(" ", innerGroups);

                            var uniqueGroup = new SubGroups()
                            {
                                InnerGroups = "",
                                Cylinders = string.Join(" ", uniqueCyls),
                                GroupName = newUniqName,
                                SubGroupName = "",
                                IsSubGroup = false
                            };
                            subGroups.Add(uniqueGroup);
                        }
                    }


                    // return the vertical group
                    await Task.Run(() =>
                    {
                        for (int i = 0; i < optimizedObjs.Count; i++)
                        {
                            var cylinders = optimizedObjs[i].Cylinders.Split(" ").ToList();
                            var cylindersCopy = new List<string>(cylinders);
                            string verticalOcc = "";
                            for (int j = 0; j < cylindersCopy.Count; j++)
                            {
                                var cylinderId = cylindersCopy[j];
                                if (cylinderId.Contains("V"))
                                {
                                    cylinders.Remove(cylinderId);
                                    verticalOcc += cylinderId.Substring(1) + " ";
                                }
                            }

                            string cylGrp = string.Join(" ", cylinders) + " " + verticalOcc.Trim();
                            optimizedObjs[i].Cylinders = cylGrp;
                        }
                    });


                    var groupFinals = new List<GroupFinal>();
                    // Save Final Group
                    for (int i = 0; i < optimizedObjs.Count; i++)
                    {
                        var item = optimizedObjs[i];
                        var keygroup = new KeyGroupCylinderAnalysis()
                        {
                            GroupID = Convert.ToInt32(item.GroupKey),
                            _Cylinder = item.Cylinders,
                            CustomerID = customerId
                        };
                        _Context.KeyGroupCylinderAnalyses.Add(keygroup);


                        var finalGroup = new GroupFinal
                        {
                            GroupID = Convert.ToInt32(optimizedObjs[i].GroupKey),
                            _RelatedGrouping = optimizedObjs[i].Cylinders,
                            _Validated = false,
                            _NumOfMatches = 0,
                            CustomerID = customerId
                        };
                        groupFinals.Add(finalGroup);
                    }
                    await _Context.SaveChangesAsync();

                    #region Printing Results


                    for (int j = 0; j < subGroups.Count(); j++)
                    {
                        var obj = new GroupSummary()
                        {
                            GroupName = subGroups[j].GroupName,
                            SubGroupName = subGroups[j].SubGroupName,
                            IsSubGroup = subGroups[j].IsSubGroup,
                            Cylinders = subGroups[j].Cylinders,
                            InnerGroups = subGroups[j].InnerGroups,
                            CustomerID = customerId
                        };
                        _Context.GroupSummaries.Add(obj);
                    }
                    await _Context.SaveChangesAsync();
                    #endregion


                    // VERIFICATION
                    var verificationObjs = optimizedObjs;
                    for (int i = 0; i < optimizedObjs.Count; i++)
                    {
                        var groupObj = optimizedObjs[i];
                        var unzippedCyls = "";
                        var splittedGrps = groupObj.Cylinders.Split(" ");
                        for (int j = 0; j < splittedGrps.Length; j++)
                        {
                            if(splittedGrps[j].Contains("V") || splittedGrps[j].Contains("U") || splittedGrps[j].Contains("S"))
                            {
                                var grp = splittedGrps[j].Trim();
                                SubGroups subGrp = null;
                                if (grp.Contains("SS"))
                                {
                                    subGrp = subGroups.Where(x => x.SubGroupName == grp).FirstOrDefault();
                                }
                                else
                                {
                                    subGrp = subGroups.Where(x => x.GroupName == grp).FirstOrDefault();
                                }
                                subGrp = new SubGroups()
                                {
                                    Cylinders = subGrp.Cylinders,
                                    GroupName = subGrp.GroupName,
                                    InnerGroups = subGrp.InnerGroups,
                                    IsSubGroup = subGrp.IsSubGroup,
                                    SubGroupName = subGrp.SubGroupName,
                                };
                                unzippedCyls += subGrp.Cylinders + " ";
                                do
                                {
                                    var otherSubGrps = "";
                                    if (!string.IsNullOrWhiteSpace(subGrp.InnerGroups))
                                    {
                                        var innerGrps = subGrp.InnerGroups.Trim().Split(" ");
                                        if(innerGrps != null && innerGrps.Count() > 0)
                                        {
                                            for (int k = 0; k < innerGrps.Length; k++)
                                            {
                                                var innerGrpItem = innerGrps[k];
                                                var subItem = subGroups.Where(x => x.GroupName == innerGrpItem).FirstOrDefault();
                                            
                                                var innerGrpValue = new SubGroups()
                                                {
                                                    Cylinders = subItem.Cylinders,
                                                    GroupName = subItem.GroupName,
                                                    InnerGroups = subItem.InnerGroups,
                                                    IsSubGroup = subItem.IsSubGroup,
                                                    SubGroupName = subItem.SubGroupName,
                                                };
                                                unzippedCyls += innerGrpValue.Cylinders + " ";
                                                if (!string.IsNullOrWhiteSpace(innerGrpValue.InnerGroups))
                                                {
                                                    otherSubGrps += innerGrpValue.InnerGroups + " ";
                                                }
                                            }
                                        }
                                    }
                                    subGrp.InnerGroups = otherSubGrps;


                                } while (!string.IsNullOrWhiteSpace(subGrp.InnerGroups));
                            }
                            else
                            {
                                unzippedCyls += splittedGrps[j] + " ";
                            }
                        }

                        var groupFinal = groupFinals.Where(x => x.GroupID.ToString() == groupObj.GroupKey).FirstOrDefault();
                        var completeCylGrp = originalData.Where(x => x.GroupKey == groupObj.GroupKey).FirstOrDefault();
                        var originalCyls = completeCylGrp.Cylinders.Split(" ").ToList();
                        var extractedCyls = unzippedCyls.Trim().Split(" ").ToList().Distinct().ToList();

                        var intersect = originalCyls.Intersect(extractedCyls);
                        if(intersect.Count() != originalCyls.Count())
                        {
                            groupFinal._Validated = false;
                        }
                        var numOfMatches = 0;
                        for (int j = 0; j < originalCyls.Count; j++)
                        {
                            if (extractedCyls.Contains(originalCyls[j]))
                            {
                                // increase the matching property
                                ++numOfMatches;
                            }
                        }
                        groupFinal._NumOfMatches = numOfMatches;
                        if(extractedCyls.Count() == originalCyls.Count())
                        {
                            groupFinal._Validated = true;
                        }
                        // return valid
                    }

                    for (int t = 0; t < groupFinals.Count; t++)
                    {
                        _Context.GroupFinals.Add(groupFinals[t]);
                    }
                    await _Context.SaveChangesAsync();

                    // final optimized objects
                    //Console.WriteLine("///////////////////////////////////////////////////");
                    //Console.WriteLine(" --------- Final Optimized Objects ----------");
                    //foreach (var item in optimizedObjs)
                    //{
                    //    string output = "GroupKey " + item.GroupKey + " || Cylinders " + item.Cylinders;
                    //    Console.WriteLine(output);
                    //}
                    await GetCylinderRelatedGrouping(customerId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
            

                //await StoreKeyGroupAnalysis(OptLevel);
                return null;
                #endregion
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "MergeRelationship", ex.Message);
                throw;
            }
        }

        public async Task StoreKeyGroupAnalysis(List<KeyGroupCylinderAnalysis> model)
        {
            foreach (var item in model)
            {
                item.CustomerID = 12;
                await _Context.KeyGroupCylinderAnalyses.AddAsync(item);
            }
            await _Context.SaveChangesAsync();
        }

        public async Task<object> UnZipLockingPlan(int customerId)
        {
            try
            {
                var groupFinals = await _Context.GroupFinals.ToListAsync();
                var groupSummaries = await _Context.GroupSummaries.ToListAsync();
                var context = new ApplicationDbContext(_Options);
                var groupDefs = new List<GroupsOptimization>();

                #region Get Key Cylinders Defs
                await Task.Run(() =>
                {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {

                        var sqlScript = StringStore.GetKeyCylinderDefinitions;

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;

                        command.Parameters.Add(new MySqlParameter("@customerId", customerId));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {

                            while (result.Read())
                            {
                                GroupsOptimization keyCylDefs = new GroupsOptimization();
                                keyCylDefs.GroupKey = result["GroupID"].ToString();
                                keyCylDefs.Cylinders = result["Cylinders"].ToString();

                                groupDefs.Add(keyCylDefs);
                            }

                        }
                    }


                });
                #endregion

                for (int i = 0; i < groupFinals.Count; i++)
                {
                    var grouping = groupFinals[i];
                    string unzipped = "";
                    var items = groupFinals[i]._RelatedGrouping.Split(" ");
                    for (int j = 0; j < items.Length; j++)
                    {
                        if(items[j].Contains("G") || items[j].Contains("V") || items[j].Contains("S"))
                        {
                            var groupItem = items[j];
                            var cyls = groupSummaries.Where(x => x.GroupName == groupItem).FirstOrDefault();
                            if(cyls != null)
                            {
                                var cylinders = cyls.Cylinders.Trim().Split(" ").ToList();
                                do
                                {
                                    for (int t = 0; t < cylinders.Count; t++)
                                    {
                                        if(cylinders[t].Contains("G") || cylinders[t].Contains("V") || cylinders[t].Contains("S"))
                                        {
                                            var groupMatch = groupSummaries.Where(x => x.GroupName == cylinders[t]).FirstOrDefault().Cylinders;
                                            var itemToReplace = cylinders[t];
                                            cylinders.Remove(itemToReplace);
                                            var cylindersMatchs = groupMatch.Trim().Split(" ");
                                            cylinders.AddRange(cylindersMatchs);
                                            break;
                                        }
                                    }
                                } while (cylinders.Any(x => x.Contains("G") || x.Contains("S") || x.Contains("V")));
                                unzipped += string.Join(" ", cylinders);
                            }
                        }
                        else
                        {
                            unzipped += items[j];
                        }

                        var unzippedItems = unzipped.Split(" ").ToList().OrderBy(x => x);
                        var groupsDefinition = groupDefs.Where(x => x.GroupKey == grouping.GroupID.ToString()).FirstOrDefault();
                        var groupCompleteCyl = groupsDefinition.Cylinders.Split(",").OrderBy(x => x);

                        var intersect = unzippedItems.Intersect(groupCompleteCyl);
                        var matchCount = intersect.Count();
                        if(intersect == groupCompleteCyl)
                        {
                            Console.WriteLine("GroupKey " + grouping.GroupID + " Validated");
                        }
                        else
                        {
                            Console.WriteLine("GroupKey " + grouping.GroupID + "Not Validated");
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "UnZipLockingPlan", ex.Message);
                throw;
            }
        }

        public async Task<object> GetCylinderRelatedGrouping(int customerId)
        {
            try
            {
                var context = new ApplicationDbContext(_Options);
                List<CylinderAndGroups> cylinderAndGroups = new List<CylinderAndGroups>();
                List<FinalGrouping> finalGroupings = new List<FinalGrouping>();

                #region Cylinders Related Groups

                await Task.Run(() =>
                {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {
                        var sqlScript = StringStore.GetCylinderRelatedGrouping;

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new MySqlParameter("@customerId", customerId));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {

                            while (result.Read())
                            {
                                CylinderAndGroups cylinderGrps = new CylinderAndGroups();
                                cylinderGrps._CylinderID = result["_CylinderID"].ToString();
                                cylinderGrps._RelatedGroups = result["_RelatedGroups"].ToString();
                                cylinderGrps._DoorName = result["_DoorName"].ToString();
                                cylinderGrps._CylinderNumber = result["_CylinderNumber"].ToString();

                                cylinderAndGroups.Add(cylinderGrps);
                            }

                        }
                    }


                });
                #endregion
                #region Get Final Groupings

                await Task.Run(() =>
                {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {
                        var sqlScript = StringStore.GetFinalGroupings;

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;

                        command.Parameters.Add(new MySqlParameter("@customerId", customerId));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {

                            while (result.Read())
                            {
                                FinalGrouping finalGroup = new FinalGrouping();
                                finalGroup._GroupID = result["_GroupID"].ToString();
                                finalGroup._GroupName = result["_GroupName"].ToString();
                                finalGroup._RelatedGrouping = result["_RelatedGrouping"].ToString();
                            
                                finalGroupings.Add(finalGroup);
                            }

                        }
                    }


                });
                #endregion

                for (int i = 0; i < cylinderAndGroups.Count; i++)
                {
                    var cylinderGroups = cylinderAndGroups[i]._RelatedGroups.Trim().Split(" ");
                    cylinderAndGroups[i]._FinalGroups = new List<RelatedGroup>();

                    for (int j = 0; j < cylinderGroups.Length; j++)
                    {
                        var groupId = cylinderGroups[j];
                        var finalGroup = finalGroupings.Where(x => x._GroupID == groupId).FirstOrDefault();
                        var item = new RelatedGroup()
                        {
                            GroupId = groupId,
                            RelatedGrouping = finalGroup._RelatedGrouping,
                            GroupName = finalGroup._GroupName
                        };
                        cylinderAndGroups[i]._FinalGroups.Add(item);
                    }
                }

                for (int i = 0; i < cylinderAndGroups.Count; i++)
                {
                    var cylinderItem = cylinderAndGroups[i];
                    for (int j = 0; j < cylinderItem._FinalGroups.Count; j++)
                    {
                        var finalGroup = cylinderItem._FinalGroups[j];
                        var cylinderRel = new CylinderGroupsRelation
                        {
                            CylinderID = Convert.ToInt32(cylinderItem._CylinderID),
                            GroupID = Convert.ToInt32(finalGroup.GroupId),
                            _GroupName = finalGroup.GroupName,
                            _RelatedGrouping = finalGroup.RelatedGrouping,
                            CustomerID = customerId
                        };
                        _Context.CylinderGroupsRelations.Add(cylinderRel);
                    }
                }
                await _Context.SaveChangesAsync();

                return cylinderAndGroups;
            }
            catch (Exception ex)
            {

                Logs.logError("ProductionRepository", "GetCylinderRelatedGrouping", ex.Message);
                throw;
            }
        }

        public async Task<object> GetTotalGroups(int customerId)
        {
            try
            {
                var totalGrps = await _Context.GroupSummaries
                                            .Where(x => x.CustomerID == customerId &&
                                                        !x.GroupName.Contains("UU"))
                                            .ToListAsync();
                var totalGrpsResource = new List<TotalGroupsResource>();
                foreach (var item in totalGrps)
                {
                    var group = new TotalGroupsResource
                    {
                        GroupName = item.GroupName,
                        Cylinders = (item.Cylinders + " " + item.InnerGroups).Trim()
                    };
                    var match = totalGrpsResource.Where(x => x.GroupName == item.GroupName).ToList();
                    if (match.Count > 0)
                    {
                        totalGrpsResource.Add(group);

                    }
                    if (item.IsSubGroup)
                    {
                        var subGroup = new TotalGroupsResource
                        {
                            GroupName = item.SubGroupName,
                            Cylinders = item.Cylinders.Trim()
                        };
                        totalGrpsResource.Add(group);
                    }
                }
                return totalGrpsResource; 
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "GetTotalGroups", ex.Message);
                throw;
            }
                                        
        }

        public async Task<List<CylinderIdsWithGroups>> GetCylindersWithGroups(int customerId)
        {
            try
            {
                var context = new ApplicationDbContext(_Options);
                List<CylinderIdsWithGroups> cylindersWithGroups = new List<CylinderIdsWithGroups>();
            
                #region Get Key Cylinders Defs
                await Task.Run(() =>
                {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {

                        var sqlScript = StringStore.GetCylindersWithRelatedGroupIDs;

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new MySqlParameter("@customerId", customerId));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {

                            while (result.Read())
                            {
                                CylinderIdsWithGroups cylinderWithGrps = new CylinderIdsWithGroups();
                                cylinderWithGrps._CylinderIDs = result["_CylinderIDs"].ToString();
                                cylinderWithGrps._RelatedGroups = result["_RelatedGroups"].ToString();

                                cylindersWithGroups.Add(cylinderWithGrps);
                            }

                        }
                    }

                });

                #endregion
                cylindersWithGroups = cylindersWithGroups.OrderByDescending(x => x._CylinderIDs.Length).ToList();
                return cylindersWithGroups;
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "GetCylindersWithGroups", ex.Message);
                throw;
            }
        }

        public async Task<List<Group>> GetCylinderGroupInfo(int customerId, int cylinderId)
        {
            try
            {
                return await _Context.CylinderGroups.Where(x => x.CustomerID == customerId &&
                                                                x.CylinderID == cylinderId)
                                                    .Include(x => x.Group)
                                                    .Select(x => x.Group)
                                                    .ToListAsync();

            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "GetCylinderGroupInfo", ex.Message);
                throw;
            }
        }

        public async Task<List<CylinderWithGroups>> GetCylindersWithRelatedGroups(int customerId)
        {
            try
            {
                var context = new ApplicationDbContext(_Options);
                List<CylinderWithGroups> cylindersWithGroups = new List<CylinderWithGroups>();

                #region Get Key Cylinders Defs
                await Task.Run(() =>
                {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {

                        var sqlScript = StringStore.GetCylindersWithAllRelatedGroups;

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new MySqlParameter("@customerId", customerId));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {

                            while (result.Read())
                            {
                                CylinderWithGroups cylinderWithGrps = new CylinderWithGroups();
                                cylinderWithGrps._CylinderID = Convert.ToInt32(result["_CylinderID"].ToString());
                                cylinderWithGrps._RelatedGroups = result["_RelatedGroups"].ToString();

                                cylindersWithGroups.Add(cylinderWithGrps);
                            }

                        }
                    }

                });

                #endregion
                return cylindersWithGroups;
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "GetCylindersWithRelatedGroups", ex.Message);
                throw;
            }
        }

        public async Task<List<CylindersByGroupResource>> GetCylindersByGroup(int customerId, int groupId)
        {
            try
            {
                var context = new ApplicationDbContext(_Options);
                List<CylindersByGroupResource> cylindersByGroups = new List<CylindersByGroupResource>();

                #region Get Cylinders by GroupId
                await Task.Run(() =>
                {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {

                        var sqlScript = StringStore.GetCylindersByGroupId;

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new MySqlParameter("@customerId", customerId));
                        command.Parameters.Add(new MySqlParameter("@groupId", groupId));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {

                            while (result.Read())
                            {
                                CylindersByGroupResource cylinderByGrp = new CylindersByGroupResource();
                                cylinderByGrp._CylinderID = Convert.ToInt32(result["CylinderID"].ToString());
                                cylinderByGrp._CustomerID = Convert.ToInt32(result["CustomerID"].ToString());
                                cylinderByGrp._GroupID = Convert.ToInt32(result["GroupID"].ToString());
                                cylinderByGrp._GroupName = result["_GroupName"].ToString();
                                cylinderByGrp._DoorName = result["_DoorName"].ToString();
                                cylinderByGrp._CylinderNumber = result["_CylinderNumber"].ToString();
                                cylinderByGrp._GroupNumber = result["_GroupNumber"].ToString();

                                cylindersByGroups.Add(cylinderByGrp);
                            }

                        }
                    }

                });

                #endregion
                return cylindersByGroups;
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "GetCylindersWithRelatedGroups", ex.Message);
                throw;
            }
        }

        public async Task<List<CylinderIdsWithGroups>> GetCylinderGroupsWithRelatedGroupsIds(int customerId)
        {
            try
            {
                var context = new ApplicationDbContext(_Options);
                List<CylinderIdsWithGroups> cylindersByGroups = new List<CylinderIdsWithGroups>();

                #region Get Cylinders by GroupId
                await Task.Run(() =>
                {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {

                        var sqlScript = StringStore.GetCylinderGrpsWithRelatedGroupIds;

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new MySqlParameter("@customerId", customerId));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {

                            while (result.Read())
                            {
                                CylinderIdsWithGroups cylinderWithGrp = new CylinderIdsWithGroups();
                                cylinderWithGrp._CylinderIDs = result["_CylinderIDs"].ToString();
                                cylinderWithGrp._RelatedGroups = result["RelatedGroups"].ToString();

                                cylindersByGroups.Add(cylinderWithGrp);
                            }

                        }
                    }

                });

                #endregion
                return cylindersByGroups;
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "GetCylindersWithRelatedGroups", ex.Message);
                throw;
            }
        }

        public async Task<List<CylinderIdsWithGroups>> GetCylinderGroupsWithNamedRelatedGroup(int customerId)
        {
            try
            {
                var context = new ApplicationDbContext(_Options);
                List<CylinderIdsWithGroups> cylindersByGroups = new List<CylinderIdsWithGroups>();
                List<CylinderIdsWithGroups> results = new List<CylinderIdsWithGroups>();

                #region Get Cylinders by GroupId
                await Task.Run(() =>
                {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {

                        var sqlScript = StringStore.GetCylinderGrpsWithRelatedGroupIds;

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new MySqlParameter("@customerId", customerId));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {

                            while (result.Read())
                            {
                                CylinderIdsWithGroups cylinderWithGrp = new CylinderIdsWithGroups();
                                cylinderWithGrp._CylinderIDs = result["_CylinderIDs"].ToString();
                                cylinderWithGrp._RelatedGroups = result["RelatedGroups"].ToString();

                                cylindersByGroups.Add(cylinderWithGrp);
                            }

                        }
                    }

                });

                for (int i = 0; i < cylindersByGroups.Count; i++)
                {
                    var group = cylindersByGroups[i];
                    group._RelatedGroups = "G" + (i + 1);
                    results.Add(group);
                }
                #endregion
                return results;
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "GetCylindersWithRelatedGroups", ex.Message);
                throw;
            }
        }
        public async Task<List<CylinderIdsNumberWithGroup>> GetCylindersWithNamedRelatedGroup(int customerId)
        {
            try
            {
                var context = new ApplicationDbContext(_Options);
                List<CylinderIdsNumberWithGroup> cylindersByGroups = new List<CylinderIdsNumberWithGroup>();
                List<CylinderIdsNumberWithGroup> results = new List<CylinderIdsNumberWithGroup>();

                #region Get Cylinders by GroupId
                await Task.Run(() =>
                {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {

                        var sqlScript = StringStore.GetCylinderGrpsAndNumWithRelatedGroupIds;

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new MySqlParameter("@customerId", customerId));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {

                            while (result.Read())
                            {
                                CylinderIdsNumberWithGroup cylinderWithGrp = new CylinderIdsNumberWithGroup();
                                cylinderWithGrp._CylinderIDs = result["_CylinderIDs"].ToString();
                                cylinderWithGrp._RelatedGroups = result["RelatedGroups"].ToString();
                                cylinderWithGrp._CylinderNumbers = result["_CylinderNumbers"].ToString();

                                cylindersByGroups.Add(cylinderWithGrp);
                            }

                        }
                    }

                });

                for (int i = 0; i < cylindersByGroups.Count; i++)
                {
                    var group = cylindersByGroups[i];
                    group._RelatedGroups = "G" + (i + 1);
                    results.Add(group);
                }
                #endregion
                return results;
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "GetCylindersWithRelatedGroups", ex.Message);
                throw;
            }
        }


        public async Task<CylinderWithNameGroups> GetCylinderGroupWithNamedRelatedGroupByGroupId(int customerId, int groupId)
        {
            try
            {
                var context = new ApplicationDbContext(_Options);
                List<CylinderIdsWithGroups> cylindersByGroups = new List<CylinderIdsWithGroups>();
                List<CylindersNamedGroups> namedCylGrps = new List<CylindersNamedGroups>();
                var groupKey = await _Context.Groups.FindAsync(groupId);
                #region Get Cylinders by GroupId
                await Task.Run(() =>
                {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {

                        var sqlScript = StringStore.GetCylinderGrpsWithRelatedGroupIds;

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new MySqlParameter("@customerId", customerId));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {

                            while (result.Read())
                            {
                                CylinderIdsWithGroups cylinderWithGrp = new CylinderIdsWithGroups();
                                cylinderWithGrp._CylinderIDs = result["_CylinderIDs"].ToString();
                                cylinderWithGrp._RelatedGroups = result["RelatedGroups"].ToString();

                                cylindersByGroups.Add(cylinderWithGrp);
                            }

                        }
                    }

                });

                for (int i = 0; i < cylindersByGroups.Count; i++)
                {
                    var group = cylindersByGroups[i];
                    var nameCylGrp = new CylindersNamedGroups
                    {
                        _CylinderIDs = group._CylinderIDs,
                        _RelatedGroups = group._RelatedGroups,
                        _NamedGroup = "G" + (i + 1)
                    };
                    namedCylGrps.Add(nameCylGrp);
                }

                var groupOccurence = namedCylGrps.Where(x => x._RelatedGroups.Contains(groupId.ToString())).ToList();
                string cylinderIds = "";
                string namedGroups = "";
                for (int j = 0; j < groupOccurence.Count; j++)
                {
                    var item = groupOccurence[j];
                    cylinderIds += item._CylinderIDs + " ";
                    namedGroups += item._NamedGroup + " ";
                }

                var result = new CylinderWithNameGroups
                {
                    _GroupID = groupId.ToString(),
                    _CylinderIDs = cylinderIds.Trim(),
                    _NamedGroups = namedGroups.Trim(),
                    _GroupName = groupKey != null ? groupKey._Name : "",
                    _GroupNumber = groupKey != null ? groupKey._GroupNumber : ""
                };
                return result;
                #endregion
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "GetCylindersWithRelatedGroups", ex.Message);
                throw;
            }
        }

        public async Task<List<CylinderWithNameGroups>> GetAllCylinderGroupWithNamedRelatedGroups(int customerId)
        {
            try
            {
                var context = new ApplicationDbContext(_Options);
                List<CylinderIdsWithGroups> cylindersByGroups = new List<CylinderIdsWithGroups>();
                List<CylindersNamedGroups> namedCylGrps = new List<CylindersNamedGroups>();
                List<Group> groups = await _Context.Groups.Where(x => x.CustomerID == customerId)
                                                            .OrderBy(x => x.GroupID)
                                                            .ToListAsync();
                List<CylinderWithNameGroups> finalGroups = new List<CylinderWithNameGroups>();
                #region Get Cylinders by GroupId
                await Task.Run(() =>
                {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {

                        var sqlScript = StringStore.GetCylinderGrpsWithRelatedGroupIds;

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new MySqlParameter("@customerId", customerId));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {

                            while (result.Read())
                            {
                                CylinderIdsWithGroups cylinderWithGrp = new CylinderIdsWithGroups();
                                cylinderWithGrp._CylinderIDs = result["_CylinderIDs"].ToString();
                                cylinderWithGrp._RelatedGroups = result["RelatedGroups"].ToString();

                                cylindersByGroups.Add(cylinderWithGrp);
                            }

                        }
                    }

                });

                for (int i = 0; i < cylindersByGroups.Count; i++)
                {
                    var group = cylindersByGroups[i];
                    var nameCylGrp = new CylindersNamedGroups
                    {
                        _CylinderIDs = group._CylinderIDs,
                        _RelatedGroups = group._RelatedGroups,
                        _NamedGroup = "G" + (i + 1)
                    };
                    namedCylGrps.Add(nameCylGrp);
                }

                for (int i = 0; i < groups.Count; i++)
                {
                    var groupId = groups[i].GroupID;
                    var group = groups[i];

                    var groupOccurence = namedCylGrps.Where(x => x._RelatedGroups.Contains(groupId.ToString())).ToList();
                    string cylinderIds = "";
                    string namedGroups = "";
                    for (int j = 0; j < groupOccurence.Count; j++)
                    {
                        var item = groupOccurence[j];
                        cylinderIds += item._CylinderIDs + " ";
                        namedGroups += item._NamedGroup + " ";
                    }

                    var result = new CylinderWithNameGroups
                    {
                        _GroupID = groupId.ToString(),
                        _CylinderIDs = cylinderIds.Trim(),
                        _NamedGroups = namedGroups.Trim(),
                        _GroupName = group._Name,
                        _GroupNumber = group._GroupNumber
                    };
                    finalGroups.Add(result);
                    
                }
                return finalGroups;
                #endregion
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "GetCylindersWithRelatedGroups", ex.Message);
                throw;
            }
        }

        public async Task<List<CylinderGroupsResource>> GetCylinderGroups(int customerId)
        {
            try
            {
                var context = new ApplicationDbContext(_Options);
                List<CylinderGroupsResource> cylinderGroups = new List<CylinderGroupsResource>();
                #region Get Cylinder Groups
                await Task.Run(() =>
                {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {

                        var sqlScript = StringStore.GetCylinderGroups;

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new MySqlParameter("@customerId", customerId));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {

                            while (result.Read())
                            {
                                CylinderGroupsResource cylinderGrp = new CylinderGroupsResource();
                                cylinderGrp._CylinderID = Convert.ToInt32(result["_CylinderID"].ToString());
                                cylinderGrp._GroupIDs = result["_GroupIDs"].ToString();
                                cylinderGrp._GroupNumbers = result["_GroupNumbers"].ToString();

                                cylinderGroups.Add(cylinderGrp);
                            }

                        }
                    }

                });
                #endregion
                return cylinderGroups;
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "GetCylinderGroups", ex.Message);
                throw;
            }
        
        }

        public async Task<List<GroupCylindersResource>> GetGroupCylinders(int customerId)
        {
            try
            {
                var context = new ApplicationDbContext(_Options);
                List<GroupCylindersResource> groupCylinders = new List<GroupCylindersResource>();
                #region Get Cylinder Groups
                await Task.Run(() =>
                {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {

                        var sqlScript = StringStore.GetGroupCylinders;

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new MySqlParameter("@customerId", customerId));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {

                            while (result.Read())
                            {
                                GroupCylindersResource groupCyl = new GroupCylindersResource();
                                groupCyl._GroupID = Convert.ToInt32(result["_GroupID"].ToString());
                                groupCyl._CylinderIDs = result["_CylinderIDs"].ToString();
                                groupCyl._CustomerID = Convert.ToInt32(result["_GroupID"].ToString());

                                groupCylinders.Add(groupCyl);
                            }

                        }
                    }

                });
                #endregion
                return groupCylinders;
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "GetGroupCylinders", ex.Message);
                throw;
            }

        }
        public async Task<List<CylinderWithNameGroups>> GetRelatedGroupsOfCylinder(int customerId, int cylinderId)
        {
            try
            {
                var context = new ApplicationDbContext(_Options);
                List<CylinderIdsWithGroups> cylindersByGroups = new List<CylinderIdsWithGroups>();
                List<CylindersNamedGroups> namedCylGrps = new List<CylindersNamedGroups>();
                List<Group> groups = await _Context.Groups.Where(x => x.CustomerID == customerId)
                                                            .OrderBy(x => x.GroupID)
                                                            .ToListAsync();
                List<CylinderWithNameGroups> finalGroups = new List<CylinderWithNameGroups>();
                #region Get Cylinders by GroupId
                await Task.Run(() =>
                {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {

                        var sqlScript = StringStore.GetCylinderGrpsWithRelatedGroupIds;

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new MySqlParameter("@customerId", customerId));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {

                            while (result.Read())
                            {
                                CylinderIdsWithGroups cylinderWithGrp = new CylinderIdsWithGroups();
                                cylinderWithGrp._CylinderIDs = result["_CylinderIDs"].ToString();
                                cylinderWithGrp._RelatedGroups = result["RelatedGroups"].ToString();

                                cylindersByGroups.Add(cylinderWithGrp);
                            }

                        }
                    }

                });

                for (int i = 0; i < cylindersByGroups.Count; i++)
                {
                    var group = cylindersByGroups[i];
                    var nameCylGrp = new CylindersNamedGroups
                    {
                        _CylinderIDs = group._CylinderIDs,
                        _RelatedGroups = group._RelatedGroups,
                        _NamedGroup = "G" + (i + 1)
                    };
                    namedCylGrps.Add(nameCylGrp);
                }

                for (int i = 0; i < groups.Count; i++)
                {
                    var groupId = groups[i].GroupID;
                    var group = groups[i];

                    var groupOccurence = namedCylGrps.Where(x => x._RelatedGroups.Contains(groupId.ToString())).ToList();
                    string cylinderIds = "";
                    string namedGroups = "";
                    for (int j = 0; j < groupOccurence.Count; j++)
                    {
                        var item = groupOccurence[j];
                        cylinderIds += item._CylinderIDs + " ";
                        namedGroups += item._NamedGroup + " ";
                    }

                    var result = new CylinderWithNameGroups
                    {
                        _GroupID = groupId.ToString(),
                        _CylinderIDs = cylinderIds.Trim(),
                        _NamedGroups = namedGroups.Trim(),
                        _GroupName = group._Name,
                        _GroupNumber = group._GroupNumber
                    };
                    if (result._CylinderIDs.Contains(cylinderId.ToString()))
                    {
                        finalGroups.Add(result);
                    }

                }
                return finalGroups;
                #endregion
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "GetRelatedGroupsOfCylinder", ex.Message);
                throw;
            }
        
        }


        public async Task<List<CylinderIdsWithGroups>> GetCylinderGroupsWithGroups(int customerId)
        {
            try
            {
                var context = new ApplicationDbContext(_Options);
                List<CylinderIdsWithGroups> cylindersByGroups = new List<CylinderIdsWithGroups>();
                List<CylinderIdsWithGroups> results = new List<CylinderIdsWithGroups>();
                var groupSummaries = await _Context.GroupSummaries.Where(x => x.CustomerID == customerId).ToListAsync();

                #region Get Cylinders by GroupId
                await Task.Run(() =>
                {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {

                        var sqlScript = StringStore.GetCylinderGrpsWithRelatedGroupIds;

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new MySqlParameter("@customerId", customerId));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {

                            while (result.Read())
                            {
                                CylinderIdsWithGroups cylinderWithGrp = new CylinderIdsWithGroups();
                                cylinderWithGrp._CylinderIDs = result["_CylinderIDs"].ToString();
                                cylinderWithGrp._RelatedGroups = result["RelatedGroups"].ToString();

                                cylindersByGroups.Add(cylinderWithGrp);
                            }

                        }
                    }

                });

                for (int i = 0; i < cylindersByGroups.Count; i++)
                {
                    var group = cylindersByGroups[i];
                    var cylinders = group._CylinderIDs.Split(" ");
                    List<string> groups = new List<string>();
                    for (int j = 0; j < cylinders.Length; j++)
                    {
                        var cylinderId = cylinders[j];
                        var relatedGrps = groupSummaries.Where(x => x.Cylinders.Contains(cylinderId)).ToList();
                        if(relatedGrps.Count > 0)
                        {
                            for (int k = 0; k < relatedGrps.Count; k++)
                            {
                                var relatedGrp = relatedGrps[k];
                                groups.Add(relatedGrp.GroupName);
                            }
                        }
                    }

                    string grpStrings = "";
                    groups = groups.Distinct().OrderBy(x => x).ToList();
                    for (int j = 0; j < groups.Count; j++)
                    {
                        grpStrings += groups[j] + " ";
                    }

                    var res = new CylinderIdsWithGroups
                    {
                        _CylinderIDs = group._CylinderIDs,
                        _RelatedGroups = grpStrings.Trim()
                    };
                    results.Add(res);
                }
                #endregion
                return results;
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "GetCylinderGroupsWithGroups", ex.Message);
                throw;
            }
        }

        public async Task<List<CylinderWithNameGroups>> GetAllCylinderGroupWithRelatedGroups(int customerId)
        {
            try
            {
                var context = new ApplicationDbContext(_Options);
                List<CylinderIdsWithGroups> cylindersByGroups = new List<CylinderIdsWithGroups>();
                //List<CylindersNamedGroups> namedCylGrps = new List<CylindersNamedGroups>();
                List<Group> groups = await _Context.Groups.Where(x => x.CustomerID == customerId)
                                                            .OrderBy(x => x.GroupID)
                                                            .ToListAsync();
                var groupSummaries = await _Context.GroupSummaries.Where(x => x.CustomerID == customerId).ToListAsync();

                List<CylinderWithNameGroups> finalGroups = new List<CylinderWithNameGroups>();
                #region Get Cylinders by GroupId
                await Task.Run(() =>
                {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {

                        var sqlScript = StringStore.GetCylinderGrpsWithRelatedGroupIds;

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new MySqlParameter("@customerId", customerId));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {

                            while (result.Read())
                            {
                                CylinderIdsWithGroups cylinderWithGrp = new CylinderIdsWithGroups();
                                cylinderWithGrp._CylinderIDs = result["_CylinderIDs"].ToString();
                                cylinderWithGrp._RelatedGroups = result["RelatedGroups"].ToString();

                                cylindersByGroups.Add(cylinderWithGrp);
                            }

                        }
                    }

                });


                for (int i = 0; i < groups.Count; i++)
                {
                    var groupId = groups[i].GroupID;
                    var group = groups[i];

                    var groupOccurence = cylindersByGroups.Where(x => x._RelatedGroups.Contains(groupId.ToString())).ToList();
                    string cylinderIds = "";
                    List<string> relatedGroups = new List<string>();
                    for (int j = 0; j < groupOccurence.Count; j++)
                    {
                        var item = groupOccurence[j];
                        cylinderIds += item._CylinderIDs + " ";
                        //namedGroups += item._NamedGroup + " ";
                    }
                    cylinderIds = cylinderIds.Trim();
                    var cylinders = cylinderIds.Split(" ");

                    for (int j = 0; j < cylinders.Length; j++)
                    {
                        var cylinderId = cylinders[j];
                        var relatedGrps = groupSummaries.Where(x => x.Cylinders.Contains(cylinderId)).ToList();
                        if (relatedGrps.Count > 0)
                        {
                            for (int k = 0; k < relatedGrps.Count; k++)
                            {
                                var relatedGrp = relatedGrps[k];
                                relatedGroups.Add(relatedGrp.GroupName);
                            }
                        }
                    }

                    string grpStrings = "";
                    relatedGroups = relatedGroups.Distinct().OrderBy(x => x).ToList();
                    for (int j = 0; j < relatedGroups.Count; j++)
                    {
                        grpStrings += relatedGroups[j] + " ";
                    }

                    var result = new CylinderWithNameGroups
                    {
                        _GroupID = groupId.ToString(),
                        _CylinderIDs = cylinderIds.Trim(),
                        _NamedGroups = grpStrings.Trim(),
                        _GroupName = group._Name,
                        _GroupNumber = group._GroupNumber
                    };
                    finalGroups.Add(result);

                }
                return finalGroups;
                #endregion
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "GetAllCylinderGroupWithRelatedGroups", ex.Message);
                throw;
            }
        }

        public async Task<CylinderWithNameGroups> GetCylinderGroupWithRelatedGroupByGroupId(int customerId, int groupId)
        {
            try
            {
                var context = new ApplicationDbContext(_Options);
                List<CylinderIdsWithGroups> cylindersByGroups = new List<CylinderIdsWithGroups>();
                var groupSummaries = await _Context.GroupSummaries.Where(x => x.CustomerID == customerId).ToListAsync();

                var groupKey = await _Context.Groups.FindAsync(groupId);
                #region Get Cylinders by GroupId
                await Task.Run(() =>
                {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {

                        var sqlScript = StringStore.GetCylinderGrpsWithRelatedGroupIds;

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new MySqlParameter("@customerId", customerId));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {

                            while (result.Read())
                            {
                                CylinderIdsWithGroups cylinderWithGrp = new CylinderIdsWithGroups();
                                cylinderWithGrp._CylinderIDs = result["_CylinderIDs"].ToString();
                                cylinderWithGrp._RelatedGroups = result["RelatedGroups"].ToString();

                                cylindersByGroups.Add(cylinderWithGrp);
                            }

                        }
                    }

                });


                var groupOccurence = cylindersByGroups.Where(x => x._RelatedGroups.Contains(groupId.ToString())).ToList();
                string cylinderIds = "";
                List<string> relatedGroups = new List<string>();
                for (int j = 0; j < groupOccurence.Count; j++)
                {
                    var item = groupOccurence[j];
                    cylinderIds += item._CylinderIDs + " ";
                }

                cylinderIds = cylinderIds.Trim();
                var cylinders = cylinderIds.Split(" ");

                for (int j = 0; j < cylinders.Length; j++)
                {
                    var cylinderId = cylinders[j];
                    var relatedGrps = groupSummaries.Where(x => x.Cylinders.Contains(cylinderId)).ToList();
                    if (relatedGrps.Count > 0)
                    {
                        for (int k = 0; k < relatedGrps.Count; k++)
                        {
                            var relatedGrp = relatedGrps[k];
                            relatedGroups.Add(relatedGrp.GroupName);
                        }
                    }
                }

                string grpStrings = "";
                relatedGroups = relatedGroups.Distinct().OrderBy(x => x).ToList();
                for (int j = 0; j < relatedGroups.Count; j++)
                {
                    grpStrings += relatedGroups[j] + " ";
                }

                var result = new CylinderWithNameGroups
                {
                    _GroupID = groupId.ToString(),
                    _CylinderIDs = cylinderIds.Trim(),
                    _NamedGroups = grpStrings.Trim(),
                    _GroupName = groupKey != null ? groupKey._Name : "",
                    _GroupNumber = groupKey != null ? groupKey._GroupNumber : ""
                };
                return result;
                #endregion
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "GetCylinderGroupWithRelatedGroupByGroupId", ex.Message);
                throw;
            }
        }


        public async Task<CylinderIdsWithGroups> GetRelatedGrpOfCylinder(int customerId, int cylId)
        {
            try
            {
                var context = new ApplicationDbContext(_Options);
                List<CylinderIdsWithGroups> cylindersByGroups = new List<CylinderIdsWithGroups>();
                var results = new CylinderIdsWithGroups();
                var groupSummaries = await _Context.GroupSummaries.Where(x => x.CustomerID == customerId).ToListAsync();
                List<CylinderWithNameGroups> finalGroups = new List<CylinderWithNameGroups>();
                #region Get Cylinders by GroupId
                await Task.Run(() =>
                {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {

                        var sqlScript = StringStore.GetCylinderGrpsWithRelatedGroupIds;

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new MySqlParameter("@customerId", customerId));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {

                            while (result.Read())
                            {
                                CylinderIdsWithGroups cylinderWithGrp = new CylinderIdsWithGroups();
                                cylinderWithGrp._CylinderIDs = result["_CylinderIDs"].ToString();
                                cylinderWithGrp._RelatedGroups = result["RelatedGroups"].ToString();

                                cylindersByGroups.Add(cylinderWithGrp);
                            }

                        }
                    }

                });

                var cylinderGrp = cylindersByGroups.Where(x => x._CylinderIDs.Contains(cylId.ToString())).FirstOrDefault();
                if(cylinderGrp != null)
                {
                    var group = cylinderGrp;
                    var cylinders = group._CylinderIDs.Split(" ");
                    List<string> groups = new List<string>();
                    for (int j = 0; j < cylinders.Length; j++)
                    {
                        var cylinderId = cylinders[j];
                        var relatedGrps = groupSummaries.Where(x => x.Cylinders.Contains(cylinderId)).ToList();
                        if (relatedGrps.Count > 0)
                        {
                            for (int k = 0; k < relatedGrps.Count; k++)
                            {
                                var relatedGrp = relatedGrps[k];
                                groups.Add(relatedGrp.GroupName);
                            }
                        }
                    }

                    string grpStrings = "";
                    groups = groups.Distinct().OrderBy(x => x).ToList();
                    for (int j = 0; j < groups.Count; j++)
                    {
                        grpStrings += groups[j] + " ";
                    }

                    //var res = new CylinderIdsWithGroups
                    //{
                    //    _CylinderIDs = group._CylinderIDs,
                    //    _RelatedGroups = grpStrings.Trim()
                    //};
                    results._CylinderIDs = group._CylinderIDs;
                    results._RelatedGroups = grpStrings.Trim();
                }
                #endregion
                return results;
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "GetRelatedGroupsOfCylinder", ex.Message);
                throw;
            }
        }

        public async Task<CylindersInGroup> GetCylindersWithCountByGroup(CylinderSearchBy model)
        {
            try
            {
                var context = new ApplicationDbContext(_Options);
                List<CylindersByGroup> cylindersByGroups = new List<CylindersByGroup>();
                Group group = null;
                if(model._SearchTerm == "GroupId")
                {
                    int groupId = Convert.ToInt32(model._SearchValue);
                    group = await _Context.Groups.FindAsync(groupId);
                }else
                {
                    group = await _Context.Groups.Where(x => x._GroupNumber == model._SearchValue).FirstOrDefaultAsync();
                }

                if(group == null)
                {
                    return new CylindersInGroup();
                }

                #region Get Cylinders by GroupId
                await Task.Run(() =>
                {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {

                        var sqlScript = StringStore.GetCylindersByGroupId;

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new MySqlParameter("@customerId", model._CustomerID));
                        command.Parameters.Add(new MySqlParameter("@groupId", group.GroupID));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {

                            while (result.Read())
                            {
                                CylindersByGroup cylinderByGrp = new CylindersByGroup();
                                cylinderByGrp._CylinderID = Convert.ToInt32(result["CylinderID"].ToString());
                                cylinderByGrp._DoorName = result["_DoorName"].ToString();
                                cylinderByGrp._CylinderNumber = result["_CylinderNumber"].ToString();

                                cylindersByGroups.Add(cylinderByGrp);
                            }

                        }
                    }

                });

                #endregion
                return new CylindersInGroup { _Count = cylindersByGroups.Count(), _Cylinders = cylindersByGroups };
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "CylindersInGroup", ex.Message);
                throw;
            }
        }

        public async Task<GroupsInCylinder> GetGroupsWithCountByCylinder(GroupSearchBy model)
        {
            try
            {
                var context = new ApplicationDbContext(_Options);
                List<GroupInfo> groupsInCylinder = new List<GroupInfo>();

                Cylinder cylinder = null;
                if (model._SearchTerm == "CylinderId")
                {
                    int cylinderId = Convert.ToInt32(model._SearchValue);
                    cylinder = await _Context.Cylinders.FindAsync(cylinderId);
                }
                else
                {
                    cylinder = await _Context.Cylinders.Where(x => x._CylinderNumber == model._SearchValue).FirstOrDefaultAsync();
                }

                if (cylinder == null)
                {
                    return new GroupsInCylinder();
                }

                #region Get Cylinders by GroupId
                await Task.Run(() =>
                {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {

                        var sqlScript = StringStore.GetGroupsByCylinderId;

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new MySqlParameter("@customerId", model._CustomerID));
                        command.Parameters.Add(new MySqlParameter("@cylinderId", cylinder.CylinderID));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {

                            while (result.Read())
                            {
                                GroupInfo groupInfo = new GroupInfo();
                                groupInfo._GroupName = result["_GroupName"].ToString();
                                groupInfo._GroupNumber = result["_GroupNumber"].ToString();
                                groupInfo._GroupID = Convert.ToInt32(result["_GroupID"].ToString());

                                groupsInCylinder.Add(groupInfo);
                            }

                        }
                    }

                });
                return new GroupsInCylinder { _Count = groupsInCylinder.Count(), _Groups = groupsInCylinder };
                #endregion
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "GetRelatedGroupsOfCylinder", ex.Message);
                throw;
            }

        }

        public async Task<List<GroupSortInfo>> GetSortedGroups(int customerId, string sortType)
        {
            try
            {
                var context = new ApplicationDbContext(_Options);
                List<GroupSortInfo> groupsInCylinder = new List<GroupSortInfo>();

                #region Get Cylinders by GroupId
                await Task.Run(() =>
                {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {

                        var sqlScript = StringStore.GetSortedGroups;

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new MySqlParameter("@customerId", customerId));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {

                            while (result.Read())
                            {
                                GroupSortInfo groupInfo = new GroupSortInfo();
                                groupInfo._GroupName = result["_GroupName"].ToString();
                                groupInfo._GroupNumber = result["_GroupNumber"].ToString();
                                groupInfo._GroupID = Convert.ToInt32(result["_GroupID"].ToString());
                                groupInfo._Count = Convert.ToInt32(result["_Count"].ToString());

                                groupsInCylinder.Add(groupInfo);
                            }

                        }
                    }

                });
                if(sortType == "ASC")
                {
                    return groupsInCylinder.OrderBy(x => x._Count).ToList();
                }else
                {
                    return groupsInCylinder.OrderByDescending(x => x._Count).ToList();
                }
                #endregion
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "GetRelatedGroupsOfCylinder", ex.Message);
                throw;
            }

        }

        public async Task<List<CylinderSortInfo>> GetSortedCylinders(int customerId, string sortType)
        {

            try
            {
                var context = new ApplicationDbContext(_Options);
                List<CylinderSortInfo> cylinders = new List<CylinderSortInfo>();

                #region Get Cylinders by GroupId
                await Task.Run(() =>
                {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {

                        var sqlScript = StringStore.GetSortedCylinders;

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new MySqlParameter("@customerId", customerId));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {

                            while (result.Read())
                            {
                                CylinderSortInfo groupInfo = new CylinderSortInfo();
                                groupInfo._CylinderNumber = result["_CylinderNumber"].ToString();
                                groupInfo._DoorName = result["_DoorName"].ToString();
                                groupInfo._CylinderID = Convert.ToInt32(result["_CylinderID"].ToString());
                                groupInfo._Count = Convert.ToInt32(result["_Count"].ToString());

                                cylinders.Add(groupInfo);
                            }

                        }
                    }

                });
                if (sortType == "ASC")
                {
                    return cylinders.OrderBy(x => x._Count).ToList();
                }
                else
                {
                    return cylinders.OrderByDescending(x => x._Count).ToList();
                }
                #endregion
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "GetRelatedGroupsOfCylinder", ex.Message);
                throw;
            }
        }

        public async Task<List<GroupCylindersResource>> GetFilteredLockingPlan(FilterLP model)
        {
            try
            {
                var context = new ApplicationDbContext(_Options);
                List<GroupCylindersResource> groupCylinders = new List<GroupCylindersResource>();

                #region Get Cylinder Groups
                await Task.Run(() =>
                {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {
                        string sqlScript = StringStore.GetFilteredLP;
                        if(model._VerificationLP)
                        {
                            sqlScript = sqlScript.Replace("{{db}}", "cylindergroupverifications");
                        }else
                        {
                            sqlScript = sqlScript.Replace("{{db}}", "cylindergroups");
                        }
                        if(model._FilterTopByGroup)
                        {
                            sqlScript += StringStore.FilterTopByGroupName;
                        }else
                        {
                            sqlScript += StringStore.FilterTopNotByGroupName;
                        }
                        if(model._FilterTopByCylinder)
                        {
                            sqlScript += StringStore.FilterTopByCylinderName;
                        }else
                        {
                            sqlScript += StringStore.FilterTopByNotByCylinderName;
                        }
                        sqlScript += StringStore.FilterGroupByGroupID;

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new MySqlParameter("@customerId", model._CustomerID));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {

                            while (result.Read())
                            {
                                GroupCylindersResource groupCyl = new GroupCylindersResource();
                                groupCyl._GroupID = Convert.ToInt32(result["_GroupID"].ToString());
                                groupCyl._Count = Convert.ToInt32(result["_Count"].ToString());
                                groupCyl._CustomerID = Convert.ToInt32(result["_GroupID"].ToString());
                                groupCyl._CylinderIDs = result["_CylinderIDs"].ToString();
                                groupCyl._GroupName = result["_Name"].ToString();
                                groupCyl._DoorName = result["_DoorName"].ToString();

                                groupCylinders.Add(groupCyl);
                            }

                        }
                    }

                });
                #endregion

                return groupCylinders;
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "GetGroupCylinders", ex.Message);
                throw;
            }
        }

        public async Task<List<GroupCylindersResource>> GetExistingLockingPlan(int customerId)
        {
            try
            {
                var context = new ApplicationDbContext(_Options);
                List<GroupCylindersResource> groupCylinders = new List<GroupCylindersResource>();
                #region Get Cylinder Groups
                await Task.Run(() =>
                {
                    using (var command = context.Database.GetDbConnection().CreateCommand())
                    {
                        string sqlScript = StringStore.GetExistingLP;

                        command.CommandText = sqlScript;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Add(new MySqlParameter("@customerId", customerId));

                        context.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {

                            while (result.Read())
                            {
                                GroupCylindersResource groupCyl = new GroupCylindersResource();
                                groupCyl._GroupID = Convert.ToInt32(result["_GroupID"].ToString());
                                groupCyl._Count = Convert.ToInt32(result["_Count"].ToString());
                                groupCyl._CustomerID = Convert.ToInt32(result["_GroupID"].ToString());
                                groupCyl._CylinderIDs = result["_CylinderIDs"].ToString();
                                groupCyl._GroupName = result["_Name"].ToString();
                                groupCyl._DoorName = result["_DoorName"].ToString();

                                groupCylinders.Add(groupCyl);
                            }

                        }
                    }

                });
                #endregion
                return groupCylinders;
            }
            catch (Exception ex)
            {
                Logs.logError("ProductionRepository", "GetGroupCylinders", ex.Message);
                throw;
            }
        }


        public async Task SaveDiscGroupInfo(int customerId)
        {
            var data = new DiscAndGroupInfoResource();
            data.CustomerId = customerId;
            var groupSummaries = await _Context.GroupSummaries.Where(x => x.CustomerID == customerId).ToListAsync();

            //int totalGroup = groupSummaries.Where(x => x.GroupName.Contains("UUSG")).Count();
            //int unique = groupSummaries.Where(x => x.GroupName.Contains("SG") && !x.GroupName.Contains("UU")).Count();

            int totalGroup = groupSummaries.Where(x => x.GroupName.Contains("SG") && !x.GroupName.Contains("UU")).Count();
            int unique = groupSummaries.Where(x => x.GroupName.Contains("UUSG")).Count();

            var configuration = await _Context.Configurations.FirstOrDefaultAsync();

            data.TotalGroup = totalGroup;
            data.UniqueGroup = unique;

            var sapProxy = new SapPartnerProxy(configuration.Sapengineuri);
            var response = sapProxy.GetClientDisc(data);
        }



    }
}
