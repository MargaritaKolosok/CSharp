using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_1
{
    class Presenter
    {
        Model model;
        MainWindow mainWindow;

        public Presenter(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            this.model = new Model();
            this.mainWindow.SomeEvent += MainWindow_SomeEvent;
        }

        private void MainWindow_SomeEvent(object sender, EventArgs e)
        {
            this.mainWindow.textBox1.Text = this.model.SomeMethod(this.mainWindow.textBox1.Text);
        }
    }
}
