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
                // Consultar os dados e projetar apenas o campo Name
                var resultados = entities.Locations
                    .Where(a => a.Name.ToLower().Contains(term.ToLower()))
                    .Select(a => a.Name) // Selecionando apenas o Name
                    .ToList();

                // Retornando a lista de nomes em formato JSON
                return Json(resultados);
            }
            catch (Exception ex)
            {
                // Caso ocorra algum erro, retornamos uma mensagem de erro
                return Json(new { error = "Ocorreu um erro ao processar a solicitação", message = ex.Message });
            }
        }
    }
}
