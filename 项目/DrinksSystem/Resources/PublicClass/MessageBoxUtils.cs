using DrinksSystem.Resources.control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrinksSystem.Resources.PublicClass
{
    public class MessageBoxUtils
    {

        private MessageBoxUtils() { }


        public static void alert(string text) 
        {
            UCMessageBox myUC = new UCMessageBox();
            myUC.BoxText = text;
            myUC.Show();
        }
    }
    
}
