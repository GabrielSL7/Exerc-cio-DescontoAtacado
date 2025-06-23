using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

class Produto
{
    public string GTIN { get; set; }
    public string Descricao { get; set; }
    public decimal PrecoVarejo { get; set; }
    public decimal? PrecoAtacado { get; set; }
    public int? QtdeAtacado { get; set; }
}

class ItemVenda
{
    public string GTIN { get; set; }
    public int Quantidade { get; set; }
    public decimal Parcial { get; set; }
}

class Program
{
    static void Main()
    {
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");

        var catalogo = new Dictionary<string, Produto>
        {
            ["7891024110348"] = new Produto { GTIN = "7891024110348", Descricao = "SABONETE OLEO DE ARGAN 90G PALMOLIVE", PrecoVarejo = 2.88m, PrecoAtacado = 2.51m, QtdeAtacado = 12 },
            ["7891048038017"] = new Produto { GTIN = "7891048038017", Descricao = "CHÁ DE CAMOMILA DR.OETKER", PrecoVarejo = 4.40m, PrecoAtacado = 4.37m, QtdeAtacado = 3 },
            ["7896066334509"] = new Produto { GTIN = "7896066334509", Descricao = "TORRADA TRADICIONAL WICKBOLD", PrecoVarejo = 5.19m },
            ["7891700203142"] = new Produto { GTIN = "7891700203142", Descricao = "BEBIDA SOJA MAÇÃ ADES", PrecoVarejo = 2.39m, PrecoAtacado = 2.38m, QtdeAtacado = 6 },
            ["7894321711263"] = new Produto { GTIN = "7894321711263", Descricao = "TODDY ACHOCOLATADO", PrecoVarejo = 9.79m },
            ["7896001250611"] = new Produto { GTIN = "7896001250611", Descricao = "ADOÇANTE LINEA", PrecoVarejo = 9.89m, PrecoAtacado = 9.10m, QtdeAtacado = 10 },
            ["7793306013029"] = new Produto { GTIN = "7793306013029", Descricao = "SUCRILHOS", PrecoVarejo = 12.79m, PrecoAtacado = 12.35m, QtdeAtacado = 3 },
            ["7896004400914"] = new Produto { GTIN = "7896004400914", Descricao = "COCO RALADO SOCOCO", PrecoVarejo = 4.20m, PrecoAtacado = 4.05m, QtdeAtacado = 6 },
            ["7898080640017"] = new Produto { GTIN = "7898080640017", Descricao = "LEITE UHT INTEGRAL ITALAC", PrecoVarejo = 6.99m, PrecoAtacado = 6.89m, QtdeAtacado = 12 },
            ["7891025301516"] = new Produto { GTIN = "7891025301516", Descricao = "DANONINHO", PrecoVarejo = 12.99m },
            ["7891030003115"] = new Produto { GTIN = "7891030003115", Descricao = "CREME DE LEITE MOCOCA", PrecoVarejo = 3.12m, PrecoAtacado = 3.09m, QtdeAtacado = 4 }
        };

        var vendas = new List<ItemVenda>
        {
            new ItemVenda { GTIN = "7891048038017", Quantidade = 1, Parcial = 4.40m },
            new ItemVenda { GTIN = "7896004400914", Quantidade = 4, Parcial = 16.80m },
            new ItemVenda { GTIN = "7891030003115", Quantidade = 1, Parcial = 3.12m },
            new ItemVenda { GTIN = "7891024110348", Quantidade = 6, Parcial = 17.28m },
            new ItemVenda { GTIN = "7898080640017", Quantidade = 24, Parcial = 167.76m },
            new ItemVenda { GTIN = "7896004400914", Quantidade = 8, Parcial = 33.60m },
            new ItemVenda { GTIN = "7891700203142", Quantidade = 8, Parcial = 19.12m },
            new ItemVenda { GTIN = "7891048038017", Quantidade = 1, Parcial = 4.40m },
            new ItemVenda { GTIN = "7793306013029", Quantidade = 3, Parcial = 38.37m },
            new ItemVenda { GTIN = "7896066334509", Quantidade = 2, Parcial = 10.38m }
        };

        var descontosPorProduto = new Dictionary<string, decimal>();
        var quantidadePorProduto = new Dictionary<string, int>();

        foreach (var venda in vendas)
        {
            if (!quantidadePorProduto.ContainsKey(venda.GTIN))
                quantidadePorProduto[venda.GTIN] = 0;

            quantidadePorProduto[venda.GTIN] += venda.Quantidade;
        }

        foreach (var gtin in quantidadePorProduto.Keys)
        {
            var produto = catalogo[gtin];
            var quantidade = quantidadePorProduto[gtin];

            if (produto.PrecoAtacado.HasValue && produto.QtdeAtacado.HasValue)
            {
                int pacotes = quantidade / produto.QtdeAtacado.Value;
                int resto = quantidade % produto.QtdeAtacado.Value;

                decimal valorVarejo = (pacotes * produto.QtdeAtacado.Value + resto) * produto.PrecoVarejo;
                decimal valorAtacado = pacotes * produto.QtdeAtacado.Value * produto.PrecoAtacado.Value + resto * produto.PrecoVarejo;

                decimal desconto = valorVarejo - valorAtacado;

                if (desconto > 0)
                    descontosPorProduto[gtin] = desconto;
            }
        }

        decimal subtotal = vendas.Sum(v => v.Parcial);
        decimal totalDescontos = descontosPorProduto.Values.Sum();
        decimal totalFinal = subtotal - totalDescontos;

        Console.WriteLine("\n--- Desconto no Atacado ---\n");
        Console.WriteLine("Descontos:");

        foreach (var d in descontosPorProduto.OrderBy(d => d.Key))
        {
            Console.WriteLine($"{d.Key,-20} R$ {d.Value:F2}");
        }

        Console.WriteLine($"\n(+) Subtotal  =    R$ {subtotal:F2}");
        Console.WriteLine($"(-) Descontos =      R$ {totalDescontos:F2}");
        Console.WriteLine($"(=) Total     =    R$ {totalFinal:F2}");
    }
}
