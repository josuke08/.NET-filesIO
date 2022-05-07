using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using ProjetGED.Models;
using System.Drawing;
using System.Data.Entity;
using System.Net;

namespace ProjetGED.Controllers
{
    public class RevisionsController : Controller
    {

        private DBC db = new DBC(); 
        // GET: Revisions
        public ActionResult Index(int? FileID)
        {
            if (FileID == null) return RedirectToAction("Index", "Files", new { FileID = Session["Directory"]});
            return View(db.Revisions.Where(r => r.parent == FileID));
        }

        [HttpGet]
        public FileResult DownloadRevision(int? RevisionID)
        {
            if (RevisionID == null) RedirectToAction("Index");
            string Name = db.Revisions.Find(RevisionID).nom;
            string path = Path.Combine(Server.MapPath("~/Revision/"), Name);

            byte[] bytes = { };
            if (System.IO.File.Exists(path))
            {
                bytes = System.IO.File.ReadAllBytes(path);
            }
            return File(bytes, System.Net.Mime.MediaTypeNames.Application.Octet, Name);

        }

        public ActionResult DeleteRevision(int? RevisionID)
        {

            string FileName = db.Revisions.Find(RevisionID).nom;
            string path = Path.Combine(Server.MapPath("~/Revision/"), FileName);

            ViewBag.deleteSuccess = "Couldn't be deleted";
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
                db.Revisions.Remove(db.Revisions.Find(RevisionID));
                db.SaveChanges();
                ViewBag.deleteSuccess = "Successfully deleted";
            }
            return RedirectToAction("Index", new { FileID = db.Documents.Find(db.Revisions.Find(RevisionID).parent).repository});
        }
    }
}