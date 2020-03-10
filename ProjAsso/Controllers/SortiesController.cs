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

        //GET
        public ActionResult Inscription(int? idSortie, int? idAdherent)
        {
            Adherent adherent = db.Adherents.Find(idAdherent);

            //Sécurité
            if (adherent == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Instanciation de la sortie à laquelle on souhaite s'inscrire
            Sortie maSortie = db.Sorties.Find(idSortie);

            //Sécurité
            if (maSortie == null)
            {
                return HttpNotFound();
            }

            //Instanciation d'une liste de sortie
            List<Sortie> mesSorties = new List<Sortie>();

            //Déclaration d'une session regroupant les sorties
            //Si elle est vide, on l'instancie avec la première inscription à une sortie
            if (Session["MesSorties"] == null)
            {
                Session["MesSorties"] = maSortie;
                adherent.Solde -= maSortie.Prix;
            }
            //Si elle contient déjà une/des sortie(s) on y ajoute la dernière
            else
            {
                mesSorties = (List<Sortie>)Session["MesSorties"];
                mesSorties.Add(maSortie);
                Session["MesSorties"] = mesSorties;
                adherent.Solde -= maSortie.Prix;
            }

            //Instanciation d'un objet vierge de type sortie adhérent, dont on renseigne les valeurs pour ses différents champs
            SortieAdherent sortieAdherent = new SortieAdherent();
            sortieAdherent.IdAdherent = adherent.IdAdherent;
            sortieAdherent.IdSortie = (int)idSortie;
            sortieAdherent.IdAssociation = adherent.IdAssociation;

            //Même chose qu'au dessous, sauf que c'est pour l'historique de paiement
            HistoriquePaiement historiquePaiement = new HistoriquePaiement();
            historiquePaiement.IdAdherent = adherent.IdAdherent;
            historiquePaiement.IdAssociation = adherent.IdAssociation;
            historiquePaiement.Paiement = maSortie.Prix;
            historiquePaiement.Date = DateTime.Now;

            //Sauvegarde en base de données
            db.SortieAdherents.Add(sortieAdherent);
            db.HistoriquePaiements.Add(historiquePaiement);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //GET
        public ActionResult CancelInscription(int? idSortie, int? idAdherent, int? idAssociation, int?idHistoriquePaiement)
        {
            Adherent adherent = db.Adherents.Find(idAdherent);

            //Sécurité
            if (adherent == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Sortie maSortie = db.Sorties.Find(idSortie);
            adherent.Solde += maSortie.Prix;

            //Sécurité
            if (maSortie == null)
            {
                return HttpNotFound();
            }

            //Instanciation de la sortie à laquelle on souhaite annuler son incription,  via les 3 paramètres de notre table sortieAdhérent, et suppression de l'incription
            SortieAdherent sortieAdherent = db.SortieAdherents.Find(idSortie, idAdherent, idAssociation);
            db.SortieAdherents.Remove(sortieAdherent);

            //Recherche de l'historique paiement lié à la sortie via l'idSortie, afin de le delete
            HistoriquePaiement historiquePaiement = db.HistoriquePaiements.FirstOrDefault(hp => hp.IdSortie == maSortie.IdSortie);
            db.HistoriquePaiements.Remove(historiquePaiement);

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
