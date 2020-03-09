using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProjAsso;

namespace ProjAsso.Controllers
{
    public class SortiesController : Controller
    {
        private ProjAssoEntities db = new ProjAssoEntities();

        // GET: Sorties
        public ActionResult Index()
        {
            Adherent adherent = (Adherent)Session["Adherent"];
            ViewBag.IsInscript = false;


            if (adherent != null)
            {
                var sorties = db.Sorties.Include(s => s.Association).Where(a => a.IdAssociation == adherent.IdAssociation);
                ViewBag.Adherent = adherent;
                return View(sorties.ToList());
                
            }

            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
        }

        public ActionResult MesSorties()
        {
            Adherent adherent = (Adherent)Session["Adherent"];

            if (adherent != null)
            {
                var mesSorties = db.SortieAdherents.Where(sa => sa.IdAdherent == adherent.IdAdherent).ToList();
                return View(mesSorties);
            }

            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

        }

        public ActionResult Inscription(int? id)
        {
            Adherent adherent = (Adherent)Session["Adherent"];

            if (adherent == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Sortie maSortie = db.Sorties.Find(id);

            if (maSortie == null)
            {
                return HttpNotFound();
            }

            List<Sortie> mesSorties = new List<Sortie>();

            if(Session["MesSorties"] == null)
            {
                Session["MesSorties"] = maSortie;
            }
            else
            {
                mesSorties = (List<Sortie>)Session["MesSorties"];
                mesSorties.Add(maSortie);
                Session["MesSorties"] = mesSorties;
            }

            SortieAdherent sortieAdherent = new SortieAdherent();
            sortieAdherent.IdAdherent = adherent.IdAdherent;
            sortieAdherent.IdSortie = (int)id;
            sortieAdherent.IdAssociation = adherent.IdAssociation;

            db.SortieAdherents.Add(sortieAdherent);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Sorties/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sortie sortie = db.Sorties.Find(id);
            if (sortie == null)
            {
                return HttpNotFound();
            }
            return View(sortie);
        }

        // GET: Sorties/Create
        public ActionResult Create()
        {
            ViewBag.IdAssociation = new SelectList(db.Associations, "IdAssociation", "Nom");
            return View();
        }

        // POST: Sorties/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdSortie,IdAssociation,Nom,Prix,Description,Date,CapaciteMinimum,CapaciteMaximum")] Sortie sortie)
        {
            if (ModelState.IsValid)
            {
                db.Sorties.Add(sortie);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdAssociation = new SelectList(db.Associations, "IdAssociation", "Nom", sortie.IdAssociation);
            return View(sortie);
        }

        // GET: Sorties/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sortie sortie = db.Sorties.Find(id);
            if (sortie == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdAssociation = new SelectList(db.Associations, "IdAssociation", "Nom", sortie.IdAssociation);
            return View(sortie);
        }

        // POST: Sorties/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdSortie,IdAssociation,Nom,Prix,Description,Date,CapaciteMinimum,CapaciteMaximum")] Sortie sortie)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sortie).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdAssociation = new SelectList(db.Associations, "IdAssociation", "Nom", sortie.IdAssociation);
            return View(sortie);
        }

        // GET: Sorties/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sortie sortie = db.Sorties.Find(id);
            if (sortie == null)
            {
                return HttpNotFound();
            }
            return View(sortie);
        }

        // POST: Sorties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Sortie sortie = db.Sorties.Find(id);
            db.Sorties.Remove(sortie);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
