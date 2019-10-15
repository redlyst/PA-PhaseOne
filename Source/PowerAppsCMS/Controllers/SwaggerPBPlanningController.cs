using PowerAppsCMS.Constants;
using PowerAppsCMS.Models;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace PowerAppsCMS.Controllers
{
    /// <summary>
    /// Controller yang berisikan fungsi-fungsi yang digunakan dihalaman PB Planning / Group Leader PB
    /// </summary>
    public class SwaggerPBPlanningController : ApiController
    {
        /// <summary>
        /// Mengambil daftar memo komponen
        /// </summary>
        /// <returns>Daftar memo komponen</returns>
        [Route("api/GetPBPlanning")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "Get specific PB planning",
            Type = typeof(MemoComponentModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError,
                Description = "Internal Server Error",
               Type = typeof(Exception))]
        [SwaggerOperation("GetPBPlanning")]
        public IHttpActionResult GetPBPlanning()
        {
            using (PowerAppsCMSEntities db = new PowerAppsCMSEntities())
            {
                List<MemoComponentModel> listMemoComponents = new List<MemoComponentModel>();
                try
                {
                   // var itemProcess = db.MemoComponents.Where(x => x.Memo.Products).Select(x => x.MemoComponents).Distinct();
                    foreach (MemoComponent itemMemoComponent in db.MemoComponents)
                    {
                        MemoComponentModel memoComponents = new MemoComponentModel();
                        memoComponents.MemoComponentID = itemMemoComponent.ID;
                        memoComponents.MemoID = itemMemoComponent.MemoID;
                        memoComponents.StatusID = itemMemoComponent.Status;
                        if (memoComponents.StatusID == (int)MemoStatus.NotStarted)
                        {
                            memoComponents.StatusName = "Not Started";
                        }
                        else if (memoComponents.StatusID == (int)MemoStatus.OnProcess)
                        {
                            memoComponents.StatusName = "On Process";
                        }
                        else if (memoComponents.StatusID == (int)MemoStatus.Complete)
                        {
                            memoComponents.StatusName = "Complete";
                        }
                        memoComponents.ComponentID = itemMemoComponent.ComponentID;
                        memoComponents.PartName = itemMemoComponent.Component.PartName;
                        memoComponents.PartNumber = itemMemoComponent.Component.PartNumber;
                        memoComponents.MemoDateCreated = itemMemoComponent.Memo.Created.ToString("dd/MM/yyyy");
                        memoComponents.PN = itemMemoComponent.Memo.Products.PN;
                        memoComponents.ProductName = itemMemoComponent.Memo.Products.Name;
                        int productCount = itemMemoComponent.Memo.MemoPROes.Sum(x => x.Quantity);
                        int componentNeed = itemMemoComponent.Memo.Products.ProductComposition.Where(x => x.ComponentID == itemMemoComponent.ComponentID).Select(x => x.Quantity).SingleOrDefault();
                        int totalQuantity = productCount * componentNeed;
                        memoComponents.TotalComponentQuantity = totalQuantity;

                        var listMemoPRO = itemMemoComponent.Memo.MemoPROes.Select(x => x.PRO.Number);
                        string joinPRO = string.Join(", ", listMemoPRO);
                        memoComponents.PRONumberDisplayText = joinPRO;
                        listMemoComponents.Add(memoComponents);
                    }
                    return Ok(listMemoComponents);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
        }

        /// <summary>
        /// Mengambil daftar process untuk komponen yang telah dipilih
        /// </summary>
        /// <param name="componentID"></param>
        /// <returns>Daftar process untuk komponen yang telah dipilih</returns>
        [Route("api/GetMaterialPreparationProcess")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "Get specific PB planning",
            Type = typeof(ComponentMaterialPreparationProcessModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError,
                Description = "Internal Server Error",
               Type = typeof(Exception))]
        [SwaggerOperation("GetMaterialPreparationProcess")]
        public IHttpActionResult GetMaterialPreparationProcess(int componentID)
        {
            using (PowerAppsCMSEntities db = new PowerAppsCMSEntities())
            {
                List<ComponentMaterialPreparationProcessModel> listMaterialPreparationProcesses = new List<ComponentMaterialPreparationProcessModel>();
                try
                {
                    foreach (ComponentMaterialPreparationProcess itemComponentMaterialPreparationProcess in db.ComponentMaterialPreparationProcesses.Where(x => x.ComponentID == componentID))
                    {
                        ComponentMaterialPreparationProcessModel MaterialPreparationProcesses = new ComponentMaterialPreparationProcessModel();
                        MaterialPreparationProcesses.PBProcessID = itemComponentMaterialPreparationProcess.MaterialPreparationProcessID;
                        MaterialPreparationProcesses.ProcessName = itemComponentMaterialPreparationProcess.MaterialPreparationProcess.Name;
                        listMaterialPreparationProcesses.Add(MaterialPreparationProcesses);
                    }
                    return Ok(listMaterialPreparationProcesses);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
        }

        /// <summary>
        /// Mengambil memo jobcard untuk memo komponen yang dipilih
        /// </summary>
        /// <param name="memoComponentID"></param>
        /// <returns>Daftar memo jobcard untuk memo komponen yang dipilih</returns>
        [Route("api/GetMemoJobCard")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "Get Memo JobCard",
            Type = typeof(MemoJobCardModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError,
                Description = "Internal Server Error",
               Type = typeof(Exception))]
        [SwaggerOperation("GetMemoJobCard")]
        public IHttpActionResult GetMemoJobCard(int memoComponentID)
        {
            using (PowerAppsCMSEntities db = new PowerAppsCMSEntities())
            {
                List<MemoJobCardModel> listJobCards = new List<MemoJobCardModel>();
                try
                {
                    foreach (MemoJobCard itemMemoJobCard in db.MemoJobCards.Where(x => x.MemoComponentID == memoComponentID))
                    {
                        MemoJobCardModel jobCards = new MemoJobCardModel();
                        jobCards.JobCardID = itemMemoJobCard.ID;
                        jobCards.Date = itemMemoJobCard.Created.ToString("dd/MM/yyyy");
                        jobCards.PBProcessID = itemMemoJobCard.MaterialPreparationProcessID;
                        jobCards.PBProcessName = itemMemoJobCard.MaterialPreparationProcess.Name;
                        jobCards.UserName = itemMemoJobCard.User.Name;
                        jobCards.Quantity = itemMemoJobCard.Quantity;
                        listJobCards.Add(jobCards);
                    }
                    return Ok(listJobCards);
                }
                catch (Exception ex)
                {
                    return InternalServerError(ex);
                }
            }
        }

        /// <summary>
        /// Fungsi untuk menambah aktual process memojobcard
        /// </summary>
        /// <param name="componentID"></param>
        /// <param name="memoComponentID"></param>
        /// <param name="PBProcessID"></param>
        /// <param name="quantity"></param>
        /// <param name="memoQuantity"></param>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <remarks>Menghitung quantity aktual process pada memo jobcard, apakah sudah terpenuhi atau belum</remarks>
        /// <returns></returns>
        [Route("api/PostMemoJobCard")]
        [HttpPost]
        public IHttpActionResult PostMemoJobCard(int componentID, int memoComponentID, int PBProcessID, int quantity, int memoQuantity, string userID, string userName)
        {
            using (PowerAppsCMSEntities db = new PowerAppsCMSEntities())
            {
                Guid guidUserID = Guid.Parse(userID.Replace(" ", string.Empty));
                DateTime now = DateTime.Now;

                MemoJobCard memoJobCard = new MemoJobCard();
                memoJobCard.MemoComponentID = memoComponentID;
                memoJobCard.MaterialPreparationProcessID = PBProcessID;
                memoJobCard.Quantity = quantity;
                memoJobCard.UserID = guidUserID;
                memoJobCard.Created = memoJobCard.LastModified = now;
                memoJobCard.CreatedBy = memoJobCard.LastModifiedBy = userName;

                db.MemoJobCards.Add(memoJobCard);
                if (db.SaveChanges() > 0)
                {
                    MemoComponent selectedMemoComponent = db.MemoComponents.Where(x => x.ID == memoComponentID).SingleOrDefault();
                    if (selectedMemoComponent.Status == (int)MemoStatus.NotStarted)
                    {
                        selectedMemoComponent.Status = (int)MemoStatus.OnProcess;
                        selectedMemoComponent.LastModifiedBy = userName;
                        selectedMemoComponent.LastModified = now;
                        db.SaveChanges();
                    }
                    bool isEqualValue = IsEqual(componentID, memoComponentID, memoQuantity);
                    if (isEqualValue == true)
                    {
                        selectedMemoComponent.Status = (int)MemoStatus.Complete;
                        selectedMemoComponent.LastModifiedBy = userName;
                        selectedMemoComponent.LastModified = now;
                        db.SaveChanges();
                    }

                }
                return Ok(HttpStatusCode.OK);
            }
        }

        /// <summary>
        /// Mengubah quantity aktual komponent pada memo jobcard
        /// </summary>
        /// <param name="memoJobCardID"></param>
        /// <param name="componentID"></param>
        /// <param name="memoComponentID"></param>
        /// <param name="PBProcessID"></param>
        /// <param name="quantity"></param>
        /// <param name="memoQuantity"></param>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <remarks>Menghitung quantity aktual process pada memo jobcard, apakah sudah terpenuhi atau belum</remarks>
        /// <returns></returns>
        [Route("api/EditMemoJobCard")]
        [HttpPost]
        public IHttpActionResult EditMemoJobCard(int memoJobCardID, int componentID, int memoComponentID, int PBProcessID, int quantity, int memoQuantity, string userID, string userName)
        {
            using (PowerAppsCMSEntities db = new PowerAppsCMSEntities())
            {
                Guid guidUserID = Guid.Parse(userID.Replace(" ", string.Empty));
                DateTime now = DateTime.Now;

                MemoJobCard selectedMemoJobCard = db.MemoJobCards.Where(x => x.ID == memoJobCardID).SingleOrDefault();
                selectedMemoJobCard.MaterialPreparationProcessID = PBProcessID;
                selectedMemoJobCard.Quantity = quantity;
                selectedMemoJobCard.UserID = guidUserID;
                selectedMemoJobCard.LastModified = now;
                selectedMemoJobCard.LastModifiedBy = userName;

                if (db.SaveChanges() > 0)
                {
                    MemoComponent selectedMemoComponent = db.MemoComponents.Where(x => x.ID == memoComponentID).SingleOrDefault();
                    bool isEqualValue = IsEqual(componentID, memoComponentID, memoQuantity);

                    if (isEqualValue == true && selectedMemoComponent.Status == (int)MemoStatus.OnProcess)
                    {
                        selectedMemoComponent.Status = (int)MemoStatus.Complete;
                        selectedMemoComponent.LastModifiedBy = userName;
                        selectedMemoComponent.LastModified = now;
                        db.SaveChanges();
                    }
                    else if (isEqualValue == false && selectedMemoComponent.Status == (int)MemoStatus.Complete)
                    {
                        selectedMemoComponent.Status = (int)MemoStatus.OnProcess;
                        selectedMemoComponent.LastModifiedBy = userName;
                        selectedMemoComponent.LastModified = now;
                        db.SaveChanges();
                    }
                }
                return Ok(HttpStatusCode.OK);
            }
        }

        /// <summary>
        /// Menghapus aktual komponent pada memo jobcard
        /// </summary>
        /// <param name="memoJobCardID"></param>
        /// <param name="componentID"></param>
        /// <param name="memoComponentID"></param>
        /// <param name="memoQuantity"></param>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <remarks>Menghitung quantity aktual process pada memo jobcard, apakah sudah terpenuhi atau belum</remarks>
        /// <returns></returns>
        [Route("api/DeleteMemoJobCard")]
        [HttpPost]
        public IHttpActionResult DeleteMemoJobCard(int memoJobCardID, int componentID, int memoComponentID, int memoQuantity, string userID, string userName)
        {
            using (PowerAppsCMSEntities db = new PowerAppsCMSEntities())
            {
                Guid guidUserID = Guid.Parse(userID.Replace(" ", string.Empty));
                DateTime now = DateTime.Now;

                MemoJobCard selectedMemoJobCard = db.MemoJobCards.Find(memoJobCardID);
                db.MemoJobCards.Remove(selectedMemoJobCard);

                if (db.SaveChanges() > 0)
                {
                    MemoComponent selectedMemoComponent = db.MemoComponents.Where(x => x.ID == memoComponentID).SingleOrDefault();
                    bool isEqualValue = IsEqual(componentID, memoComponentID, memoQuantity);

                    if (isEqualValue == false && selectedMemoComponent.Status == (int)MemoStatus.Complete)
                    {
                        selectedMemoComponent.Status = (int)MemoStatus.OnProcess;
                        selectedMemoComponent.LastModifiedBy = userName;
                        selectedMemoComponent.LastModified = now;
                        db.SaveChanges();
                    }

                    int countListMemoJobCards = db.MemoJobCards.Where(x => x.MemoComponentID == memoComponentID).Count();
                    if (countListMemoJobCards == 0)
                    {
                        selectedMemoComponent.Status = (int)MemoStatus.NotStarted;
                        selectedMemoComponent.LastModifiedBy = userName;
                        selectedMemoComponent.LastModified = now;
                        db.SaveChanges();
                    }

                }
                return Ok(HttpStatusCode.OK);
            }
        }

        private static bool IsEqual(int componentID, int memoComponentID, int memoQuantity)
        {
            using (PowerAppsCMSEntities db = new PowerAppsCMSEntities())
            {
                bool isEqual = false;
                List<ComponentMaterialPreparationProcess> listPBProcess = db.ComponentMaterialPreparationProcesses.Where(x => x.ComponentID == componentID).ToList();
                foreach (var item in listPBProcess)
                {
                    int totalQUantity = db.MemoJobCards.Where(x => x.MemoComponentID == memoComponentID && x.MaterialPreparationProcessID == item.MaterialPreparationProcessID).Sum(x => (int?)x.Quantity) ?? 0;
                    if (totalQUantity >= memoQuantity)
                    {
                        isEqual = true;
                    }
                    else
                    {
                        isEqual = false;
                    }
                }
                return isEqual;
            }
        }
    }
}