#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;

#endregion

namespace Template_Generator
{
    [Transaction(TransactionMode.Manual)]
    public class Materials : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            //Define variables
            string matName = "Material_01";
            string matClass = "Masonry";
            string assetName = "AppearanceAsset_01";
            string surforePat = "Block 200x200";
            string surbackPat = "<Solid fill>";
            string cutforePat = "Diagonal crosshatch";
            string cutbackPat = "<Solid fill>";
            Color uniColor = new Color(255, 230, 230);
            Color patColor = new Color(0, 0, 0);
            int transPar = 0;

            //Some tooltips for the user
            TaskDialog tipPed = new TaskDialog("Texture");
            tipPed.MainContent = "Choose your texture!";
            tipPed.Show();

            //Create and show FileOpenDialog
            FileOpenDialog textBitmap = new FileOpenDialog("Image Files|*.jpg;*.jpeg;*.png;");
            textBitmap.Show();

            //Set the file path to the texture
            string texturePath = ModelPathUtils.ConvertModelPathToUserVisiblePath(textBitmap.GetSelectedModelPath());

            // Modify document within a transaction

            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Transaction Name");
                ElementId newMat = Material.Create(doc, matName);

                //Harvest the newly created material from Revit
                Material solMat = doc.GetElement(newMat) as Material;

                //Set the material class
                solMat.MaterialClass = matClass;

                //Set ther color
                solMat.Color = uniColor;

                //Set the material transparency
                solMat.Transparency = transPar;

                //Set the SurfaceForegoundPattern and Color
                solMat.SurfaceForegroundPatternId = FillPatternElement.GetFillPatternElementByName(doc, FillPatternTarget.Model, surforePat).Id;
                solMat.SurfaceForegroundPatternColor = patColor;

                //Set the SurfaceBackgroundPattern and Color
                solMat.SurfaceBackgroundPatternId = FillPatternElement.GetFillPatternElementByName(doc, FillPatternTarget.Drafting, surbackPat).Id;
                solMat.SurfaceBackgroundPatternColor = uniColor;

                //Set the CutforegroundPattern and Color
                solMat.CutForegroundPatternId = FillPatternElement.GetFillPatternElementByName(doc, FillPatternTarget.Drafting, cutforePat).Id;
                solMat.CutForegroundPatternColor = patColor;

                //Set the CutBackgroundPattern and Color
                solMat.CutBackgroundPatternId = FillPatternElement.GetFillPatternElementByName(doc, FillPatternTarget.Drafting, cutbackPat).Id;
                solMat.CutBackgroundPatternColor = uniColor;

                //Get your starting ApperanceAssetElement and duplicate it
                AppearanceAssetElement temlAsset = AppearanceAssetElement.GetAppearanceAssetElementByName(doc, "Smooth Precast Structural");
                AppearanceAssetElement newAsset = temlAsset.Duplicate(assetName);
                solMat.AppearanceAssetId = newAsset.Id;

                //Change the Image In The AppearanceAsset
                using (AppearanceAssetEditScope editEd  = new AppearanceAssetEditScope(newAsset.Document))
                {
                    Asset editableAsset = editEd.Start(newAsset.Id);
                    AssetProperty texTure = editableAsset.FindByName("generic_diffuse");
                    Asset connectedAsset = texTure.GetSingleConnectedAsset() as Asset;

                    if (connectedAsset.Name=="UnifiedBitmapSchema")
                    {
                        AssetPropertyString path = connectedAsset.FindByName(UnifiedBitmap.UnifiedbitmapBitmap) as AssetPropertyString;

                        if (path.IsValidValue(texturePath))
                            path.Value = texturePath;
                    }

                    editEd.Commit(true);
                }
               
                tx.Commit();
            }

            //Some tooltips for the user
            TaskDialog scMessage = new TaskDialog("Status");
            scMessage.MainContent = "Success!";
            scMessage.Show();

            return Result.Succeeded;
        }
    }
}
