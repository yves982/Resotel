using ResotelApp.Models;
using ResotelApp.ViewModels.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace CustomControlsTest
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {

            base.OnStartup(e);
            CustomControlContainer container = new CustomControlContainer();

            //OptionChoice optC = new OptionChoice
            //{
            //    Option = new Option
            //    {
            //        Label = "clientAdd",
            //        CurrentDiscount = new Discount
            //        {
            //            ReduceByPercent = 15d
            //        },
            //        BasePrice = 40d
            //    },
            //    TakenDates = new DateRange
            //    {
            //        Start = new DateTime(2016, 2, 15),
            //        End = new DateTime(2016, 3, 1)
            //    }
            //};
            //OptionChoiceEntity opt = new OptionChoiceEntity(optC);

            FlowDocViewModel flowDocVM = new CustomControlsTest.FlowDocViewModel(
                new FlowDocDataItem { Label = "tutu" },
                new FlowDocDataItem { Label = "tata" }
            );

            container.DataContext = flowDocVM;
            MainWindow = container;
            MainWindow.ShowDialog();
        }
    }
}
