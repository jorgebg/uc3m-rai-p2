using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RAI.Modelos
{
    public class Tokenizer
    {
        /// <summary>
        /// Expresión regular para eliminar etiquetas HTML
        /// </summary>
        protected static readonly Regex HtmlRegex = new Regex("<[^>]+>", RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        /// Eliminia las etiquetas del texto y devuelve los términos que contiene.
        /// </summary>
        /// <param name="text">El texto a tokenizar.</param>
        /// <returns>Los términos que aparecen en el texto.</returns>
        public string[] Tokenize(string text)
        {
            string noHtml = System.Web.HttpUtility.HtmlDecode(text); // desescapar caracteres especiales
            noHtml = Tokenizer.HtmlRegex.Replace(noHtml, " "); // eliminar etiqueta
            return noHtml.Split(new char[] { '\r', '\n', '\t', ' ', '.', ';', '|', ',', '-', '(', ')', '?', '\'' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
