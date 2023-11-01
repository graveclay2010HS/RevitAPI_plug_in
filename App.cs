#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using System.Windows.Media;
using Microsoft.Office.Interop.Excel;
using System.Windows.Forms;
using static System.Console;

#endregion

namespace Template_Generator
{
    class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {

            //Tab name & Panel name
            string tabName = "Template Generator";
            string panelName = "Architecture";

            //Create your tab and panel
            a.CreateRibbonTab(tabName);
            RibbonPanel riBBon =a.CreateRibbonPanel(tabName, panelName);

            //Get the Assembly path
            string thisPath = Assembly.GetExecutingAssembly().Location;

            //define PushButtonData
            PushButtonData pbData1 = new PushButtonData("cmdMaterials", "Materials", thisPath, "Template_Generator.Materials");
            BitmapImage pdIamge1 = new BitmapImage(new Uri("pack://application:,,,/Template Generator;component/Resources/Materials.png"));
            pbData1.LargeImage = pdIamge1;

            //add Pushbutton to Your Panal
            PushButton pbButton1 = riBBon.AddItem(pbData1) as PushButton;

            //get these in a list instead 
            List<string> pdName = new List<string>() { "Types", "Fenestration", "MEP", "Interior" };
            List<PulldownButtonData> pdData = new List<PulldownButtonData>();

            foreach (string item in pdName)
            {
                PulldownButtonData pdItem = new PulldownButtonData("cmd" + item, item);
                BitmapImage pdIamge = new BitmapImage(new Uri("pack://application:,,,/Template Generator;component/Resources/"+item+".png"));
                pdItem.Image = pdIamge;
                pdData.Add(pdItem);
            }

            //create PullDown Stacked Buttons
            List<RibbonItem> stacked = new List<RibbonItem>();
            stacked.AddRange(riBBon.AddStackedItems(pdData[0], pdData[1]));
            stacked.AddRange(riBBon.AddStackedItems(pdData[2], pdData[3]));

            //create PullDown SubItem List
            List<List<string>> subElem = new List<List<string>>();
            subElem.Add(new List<string> { "Walls", "Floors", "Roofs" });
            subElem.Add(new List<string> { "Windows", "Doors" });
            subElem.Add(new List<string> { "Furniture", "Appliances", "Lighting" });
            subElem.Add(new List<string> { "Plumbing", "HVAC" });

            foreach (RibbonItem rbItem in stacked)
            {
                PulldownButton pdButton = rbItem as PulldownButton;
                foreach (string listMe in subElem[stacked.IndexOf(rbItem)])
                {
                    PushButtonData pbdataItem = new PushButtonData("cmd"+listMe, listMe, thisPath, "Template_Generator."+listMe);
                    pdButton.AddPushButton(pbdataItem);
                }
            }


            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
    }
}
