using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")] 

public class ProductosController : ControllerBase
{
    private readonly ProductoRepository _repository = new ProductoRepository();

    [HttpPost]
    public IActionResult CrearProducto([FromBody] Productos producto)
    {
        _repository.Alta(producto);
        return Ok("Producto creado correctamente");
    }

    [HttpPut("{id}")]
    public IActionResult ModificarProducto(int id, [FromBody] Productos producto)
    {
        bool actualizado = _repository.Modificar(id, producto);
        if (actualizado)
        {
            return Ok("Producto actualizado correctamente");
        }
        else
        {
            return NotFound("No se encontro el producto para actualizar");
        }
    }

    [HttpGet]
    public IActionResult ObtenerProductos()
    {
        var lista = _repository.GetAll();
        return Ok(lista);
    }

    [HttpGet("id")]
    public IActionResult ObtenerProductoID(int id)
    {
        var producto = _repository.GetById(id);
        if (producto != null)
        {
            return Ok(producto);
        }
        return NotFound("Producto no encontrado");
    }

    [HttpDelete("{id}")]
    public IActionResult EliminarProducto(int id)
    {
        bool eliminado = _repository.Eliminar(id);
        if (eliminado)
        {
            return Ok("Producto eliminado correctamente");
        }
        return NotFound("No se encontro el producto para eliminar");
    }
}
