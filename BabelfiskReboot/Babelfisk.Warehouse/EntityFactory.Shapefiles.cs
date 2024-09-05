using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Globe.Core.CollisionDetection;
using Globe.Core;

namespace Babelfisk.Warehouse
{
    public partial class EntityFactory
    {
        private static bool _blnShapefilesInitialized = false;
        private static Globe.Core.CollisionDetection.ShapefileIntersector _sfAreas = null;
        private static Globe.Core.CollisionDetection.ShapefileIntersector _sfRectangles = null;

        public string GetArea(double lat, double lon)
        {
            if(_sfAreas == null)
                return null;

            List<ISpatialObject> lst = _sfAreas.Intersections(new GeometricLibrary.Core.Vector.Vec2d(lon, lat));

            if (lst != null && lst.Count > 0)
            {
                var shp = lst.First() as Shape;
                return shp.GetMetadata("DFUArea");
            }

            return null;
        }


        public string GetRectangle(double lat, double lon)
        {
            if (_sfRectangles == null)
                return null;

            List<ISpatialObject> lst = _sfRectangles.Intersections(new GeometricLibrary.Core.Vector.Vec2d(lon, lat));

            if (lst != null && lst.Count > 0)
            {
                var shp = lst.First() as Shape;
                return shp.GetMetadata("ices_rect");
            }

            return null;
        }


        private void InitializeShapefiles()
        {
            if (_blnShapefilesInitialized)
                return;

            _blnShapefilesInitialized = true;

            //string strDbfFilePath = CopyResourceToAppDataIfDifferent("icesarea.dbf", "ShapefileTmp\\icesarea.dbf");

            _sfAreas = LoadShapefileIntersector("ICES_areas_fiskeline");
            _sfRectangles = LoadShapefileIntersector("ices_squares_fiskeline");
        }


        private Globe.Core.CollisionDetection.ShapefileIntersector LoadShapefileIntersector(string strShapefileName)
        {
            Stream sShp = GetLocalResource(strShapefileName + ".shp");
            Stream sPrj = GetLocalResource(strShapefileName + ".prj");
            Stream sDbf = GetLocalResource(strShapefileName + ".dbf");

            Globe.Core.Shapefile sf = new Globe.Core.Shapefile();
            sf.ParseShapefile(sShp, sPrj, sDbf);
            var sfi = new Globe.Core.CollisionDetection.ShapefileIntersector(sf);

            return sfi;
        }


        private Stream GetLocalResource(string strResourceName)
        {
            string strManifestURI = "Babelfisk.Warehouse.Resources." + strResourceName;
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(strManifestURI);
        }


       /* [Obsolete("This method is obsolete. ")]
        public string CopyResourceToAppDataIfDifferent(string strResourceName, string strRelativeDestinationPath)
        {
            string strFilePath = null;

            try
            {
                var resStream = GetLocalResource(strResourceName);

                if (resStream == null)
                    return strFilePath;

                if (!Directory.Exists(_strTmpFilesPath))
                    Directory.CreateDirectory(_strTmpFilesPath);

                string strDirPath = Path.Combine(_strTmpFilesPath, Path.GetDirectoryName(strRelativeDestinationPath));

                if (!Directory.Exists(strDirPath))
                    Directory.CreateDirectory(strDirPath);

                strFilePath = Path.Combine(_strTmpFilesPath, strRelativeDestinationPath);

                if (File.Exists(strFilePath))
                {
                    var fi = new FileInfo(strFilePath);

                    if (fi.Length == resStream.Length)
                        return strFilePath;
                }

                Stream s;
                byte[] b;

                using (BinaryReader br = new BinaryReader(resStream))
                {
                    b = br.ReadBytes((int)resStream.Length);
                }

                File.WriteAllBytes(strFilePath, b);
                resStream.Dispose();
            }
            catch (Exception e)
            {
                Messages.Add(DWMessage.NewError(null, "Error while transferring shapefile db file.", "", "", e.Message));
            }


            return strFilePath;
        }*/
    }
}
