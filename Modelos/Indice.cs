using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RAI.Modelos
{
    public class Indice
    {
        /// <summary>
        /// Pares (t, (d, n)) donde n es el número de ocurrencias del término t en el documento d.
        /// </summary>
        public Dictionary<string, Dictionary<string, int>> TF { get; protected set; }
        /// <summary>
        /// Pares (t, n), donde n es el número de documentos que contienen el término t.
        /// </summary>
        public Dictionary<string, int> IDF { get; protected set; }
        /// <summary>
        /// Número de documentos en el índice.
        /// </summary>
        public int NumDocuments { get; protected set; }
        /// <summary>
        /// Tokenizer para obtener los términos de un documento.
        /// </summary>
        protected Tokenizer Tokenizer { get; set; }

        public Indice(Tokenizer tok)
        {
            this.IDF = new Dictionary<string, int>();
            this.TF = new Dictionary<string, Dictionary<string, int>>();
            this.Tokenizer = tok;
            this.NumDocuments = 0;
        }

        /// <summary>
        /// Construye un índice con los documentos del directorio especificado.
        /// </summary>
        /// <param name="pathToDocs">Path al directorio que contiene los documentos.</param>
        public void Build(string pathToDocs)
        {
            foreach (string file in Directory.GetFiles(pathToDocs, "*.html")) { // Iteramos los documentos
                this.NumDocuments++;

                string docID = Path.GetFileNameWithoutExtension(file);
                string texto = File.ReadAllText(file);
                string[] terminos = this.Tokenizer.Tokenize(texto);

                var grupos = terminos.GroupBy(t => t); // LINQ: Devuelve un enumerado de pares <término, ocurrencias>
                
                foreach (var g in grupos) {
                    // Actualizar TF e IDF
                    int termIDF;
                    if (this.IDF.TryGetValue(g.Key, out termIDF)) {
                        // El término ya está en el índice
                        this.IDF[g.Key] = termIDF++;
                        this.TF[g.Key][docID] = g.Count();
                        
                    } else {
                        // El término no está en el índice
                        this.IDF[g.Key] = 1;
                        Dictionary<string, int> termTF = new Dictionary<string, int>();
                        termTF[docID] = g.Count();
                        this.TF[g.Key] = termTF;
                    }
                }
            }
        }
    }
}
