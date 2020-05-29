using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MornaMapEditor
{
    public class BatchConverter
    {
        private IEnumerable<string> mapFilenames;
        private ConcurrentDictionary<string, bool> mapNamesWritten = new ConcurrentDictionary<string, bool>();
        private string destFolder;

        public BatchConverter(string sourceFolder, string destinationFolder)
        {
            destFolder = destinationFolder;
            mapFilenames = Directory.GetFiles(sourceFolder).Where(s => s.EndsWith(".map") || s.EndsWith(".cmp"));
        }

        public int NumberToConvert()
        {
            return mapFilenames.Count();
        }
        public void ConvertMaps()
        {
            Parallel.ForEach(mapFilenames, (mapFilename) =>
            {
                var map = new Map(mapFilename);
                var mapImage = map.GetRenderedMap(true, true);
                var extension = Path.GetExtension(mapFilename).TrimStart('.');
                lock (mapNamesWritten)
                {
                    if (mapNamesWritten.ContainsKey(map.Name))
                    {
                        ImageRenderer.Singleton.WriteImageToFile(mapImage, $"{destFolder}\\{map.Name}-{extension}.png");
                        mapNamesWritten.TryAdd($"{map.Name}-{extension}", true);
                    }
                    else
                    {
                        ImageRenderer.Singleton.WriteImageToFile(mapImage, $"{destFolder}\\{map.Name}.png");
                        mapNamesWritten.TryAdd(map.Name, true);
                    }
                }

                mapImage.Dispose();
            });
        }
    }
}