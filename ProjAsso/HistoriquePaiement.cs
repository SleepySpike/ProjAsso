//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProjAsso
{
    using System;
    using System.Collections.Generic;
    
    public partial class HistoriquePaiement
    {
        public int IdHistoriquePaiement { get; set; }
        public int IdAdherent { get; set; }
        public int IdAssociation { get; set; }
        public decimal Paiement { get; set; }
        public System.DateTime Date { get; set; }
    
        public virtual Adherent Adherent { get; set; }
        public virtual Association Association { get; set; }
    }
}