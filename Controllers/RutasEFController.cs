using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Transportes_MVC.Models;

namespace Transportes_MVC.Controllers
{
    public class RutasEFController : Controller
    {
        private TransportesEntities db = new TransportesEntities();

        // GET: RutasEF
        public ActionResult Index()
        {
            var rutas = db.Rutas.Include(r => r.Camiones).Include(r => r.Choferes).Include(r => r.Direcciones).Include(r => r.Direcciones1);
            return View(rutas.ToList());
        }

        // GET: RutasEF/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rutas rutas = db.Rutas.Find(id);
            if (rutas == null)
            {
                return HttpNotFound();
            }
            return View(rutas);
        }

        // GET: RutasEF/Create
        public ActionResult Create()
        {
            ViewBag.Camion_ID = new SelectList(db.Camiones, "ID_Camion", "Matricula");
            ViewBag.Chofer_ID = new SelectList(db.Choferes, "ID_Chofer", "Nombre");
            ViewBag.Direccionorigen_ID = new SelectList(db.Direcciones, "ID_Direccion", "Calle");
            ViewBag.Direcciondestino_ID = new SelectList(db.Direcciones, "ID_Direccion", "Calle");
            return View();
        }

        // POST: RutasEF/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_Ruta,Camion_ID,Distancia,Fecha_salida,Fecha_llegadaestimada,Fecha_llegadareal,Chofer_ID,Direccionorigen_ID,Direcciondestino_ID")] Rutas rutas)
        {
            if (ModelState.IsValid)
            {
                db.Rutas.Add(rutas);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Camion_ID = new SelectList(db.Camiones, "ID_Camion", "Matricula", rutas.Camion_ID);
            ViewBag.Chofer_ID = new SelectList(db.Choferes, "ID_Chofer", "Nombre", rutas.Chofer_ID);
            ViewBag.Direccionorigen_ID = new SelectList(db.Direcciones, "ID_Direccion", "Calle", rutas.Direccionorigen_ID);
            ViewBag.Direcciondestino_ID = new SelectList(db.Direcciones, "ID_Direccion", "Calle", rutas.Direcciondestino_ID);
            return View(rutas);
        }

        // GET: RutasEF/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rutas rutas = db.Rutas.Find(id);
            if (rutas == null)
            {
                return HttpNotFound();
            }
            ViewBag.Camion_ID = new SelectList(db.Camiones, "ID_Camion", "Matricula", rutas.Camion_ID);
            ViewBag.Chofer_ID = new SelectList(db.Choferes, "ID_Chofer", "Nombre", rutas.Chofer_ID);
            ViewBag.Direccionorigen_ID = new SelectList(db.Direcciones, "ID_Direccion", "Calle", rutas.Direccionorigen_ID);
            ViewBag.Direcciondestino_ID = new SelectList(db.Direcciones, "ID_Direccion", "Calle", rutas.Direcciondestino_ID);
            return View(rutas);
        }

        // POST: RutasEF/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_Ruta,Camion_ID,Distancia,Fecha_salida,Fecha_llegadaestimada,Fecha_llegadareal,Chofer_ID,Direccionorigen_ID,Direcciondestino_ID")] Rutas rutas)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rutas).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Camion_ID = new SelectList(db.Camiones, "ID_Camion", "Matricula", rutas.Camion_ID);
            ViewBag.Chofer_ID = new SelectList(db.Choferes, "ID_Chofer", "Nombre", rutas.Chofer_ID);
            ViewBag.Direccionorigen_ID = new SelectList(db.Direcciones, "ID_Direccion", "Calle", rutas.Direccionorigen_ID);
            ViewBag.Direcciondestino_ID = new SelectList(db.Direcciones, "ID_Direccion", "Calle", rutas.Direcciondestino_ID);
            return View(rutas);
        }

        // GET: RutasEF/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rutas rutas = db.Rutas.Find(id);
            if (rutas == null)
            {
                return HttpNotFound();
            }
            return View(rutas);
        }

        // POST: RutasEF/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Rutas rutas = db.Rutas.Find(id);
            db.Rutas.Remove(rutas);
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
