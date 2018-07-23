using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MiniTorrent
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ReadDll : Window
    {

        private Dictionary<string, Type> dllTypes = new Dictionary<string, Type>();
        public ObservableCollection<string> ObserverFuncsNames = new ObservableCollection<string>();

        public ReadDll()
        {
            InitializeComponent();
            listFuncation.ItemsSource = ObserverFuncsNames;

        }

        private void ClearAllListBox()
        {
            dllTypes = new Dictionary<string, Type>();
            ListTypes.Items.Clear();
            // listFuncation.Items.Clear();
            listParameters.Items.Clear();
            ObserverFuncsNames.Clear();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            ClearAllListBox();
            setListboxTypes();
        }

        //Read All Class in DLL
        private void setListboxTypes()
        {
            try
            {
                Assembly assembly = Assembly.LoadFrom(Search_textBox.Text);
                foreach (Type type in assembly.GetTypes())
                {
                    ListTypes.Items.Add(type.FullName);
                    dllTypes.Add(type.FullName, type);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("DLL could not load or could not read well.", "DLL Exeption", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        //Read all Method on class in DLL
        private void getFuncationAndPropertiesInType()
        {
            ObserverFuncsNames.Clear();
            if (ListTypes.SelectedItem != null)
            {
                string selectedType = ListTypes.SelectedItem.ToString();
                Type type = dllTypes[selectedType];

                foreach (var item in type.GetMembers())
                {
                    ObserverFuncsNames.Add(item.Name);
                    //listFuncation.Items.Add(item.Name);
                }
            }
        }


        private void listTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            getFuncationAndPropertiesInType();
        }

        private void listFuncation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //listFuncation.Items.Clear();
            //ObserverFuncsNames;
            if (ListTypes.SelectedItem != null && listFuncation.SelectedItem != null)
            {
                listParameters.Items.Clear();
                txtInput.Text = string.Empty;
                txtParameters.Text = string.Empty;
                string selectedType = ListTypes.SelectedItem.ToString();
                string selectedMethod = listFuncation.SelectedItem.ToString();
                Type type = dllTypes[selectedType];
                MethodInfo method = type.GetMethod(selectedMethod);
                if (method != null)
                {
                    foreach (var parameter in method.GetParameters())
                    {
                        listParameters.Items.Add(parameter);
                    }
                }
            }
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            object[] parameters = null;
            object[] objectParameter = null;
            if (!txtParameters.Text.Equals(""))
            {
                parameters = txtParameters.Text.Split(',');
                objectParameter = new object[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    ParameterInfo p = (ParameterInfo)listParameters.Items[i];
                    if (p.ParameterType.Name.Equals("Int32"))
                        objectParameter[i] = int.Parse(parameters[i].ToString());
                    else
                        objectParameter[i] = parameters[i];
                }
            }
            string selectedType = ListTypes.SelectedItem.ToString();
            string selectedMethod = listFuncation.SelectedItem.ToString();
            Type type = dllTypes[selectedType];
            MethodInfo method = type.GetMethod(selectedMethod);
            object myInstance = Activator.CreateInstance(type);
            var input = method.Invoke(myInstance, objectParameter);
            if (input != null)
                txtInput.Text = input.ToString();
        }

        public static Type GetType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null)
                    return type;
            }

            return null;
        }
    }
}
