using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using Web1_abr13.Models;

namespace Web1_abr13.Controllers
{
    public class IstecController : Controller
    {
        public ActionResult EditAluno(int? num)
        {
            using (BD bd = new BD())
            {
                List<String> turmaslista = new List<String>() { "A", "B", "C" };
                ViewBag.turmas = new SelectList(turmaslista);

                int n = num ?? -1;
                Aluno editado = bd.getAlunos().Find(a => a.Num == n);
                if (editado != null)
                {
                    return View(editado);
                }
                else
                {
                    return RedirectToAction("Alunos", "Istec", new { message = "Aluno não existe" });
                }
            }
        }
        [HttpPost]
        public ActionResult EditAluno(Aluno este)
        {
            using (BD bd = new BD())
            {
                bd.EditarAluno(este);
                return RedirectToAction("Alunos", "Istec", new { message = "Editado com sucesso" });
            }
        }

        public ActionResult DeleteAluno(int? num)
        {
            using (BD bd = new BD())
            {
                Aluno morto = bd.getAlunos().Where(a => a.Num == (num ?? -1)).FirstOrDefault();
                if (morto != null)
                {
                    return View(morto);
                }
                else return RedirectToAction("Alunos", "Istec", new { message = "Aluno não existe" });
            }
        }
        [HttpPost]

        [ActionName("DeleteAluno")]
        public ActionResult Delete(int? num)
        {
            using (BD bd = new BD())
            {
                Aluno morto = bd.getAlunos().Where(a => a.Num == (num ?? -1)).FirstOrDefault();
                if (morto != null)
                {
                    bd.ApagarAluno(morto);
                    return RedirectToAction("Alunos", "Istec", new { message = "Aluno eliminado" });
                }
                else return RedirectToAction("Alunos", "Istec", new { message = "Aluno não existe" });
            }
        }

        public ActionResult Create()
        {
            using (BD bd = new BD())
            {
                int novoid = bd.getAlunos().Count() > 0 ? bd.getAlunos().Max(x => x.Num) + 1 : 1;
                Aluno novo = new Aluno() { Num = novoid };
                return View(novo);
            }
        }
        [HttpPost]
        public ActionResult Create(Aluno novo)
        {
            using (BD bd = new BD())
            {
                bd.InserirAluno(novo);
                return RedirectToAction("Alunos", "Istec");
            }
        }

        public ActionResult Alunos(string message)
        {
            ViewBag.message = message;
            using (BD bd = new BD())
            {
                List<Aluno> lista = bd.getAlunos().ToList<Aluno>();
                return View(lista);
            }
        }

        public ActionResult Dobro(int? num)
        {
            int rslt = (num ?? 0) * 2;
            return Json(new { resultado = rslt }, JsonRequestBehavior.AllowGet);
        }
        // GET: Istec
        public ActionResult Eco()
        {
            ViewBag.titulo = Request.QueryString["tit"];
            return View();
        }

        [HttpPost]
        public ActionResult Eco(int? numero)
        {
            //Request.Form["numero"]
            //Request.QueryString["x"]

            ViewBag.resultado = (numero ?? 0) * 2;
            return View();
        }
    }
}