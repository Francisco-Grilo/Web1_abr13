using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web1_abr13.Models;

namespace Web1_abr13.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Bravo()
        {
            ViewBag.escola = "Istec";
            ViewData["disciplina"] = "Tecnologia Internet III";
            TempData["professor"] = "José Neves";

            Aluno ze = new Aluno() { Num = 1, Nome = "Zé Carioca", Turma = "A"};

            return View(ze);
        }

        [HttpPost]
        public ActionResult Bravo(int ? numero)
        {
            ViewBag.escola = "Istec";
            ViewData["disciplina"] = "Tecnologia Internet III";
            TempData["professor"] = "José Neves";

            Aluno ze = new Aluno() { Num = 1, Nome = "Zé Carioca", Turma = "A" };
            
            ViewBag.numero = numero;
            ViewBag.result = (numero ?? 0) * 2;


            return View(ze);
        }

        public ActionResult Alpha()
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}