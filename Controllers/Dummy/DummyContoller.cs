using Microsoft.AspNetCore.Mvc;
using Models;
using Sprache;
using System.Text;

namespace Controllers;

[ApiController]
[Route("/")]
public class DummyController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        string DummyOut = "I am a dummy controller for creating a root.";
        return base.Content(DummyOut, "text/html", Encoding.UTF8);
    }
}