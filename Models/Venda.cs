using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tech_test_payment_api.Models
{
    public class Venda
    {   
        [Required]
        public int Id { get; set; }
        [Required]
        public Vendedor _vendedor { get; set; }
        [Required]
        public DateTime Data { get; set; }
        [Required]
        public List<Item> ListaItems { get; set; }
        [Required]
        public EnumStatusVenda StatusVenda { get; set; }
        
    }
}