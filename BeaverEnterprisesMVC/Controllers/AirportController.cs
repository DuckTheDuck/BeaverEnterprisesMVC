using BeaverEnterprisesMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BeaverEnterprisesMVC.Controllers
{
    public class AirportController : Controller
    {

        
        BeaverEnterprisesContext entities = new BeaverEnterprisesContext();

        [HttpGet]
        public JsonResult GetSuggestions(string term)
        {
            try
            {
                
                var resultados = entities.Locations
                    .Where(a => a.Name.ToLower().StartsWith(term.ToLower()))
                    .Select(a => a.Name) 
                    .ToList();

                return Json(resultados);
            }
            catch (Exception ex)
            {
                
                return Json(new { error = "Ocorreu um erro ao processar a solicitação", message = ex.Message });
            }
        }
    }
}
