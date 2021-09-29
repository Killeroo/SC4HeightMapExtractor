using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using SC4Parser.Files;

namespace SC4HeightMapExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> paths = new List<string>();
            foreach (var arg in args)
            {
                FileAttributes attributes = new FileAttributes();
                try
                {
                    attributes = File.GetAttributes(arg);
                }
                catch (Exception ex) 
                {
                    Console.WriteLine("Could not read files at {0} ({1})",
                        arg,
                        ex.GetType().ToString());

                    continue;
                }

                if (attributes.HasFlag(FileAttributes.Directory))
                {
                    paths.AddRange(Directory.GetFiles(arg, "*.sc4", SearchOption.AllDirectories));
                } 
                else
                {
                    paths.Add(arg);
                }
            }

            foreach (var path in paths)
            {
                SC4SaveFile file = null;
                try
                {
                    file = new SC4SaveFile(path);

                    var regionInfo = file.GetRegionViewSubfile();
                    float[][] terrainData = file.GetTerrainMapSubfile().Map;

                    string filename = string.Format("{0}_size-{1}x{2}_pos-{3}x{4}.csv",
                        regionInfo.CityName,
                        regionInfo.CitySizeX,
                        regionInfo.CitySizeY,
                        regionInfo.TileXLocation,
                        regionInfo.TileYLocation);

                    string fileData = "";
                    for (int y = 0; y < regionInfo.CitySizeY; y++)
                    {
                        for (int x = 0; x < regionInfo.CitySizeX; x++)
                        {
                            fileData += terrainData[x][y];
                            if (x != regionInfo.CitySizeX - 1)
                            {
                                fileData += ",";
                            }
                        }
                        fileData += Environment.NewLine;
                    }

                    File.WriteAllText(filename, fileData);
                    Console.WriteLine("Height map extracted for {0} @ {1}",
                        Path.GetFileName(path),
                        filename);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Can't read SC4 save files {0} ({1})",
                        path,
                        e.GetType().ToString());

                    continue;
                }

            }

        }

    }
}
