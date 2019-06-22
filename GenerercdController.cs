using CIS.Business.Entity;
using CIS.Business.Service.Impl;
using CIS.Business.Shared;
using GEME.Business.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SH.Api.Infra;
using SH.Business.Service;
using Microsoft.EntityFrameworkCore;
using SH.Kernel.Web;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics;

namespace CIS.Api.Controllers
{
    [Produces("application/json")]
    [Route(WebConsts.Urls.API_V1_TEMPLATE)]
    [Authorize]
    [AllowAnonymous]
    public class GenerercdController : SHServiceController<IAgreementPrincipleService>
    {
        private readonly CisDbContext _db;
        IHostingEnvironment _env;

        public GenerercdController(ControllerProvider provider,
            CisDbContext db,
            IAgreementPrincipleService ser, IHostingEnvironment env) : base(ser, provider)
        {
            this._db = db;
            _env = env;
        }



        /// <summary>
        /// adds an item
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("{id}/expatriers")]
        public async Task<IActionResult> GetExpatriers(int id)
        {
            var result = new List<ExportExpatrierModel>();

            if (ClaimHelper.GetUserScope(HttpContext.User) == GEMOEScope.DEW)
            {
               // var employerId = ClaimHelper.GetEmployerId(HttpContext.User);

                var listeExpatrier = this._db.TitlesOfWork
                          .Include(e => e.PasseportIssuePlace)
                          .Include(e => e.Employee)
                          .Include(e => e.Profile)
                          .Include(e => e.Profile.Position)
                          .Include(e => e.Profile.AgreementInPrinciple)
                          .Include(e => e.Profile.AgreementInPrinciple.Project)
                          .Include(e => e.Profile.AgreementInPrinciple.Employer)
                          .Include(e => e.Profile.AgreementInPrinciple.Employer.ActivityNature)
                          // .Where( e=> e.Profile.AgreementInPrinciple.EmployerId == employerId)
                          .Where(e => e.Profile.AgreementInPrinciple.Id == id)
                          .ToList();

                //// var dewId = ClaimHelper.GetUserId(HttpContext.User);
                //  var dewWilayaId = ClaimHelper.GetWilayaId(HttpContext.User);

                result =
                     listeExpatrier.Select(e => new ExportExpatrierModel()
                     {
                         nom_orga = e.Profile.AgreementInPrinciple.Employer.CompanyNameLt,
                         Projet = e.Profile.AgreementInPrinciple.Project.NameLt,
                         Nom = e.Employee.FirstNameLt,
                         Prenom = e.Employee.LastNameLt,
                         Date_nais = e.Employee.BirthDate,
                         Date_entrer_alg = e.DateOfEntryInAlgeria,
                         Sexe = e.Employee.Gender.ToString(),
                         Nation = e.Employee.Nationality.Name,
                         Situation_familiale = e.Employee.FamilySituation.ToString(),
                         Adrs_alg = e.AddressInAlgeriaAr,
                         Lieu_emploi = e.Profile.AgreementInPrinciple.Project.LocationLt,
                         Brut = Convert.ToString(e.GrossSalary),
                         Net = Convert.ToString(e.NetSalary),
                         I_avantage = e.OtherAdvantages,
                         Refugie_politique = Convert.ToString(e.Employee.PoliticalRefugee),
                         Apatride = Convert.ToString(e.Employee.Stateless),
                         Passeport_num = e.PasseportNumber,
                         Passeport_date_debut = e.PasseportIssueDate,
                         Passeport_date_fin = e.PasseportEndDate,
                         Lieu_nais = e.Employee.BirthPlaceLt,
                         Qualification = e.Employee.Qualifications,
                         Emploi_objet = e.Profile.Position.Label,
                         Duree = e.Duration,
                         Ss_alg = e.Employee.AlgerianSSN,
                         Ss_etranger = e.Employee.AbroadSSN,
                         Nom_pere = e.Employee.FatherFirstNameLt,
                         Nom_mere = e.Employee.MotherFirstNameLt,
                         Exper = e.Employee.Experiences,
                         DemandeType = e.Employee.DemandeType.ToString(),
                         Pays = e.PasseportIssuePlace.Name,
                         Nature_activite = e.Profile.AgreementInPrinciple.Employer.ActivityNature.Name



                     }).ToList();
            }


                return Json(result);
            


        }


        /// <summary>
        /// adds an item
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("{id}/accord")]
        public async Task<IActionResult> GetAccord(int id)
        {
            var result = new List<ExportAccordModel>();

            if (ClaimHelper.GetUserScope(HttpContext.User) == GEMOEScope.DEW)
            {

                //   int? employerId = 1035;

                var listeAccord = this._db.Profiles
                      .Include(e => e.AgreementInPrinciple)
                      .Include(e => e.Position)
                      .Include(e => e.AgreementInPrinciple.Project)
                      .Include(e => e.AgreementInPrinciple.Employer)
                      //    .Where(e => e.AgreementInPrinciple.EmployerId == employerId)
                      .Where(e => e.AgreementInPrinciple.Id == id)
                      .ToList();

                //// var dewId = ClaimHelper.GetUserId(HttpContext.User);
                //  var dewWilayaId = ClaimHelper.GetWilayaId(HttpContext.User);

                result =
                     listeAccord.Select(e => new ExportAccordModel()
                     {
                         nom_orga = e.AgreementInPrinciple.Employer.CompanyNameLt,
                         Projet = e.AgreementInPrinciple.Project.NameLt,
                         poste = e.Position.Label,
                         Diplome = e.Degree,
                         experience = e.Experiences,
                         autre_condition = e.OtherConditions,
                         Algériens = e.NumberOfGrantedAlgerianEmployers,
                         étrangers = e.NumberOfGrantedForeignEmployers


                     }).ToList();
            }

                return Json(result);
            

        }

        /// <summary>
        /// adds an item
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("{id}/employeur")]
        public async Task<IActionResult> GetEmployeur(int id)
        {
            var result = new List<ExportEmployeurModel>();

            if (ClaimHelper.GetUserScope(HttpContext.User) == GEMOEScope.DEW)
            {

                // int? employerId = 1035;

                var listeEmployeur = this._db.AgreementsInPrinciple
                      .Include(e => e.Project)
                      .Include(e => e.Employer)
                      .Include(e => e.Employer.NativeCountry)
                      .Include(e => e.Employer.LegalSector)
                      .Include(e => e.Employer.LegalStatus)
                      //  .Where(e => e.EmployerId == employerId)
                      .Where(e => e.Id == id)
                      .ToList();

                //// var dewId = ClaimHelper.GetUserId(HttpContext.User);
                //  var dewWilayaId = ClaimHelper.GetWilayaId(HttpContext.User);

                result =
                     listeEmployeur.Select(e => new ExportEmployeurModel()
                     {
                         nom_orga = e.Employer.CompanyNameLt,
                         Projet = e.Project.NameLt,
                         Adrs_orga = e.Employer.AdressInAlgeriaLt,

                         Nation_orga = e.Employer.NativeCountry.Name,
                         Nom_raison_sc_orga = e.Employer.LegalSector.Name,
                         Statut_juridique = e.Employer.LegalStatus.Name,
                         Accord = e.CodeNumber,
                         Nb_et = e.Employer.NumberOfForeignEmployees,
                         Nb_al = e.Employer.NumberOfAlgerianEmployees,
                         Adrs_etranger = e.Employer.AdressAbroadLt

                     }).ToList();
            }

            return Json(result);

        }


        /// <summary>
        /// adds an item
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("{id}/contact")]
        public async Task<IActionResult> GetContact(int id)
        {
            var result = new List<ExportContactModel>();

            if (ClaimHelper.GetUserScope(HttpContext.User) == GEMOEScope.DEW)
            {

                // int? employerId = 1035;

                var listeContact = this._db.AgreementsInPrinciple
                      .Include(e => e.Project)
                      .Include(e => e.Project.Representative)
                      .Include(e => e.Project.Representative.Position)
                      .Include(e => e.Employer)
                      .Include(e => e.Employer.NativeCountry)
                      .Include(e => e.Employer.LegalSector)
                      .Include(e => e.Employer.LegalStatus)
                      .Include(e => e.Employer.ApplicableRight)
                      //  .Where(e => e.EmployerId == employerId)
                      .Where(e => e.Id == id)
                      .ToList();

                //// var dewId = ClaimHelper.GetUserId(HttpContext.User);
                //  var dewWilayaId = ClaimHelper.GetWilayaId(HttpContext.User);

                result =
                     listeContact.Select(e => new ExportContactModel()
                     {
                         nom_orga = e.Employer.CompanyNameLt,
                         Projet = e.Project.NameLt,
                         Tel = e.Employer.Tel1,

                         Fax = e.Employer.Fax,
                         email = e.Employer.Email,
                         Nom_c = e.Project.Representative.FirstNameLt,
                         Prenom_c = e.Project.Representative.LastNameLt,
                         date_nais_c = e.Project.Representative.BirthDate,
                         Lieu_nais_c = e.Project.Representative.BirthPlace,
                         Poste = e.Project.Representative.Position.Label,
                         Secteur_juridique = e.Employer.LegalSector.Name,
                         droit_applicable = e.Employer.ApplicableRight.Name,
                         date_installation = e.Employer.InstallationDate,

                         Num_registre = e.Employer.BusinessRegisterNumber,
                         date_registre = e.Employer.BRNDate,
                         Num_SS = e.Employer.SocialSecurityNumber,
                         Num_fiscal = e.Employer.TaxRegistrationNumber,



                     }).ToList();
            }

                           
                return Json(result);
            

        }


        /// <summary>
        /// adds an item
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCDEmployer(int id)
        {
            var nomfich = "";
            var telecharger = false;
            var ap = await this._db.AgreementsInPrinciple.FindAsync(id);

            //   if (ClaimHelper.GetUserScope(HttpContext.User) == GEMOEScope.DEW)
            //  {

            try
             {
                
               
                 //   using (var transaction = this._db.Database.BeginTransaction())
                  //  {
                       

                Process process = new Process();
                process.StartInfo.WorkingDirectory = Path.Combine(GetRootPath(), @"Content");
                process.StartInfo.FileName = Path.Combine(GetRootPath(), @"Content\CreateCDEmployer.exe");
                nomfich = process.StartInfo.FileName;


                //  process.StartInfo.FileName = Path.Combine("D:/ProjDelphi010619/CreateCDEmployer.exe","");
               
                process.StartInfo.Arguments = ap.CodeNumber;

                process.StartInfo.RedirectStandardOutput = true; 
               process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardError = true;

                process.Start();
                
                process.Close();

                telecharger = true;
                //         // attendre 7 secondes avant lancement du telechargement
                //         await Task.Delay(7000);


                //         // downlowding file created
                //         // pour récupérer le nom de l'employeur
                //         var emp = await this._db.AgreementsInPrinciple
                //                                 .Include(e => e.Employer)
                //                                 //   .Include(e=>e.Project)
                //                                 .FirstOrDefaultAsync(e => e.Id == id);

                //         var filename = emp.Employer.CompanyNameLt + " - " + DateTime.Today.ToString("dd-MM-yyyy") + ".zip";
                //         //  return Ok(filename);
                //         if (filename == null)
                //             return Content("filename not present");

                //         // recupérer le chemin de la base paradox créée
                //         var path = Path.Combine(
                //                      // Directory.GetCurrentDirectory(),
                //                      @"C:\\inetpub\\wwwroot\\EmpFolder\\",
                //                      //GetRootPath(),"temp\\",

                //                     filename);

                //         var memory = new MemoryStream();
                //         using (var stream = new FileStream(path, FileMode.Open))
                //         {
                //             await stream.CopyToAsync(memory);
                //         }

                //         // confirmer la transaction
                //      //   transaction.Commit();

                //         // retourner le fichier aprés la confiramati de la transaction
                //         memory.Position = 0;
                //       return File(memory, GetContentType(path), Path.GetFileName(path));
                //     //return Ok(path);

                //// }


            }
                catch (Exception ex)
                {
                //return Ok(ex.Message);
                telecharger = false;
                }

            return Json(ap.CodeNumber+" "+ telecharger.ToString()+" "+nomfich+"  "+GetRootPath());
        }


       

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats officedocument.spreadsheetml.sheet"},  
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"},
                {".zip", "zip/rar"}
            };
        }

        private string GetRootPath()
        {

            if (!_env.IsDevelopment())
            {
                // return @"h:\root\home\thanos-001\www\geme\api";
                return @"C:\inetpub\wwwroot\gemoe-api";
            }

            if (string.IsNullOrWhiteSpace(_env.ContentRootPath))
            {
                // return Path.Combine(Directory.GetCurrentDirectory());
                return @"C:\inetpub\wwwroot\gemoe-api";
            }
            else
            {
                return _env.ContentRootPath;
            }
        }





    }
}