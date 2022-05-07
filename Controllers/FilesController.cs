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
    public class FilesController : Controller
    {
        private readonly DBC db = new DBC();
        // GET: Files
        /*public ActionResult Index()
        {
            ViewBag.docs = db.Documents.Where(d => d.repository == null).ToList();
            ViewBag.Reps = db.Repertoires.Where(r => r.parent == null).ToList();
            
            return View();
        }*/


        public ActionResult Index(int? RepID)
        {
            
            Session["Directory"] = RepID;
            int user = Convert.ToInt32(Session["UserId"]);
            ViewBag.Docs = db.Documents.Where(d => d.repository == RepID && (d.UserID == user ||  !(!d.isEditable && !d.isDowloadable && !d.isVisible)));
            ViewBag.Reps = db.Repertoires.Where(d => d.parent == RepID && (d.proprietaire == user || !(!d.isEditable && !d.isDowloadable && !d.isVisible)));


            Repertoire r;
            if (RepID != null)
            {
                r = db.Repertoires.Find(RepID);
               
            }
            else
            {
                r = new Repertoire();
                r.NomRep = "Uploads";
                r.rep = 0;
                r.isEditable = true;
                r.proprietaire = null;
            }

            List<Repertoire> path =  new List<Repertoire>();
            int? i = RepID;
            while (i != null)
            {
                path.Insert(0, db.Repertoires.Find(i));
                i = db.Repertoires.Find(i).parent; 
            }
            
            ViewBag.path = path;

            return View(r);
        }


        [HttpGet]
        public FileResult DownloadFile(int? FileID)
        {
            if (FileID == null) RedirectToAction("Index");
            string Name = db.Documents.Find(FileID).titre;
            string path = Path.Combine(Server.MapPath(FindPath(Convert.ToInt32(Session["Directory"]))), Name);
            Console.WriteLine(path);

            byte[] bytes = { };
            if (System.IO.File.Exists(path))
            {
                bytes = System.IO.File.ReadAllBytes(path);
            }
            return File(bytes, System.Net.Mime.MediaTypeNames.Application.Octet, Name);

        }

        public ActionResult OpenFile(int? FileID)
        {
            if (FileID == null) RedirectToAction("Index");
            string Name = db.Documents.Find(FileID).titre;
            string path = Path.Combine(Server.MapPath(FindPath(Convert.ToInt32(Session["Directory"]))), Name);
            byte[] FileBytes = System.IO.File.ReadAllBytes(path);
            return File(FileBytes, "application / " + Path.GetExtension(Name).TrimStart('.'));
        }

        

        public ActionResult UploadFile()
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Utilisateurs", null);
            if (Session["Directory"] != null)
                ViewBag.location = db.Repertoires.Find(Session["Directory"]).NomRep;
            else
                ViewBag.location = "Uploads";

            return View();
        }

        private string FindPath(int? rep)
        {
            if (rep == null) return "~/uploads/";

            string path = "";
            Repertoire r = db.Repertoires.Find(rep);

            if (r != null)
                do
                {
                    path = r.NomRep + "/";
                    r = db.Repertoires.Find(r.parent);
                } while (r != null && r.parent != null);

            return "~/Uploads/" + path;
        }

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase[] files, bool IsDownloadable, bool IsEditable, bool IsVisible)
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Utilisateurs", null);

            Response.Write("<script>alert(' we are here " + Session["Directory"] + " ')</script>");

            int? RepID = Convert.ToInt32(Session["Directory"]);
            if (Session["Directory"] != null)
                ViewBag.location = db.Repertoires.Find(RepID).NomRep;
            else
                ViewBag.location = "Uploads";

            Document doc = new Document();
            if (ModelState.IsValid)
                foreach(HttpPostedFileBase file in files) {
                    if (file == null) continue;
                    string path = FindPath(RepID);
                    string name = file.FileName;
                    int i = 0;
                    string newpath = Path.Combine(Server.MapPath(path), Path.GetFileName(name));
                    while (System.IO.File.Exists(newpath))
                    {
                        name = Path.GetFileNameWithoutExtension(file.FileName) + "(" + (++i) + ")" + Path.GetExtension(file.FileName);
                        newpath = Path.Combine(Server.MapPath(path), Path.GetFileName(name));
                    }

                    file.SaveAs(newpath);
                    
                    
                    doc.titre = name;
                    doc.UserID = Convert.ToInt32(Session["UserID"]);
                    doc.DateDepos = DateTime.Now;
                    if (RepID != 0) doc.repository = RepID;
                    else doc.repository = null;
                    doc.isEditable = IsEditable;
                    doc.isDowloadable = IsDownloadable;
                    doc.isVisible = IsVisible;
                    db.Documents.Add(doc);
                }
            db.SaveChanges();
            ViewBag.UploadStatus = files.Count().ToString() + " files uploaded successfully.";

            return RedirectToAction("Index", new { RepID = Session["Directory"] });
        }

        public ActionResult UploadRevision(int FileID)
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Utilisateurs", null);
            ViewBag.parent = FileID;
            return View();
        }

        [HttpPost]
        public ActionResult UploadRevision(HttpPostedFileBase[] files, int doc) 
        {
            var file = files[0];
            //int doc = Convert.ToInt32(ViewBag.parent);
            string number = db.Revisions.Where(r => r.parent == doc).ToList().Count().ToString();
            if (number == "0") number = "";
            if (ModelState.IsValid)
            {
                Revision r = new Revision();
                r.nom = db.Documents.Find(doc).titre;
                r.nom = Path.GetFileNameWithoutExtension(r.nom) + number + Path.GetExtension(r.nom);
                file.SaveAs(Path.Combine(Server.MapPath("~/Revision/"), r.nom));

                string path = FindPath(db.Documents.Find(doc).repository);
                db.Documents.Find(doc).titre = Path.GetFileNameWithoutExtension(r.nom) + Path.GetExtension(file.FileName);
                file.SaveAs(Path.Combine(Server.MapPath(path), Path.GetFileName(db.Documents.Find(doc).titre)));
                
                db.Documents.Find(doc).DateDepos = r.DateDepos = DateTime.Now;
                r.parent = doc;
                r.autheur = Convert.ToInt32(Session["UserId"]);
                db.Revisions.Add(r);
                db.SaveChanges();

                return RedirectToAction("Index", new { RepID = Session["Directory"] });

            }
            return View();
        }
        

        public ActionResult MakeDirectory()
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "Utilisateurs", null);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MakeDirectory(Repertoire rep)
        {
            ViewBag.UploadStatus = "Not Created!";
            if(Session["UserID"] == null) return RedirectToAction("Login", "Utilisateurs", null);

            rep.parent = Convert.ToInt32(Session["Directory"]);
            if (rep.parent == 0) rep.parent = null;
            rep.proprietaire = Convert.ToInt32(Session["UserID"]);
            rep.DateDepos = DateTime.Now;

            string path = FindPath(rep.parent);
            string name = rep.NomRep;
            int i = 0;
            string newpath = Path.Combine(Server.MapPath(path), name);
            while (System.IO.Directory.Exists(newpath))
            {
                name = name + "(" + (++i) + ")";
                newpath = Path.Combine(Server.MapPath(path), name);
            }

            rep.NomRep = name;
            if (ModelState.IsValid)
            {
                Directory.CreateDirectory(newpath);
                db.Repertoires.Add(rep);
                db.SaveChanges();
                return RedirectToAction("Index", new { RepID = Session["Directory"]});
                
            }
            return View(rep);
        }

        /*public ActionResult DownloadFolder(int FolderID)
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "Utilisateurs", null);
            ViewBag.FolderID = FolderID;
            return View();
        }

        public ActionResult DownloadFolder(int FolderID, string Destination)
        {
            foreach(Document doc in db.Documents.Where(d => d.repository == FolderID).ToList())
            {
                HttpPostedFileBase file = new HttpPostedFile("ndnnd");
            }
            return View();
        }*/

        
        public ActionResult DeleteFile(int? FileID)
        {

            string FileName = db.Documents.Find(FileID).titre;
            string path = FindPath(db.Documents.Find(FileID).repository);
            path = Path.Combine(Server.MapPath(path), FileName);

            ViewBag.Message = "*" + FileName +  " Couldn't be deleted";
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
                db.Documents.Remove(db.Documents.Find(FileID));
                db.SaveChanges();
                ViewBag.Message = "*" + FileName + " is deleted";
            }
            return RedirectToAction("Index", new { RepID = Session["Directory"]});
        }

        private void DeleteChildren(Repertoire rep)
        {
            IQueryable<Document> docs = db.Documents.Where(c => c.repository == rep.rep);
            foreach (var doc in docs)
                db.Documents.Remove(doc);

            IQueryable<Repertoire> children = db.Repertoires.Where(c => c.parent == rep.rep);
            foreach (var child in children)
                DeleteChildren(child);

            db.Repertoires.Remove(rep);
            
        }

        public ActionResult DeleteFolder(int? FolderID)
        {
            if (FolderID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Repertoire repertoire = db.Repertoires.Find(FolderID);
            if (repertoire == null)
            {
                return HttpNotFound();
            }
            
            string path = FindPath(FolderID);
            path = Path.Combine(Server.MapPath(path), db.Repertoires.Find(FolderID).NomRep);

            if (Directory.Exists(path))
                Directory.Delete(path, true);

            DeleteChildren(repertoire);

            db.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}