using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class View_Rutas_DTO
    {
        public int C_ { get; set; }
        public int ID_Cargamento { get; set; }
        public string cargamento { get; set; }
        public int ID_Direccion_Origen { get; set; }
        public string Origen { get; set; }
        public string Estado_Origen { get; set; }
        public int ID_Direccion_Destino { get; set; }
        public string Destino { get; set; }
        public string Estado_Destino { get; set; }
        public int ID_Chofer { get; set; }
        public string Chofer { get; set; }
        public int ID_Camion { get; set; }
        public string Camión { get; set; }
        public Nullable<System.DateTime> Salida { get; set; }
        public Nullable<System.DateTime> LLegada_Estimada { get; set; }
    
    }
}
