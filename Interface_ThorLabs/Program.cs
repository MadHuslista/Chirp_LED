using System;
/* 
 * Comentadas por no ser utilizadas
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
*/
using System.Windows.Forms;

namespace Driver_LED
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
