using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Web1_abr13.Models
{
    public class Aluno
    {
        [Key]
        public int Num { get; set; }
        [Required(ErrorMessage = "Campo Obrigatório")]
        public String Nome { get; set; }
        public String Turma { get; set; }
    }

    public class BD : IDisposable
    {
        private List<Aluno> alunos = new List<Aluno>();

        public BD()
        {
            carregar();
        }

        public List<Aluno> getAlunos() => alunos.ToList<Aluno>();

        public bool EditarAluno(Aluno editado)
        {
            try
            {
                Aluno este = alunos.Where(a => a.Num == editado.Num).FirstOrDefault();
                if (este != null)
                {
                    este.Nome = editado.Nome;
                    este.Turma = editado.Turma;
                    guardar();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool InserirAluno(Aluno novo)
        {
            try
            {
                alunos.Add(novo);
                guardar();
                return true;
            }
            catch (Exception erro)
            {

                return false;
            }
        }

        public bool ApagarAluno(Aluno morto)
        {
            try
            {
                Aluno este = alunos.Where(a => a.Num == morto.Num).FirstOrDefault();
                if (este != null)
                {
                    alunos.Remove(este);
                    guardar();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void carregar() 
        {
            HttpServerUtility server = HttpContext.Current.Server;
            string path = server.MapPath("~/App_Data/alunos.json");

            if (System.IO.File.Exists(path)) 
            {
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                DataContractJsonSerializer dc = new DataContractJsonSerializer(typeof(List<Aluno>));
                alunos = dc.ReadObject(fs) as List<Aluno>;
                if(alunos == null || alunos.Count() <= 0) alunos = new List<Aluno>();
                fs.Close();
            }
        }

        public void guardar() 
        {
            HttpServerUtility server = HttpContext.Current.Server;
            string path = server.MapPath("~/App_Data/alunos.json");
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            DataContractJsonSerializer dc = new DataContractJsonSerializer(typeof(List<Aluno>));
            dc.WriteObject(fs, alunos);
            fs.Close();
        }

        #region dispose
        private bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                alunos.Clear();
                alunos = null;
            }

            _disposed = true;
            
        }
        #endregion

    }

}