using AssetsGrid.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AssetsGrid.Controllers
{
    
    public class AssetController : Controller
    {
        private ApplicationDbContext _dbContext;

        public ApplicationDbContext DbContext
        {
            get
            {
                return _dbContext ?? HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            }
            private set
            {
                _dbContext = value;
            }

        }
        public AssetController()
        {

        }

        public AssetController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        // GET: Asset
        public ActionResult Index()
        {
            return View(DbContext.Assets.ToList());
        }
        private SelectList GetFacilitiySitesSelectList(object selectedValue = null)
        {
            return new SelectList(DbContext.FacilitySites
                                            .Where(facilitySite => facilitySite.IsActive && !facilitySite.IsDeleted)
                                            .Select(x => new { x.FacilitySiteID, x.FacilityName }),
                                                "FacilitySiteID",
                                                "FacilityName", selectedValue);
        }


        public ActionResult Create()
        {
            var model = new AssetViewModel();
            model.FacilitySitesSelectList = GetFacilitiySitesSelectList();
            return View("_CreatePartial", model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(AssetViewModel assetVM)
        {
            if (!ModelState.IsValid)
                return View("_CreatePartial", assetVM);

            Assets asset = MaptoModel(assetVM);

            DbContext.Assets.Add(asset);
            var task = DbContext.SaveChangesAsync();
            await task;

            if (task.Exception != null)
            {
                ModelState.AddModelError("", "Unable to add the Asset");
                return View("_CreatePartial", assetVM);
            }

            return Content("success");
        }

        public ActionResult Edit(Guid id)
        {
            var asset = DbContext.Assets.FirstOrDefault(x => x.AssetID == id);

            AssetViewModel assetViewModel = MapToViewModel(asset);

            if (Request.IsAjaxRequest())
                return PartialView("_EditPartial", assetViewModel);

            return View(assetViewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(AssetViewModel assetVM)
        {

            assetVM.FacilitySitesSelectList = GetFacilitiySitesSelectList(assetVM.FacilitySiteID);
            if (!ModelState.IsValid)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return View(Request.IsAjaxRequest() ? "_EditPartial" : "Edit", assetVM);
            }

            Assets asset = MaptoModel(assetVM);

            DbContext.Assets.Attach(asset);
            DbContext.Entry(asset).State = System.Data.Entity.EntityState.Modified;
            var task = DbContext.SaveChangesAsync();
            await task;

            if (task.Exception != null)
            {
                ModelState.AddModelError("", "Unable to update the Asset");
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return View(Request.IsAjaxRequest() ? "_EditPartial" : "Edit", assetVM);
            }

            if (Request.IsAjaxRequest())
            {
                return Content("success");
            }

            return RedirectToAction("Index");

        }

        public async Task<ActionResult> Details(Guid id)
        {
            var asset = await DbContext.Assets.FirstOrDefaultAsync(x => x.AssetID == id);
            var assetVM = MapToViewModel(asset);

            if (Request.IsAjaxRequest())
                return PartialView("_detailsPartial", assetVM);

            return View(assetVM);
        }

        public ActionResult Delete(Guid id)
        {
            var asset = DbContext.Assets.FirstOrDefault(x => x.AssetID == id);

            AssetViewModel assetViewModel = MapToViewModel(asset);

            if (Request.IsAjaxRequest())
                return PartialView("_DeletePartial", assetViewModel);
            return View(assetViewModel);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteAsset(Guid AssetID)
        {
            var asset = new Assets { AssetID = AssetID };
            DbContext.Assets.Attach(asset);
            DbContext.Assets.Remove(asset);

            var task = DbContext.SaveChangesAsync();
            await task;

            if (task.Exception != null)
            {
                ModelState.AddModelError("", "Unable to Delete the Asset");
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                AssetViewModel assetVM = MapToViewModel(asset);
                return View(Request.IsAjaxRequest() ? "_DeletePartial" : "Delete", assetVM);
            }

            if (Request.IsAjaxRequest())
            {
                return Content("success");
            }

            return RedirectToAction("Index");

        }
        private AssetViewModel MapToViewModel(Assets asset)
        {
            var facilitySite = DbContext.FacilitySites.Where(x => x.FacilitySiteID == asset.FacilitySiteID).FirstOrDefault();

            AssetViewModel assetViewModel = new AssetViewModel()
            {
                AssetID = asset.AssetID,
                Barcode = asset.Barcode,
                AstID = asset.AstID,
                Building = asset.Building,
                ChildAsset = asset.ChildAsset,
                Comments = asset.Comments,
                Corridor = asset.Corridor,
                EquipSystem = asset.EquipSystem,
                FacilitySiteID = asset.FacilitySiteID,
                FacilitySite = facilitySite != null ? facilitySite.FacilityName : String.Empty,
                Floor = asset.Floor,
                GeneralAssetDescription = asset.GeneralAssetDescription,
                Issued = asset.Issued,
                Manufacturer = asset.Manufacturer,
                MERNo = asset.MERNo,
                ModelNumber = asset.ModelNumber,
                PMGuide = asset.PMGuide,
                Quantity = asset.Quantity,
                RoomNo = asset.RoomNo,
                SecondaryAssetDescription = asset.SecondaryAssetDescription,
                SerialNumber = asset.SerialNumber,
                FacilitySitesSelectList = new SelectList(DbContext.FacilitySites
                                                                    .Where(fs => fs.IsActive && !fs.IsDeleted)
                                                                    .Select(x => new { x.FacilitySiteID, x.FacilityName }),
                                                                      "FacilitySiteID",
                                                                      "FacilityName", asset.FacilitySiteID)
            };

            return assetViewModel;
        }

        private Assets MaptoModel(AssetViewModel assetVM)
        {
            Assets asset = new Assets()
            {
                AssetID = assetVM.AssetID,
                Barcode = assetVM.Barcode,
                AstID = assetVM.AstID,
                Building = assetVM.Building,
                ChildAsset = assetVM.ChildAsset,
                Comments = assetVM.Comments,
                Corridor = assetVM.Corridor,
                EquipSystem = assetVM.EquipSystem,
                FacilitySiteID = assetVM.FacilitySiteID,
                Floor = assetVM.Floor,
                GeneralAssetDescription = assetVM.GeneralAssetDescription,
                Issued = assetVM.Issued,
                Manufacturer = assetVM.Manufacturer,
                MERNo = assetVM.MERNo,
                ModelNumber = assetVM.ModelNumber,
                PMGuide = assetVM.PMGuide,
                Quantity = assetVM.Quantity,
                RoomNo = assetVM.RoomNo,
                SecondaryAssetDescription = assetVM.SecondaryAssetDescription,
                SerialNumber = assetVM.SerialNumber
            };

            return asset;
        }
    }
}