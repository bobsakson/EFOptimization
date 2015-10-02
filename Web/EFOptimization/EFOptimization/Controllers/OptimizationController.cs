using EFOptimization.DataAccess;
using EFOptimization.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Diagnostics;

namespace EFOptimization.Controllers
{
  public class OptimizationController : Controller
  {
    public ActionResult Index()
    {
      var awContext = new AdventureWorks();

      var products = awContext.Products.Take(2).ToList();

      return View();
    }

    public ActionResult GetProductsDuplicateList()
    {
      var awContext = new AdventureWorks();

      var productFullList = awContext.Products.ToList();

      var inventoryCount = productFullList.Count(p => p.ProductInventories.Count() > 1);

      return Json("Success!", JsonRequestBehavior.AllowGet);
    }
    public JsonResult GetProductsFullList()
    {
      var awContext = new AdventureWorks();

      var productFullList = awContext.Products.Include(p => p.ProductInventories).ToList();

      var inventoryCount = productFullList.Count(p => p.ProductInventories.Count() > 1);

      return Json("Success!", JsonRequestBehavior.AllowGet);
    }

    // GET: Optimization
    public ActionResult GetProductsList()
    {
      var awContext = new AdventureWorks();

      var productList = awContext.Products.Select(p => new ProductListModel()
      {
        Name = p.Name,
        ListPrice = p.ListPrice,
        ProductID = p.ProductID,
        InventoryCount = p.ProductInventories.Count()
      }).ToList();

      var inventoryCount = productList.Count(p => p.InventoryCount > 1);

      return Json("Success!", JsonRequestBehavior.AllowGet);
    }

    public ActionResult UpdateProductsList()
    {
      var awContext = new AdventureWorks();
      awContext.Configuration.AutoDetectChangesEnabled = true;

      var productFullList = awContext.Products.Take(2).ToList();
      var random = new Random();

      foreach (var product in productFullList)
      {
        product.Weight = random.Next();
      }

      awContext.SaveChanges();

      return Json("Success!", JsonRequestBehavior.AllowGet);
    }
    public ActionResult GetTransactionHistoryNoChangesList()
    {
      var awContext = new AdventureWorks();
      var timer = Stopwatch.StartNew();

      var productFullList = awContext.TransactionHistories.AsNoTracking().ToList();

      timer.Stop();
      var time = timer.Elapsed;

      return Json("Success!", JsonRequestBehavior.AllowGet);
    }
    public ActionResult GetTransactionHistoryList()
    {
      var awContext = new AdventureWorks();
      var timer = Stopwatch.StartNew();

      var productFullList = awContext.TransactionHistories.ToList();

      timer.Stop();
      var time = timer.Elapsed;

      return Json("Success!", JsonRequestBehavior.AllowGet);
    }

    public ActionResult GetProductsBigInclude()
    {
      var awContext = new AdventureWorks();

      var productFullList = awContext.Products
        .Include(p => p.ProductVendors)
        .Include(p => p.ProductVendors.Select(pi => pi.Vendor))
        .ToList();

      return Json("Success!", JsonRequestBehavior.AllowGet);
    }

    public ActionResult GetProductsBiggerInclude()
    {
      var awContext = new AdventureWorks();

      var productFullList = awContext.Products
        .Include(p => p.ProductVendors)
        .Include(p => p.ProductVendors.Select(pv => pv.Vendor))
        .Include(p => p.ProductVendors.Select(pv => pv.Vendor.PurchaseOrderHeaders))
        .ToList();

      return Json("Success!", JsonRequestBehavior.AllowGet);
    }
    

    public ActionResult GetProductReviews(int productId = 1)
    {
      var awContext = new AdventureWorks();

      var product = awContext.Products.Include(p => p.ProductReviews).Single(p => p.ProductID == productId);

      var productReviews = product.ProductReviews;

      return View("Index");
    }

    public ActionResult GetProductReviewsFast(int productId = 1)
    {
      var awContext = new AdventureWorks();

      var productReviews = awContext.ProductReviews.Where(p => p.ProductID == productId).ToList();

      return View("Index");
    }





































    public ActionResult InsertProductReviews(int count, int productId = 1)
    {
      var awContext = new AdventureWorks();

      var timer = Stopwatch.StartNew();

      for (var i = 0; i < count; i++)
      {
        awContext.ProductReviews.Add(new ProductReview()
        {
          Comments = "test",
          EmailAddress = "deleteMe@delete.com",
          ProductID = productId,
          ModifiedDate = DateTime.Now,
          Rating = 3,
          ReviewDate = DateTime.Now.AddDays(5),
          ReviewerName = "John Wiegert"
        });
      }

      awContext.SaveChanges();

      timer.Stop();
      var time = timer.Elapsed;

      return View("Index", (object)time.ToString());
    }
    public ActionResult InsertProductReviewsFast(int count, int commitCount)
    {
      var awContext = new AdventureWorks();

      var firstProduct = awContext.Products.Select(p => p.ProductID).First();

      var timer = Stopwatch.StartNew();
      awContext.Configuration.AutoDetectChangesEnabled = false;
      awContext.Configuration.ValidateOnSaveEnabled = false;

      for (var i = 0; i < count; i++)
      {
        awContext.ProductReviews.Add(new ProductReview()
        {
          Comments = "test",
          EmailAddress = "deleteMe@delete.com",
          ProductID = firstProduct,
          ModifiedDate = DateTime.Now,
          Rating = 3,
          ReviewDate = DateTime.Now.AddDays(5),
          ReviewerName = "John Wiegert"
        });

        if (i % commitCount == 0)
        {
          awContext.SaveChanges();
        }
      }

      awContext.SaveChanges();

      timer.Stop();
      var time = timer.Elapsed;

      return View("Index", (object)time.ToString());
    }
  }
}