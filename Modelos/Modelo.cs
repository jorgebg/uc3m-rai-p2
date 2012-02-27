using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RAI.Modelos
{
    public interface IModelo
    {
        /// <summary>
        /// Ejecuta la consulta para los términos de la query especificados.
        /// </summary>
        /// <param name="queryTerms">Los términos de la query.</param>
        /// <param name="ind">El índice de la colección.</param>
        /// <returns>Un diccionario de pares (d, s) con la similitud s de cada documento d para la query indicada.</returns>
        Dictionary<string, double> RunQuery(string[] queryTerms, Indice ind);
    }

    /// <summary>
    /// Implementa la función de similitud del producto escalar con los pesos según TF.
    /// </summary>
    public class ProductoEscalarTF : IModelo
    {
        virtual public Dictionary<string, double> RunQuery(string[] queryTerms, Indice ind)
        {
            Dictionary<string, double> resultados = new Dictionary<string, double>();

            var grupos = queryTerms.GroupBy(t => t); // LINQ: Devuelve un enumerado de pares <término, ocurrencias>
            
            // Sólo hace falta iterar los términos de la consulta.
            // Los que no estén tienen peso 0 así que no contribuyen al sumatorio.
            foreach (var g in grupos) {
                Dictionary<string, int> TFs;

                if (ind.TF.TryGetValue(g.Key, out TFs)) {
                    // El término de la query está en el índice. Si no está no se hace nada porque no contribuiría al sumatorio al tener peso 0.
                    foreach (var docTF in TFs) {
                        double score;
                        if (resultados.TryGetValue(docTF.Key, out score)) {
                            // El documento ya está en los resultados, incrementar
                            resultados[docTF.Key] = score + docTF.Value * g.Count();
                        } else {
                            // El documento no está en los resultados
                            resultados[docTF.Key] = docTF.Value * g.Count();
                        }
                    }
                }
            }

            return resultados;
        }
    }
    /// <summary>
    /// Implementa la función de similitud del producto escalar con los pesos según TFxIDF.
    /// </summary>
    public class ProductoEscalarTFIDF : IModelo
    {
        virtual public Dictionary<string, double> RunQuery(string[] queryTerms, Indice ind)
        {
            Dictionary<string, double> resultados = new Dictionary<string, double>();
			
            var grupos = queryTerms.GroupBy(t => t); // LINQ: Devuelve un enumerado de pares <término, ocurrencias>
            
            
            // Sólo hace falta iterar los términos de la consulta.
            // Los que no estén tienen peso 0 así que no contribuyen al sumatorio.
            foreach (var g in grupos) {
				int idf;
                Dictionary<string, int> TFs;
				ind.IDF.TryGetValue(g.Key, out idf);
				
                if (ind.TF.TryGetValue(g.Key, out TFs)) {
                    // El término de la query está en el índice. Si no está no se hace nada porque no contribuiría al sumatorio al tener peso 0.
                    foreach (var docTF in TFs) {
                        double score;
						double score_incr = docTF.Value * g.Count() * idf;
                        if (resultados.TryGetValue(docTF.Key, out score)) {
                            // El documento ya está en los resultados, incrementar
                            resultados[docTF.Key] = score + score_incr;
                        } else {
                            // El documento no está en los resultados
                            resultados[docTF.Key] = score_incr;
                        }
                    }
                }
            }
            
            return resultados;
        }
    }
    /// <summary>
    /// Implementa la función de similitud del coseno con los pesos según TF.
    /// </summary>
    public class CosenoTF : ProductoEscalarTF
    {
        override public Dictionary<string, double> RunQuery(string[] queryTerms, Indice ind)
        {
			
            Dictionary<string, double> resultados = new Dictionary<string, double>();

            resultados = base.RunQuery(queryTerms, ind);
			
            var grupos = queryTerms.GroupBy(t => t); // LINQ: Devuelve un enumerado de pares <término, ocurrencias>
            
            double queryProduct = 0;
            foreach (var g in grupos) {
				queryProduct += g.Count();
			}
			
			Dictionary<string, double> documentProducts = new Dictionary<string, double>();
			foreach (var TF in ind.TF) {
				foreach(var document in TF.Value) {
					double product = 0;
					documentProducts.TryGetValue(document.Key, out product);
					documentProducts[document.Key] = product + document.Value*document.Value;
				}
			}
			
			foreach(var product in documentProducts) {
				double score = 0;
				if(resultados.TryGetValue(product.Key, out score)) {
					//Console.WriteLine(product.Key + ": " + score + "/sqrt(" + product.Value + "*" + queryProduct+") = "+score+"/"+Math.Sqrt(product.Value*queryProduct)+" = "+(score/Math.Sqrt(product.Value*queryProduct)));
					resultados[product.Key] = score / Math.Sqrt(product.Value*queryProduct);
				}
			}
			
			
            return resultados;
			
        }
    }
    /// <summary>
    /// Implementa la función de similitud del coseno con los pesos según TFxIDF.
    /// </summary>
    public class CosenoTFIDF : ProductoEscalarTFIDF
    {
        override public Dictionary<string, double> RunQuery(string[] queryTerms, Indice ind)
        {
            Dictionary<string, double> resultados = new Dictionary<string, double>();

            resultados = base.RunQuery(queryTerms, ind);
			
            var grupos = queryTerms.GroupBy(t => t); // LINQ: Devuelve un enumerado de pares <término, ocurrencias>
            
            double queryProduct = 0;
            foreach (var g in grupos) {
				queryProduct += g.Count();
			}
			
			Dictionary<string, double> documentProducts = new Dictionary<string, double>();
			foreach (var TF in ind.TF) {
				foreach(var document in TF.Value) {
					double product = 0;
					documentProducts.TryGetValue(document.Key, out product);
					documentProducts[document.Key] = product + document.Value*document.Value;
				}
			}
			
			foreach(var product in documentProducts) {
				double score = 0;
				if(resultados.TryGetValue(product.Key, out score)) {
					//Console.WriteLine(product.Key + ": " + score + "/sqrt(" + product.Value + "*" + queryProduct+") = "+score+"/"+Math.Sqrt(product.Value*queryProduct)+" = "+(score/Math.Sqrt(product.Value*queryProduct)));
					resultados[product.Key] = score / Math.Sqrt(product.Value*queryProduct);
				}
			}

            return resultados;
        }
    }
}