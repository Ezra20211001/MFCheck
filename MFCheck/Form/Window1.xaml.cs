using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MFCheck.Form
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        public ObservableCollection<Student> ObservableCollection { get; set; } = new ObservableCollection<Student>();

        public Window1()
        {
            InitializeComponent();

            this.DataContext = this;
            ObservableCollection.Add(new Student() { Name = "Tom", Age = 16, FavorColor = "Red", Hobby = "Swim" });
            ObservableCollection.Add(new Student() { Name = "Maty", Age = 18, FavorColor = "Green", Hobby = "Football" });
            ObservableCollection.Add(new Student() { Name = "Alice", Age = 19, FavorColor = "Yellow", Hobby = "Running" });
            ObservableCollection.Add(new Student() { Name = "Tom", Age = 16, FavorColor = "Red", Hobby = "Swim" });
            ObservableCollection.Add(new Student() { Name = "Maty", Age = 18, FavorColor = "Green", Hobby = "Football" });
            ObservableCollection.Add(new Student() { Name = "Alice", Age = 19, FavorColor = "Yellow", Hobby = "Running" });
            ObservableCollection.Add(new Student() { Name = "Tom", Age = 16, FavorColor = "Red", Hobby = "Swim" });
            ObservableCollection.Add(new Student() { Name = "Maty", Age = 18, FavorColor = "Green", Hobby = "Football" });
            ObservableCollection.Add(new Student() { Name = "Alice", Age = 19, FavorColor = "Yellow", Hobby = "Running" });
            ObservableCollection.Add(new Student() { Name = "Tom", Age = 16, FavorColor = "Red", Hobby = "Swim" });
            ObservableCollection.Add(new Student() { Name = "Maty", Age = 18, FavorColor = "Green", Hobby = "Football" });
            ObservableCollection.Add(new Student() { Name = "Alice", Age = 19, FavorColor = "Yellow", Hobby = "Running" });
            ObservableCollection.Add(new Student() { Name = "Tom", Age = 16, FavorColor = "Red", Hobby = "Swim" });
            ObservableCollection.Add(new Student() { Name = "Maty", Age = 18, FavorColor = "Green", Hobby = "Football" });
            ObservableCollection.Add(new Student() { Name = "Alice", Age = 19, FavorColor = "Yellow", Hobby = "Running" });

        }
    }

    public class Student
    {
        public int Age { get; set; }
        public string Name { get; set; }
        public string FavorColor { get; set; }
        public string Hobby { get; set; }
    }
}
