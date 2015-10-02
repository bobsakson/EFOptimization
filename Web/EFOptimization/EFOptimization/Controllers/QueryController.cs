using EFOptimization.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using EFOptimization.Models;
using System.Data.Entity.Infrastructure;

namespace EFOptimization.Controllers
{
  public class QueryController : Controller
  {
    // GET: Query
    public ActionResult Index()
    {
      return View();
    }

    public ActionResult SeeEFQueryWithToString()
    {
      var awContext = new AdventureWorks();

      var query =   awContext.Customers
                    .Where(x => x.Person.LastName == "Johnson")
                    .Include(x => x.SalesOrderHeaders)
                    .Include(x => x.Person);

      var queryTextModel = new QueryTextModel() { QueryText = query.ToString() };

      return View("QueryText", queryTextModel);
    }

    public ActionResult AnyQuery()
    {
      var awContext = new AdventureWorks();

      var query = awContext.SalesOrderDetails.Any(x => x.SalesOrderHeader.Customer.Person.LastName == "Johnson");

      var queryTextModel = new QueryTextModel() { QueryText = "AnyQuery" };

      return View("QueryText", queryTextModel);
    }

    public ActionResult FirstOrDefaultQuery()
    {
      var awContext = new AdventureWorks();

      var query = awContext.SalesOrderDetails.FirstOrDefault(x => x.SalesOrderHeader.Customer.Person.LastName == "Johnson");

      var queryTextModel = new QueryTextModel() { QueryText = "FirstOrDefaultQuery" };

      return View("QueryText", queryTextModel);
    }

    public ActionResult SingleQuery()
    {
      var awContext = new AdventureWorks();

      var query = awContext.SalesOrderDetails.Single(x => x.SalesOrderHeader.Customer.Person.LastName == "Sacksteder");

      var queryTextModel = new QueryTextModel() { QueryText = "SingleQuery" };

      return View("QueryText", queryTextModel);
    }

    public ActionResult DefaultIfEmptyQuery()
    {
      var awContext = new AdventureWorks();

      var person = new Person()
      {
        LastName = "Sakson"
      };

      var people = awContext.People.Where(x => x.LastName == "Sakson").ToList().DefaultIfEmpty(person);

      var queryTextModel = new QueryTextModel() { QueryText = people.First().LastName };

      return View("QueryText", queryTextModel);
    }

    public ActionResult EagerLoadQuery(int id)
    {
      // 51739
      var awContext = new AdventureWorks();
      var query =   awContext.SalesOrderHeaders
                    .Where(x => x.SalesOrderID == id)
                    .Include(x => x.SalesOrderDetails)
                    .FirstOrDefault();

      var salesOrderDetails = query.SalesOrderDetails.Where(x => x.UnitPrice > 100).ToList();

      var queryTextModel = new QueryTextModel() { QueryText = "EagerLoadQuery" };

      return View("QueryText", queryTextModel);
    }

    public ActionResult LazyLoadQuery(int id)
    {
      // 51739

      var awContext = new AdventureWorks();
      var query = awContext.SalesOrderHeaders
                    .Where(x => x.SalesOrderID == id)
                    .FirstOrDefault();

      var salesOrderDetail = query.SalesOrderDetails.Where(x => x.UnitPrice > 100).ToList();

      var queryTextModel = new QueryTextModel() { QueryText = "LazyLoadQuery" };

      return View("QueryText", queryTextModel);
    }

    public ActionResult IncludeQuery()
    {
      var awContext = new AdventureWorks();

      var query =   awContext.SalesOrderDetails
                    .Where(x => x.SalesOrderHeader.Customer.Person.LastName == "Johnson")
                    .Include(x => x.SalesOrderHeader)
                    .Include(x => x.SalesOrderHeader.Customer.Person)
                    .Include(x => x.SalesOrderHeader.Customer.Person.PersonPhones)
                    .Include(x => x.SalesOrderHeader.Customer.Person.EmailAddresses).ToList();

      var queryTextModel = new QueryTextModel() { QueryText = "IncludeQuery" };

      return View("QueryText", queryTextModel);
    }

    public ActionResult LazyLoadDisposedContextQuery()
    {
      SalesOrderHeader query;
      var queryTextModel = new QueryTextModel();

      using (var awContext = new AdventureWorks())
      {
        query = awContext.SalesOrderHeaders
                .Where(x => x.SalesOrderID == 51739)
                .FirstOrDefault();
      }
      
      try
      {
        var salesOrderDetail = query.SalesOrderDetails.Where(x => x.UnitPrice > 100).ToList();
      }  
      catch(Exception ex)
      {
        queryTextModel.QueryText = ex.Message;

        return View("QueryText", queryTextModel);
      }

      queryTextModel.QueryText = "Never going to get here.";

      return View("QueryText", queryTextModel);
    }

    public ActionResult LazyLoadForEachQuery()
    {
      var awContext = new AdventureWorks();
      var query = awContext.SalesOrderHeaders
                    .Take(1000);

      var totalUnitPrice = 0m;

      foreach(var order in query)
      {
        foreach(var detail in order.SalesOrderDetails)
        {
          totalUnitPrice += detail.UnitPrice;
        }        
      }

      var queryTextModel = new QueryTextModel() { QueryText = "LazyLoadForEachQuery" };

      return View("QueryText", queryTextModel);
    }

    public ActionResult EagerLoadForEachQuery()
    {
      var awContext = new AdventureWorks();
      var query = awContext.SalesOrderHeaders
                  .Include(x => x.SalesOrderDetails)
                  .Take(1000);

      var totalUnitPrice = 0m;

      foreach (var order in query)
      {
        foreach (var detail in order.SalesOrderDetails)
        {
          totalUnitPrice += detail.UnitPrice;
        }
      }

      var queryTextModel = new QueryTextModel() { QueryText = "LazyLoadForEachQuery" };

      return View("QueryText", queryTextModel);
    }

    public ActionResult EFCasingQuery()
    {
      var awContext = new AdventureWorks();

      var lower = awContext.People.Where(x => x.LastName == "Johnson").ToList().Count;
      var upper = awContext.People.Where(x => x.LastName == "JOHNSON").ToList().Count;
      var mixed = awContext.People.Where(x => x.LastName == "JoHnSON").ToList().Count;

      var queryTextModel = new QueryTextModel() { QueryText = "EFCasingQuery" };

      return View("QueryText", queryTextModel);
    }
  }
}