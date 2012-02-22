using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RAI.Modelos
{
    class Program
    {
        static void Main(string[] args)
        {
            // Tutorial de LINQ: http://www.codeproject.com/KB/linq/UnderstandingLINQ.aspx
			// http://www.codeproject.com/KB/vista/LINQ_1.aspx

            Tokenizer tok = new Tokenizer();

            // Indizar la colección
            Indice ind = new Indice(tok);
            ind.Build(Path.GetFullPath("../../Documentos"));
            Console.WriteLine(ind.IDF.Count + " términos en " + ind.NumDocuments + " documentos");

            // Instanciar el modelo
            IModelo mod = new ProductoEscalarTF();

            // Ejecutar consultas
            bool salir = false;
            while (!salir) {
                Console.Write("Query: ");
                string query = Console.ReadLine();
                
                if (string.IsNullOrEmpty(query)) {
                    // Si no se indica nada, terminar el programa
                    salir = true;
                }else{
                    // Sí hay términos de consulta. Ejecutar la query
                    Dictionary<string, double> resultados = mod.RunQuery(tok.Tokenize(query), ind);
                    
                    foreach (var resultado in resultados.OrderByDescending(r => r.Value)) { // LINQ: ordena los pares (documento, similitud) por orden descendiente de similitud
                        Console.WriteLine("\t" + resultado.Key + "\t" + resultado.Value.ToString("0.0000"));
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}
