using System.Runtime.InteropServices;
using Microsoft.Data.Sqlite;

public class PresupuestosRepository
{
    private string cadenaDeConexion = "Data Source = Tienda.db;";

    public void crearPresupuesto(Presupuestos p)
    {
        using (var conexion = new SqliteConnection(cadenaDeConexion))
        {
            conexion.Open();
            string sql = "INSERT INTO Presupuestos (nombreDestinatario, fechaCreacion) VALUES (@nombre, @fecha);";

            using (SqliteCommand comando = new SqliteCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@nombre", p.NombreDestinatario);
                comando.Parameters.AddWithValue("@fecha", p.FechaCreacion);
                comando.ExecuteNonQuery();
            }

        }
    }

    public List<Presupuestos> GetAll()
    {
        List<Presupuestos> listaPresupuestos = new List<Presupuestos>();

        using (SqliteConnection conexion = new SqliteConnection(cadenaDeConexion))
        {
            conexion.Open();

            string sql = "SELECT * FROM Presupuestos";

            using (SqliteCommand comando = new SqliteCommand(sql, conexion))
            using (SqliteDataReader lector = comando.ExecuteReader())
            {
                while (lector.Read())
                {
                    var p = new Presupuestos
                    (
                        Convert.ToInt32(lector["idPresupuesto"]),
                        lector["nombreDestinatario"].ToString(),
                        Convert.ToDateTime(lector["fechaCreacion"]),
                        new List<PresupuestoDetalles>()
                    );
                    listaPresupuestos.Add(p);
                }
            }
        }
        return listaPresupuestos;
    }

    public Presupuestos GetById(int id)
    {
        Presupuestos presupuestos = null;

        using (SqliteConnection conexion = new SqliteConnection())
        {
            conexion.Open();

            string sql = "SELECT * FROM Presupuestos WHERE idPresupuesto = @id";

            using (SqliteCommand comando = new SqliteCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@id", id);

                using (SqliteDataReader lector = comando.ExecuteReader())
                {
                    if (lector.Read())
                    {
                        presupuestos = new Presupuestos
                        (
                            Convert.ToInt32(lector["idPresupuesto"]),
                            lector["nombreDestinatario"].ToString(),
                            Convert.ToDateTime(lector["fechaCreacion"]),
                            new List<PresupuestoDetalles>()
                        );
                    }
                }
            }
            if (presupuestos != null)
            {
                string sqlDetalle = @"
                    SELECT pd.cantidad, pr.idProducto, pr.descripcion, pr.precio FROM PresupuestosDetalle pd
                    INNER JOIN Productos pr ON pd.ipProducto = pr.idProducto
                    WHERE pd.idPresupuesto = @id";

                using (SqliteCommand comando = new SqliteCommand(sqlDetalle, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    using (SqliteDataReader lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            var producto = new Productos(
                                Convert.ToInt32(lector["idProducto"]),
                                lector["descripcion"].ToString(),
                                Convert.ToInt32(lector["precio"])
                            );
                            var detalle = new PresupuestoDetalles(producto, Convert.ToInt32(lector["cantidad"]));
                            presupuestos.Detalles.Add(detalle);
                        }
                    }
                }
            }
        }
        return presupuestos;
    }

    public void AgregarProducto(int idPresupuesto, int idProducto, int cantidad)
    {
        using (SqliteConnection conexion = new SqliteConnection(cadenaDeConexion))
        {
            conexion.Open();

            string sql = "INSERT INTO PresupuestoDetalle (idPresupuesto, idProducto, cantidad) VALUES (@idPresupuesto, @idProducto, @cantidad)";

            using (SqliteCommand comando = new SqliteCommand(sql, conexion))
            {
                comando.Parameters.AddWithValue("@idPresupuesto", idPresupuesto);
                comando.Parameters.AddWithValue("@idProducto", idProducto);
                comando.Parameters.AddWithValue("@cantidad", cantidad);
                comando.ExecuteNonQuery();
            }
        }
    }

    public bool Eliminar(int id)
    {
        using (SqliteConnection conexion = new SqliteConnection(cadenaDeConexion))
        {
            conexion.Open();

            string sqlDetalle = "DELETE FROM PresupuestoDetalle WHERE idPresupuesto = @id";

            using (SqliteCommand comando = new SqliteCommand(sqlDetalle, conexion))
            {
                comando.Parameters.AddWithValue("@id", id);
                comando.ExecuteNonQuery();
            }

            string sqlPresupuesto = "DELETE FROM Presupuesto WHERE idPresupuesto = @id";

            using (SqliteCommand comando = new SqliteCommand(sqlPresupuesto, conexion))
            {
                comando.Parameters.AddWithValue("@id", id);
                int filasAfectadas = comando.ExecuteNonQuery();
                return filasAfectadas > 0;
            }
        }
    }
}

