//------------------------------------------------------------------------------
// <auto-generated>
//    Este código se generó a partir de una plantilla.
//
//    Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//    Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Drako_Facturacion.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tax
    {
        public tax()
        {
            this.producto_tax = new HashSet<producto_tax>();
        }
    
        public int id { get; set; }
        public Nullable<System.DateTime> date_create { get; set; }
        public Nullable<int> idState { get; set; }
        public string names { get; set; }
        public string Typess_ambit { get; set; }
        public string Typess { get; set; }
        public Nullable<decimal> tasa { get; set; }
        public string claveImpuesto { get; set; }
        public string tipo_factor { get; set; }
        public string tasaOCuota { get; set; }
        public Nullable<int> idUser { get; set; }
    
        public virtual cstate cstate { get; set; }
        public virtual ICollection<producto_tax> producto_tax { get; set; }
        public virtual users users { get; set; }
    }
}
