using Microsoft.AspNetCore.Mvc;

namespace StoreFront.UI.MVC.Controllers
{
    public class ErrorsController : Controller
    {
        public IActionResult Status(int id)
        {
            (int id, string message) error; //Tuple -> New version of an anonymous object.
            
            error.id = id;
            error.message = id switch
            {
                404 => "Page Not Found",
                500 => "Internal Server Error",
                _ => "Unknown Error"
            }; 

            return View(error);
        }
    }
}
