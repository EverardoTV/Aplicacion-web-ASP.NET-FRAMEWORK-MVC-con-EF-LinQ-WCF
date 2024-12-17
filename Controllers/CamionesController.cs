using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Management;
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

                        if (imagen != null && imagen.ContentLength > 0)
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
            catch (Exception ex)
            {
                //En caso de que ocurra una exepción, voy a mostrar un mensaje con el error(SweetAlert), voy a devolerle a la vista el modelo que causo el conflicto (Return View(model)) y vuelvo a cargar el DDL para que estén disponibles esas opciones (cargarDDL())
                //Sweet Alert
                cargarDDL();
                return View(model);




            }
        }

        //GET: Editar_Camion/{id}
        public ActionResult Editar_Camion(int id)
        {
            if (id > 0)//validar que realmente llegue un ID valido
            {
                Camiones_DTO camion = new Camiones_DTO(); //creo una instancia del tipo DTO para pasar informacion desde el contexto a la vista con ayuda de EF y LinQ
                using (TransportesEntities context = new TransportesEntities()) //creo una instancia de un solo uso de mi contexto
                {
                    //busco a aquel elemento que coincida con el ID
                    //Bajo método
                    //no puedo colocar directamente un tipo de datos (modelo original) en un DTO, por lo que primero me valgo de recuperarlo y posteriormente asigno sus valores(mapeo)



                    var camion_aux = context.Camiones.Where(x => x.ID_Camion == id).FirstOrDefault();
                    var camion_aux2 = context.Camiones.FirstOrDefault(x => x.ID_Camion == id);

                    camion.Matricula = camion_aux.Matricula;
                    camion.Marca = camion_aux.Marca;
                    camion.Modelo = camion_aux.Modelo;
                    camion.Capacidad = camion_aux.Capacidad;
                    camion.Kilometraje = camion_aux.Kilometraje;
                    camion.Tipo_Camion = camion.Tipo_Camion;
                    camion.Disponibilidad = camion_aux.Disponibilidad;
                    camion.UrlFoto = camion_aux.UrlFoto;
                    camion.ID_Camion = camion_aux.ID_Camion;



                    //bajo una consulta (usando LinQ)
                    //cuando hago una consulta directa, tengo la oportunidad de asignar valores a tipos de datos más complejos o diferentes, incluso, pudiendo crear nuevos datos a partir de datos existentes (instancias de clases)
                    camion = (from c in context.Camiones
                              where c.ID_Camion == id
                              select new Camiones_DTO()
                              {
                                  ID_Camion = c.ID_Camion,
                                  Matricula = c.Matricula,
                                  Marca = c.Marca,
                                  Modelo = c.Modelo,
                                  Capacidad = c.Capacidad,
                                  Kilometraje = c.Kilometraje,
                                  Tipo_Camion = c.Tipo_Camion,
                                  Disponibilidad = c.Disponibilidad,
                                  UrlFoto = c.UrlFoto
                              }).FirstOrDefault();

                }//cierre del using(context)

                if (camion == null)//Valido si realmente recuperé los datos de la BD
                {
                    //Sweet Alert
                    return RedirectToAction("Index");
                }
                //si todo sale bien, envío a la vista con los datos a Editar
                ViewBag.Titulo = $"Editar Camión #{camion.ID_Camion}";
                cargarDDL();
                return View(camion);


            }
            else
            {
                //Sweet alert
                return RedirectToAction("Index");

            }
        }



        //POST: Editar_Camion
        [HttpPost]

        public ActionResult Editar_Camion(Camiones_DTO model, HttpPostedFileBase imagen)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (TransportesEntities context = new TransportesEntities())
                    {
                        var camion = new Camiones();

                        camion.ID_Camion = model.ID_Camion;
                        camion.Matricula = model.Matricula;
                        camion.Marca = model.Marca;
                        camion.Modelo = model.Modelo;
                        camion.Capacidad = model.Capacidad;
                        camion.Tipo_Camion = model.Tipo_Camion;
                        camion.Disponibilidad = model.Disponibilidad;
                        camion.Kilometraje = model.Kilometraje;

                        if (imagen != null && imagen.ContentLength > 0)
                        {
                            string filename = Path.GetFileName(imagen.FileName);
                            string pathdir = Server.MapPath("~/Assets/Imagenes/Camiones/");
                            if (model.UrlFoto.Length == 0)
                            {
                                //la imagen en la BD es null y hay que darle la imagen
                                if (!Directory.Exists(pathdir))
                                {
                                    Directory.CreateDirectory(pathdir);
                                }

                                imagen.SaveAs(pathdir + filename);
                                camion.UrlFoto = "/Assets/Imagenes/Camiones/" + filename;
                            }
                            else
                            {
                                //validar si es la misma o es nueva
                                if (model.UrlFoto.Contains(filename))
                                {
                                    //es la misma
                                    camion.UrlFoto = "/Assets/Imagenes/Camiones/" + filename;
                                }
                                else
                                {
                                    //es diferente
                                    if (!Directory.Exists(pathdir))
                                    {
                                        Directory.CreateDirectory(pathdir);
                                    }

                                    //Borro la imagen anterios
                                    //valido si existe

                                    try
                                    {
                                        string pathdir_old = Server.MapPath("~" + model.UrlFoto); //busco la imagen que catualmente tiene el camión
                                        if (System.IO.File.Exists(pathdir_old)) //valido si existe dicho archivo
                                        {
                                            //procedo a eliminarlo
                                            System.IO.File.Delete(pathdir_old);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        //Sweet Alert
                                    }

                                    imagen.SaveAs(pathdir + filename);
                                    camion.UrlFoto = "/Assets/Imagenes/Camiones/" + filename;
                                }
                            }
                        }
                        else //si no hya una nueva imagen, paso la misma
                        {
                            camion.UrlFoto = model.UrlFoto;
                        }

                        //Guardar cambios, validar excepciones, redirigir
                        //actualizar el estado de nuestro elemento
                        //.Entry() registrar la entrada de nueva información al contexto y notificar un cambio de estado usando System.Data.Entity.EntityState.Modified
                        context.Entry(camion).State = System.Data.Entity.EntityState.Modified;
                        //impactamos la BD
                        try
                        {
                            context.SaveChanges();
                        }
                        //agregar using desde 'using System.Data.Entity.Validation;'
                        catch (DbEntityValidationException ex)
                        {
                            string resp = "";
                            //recorro todos los posibles errores de la Entidad Referencial
                            foreach (var error in ex.EntityValidationErrors)
                            {
                                //recorro los detalles de cada error
                                foreach (var validationError in error.ValidationErrors)
                                {
                                    resp += "Error en la Entidad: " + error.Entry.Entity.GetType().Name;
                                    resp += validationError.PropertyName;
                                    resp += validationError.ErrorMessage;
                                }
                            }
                            //Sweet Alert
                        }
                        //Sweet Alert
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    //Sweet Alert
                    cargarDDL();
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                //Sweet Alert
                cargarDDL();
                return View(model);
            }
        }



        //GET: Eliminar_Camion/{id}
        public ActionResult Eliminar_Camion(int id)
        {
            try
            {
                using (TransportesEntities context = new TransportesEntities())
                {
                    //Voy a recuperar el camión que deseo eliminar
                    var camion = context.Camiones.FirstOrDefault(x => x.ID_Camion == id);
                    //Valido si realmente existe dicho camion
                    if (camion == null)
                    {
                        //me voy pa'tras
                        //Sweet alert
                        return RedirectToAction("Index");
                    }

                    //Procedo a eliminar 
                    context.Camiones.Remove(camion);
                    context.SaveChanges();
                    //Sweet alert
                    SweetAlert("Eliminado", "Camión eliminado con éxito", NotificationType.success);
                    return RedirectToAction("Index");
                }

            }
            catch (Exception ex)
            {
                //Sweet Alert
                SweetAlert("Opsss....", $"Ha ocurrido un error: {ex.Message}", NotificationType.error);
                return RedirectToAction("Index");

            }

        }


        //GET: ConfirmarEliminar

        public ActionResult Confirmar_Eliminar(int id)
        {
            SweetAlert_Eliminar(id);
            return RedirectToAction("Index");
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


        #region Sweet Alert
        private void SweetAlert(string title, string msg, NotificationType type)
        {
            var script = "<script languaje='javascript'> " +
                         "Swal.fire({" +
                         "title: '" + title + "'," +
                         "text: '" + msg + "'," +
                         "icon: '" + type + "'" +
                         "});" +
                         "</script>"; ;
            //TempData funciona como un ViewBag, pasando informacion del controlador a cualquier parte de mi proyecto, siendo este, mas observable y con un tiempo de vida mayor
            TempData["sweetalert"] = script;
        }
        private void SweetAlert_Eliminar(int id)
        {
            var script = "<script languaje='javascript'>" +
                "Swal.fire({" +
                "title: '¿Estás Seguro?'," +
                "text: 'Estás apunto de Eliminar el Camión: " + id.ToString() + "'," +
                "icon: 'info'," +
                "showDenyButton: true," +
                "showCancelButton: true," +
                "confirmButtonText: 'Eliminar'," +
                "denyButtonText: 'Cancelar'" +
                "}).then((result) => {" +
                "if (result.isConfirmed) {  " +
                "window.location.href = '/Camiones/Eliminar_Camion/" + id + "';" +
                "} else if (result.isDenied) {  " +
                "Swal.fire('Se ha cancelado la operación','','info');" +
                "}" +
                "});" +
                "</script>";

            TempData["sweetalert"] = script;
        }

      private enum NotificationType
        {
            success,
            error,
            question,
            warning,
            info
        }
        #endregion
    }
}