using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PresupuestosController : ControllerBase
{
    private readonly PresupuestosRepository _repository = new PresupuestosRepository();

    [HttpPost]
    public IActionResult CrearPresuesto([FromBody] Presupuestos presupuesto)
    {
        _repository.crearPresupuesto(presupuesto);
        return Ok("Presupuesto creado correctamente");
    }

}