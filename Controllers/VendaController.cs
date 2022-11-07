using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using tech_test_payment_api.Models;
using System.ComponentModel.DataAnnotations;
using tech_test_payment_api.Context;
using Microsoft.EntityFrameworkCore;

namespace tech_test_payment_api.Controllers
{
    [ApiController]
    [Route("api-docs/[controller]")]
    public class VendaController : ControllerBase
    {

        private readonly VendaContext _context;
        public VendaController(VendaContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Cria nova venda
        /// </summary>
        /// <param name="venda"></param>
        /// <returns></returns>
        /// <remarks>
        /// Toda nova venda inicia com o status Aguardando Pagamento <br/>
        /// <br/>
        /// *Todos os campos são obrigatórios
        ///
        /// </remarks>
        [HttpPost]
        public IActionResult Create(Venda venda)
        {
            venda.StatusVenda = 0;
            _context.Add(venda);
            _context.SaveChanges();

            return Ok(venda);
        }

        /// <summary>
        /// Pesquisa venda pelo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Procurar{id}")]
        public async Task<ActionResult<Venda>> Procurar(int id)
        {
        
            var venda = await _context.Vendas
             .Include(i=> i._vendedor)
             .Where(i=> i._vendedor.Id == id)
             .Include(i=> i.ListaItems)
             .Where(i=> i.Id == id)
             .FirstOrDefaultAsync();            

            return Ok(venda);
        }


        /// <summary>
        /// Atualiza o status da venda
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        /// <remarks>
        /// Status só podem ser atualizados da seguinte forma: <br/>
        /// <br/>
        /// De status Aguardando Pagamento para status Pagamento Aprovado ou
        /// status Cancelada <br/>
        /// De status Pagamento Aprovado para status Enviado para Transportadora
        /// ou status Cancelada <br/>
        /// De status Enviado para Transportadora para status Entregue <br/>
        ///
        /// </remarks>
        [HttpPut("Atualizar{id}")]
        public async Task<ActionResult<Venda>> Atualizar(int id, EnumStatusVenda status)
        {
        
            var venda = await _context.Vendas
             .Include(i=> i._vendedor)
             .Where(i=> i._vendedor.Id == id)
             .Include(i=> i.ListaItems)
             .Where(i=> i.Id == id)
             .FirstOrDefaultAsync();

             if (venda.StatusVenda == EnumStatusVenda.AguardandoPagamento && (status == EnumStatusVenda.PagamentoAprovado || status == EnumStatusVenda.Cancelada))
             {
                venda.StatusVenda = status;
            }
            else if (venda.StatusVenda == EnumStatusVenda.PagamentoAprovado && (status == EnumStatusVenda.EnviadoParaTransportadora || status == EnumStatusVenda.Cancelada))
            {
                venda.StatusVenda = status;
            }
            else if (venda.StatusVenda == EnumStatusVenda.EnviadoParaTransportadora && status == EnumStatusVenda.Entregue)
            {
                venda.StatusVenda = status;
            }
            else 
            {
                return BadRequest(new { Erro = "Transição não permitida, verifique quais transições de status são permitidas e tente novamente" });
            }
             

            _context.Vendas.Update(venda);
            _context.SaveChanges();

            return Ok(venda);
        }
        
    }
}