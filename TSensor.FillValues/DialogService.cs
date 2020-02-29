using System.IO;
using System.Windows.Forms;

namespace TSensor.FillValues
{
    public class DialogService
    {
        public Stream Open(string filter)
        {
            using (var dialog = new OpenFileDialog { Filter = filter, Title = "Выберите файл с исходными данными" })
            {
                dialog.RestoreDirectory = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    return dialog.OpenFile();
                }
                else
                {
                    return null;
                }
            }
        }

        public Stream Save(string filter)
        {
            using (var dialog = new SaveFileDialog { Filter = filter, Title = "Укажите файл для сохранения результата" })
            {
                dialog.RestoreDirectory = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    return dialog.OpenFile();
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
