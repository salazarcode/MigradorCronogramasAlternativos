using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace Reader
{
    class Program
    {
        private static string connStr = "Server=192.168.200.248\\srvym;Database=VentaCartera;User Id=Uexterno2; Password=Ext3rn0201";
        private static string file = "result.csv";

        static void Main(string[] args)
        {
            TextReader reader = new StreamReader(file);
            List<Cronograma> cuotas = new List<Cronograma>();

            while (reader.Read() != -1)
            {
                string cadena = reader.ReadLine();
                string[] resArray = cadena.Split(";");

                Cronograma cro = new Cronograma();
                cro.CodigoCredito = resArray[0].Trim();
                cro.NumeroCuota = Convert.ToInt32(resArray[1].Trim());
                cro.FechaPago = Convert.ToDateTime(resArray[2].Trim());
                cro.Amortizacion = Convert.ToDouble(resArray[3].Trim());
                cro.Interes = Convert.ToDouble(resArray[4].Trim());
                cro.PeriodoGracia = Convert.ToDouble(resArray[5].Trim());
                cro.Encaje = 0; 
                cro.TotalCuota = Convert.ToDouble(resArray[7].Trim());

                cuotas.Add(cro);
            }

            cuotas.ForEach(x => { x.CodigoCredito = "122809"; });

            cuotas.ForEach(async x=> {
                try
                {            
                    using var conn = new SqlConnection(connStr);
                    conn.Open();
                    Dictionary<string, object> param = new Dictionary<string, object>();
                    param.Add("@nCodCred", x.CodigoCredito);
                    param.Add("@nNroCuota", x.NumeroCuota);
                    param.Add("@dFecPago", x.FechaPago);
                    param.Add("@amortizacion", x.Amortizacion);
                    param.Add("@interes", x.Interes);
                    param.Add("@periodoGracia", x.PeriodoGracia);
                    param.Add("@encaje", x.Encaje);
                    param.Add("@totalCuota", x.TotalCuota);

                    string query = "insert into dbo.CronogramasAlternativos values(@nCodCred, @nNroCuota, @dFecPago, @amortizacion, @interes, @periodoGracia, @encaje, @totalCuota)";
                    var res = await conn.ExecuteAsync(query, param);
                    Console.WriteLine(res);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            });
        }
        public class Cronograma
        {
            public string CodigoCredito { get; set; }
            public int NumeroCuota { get; set; }
            public DateTime FechaPago { get; set; }
            public double Amortizacion { get; set; }
            public double Interes { get; set; }
            public double PeriodoGracia { get; set; }
            public double Encaje { get; set; }
            public double TotalCuota { get; set; }
        }
    }
}
