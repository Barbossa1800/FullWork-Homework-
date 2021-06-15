using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FullWork
{
    public class Configuration
    {
        public Configuration(bool checkBox, string textBox, double height, double width)
        {
            CheckBox = checkBox;
            TextBox = textBox;
            Height = height;
            Width = width;
        }

        public bool CheckBox { get; set; }
        public string TextBox { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
    }
}