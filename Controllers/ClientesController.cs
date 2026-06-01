using Web1_abr13.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web1_abr13.Controllers
{
    public class ClientesController : Controller
    {
        [HttpGet]
        public ActionResult EditCliente(int? id)
        {
            using (BMW1 bd = new BMW1())
            {
                int id1 = id ?? -1;

                cliente este = bd.clientes.Find(id1);
                if (este != null)
                {
                    List<string> lstcategorias = new List<string>() { "Alfa", "Bravo", "Charlie" };
                    ViewBag.categorias = new SelectList(lstcategorias, este.categoria);

                    List<cliente> tutores = bd.clientes.ToList();
                    ViewBag.tutores = new SelectList(tutores, "num", "nome", este.tutor);

                    return View(este);
                }
                else
                {
                    return RedirectToAction("GetClientes", "Clientes", new { message = "Cliente não existe" });
                }
            }
        }

        [HttpPost]
        [ActionName("EditCliente")]
        public ActionResult EditCliente(cliente clienteEditado, HttpPostedFileBase fich)
        {
            if (ModelState.IsValid)
            {
                using (BMW1 bd = new BMW1())
                {
                    try
                    {
                        // Atualizar foto se um novo ficheiro for enviado
                        if (fich != null && fich.ContentLength > 0 && fich.ContentType.Contains("image"))
                        {
                            string fichnome = clienteEditado.num.ToString() + System.IO.Path.GetExtension(fich.FileName);
                            clienteEditado.fotopath = fichnome;
                            string fichpath = Server.MapPath($"~/fotos/{fichnome}");
                            fich.SaveAs(fichpath);
                        }

                        bd.Entry(clienteEditado).State = System.Data.Entity.EntityState.Modified;
                        bd.SaveChanges();
                        return RedirectToAction("GetClientes", "Clientes", new { message = "Cliente atualizado com sucesso." });
                    }
                    catch (Exception erro)
                    {
                        ModelState.AddModelError("", "Ocorreu um erro ao guardar as alterações: " + erro.Message);
                    }
                }
            }

            // Se o modelo não for válido ou ocorrer um erro, vai recarregar os dados necessários para a View
            using (BMW1 bd = new BMW1())
            {
                List<string> lstcategorias = new List<string>() { "Alfa", "Bravo", "Charlie" };
                ViewBag.categorias = new SelectList(lstcategorias, clienteEditado.categoria);

                List<cliente> tutores = bd.clientes.ToList();
                ViewBag.tutores = new SelectList(tutores, "num", "nome", clienteEditado.tutor);
            }
            return View(clienteEditado);
        }

        public ActionResult DeleteCliente(int? id)
        {
            using (BMW1 bd = new BMW1())
            {
                try
                {
                    int id1 = id ?? -1;

                    cliente este = bd.clientes.Find(id1);
                    if (este != null)
                    {
                        bd.clientes.Remove(este);
                        bd.SaveChanges();
                        return Json(new { msg = "ok" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { msg = "erro" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception erro)
                {
                    return Json(new { msg = erro.Message }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost]
        public ActionResult CreateCliente(cliente novo, HttpPostedFileBase fich)
        {
            try
            {
                using (BMW1 bd = new BMW1())
                {
                    if (ModelState.IsValid)
                    {
                        bd.clientes.Add(novo);
                        bd.SaveChanges();
                        if (fich != null && fich.ContentLength > 0 && fich.ContentType.Contains("image"))
                        {
                            string fichnome = novo.num.ToString() + System.IO.Path.GetExtension(fich.FileName);
                            novo.fotopath = fichnome;
                            string fichpath = Server.MapPath($"~/fotos/{fichnome}");
                            fich.SaveAs(fichpath);
                            bd.SaveChanges();
                        }
                        return RedirectToAction("GetClientes", new { message = "Registo inserido com sucesso" });
                    }
                    else
                    {
                        List<string> lstcategorias = new List<string>() { "Alfa", "Bravo", "Charlie" };
                        ViewBag.categorias = new SelectList(lstcategorias);

                        List<cliente> tutores = bd.clientes.ToList();
                        ViewBag.tutores = new SelectList(tutores, "num", "nome");
                        return View(novo);
                    }
                }
            }
            catch (Exception erro)
            {

                return RedirectToAction("GetClientes", new { message = erro.Message });
            }
        }


        public ActionResult CreateCliente()
        {
            using (BMW1 bd = new BMW1())
            {
                List<string> lstcategorias = new List<string>() { "Alfa", "Bravo", "Charlie" };
                ViewBag.categorias = new SelectList(lstcategorias);

                List<cliente> tutores = bd.clientes.ToList();
                ViewBag.tutores = new SelectList(tutores, "num", "nome");
                cliente novo = new cliente() { categoria = "Bravo" };
                return View(novo);
            }
        }




        // GET: Clientes
        public ActionResult GetClientes(string message)
        {
            using (BMW1 bd = new BMW1())
            {
                ViewBag.message = message;
                List<cliente> clientes = bd.clientes.ToList();
                return View(clientes);
            }
        }
    }
}