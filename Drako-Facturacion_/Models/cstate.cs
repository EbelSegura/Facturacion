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
    
    public partial class cstate
    {
        public cstate()
        {
            this.customer = new HashSet<customer>();
            this.product = new HashSet<product>();
            this.tax = new HashSet<tax>();
        }
    
        public int id { get; set; }
        public string names { get; set; }
    
        public virtual ICollection<customer> customer { get; set; }
        public virtual ICollection<product> product { get; set; }
        public virtual ICollection<tax> tax { get; set; }
        public virtual users users { get; set; }
    }
}
