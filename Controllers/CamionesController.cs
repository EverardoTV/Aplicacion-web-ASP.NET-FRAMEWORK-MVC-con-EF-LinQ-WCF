using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DTO;
using Transportes_MVC.Models;

namespace Transportes_MVC.Controllers
{
    public class CamionesController : Controller
    {
        // GET: Camiones
        public ActionResult Index()
        {
            //crear una lista de camiones del modelo original
            List<Camiones> lista_camiones = new List<Camiones>();

            //lleno la lista con elementos existentes dentro del contexto(BD) utilizando EntityFramework y LinQ

            using (TransportesEntities context = new TransportesEntities())
            {
                //lleno mi lista directamento usando LinQ
                lista_camiones = (from camion in context.Camiones select camion).ToList();
                //otra forma de usar LinQ
                // lista_camiones = context.Camiones.ToList();
                //otra forma de hacerlo 
                // foreach (Camiones cam in context.Camiones)
                // {
                //     lista_camiones.Add(cam);
                // }
            }

            //ViewBag (forma parte de Razor) se caracteriza por hacer uso de ua propiedad arbitraria que sirve para pasar informacion desde el controlador a la vista
            ViewBag.Titulo = "Lista de Camiones";
            ViewBag.Subtitulo = "Utilizando ASP.NET MVC";

            //ViewData se caracteriza por hacer uso de un atributo arbitrario y tiene el mismo funcionamiento que el ViewBag
            ViewData["Titulo2"] = "Segundo Título";

            //TempData se caracteriza por permitit crear variables temporales que existen durante la ejecucion del Runtime de ASP
            //además, los temdata me permite compartir informacion no solo del cotrolador a la vista, sino tambien entre otras vistas y otros controladores
            //TempData.Add("Clave", "Valor");


            //Retorno la vista con los datos del modelo
            return View(lista_camiones);
        }

        //GET: Nuevo_Camion
        public ActionResult Nuevo_Camion()
        {
            ViewBag.Titulo = "Nuevo Camión";
            //cargo los DDL con las opciones del tipo camión
            cargarDDL();
            return View();
        }


        //POST: Nuevo_Camión
        [HttpPost]

        public ActionResult Nuevo_Camion(Camiones_DTO model, HttpPostedFileBase imagen)
        {
            try
            {
                if (ModelState.IsValid) //verifica si los campos y los tipos de datos corresponden al modelo(DTO)
                {
                    using (TransportesEntities context = new TransportesEntities()) //crea una instancia de un unico uso del contexto

                    {
                        var camion = new Camiones(); //creo una instancia de un objeto del modelo original (<PROYECTO>.Models)

                        //Asigno todos los valores del modelo de entrada (DTO) al objeto que será enviado a la BD (Modelo Original)

                        camion.Matricula = model.Matricula;
                        camion.Marca = model.Marca;
                        camion.Modelo = model.Modelo;
                        camion.Tipo_Camion = model.Tipo_Camion;
                        camion.Capacidad = model.Capacidad;
                        camion.Kilometraje = model.Kilometraje;
                        camion.Disponibilidad = model.Disponibilidad;
                 
                        //Valido si existe una imagen en la peticion 

                        if(imagen != null && imagen.ContentLength > 0)
                        {
                            string filename = Path.GetFileName(imagen.FileName); //recupero el nombre de la imagen que viene de la peticion 

                            string pathdir = Server.MapPath("~/Assets/Imagenes/Camiones/");//mapeo la ruta donde guardaré mi imagenes en el servidor
                            if (!Directory.Exists(pathdir))//si no existe el directorio, lo creo
                            {
                                Directory.CreateDirectory(pathdir);
                            }

                            imagen.SaveAs(pathdir + filename); //guardo la imagen en el servidor
                            camion.UrlFoto = "~/Assets/Imagenes/Camiones" + filename; //guardo la ruta y el nombre del archivo para enviarlo a la BD

                            //Impacto sobre la BD usando Entity Framework
                            context.Camiones.Add(camion); //agrego un nuevo camion al contexto
                            context.SaveChanges(); //impacto la base de datos enviando las modificaciones sufridas en el contexto
                            //Sweet Alert
                            return RedirectToAction("Index"); //finalmente, regreso al listado de este mismo controlador (Camiones) si es que todo salió bien.

                        }
                        else
                        {
                            //Sweet Alert
                            cargarDDL();
                            return View(model);
                        }

                    }
                }
                else
                {
                    //Sweet Alert
                    cargarDDL();
                    return View(model);
                }
            }
            catch(Exception ex) 
            {
                //En caso de que ocurra una exepción, voy a mostrar un mensaje con el error(SweetAlert), voy a devolerle a la vista el modelo que causo el conflicto (Return View(model)) y vuelvo a cargar el DDL para que estén disponibles esas opciones (cargarDDL())
                //Sweet Alert
                cargarDDL();
                return View(model);




            }
        }









        #region Auxiliares
        private class Opciones
        {
            public string Numero { get; set; }

            public string Descripcion { get; set; }
        }

        public void cargarDDL()
        {
            List<Opciones> lista_opciones = new List<Opciones>()
            {
                new Opciones() { Numero = "0", Descripcion="Seleccione una opción"},
                new Opciones() { Numero = "1", Descripcion = "Volteo" },
                new Opciones() { Numero = "2", Descripcion = "Redilas" },
                new Opciones() { Numero = "3", Descripcion = "Transporte" }

            };
            ViewBag.ListaTipos = lista_opciones;
        }
        #endregion
    }
}