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
    public class HistoriquePaiementsController : Controller
    {
        private ProjAssoEntities db = new ProjAssoEntities();

        // GET: HistoriquePaiements
        public ActionResult Index(int? idAdherent)
        {
            Adherent adherent = db.Adherents.Find(idAdherent);
             
            if(idAdherent != null && adherent.Responsable == true)
            {
            var historiquePaiements = db.HistoriquePaiements.Include(h => h.Adherent).Include(h => h.Association).Include(h => h.Sortie).Where(h => h.IdAssociation == adherent.IdAssociation && h.Statut == true);
            return View(historiquePaiements.ToList());
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
        }

        // GET: HistoriquePaiements/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HistoriquePaiement historiquePaiement = db.HistoriquePaiements.Find(id);
            if (historiquePaiement == null)
            {
                return HttpNotFound();
            }
            return View(historiquePaiement);
        }

        // GET: HistoriquePaiements/Create
        public ActionResult Create()
        {
            ViewBag.IdAdherent = new SelectList(db.Adherents, "IdAdherent", "Matricule");
            ViewBag.IdAssociation = new SelectList(db.Associations, "IdAssociation", "Nom");
            ViewBag.IdSortie = new SelectList(db.Sorties, "IdSortie", "Nom");
            return View();
        }

        // POST: HistoriquePaiements/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdHistoriquePaiement,IdAdherent,IdAssociation,Paiement,Date,IdSortie")] HistoriquePaiement historiquePaiement)
        {
            if (ModelState.IsValid)
            {
                db.HistoriquePaiements.Add(historiquePaiement);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdAdherent = new SelectList(db.Adherents, "IdAdherent", "Matricule", historiquePaiement.IdAdherent);
            ViewBag.IdAssociation = new SelectList(db.Associations, "IdAssociation", "Nom", historiquePaiement.IdAssociation);
            ViewBag.IdSortie = new SelectList(db.Sorties, "IdSortie", "Nom", historiquePaiement.IdSortie);
            return View(historiquePaiement);
        }

        // GET: HistoriquePaiements/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HistoriquePaiement historiquePaiement = db.HistoriquePaiements.Find(id);
            if (historiquePaiement == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdAdherent = new SelectList(db.Adherents, "IdAdherent", "Matricule", historiquePaiement.IdAdherent);
            ViewBag.IdAssociation = new SelectList(db.Associations, "IdAssociation", "Nom", historiquePaiement.IdAssociation);
            ViewBag.IdSortie = new SelectList(db.Sorties, "IdSortie", "Nom", historiquePaiement.IdSortie);
            return View(historiquePaiement);
        }

        // POST: HistoriquePaiements/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdHistoriquePaiement,IdAdherent,IdAssociation,Paiement,Date,IdSortie")] HistoriquePaiement historiquePaiement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(historiquePaiement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdAdherent = new SelectList(db.Adherents, "IdAdherent", "Matricule", historiquePaiement.IdAdherent);
            ViewBag.IdAssociation = new SelectList(db.Associations, "IdAssociation", "Nom", historiquePaiement.IdAssociation);
            ViewBag.IdSortie = new SelectList(db.Sorties, "IdSortie", "Nom", historiquePaiement.IdSortie);
            return View(historiquePaiement);
        }

        // GET: HistoriquePaiements/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HistoriquePaiement historiquePaiement = db.HistoriquePaiements.Find(id);
            if (historiquePaiement == null)
            {
                return HttpNotFound();
            }
            return View(historiquePaiement);
        }

        // POST: HistoriquePaiements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HistoriquePaiement historiquePaiement = db.HistoriquePaiements.Find(id);
            db.HistoriquePaiements.Remove(historiquePaiement);
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
